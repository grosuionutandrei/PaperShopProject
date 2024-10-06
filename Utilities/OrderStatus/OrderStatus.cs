namespace utilities.OrderStatus;

public enum OrderStatus
{
    Completed,Pending,Cancelled,Shipped,Declined
}
public static class OrderStatusMessage
{
    private static readonly Dictionary<OrderStatus, string> _errorMessages = new Dictionary<OrderStatus, string>
    {
        { OrderStatus.Shipped,"Shipped" },
        { OrderStatus.Cancelled,"Cancelled" },
        { OrderStatus.Declined,"Declined" },
        { OrderStatus.Pending,"Pending" },
        { OrderStatus.Completed,"Completed" },
    };
    
    public static string GetMessage(OrderStatus orderStatus)
    {
        return _errorMessages.GetValueOrDefault(orderStatus,"This status is undefined");
    }
}