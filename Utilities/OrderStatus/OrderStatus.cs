namespace utilities.OrderStatus;

public enum OrderStatus
{
    Completed,
    Pending,
    Cancelled,
    Shipped,
    Declined
}

public static class OrderStatusMessage
{
    private static readonly Dictionary<OrderStatus, string> StatusMessages = new Dictionary<OrderStatus, string>
    {
        { OrderStatus.Shipped, "Shipped" },
        { OrderStatus.Cancelled, "Cancelled" },
        { OrderStatus.Declined, "Declined" },
        { OrderStatus.Pending, "Pending" },
        { OrderStatus.Completed, "Completed" },
    };

    public static string GetMessage(OrderStatus orderStatus)
    {
        return StatusMessages.GetValueOrDefault(orderStatus, "This status is undefined");
    }

    public static bool IsStatusValid(string status)
    {
        Console.WriteLine(StatusMessages.ContainsValue(status.Trim()) + " e54g2rwgregargw455thw5");
        Console.WriteLine(status + "erefeqrfqrefqerfqef");
        return StatusMessages.ContainsValue(status);
    }
}