using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestService.Models;

public class OrderItem
{
	[Key]
	public int Id { get; set; }

	[StringLength(220)]
	public string ProductName { get; set; } = null!;

	[Required]
	public int Quantity { get; set; }

	[Required]
	[Column(TypeName = "decimal(8, 2)")]
	public decimal Price { get; set; }

	public Order Order { get; set; } = null!;
}