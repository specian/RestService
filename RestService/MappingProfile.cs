using RestService.Dtos;
using RestService.Models;
using AutoMapper;

namespace RestService;

public class MappingProfile : Profile
{
	public MappingProfile()
	{
		CreateMap<Order, OrderDto>();
		CreateMap<OrderItem, OrderItemDto>();
		CreateMap<OrderCreateDto, Order>();
		CreateMap<OrderItemDto, OrderItem>();
	}
}
