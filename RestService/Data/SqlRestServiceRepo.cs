using RestService.Domain;
using RestService.Models;
using Microsoft.EntityFrameworkCore;

namespace RestService.Data;

public class SqlRestServiceRepo(RestServiceContext dbContext) : IRestServiceRepo
{
	public async Task<IEnumerable<Order>> GetAllOrdersAsync()
	{
		return await dbContext.Orders
			.Include(order => order.Items)
			.AsNoTracking()
			.ToListAsync();
	}

	public async Task AddOrderAsync(Order order)
	{
		order.CreationDate = DateOnly.FromDateTime(DateTime.Now); // TODO MŠp: Nahradit DateTime providerem, aby bylo lépe testovatelné
		order.Status = OrderStatusEnum.New;
		dbContext.Orders.Add(order);
		await dbContext.SaveChangesAsync();
	}

	public async Task<OrderStatusEnum?> GetOrderStatusAsync(int orderNumber)
	{
		Order? order = await dbContext.Orders
			.AsNoTracking()
			.FirstOrDefaultAsync(order => order.OrderNumber == orderNumber);
		return order?.Status;
	}

	public async Task SetPaymentAsync(PaymentRequest payment)
	{
		Order order = await dbContext.Orders.SingleAsync(order => order.OrderNumber == payment.OrderNumber);
		order.Status = payment.IsPaid ? OrderStatusEnum.Paid : OrderStatusEnum.Cancelled;
		await dbContext.SaveChangesAsync();
	}
}
