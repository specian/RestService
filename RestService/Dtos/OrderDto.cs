using RestService.Domain;

namespace RestService.Dtos;

public class OrderDto
{
    public int OrderNumber { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public DateOnly CreationDate { get; set; }
    public OrderStatusEnum Status { get; set; } = OrderStatusEnum.New;
    public ICollection<OrderItemDto> Items { get; set; } = [];
}
