namespace RestService.Domain;

public class PaymentRequest
{
	public int OrderNumber { get; set; }
	public bool IsPaid { get; set; }
}
