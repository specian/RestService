namespace RestService.Dtos;

public class OrderCreateDto
{
    public int OrderNumber { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public ICollection<OrderItemDto> Items { get; set; } = [];
}
