using OrderManagement.Domain.Enums;

namespace OrderManagement.Domain.Tests.Orders;

public class OrderTests
{
	[Fact]
	public void Create_ShouldCreatePendingOrder()
	{
		// Arrange
		var customerId = Guid.NewGuid();

		// Act
		var order = Order.Create(customerId);

		//haaa haaa
		
		// Assert
		Assert.NotEqual(Guid.Empty, order.Id);
		Assert.Equal(customerId, order.CustomerId);
		Assert.Equal(OrderStatus.Pending, order.Status);
		Assert.Empty(order.Items);
		Assert.Equal(0m, order.TotalAmount);
	}

	[Fact]
	public void Create_WithEmptyCustomerId_ShouldThrow()
	{
		// Act & Assert
		Assert.Throws<ArgumentException>(
			() => Order.Create(Guid.Empty));
	}

	[Fact]
	public void AddItem_ShouldAddItemToOrder()
	{
		// Arrange
		var order = Order.Create(Guid.NewGuid());
		var productId = Guid.NewGuid();

		// Act
		order.AddItem(productId, 100m, 2);

		// Assert
		var item = Assert.Single(order.Items);

		Assert.Equal(productId, item.ProductId);
		Assert.Equal(100m, item.UnitPrice);
		Assert.Equal(2, item.Quantity);
		Assert.Equal(200m, item.Total);
		Assert.Equal(200m, order.TotalAmount);
	}

	[Fact]
	public void AddItem_WithInvalidProductId_ShouldThrow()
	{
		// Arrange
		var order = Order.Create(Guid.NewGuid());

		// Act & Assert
		Assert.Throws<ArgumentException>(
			() => order.AddItem(
				Guid.Empty,
				100m,
				1));
	}

	[Fact]
	public void AddItem_WithNegativePrice_ShouldThrow()
	{
		// Arrange
		var order = Order.Create(Guid.NewGuid());

		// Act & Assert
		Assert.Throws<ArgumentException>(
			() => order.AddItem(
				Guid.NewGuid(),
				-10m,
				1));
	}

	[Fact]
	public void AddItem_WithZeroQuantity_ShouldThrow()
	{
		// Arrange
		var order = Order.Create(Guid.NewGuid());

		// Act & Assert
		Assert.Throws<ArgumentException>(
			() => order.AddItem(
				Guid.NewGuid(),
				100m,
				0));
	}

	[Fact]
	public void AddItem_WithNegativeQuantity_ShouldThrow()
	{
		// Arrange
		var order = Order.Create(Guid.NewGuid());

		// Act & Assert
		Assert.Throws<ArgumentException>(
			() => order.AddItem(
				Guid.NewGuid(),
				100m,
				-1));
	}

	[Fact]
	public void RemoveItem_ShouldRemoveItemFromOrder()
	{
		// Arrange
		var order = Order.Create(Guid.NewGuid());
		var productId = Guid.NewGuid();

		order.AddItem(productId, 100m, 2);

		// Act
		order.RemoveItem(productId);

		// Assert
		Assert.Empty(order.Items);
		Assert.Equal(0m, order.TotalAmount);
	}

	[Fact]
	public void RemoveItem_WithNonExistingProduct_ShouldThrow()
	{
		// Arrange
		var order = Order.Create(Guid.NewGuid());

		// Act & Assert
		Assert.Throws<InvalidOperationException>(
			() => order.RemoveItem(Guid.NewGuid()));
	}

	[Fact]
	public void Confirm_WithoutItems_ShouldThrow()
	{
		// Arrange
		var order = Order.Create(Guid.NewGuid());

		// Act & Assert
		Assert.Throws<InvalidOperationException>(
			() => order.Confirm());
	}

	[Fact]
	public void Confirm_WithItems_ShouldChangeStatusToConfirmed()
	{
		// Arrange
		var order = Order.Create(Guid.NewGuid());

		order.AddItem(
			Guid.NewGuid(),
			100m,
			1);

		// Act
		order.Confirm();

		// Assert
		Assert.Equal(
			OrderStatus.Confirmed,
			order.Status);
	}

	[Fact]
	public void Confirm_AlreadyConfirmedOrder_ShouldThrow()
	{
		// Arrange
		var order = Order.Create(Guid.NewGuid());

		order.AddItem(
			Guid.NewGuid(),
			100m,
			1);

		order.Confirm();

		// Act & Assert
		Assert.Throws<InvalidOperationException>(
			() => order.Confirm());
	}

	[Fact]
	public void AddItem_ToConfirmedOrder_ShouldThrow()
	{
		// Arrange
		var order = Order.Create(Guid.NewGuid());

		order.AddItem(
			Guid.NewGuid(),
			100m,
			1);

		order.Confirm();

		// Act & Assert
		Assert.Throws<InvalidOperationException>(
			() => order.AddItem(
				Guid.NewGuid(),
				200m,
				1));
	}

	[Fact]
	public void RemoveItem_FromConfirmedOrder_ShouldThrow()
	{
		// Arrange
		var order = Order.Create(Guid.NewGuid());

		var productId = Guid.NewGuid();

		order.AddItem(productId, 100m, 1);
		order.Confirm();

		// Act & Assert
		Assert.Throws<InvalidOperationException>(
			() => order.RemoveItem(productId));
	}

	[Fact]
	public void Cancel_PendingOrder_ShouldChangeStatusToCancelled()
	{
		// Arrange
		var order = Order.Create(Guid.NewGuid());

		// Act
		order.Cancel();

		// Assert
		Assert.Equal(
			OrderStatus.Cancelled,
			order.Status);
	}

	[Fact]
	public void Cancel_ConfirmedOrder_ShouldChangeStatusToCancelled()
	{
		// Arrange
		var order = Order.Create(Guid.NewGuid());

		order.AddItem(
			Guid.NewGuid(),
			100m,
			1);

		order.Confirm();

		// Act
		order.Cancel();

		// Assert
		Assert.Equal(
			OrderStatus.Cancelled,
			order.Status);
	}

	[Fact]
	public void Cancel_AlreadyCancelledOrder_ShouldDoNothing()
	{
		// Arrange
		var order = Order.Create(Guid.NewGuid());

		order.Cancel();

		// Act
		order.Cancel();

		// Assert
		Assert.Equal(
			OrderStatus.Cancelled,
			order.Status);
	}

	[Fact]
	public void AddItem_ToCancelledOrder_ShouldThrow()
	{
		// Arrange
		var order = Order.Create(Guid.NewGuid());

		order.Cancel();

		// Act & Assert
		Assert.Throws<InvalidOperationException>(
			() => order.AddItem(
				Guid.NewGuid(),
				100m,
				1));
	}

	[Fact]
	public void RemoveItem_FromCancelledOrder_ShouldThrow()
	{
		// Arrange
		var order = Order.Create(Guid.NewGuid());

		var productId = Guid.NewGuid();

		order.AddItem(productId, 100m, 1);

		order.Cancel();

		// Act & Assert
		Assert.Throws<InvalidOperationException>(
			() => order.RemoveItem(productId));
	}
}
