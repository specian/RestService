using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RestService.Domain;

namespace RestService.Models;

public class Order
{
	[Key]
	public int Id { get; set; }

	[Required]
	public int OrderNumber { get; set; }

	[StringLength(160)]
	public string CustomerName { get; set; } = null!;

	[Column(TypeName = "date")]
	public DateOnly CreationDate { get; set; }

	[Required]
	public OrderStatusEnum Status { get; set; } = OrderStatusEnum.New;

	public ICollection<OrderItem> Items { get; set; } = null!;
}
