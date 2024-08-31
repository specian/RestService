using System.Collections.Concurrent;
using RestService.Data;
using RestService.Domain;
using RestService.Dtos;
using RestService.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace RestService.Controllers;

[ApiController]
[Route("api/order")]
public class OrderController(IRestServiceRepo repo, IMapper mapper, ILogger<OrderController> logger) : Controller
{
	private static readonly ConcurrentQueue<PaymentRequest> _paymentQueue = new();

	/// <summary>
	/// Vrátí všechny objednávky.
	/// </summary>
	/// <returns></returns>
	[HttpGet]
	public async Task<ActionResult<IEnumerable<OrderDto>>> Get()
	{
		try
		{
			return Ok(mapper.Map<IEnumerable<OrderDto>>(await repo.GetAllOrdersAsync()));
		}
		catch (DbUpdateException ex)
		{
			logger.LogError(ex, "Reading orders from database");
			return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error reading orders from the database.", error = ex.Message });
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Unknown error while reading orders");
			return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Unknown error.", error = ex.Message });
		}
	}

	/// <summary>
	/// Přidá novou objednávku.
	/// </summary>
	/// <param name="orderCreate"></param>
	/// <returns></returns>
	[HttpPost]
	public async Task<ActionResult> Add(OrderCreateDto orderCreate)
	{
		if (orderCreate.OrderNumber <= 0 ||
			string.IsNullOrWhiteSpace(orderCreate.CustomerName)
			|| orderCreate.Items.Count == 0)
		{
			return BadRequest("Wrong order.");
		}

		// Pokud už objednávka daného čísla v DB existuje, je to ošetřeno unikátním klíčem
		// – není třeba kvůli tomu vystřelovat další dotaz do DB
		try
		{
			Order order = mapper.Map<Order>(orderCreate);
			await repo.AddOrderAsync(order);
			return Ok();
		}
		catch (DbUpdateException ex)
		{
			logger.LogError(ex, "Storing order into database");
			return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error storing the order into the database.", error = ex.Message });
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Unknown error while storing orders");
			return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Unknown error while storing orders.", error = ex.Message });
		}
	}

	/// <summary>
	/// Okamžitá změna stavu nové objednávky na "zaplacená" nebo "zrušená".
	/// Je to POST, protože to není idempotentní operace – je to jednorázová akce,
	/// posunutí objednávky do nového stavu, může se tedy volat jen jednou.
	/// </summary>
	/// <param name="orderNumber"></param>
	/// <param name="paymentStatus"></param>
	/// <returns></returns>
	[HttpPost("payment")]
	public async Task<ActionResult> Payment([FromBody] PaymentRequest payment)
	{
		try
		{
			OrderStatusEnum? orderStatus = await repo.GetOrderStatusAsync(payment.OrderNumber);
			if (orderStatus is null)
			{
				return BadRequest("Order not found.");
			}

			if (orderStatus != OrderStatusEnum.New)
			{
				return BadRequest("Order is not in a valid state for payment.");
			}

			await repo.SetPaymentAsync(payment);

			return Ok();
		}
		catch (DbUpdateException ex)
		{
			logger.LogError(ex, "Updating order in database");
			return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error updating the order in the database.", error = ex.Message });
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Unknown error while updating order status");
			return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Unknown error while updating order status.", error = ex.Message });
		}
	}

	/// <summary>
	/// Pro účely testu.
	/// Nastaví platbu objednávky pro pozdější zpracování pomocí fronty.
	/// </summary>
	/// <param name="payment"></param>
	/// <returns></returns>
	[HttpPost("set-payment")]
	public ActionResult SetPayment([FromBody] PaymentRequest payment)
	{
		_paymentQueue.Enqueue(payment);
		return Accepted();
	}

	/// <summary>
	/// Pro účely testu.
	/// Zpracuje všechny platby ve frontě, nastavené dříve pomocí set-payment.
	/// </summary>
	/// <returns></returns>
	[HttpGet("process-payments")]
	public async Task<ActionResult> ProcessPayments()
	{

		try
		{
			while (_paymentQueue.TryDequeue(out PaymentRequest? payment))
			{
				OrderStatusEnum? orderStatus = await repo.GetOrderStatusAsync(payment.OrderNumber);
				if (orderStatus is not null && orderStatus == OrderStatusEnum.New)
				{
					await repo.SetPaymentAsync(payment);
				}
				else
				{
					logger.LogWarning("Order {OrderNumber} not found or not in a valid state for payment.", payment.OrderNumber);
				}
			}
		}
		catch (DbUpdateException ex)
		{
			logger.LogError(ex, "Asynchronous updating order in database");
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Unknown error while asynchronous updating order status");
		}

		return Ok();
	}
}
