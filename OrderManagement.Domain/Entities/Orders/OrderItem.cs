namespace OrderManagement.Domain.Entities.Orders;

public class OrderItem
{
	public Guid Id { get; private set; }

	public Guid ProductId { get; private set; }

	public decimal UnitPrice { get; private set; }

	public int Quantity { get; private set; }

	public decimal Total => UnitPrice * Quantity;

	private OrderItem()
	{
	}

	internal OrderItem(
		Guid productId,
		decimal unitPrice,
		int quantity)
	{
		if (productId == Guid.Empty)
			throw new ArgumentException(
				"ProductId is required.",
				nameof(productId));

		if (unitPrice < 0)
			throw new ArgumentException(
				"Unit price cannot be negative.",
				nameof(unitPrice));

		if (quantity <= 0)
			throw new ArgumentException(
				"Quantity must be greater than zero.",
				nameof(quantity));

		Id = Guid.NewGuid();
		ProductId = productId;
		UnitPrice = unitPrice;
		Quantity = quantity;
	}


}