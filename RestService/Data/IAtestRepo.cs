using RestService.Domain;
using RestService.Models;

namespace RestService.Data;

public interface IRestServiceRepo
{
	Task<IEnumerable<Order>> GetAllOrdersAsync();
	Task AddOrderAsync(Order order);
	Task<OrderStatusEnum?> GetOrderStatusAsync(int orderNumber);
	Task SetPaymentAsync(PaymentRequest payment);
}
