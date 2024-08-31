using RestService.Controllers;
using RestService.Data;
using RestService.Dtos;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace RestService.Tests;

// Lehký základ testů

public class OrderControllerTests
{
	private readonly IMapper _mapper;
	private readonly SqlRestServiceRepo _repo;

	public OrderControllerTests()
    {
		var config = new MapperConfiguration(cfg =>
		{
			cfg.AddProfile<MappingProfile>();
		});

		_mapper = config.CreateMapper();

		_repo = new SqlRestServiceRepo(GetInMemoryDbContext());
	}

	[Fact]
	public void GetOrders_ShouldReturnOkWithOrders()
	{
		var controller = new OrderController(
			_repo, 
			_mapper, 
			new Mock<ILogger<OrderController>>().Object);

		ActionResult<IEnumerable<Dtos.OrderDto>> result = controller.Get().Result;
		var okResult = Assert.IsType<OkObjectResult>(result.Result);
		IEnumerable<OrderDto> orders = Assert.IsAssignableFrom<IEnumerable<Dtos.OrderDto>>(okResult.Value);
		Assert.Equal(200, okResult.StatusCode);
		//Assert.True(orders.Count() > 0);
	}

	[Fact]
	public void AddOrder_AddShouldReturnOk()
	{
		var controller = new OrderController(
			_repo, 
			_mapper, 
			new Mock<ILogger<OrderController>>().Object);

		OrderCreateDto orderCreate = new OrderCreateDto
		{
			OrderNumber = 226688,
			CustomerName = "ACME s r. o.",
			Items =
			[
				new OrderItemDto
				{
					ProductName = "Tester jumbo",
					Price = 2495.00m,
					Quantity = 1
				}
			]
		};

		ActionResult result = controller.Add(orderCreate).Result;
		var okResult = Assert.IsType<OkResult>(result);
		Assert.Equal(200, okResult.StatusCode);
	}

	private static RestServiceContext GetInMemoryDbContext()
	{
		var options = new DbContextOptionsBuilder<RestServiceContext>()
			.UseInMemoryDatabase(databaseName: "TestDatabase")
			.Options;

		return new RestServiceContext(options);
	}
}