using RestService.Models;
using Microsoft.EntityFrameworkCore;

namespace RestService.Data;

public class RestServiceContext(DbContextOptions<RestServiceContext> options) : DbContext(options)
{
	public DbSet<Order> Orders { get; set; }

	public DbSet<OrderItem> OrderItems { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Order>()
			.HasIndex(o => o.OrderNumber)
			.IsUnique();
	}
}