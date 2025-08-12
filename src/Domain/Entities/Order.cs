namespace Dastardly.Domain.Entities;

public sealed class Order
{
    public Guid Id { get; private set; }
    public Guid CustomerId { get; private set; }
    public IReadOnlyList<OrderItem> Items { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public OrderStatus Status { get; private set; }

    private readonly List<OrderItem> _items = [];

    private Order() // For EF Core
    {
        Items = _items.AsReadOnly();
    }

    public Order(Guid customerId, IEnumerable<string> itemNames)
    {
        if (customerId == Guid.Empty)
            throw new ArgumentException("Customer ID cannot be empty", nameof(customerId));

        var itemList = itemNames.ToList();
        if (itemList.Count == 0)
            throw new ArgumentException("Order must contain at least one item", nameof(itemNames));

        Id = Guid.NewGuid();
        CustomerId = customerId;
        CreatedAt = DateTime.UtcNow;
        Status = OrderStatus.Pending;

        foreach (var itemName in itemList)
        {
            _items.Add(new OrderItem(itemName));
        }

        Items = _items.AsReadOnly();
    }

    public void MarkAsProcessing()
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException($"Cannot mark order as processing when status is {Status}");

        Status = OrderStatus.Processing;
    }

    public void MarkAsCompleted()
    {
        if (Status != OrderStatus.Processing)
            throw new InvalidOperationException($"Cannot mark order as completed when status is {Status}");

        Status = OrderStatus.Completed;
    }
}

public sealed record OrderItem(string Name)
{
    public Guid Id { get; } = Guid.NewGuid();
}

public enum OrderStatus
{
    Pending,
    Processing,
    Completed,
    Cancelled
}
