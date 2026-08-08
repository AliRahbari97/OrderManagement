using OrderManagement.Domain.Entities.Orders;
using OrderManagement.Domain.Enums;

public class Order
{
	private readonly List<OrderItem> _items = new();

	public Guid Id { get; private set; }

	public Guid CustomerId { get; private set; }

	public OrderStatus Status { get; private set; }

	public IReadOnlyCollection<OrderItem> Items =>
		_items.AsReadOnly();

	public decimal TotalAmount =>
		_items.Sum(x => x.Total);

	private Order()
	{
	}

	private Order(Guid customerId)
	{
		if (customerId == Guid.Empty)
			throw new ArgumentException(
				"CustomerId is required.",
				nameof(customerId));

		Id = Guid.NewGuid();
		CustomerId = customerId;
		Status = OrderStatus.Pending;
	}

	public static Order Create(Guid customerId)
	{
		return new Order(customerId);
	}

	public void AddItem(
		Guid productId,
		decimal unitPrice,
		int quantity)
	{
		if (Status != OrderStatus.Pending)
			throw new InvalidOperationException(
				"Only pending orders can be modified.");

		var item = new OrderItem(
			productId,
			unitPrice,
			quantity);

		_items.Add(item);
	}

	public void Confirm()
	{
		if (Status != OrderStatus.Pending)
			throw new InvalidOperationException(
				"Only pending orders can be confirmed.");

		if (_items.Count == 0)
			throw new InvalidOperationException(
				"An order must contain at least one item.");

		Status = OrderStatus.Confirmed;
	}

	public void RemoveItem(Guid productId)
	{
		if (Status != OrderStatus.Pending)
			throw new InvalidOperationException(
				"Only pending orders can be modified.");

		var item = _items.FirstOrDefault(x => x.ProductId == productId);

		if (item is null)
			throw new InvalidOperationException(
				"Product does not exist in the order.");

		_items.Remove(item);
	}

	public void Cancel()
	{
		if (Status == OrderStatus.Cancelled)
			return;

		if (Status != OrderStatus.Pending &&
			Status != OrderStatus.Confirmed)
			throw new InvalidOperationException(
				"Order cannot be cancelled.");

		Status = OrderStatus.Cancelled;
	}
}