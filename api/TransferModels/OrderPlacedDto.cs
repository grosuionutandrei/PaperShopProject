namespace api.TransferModels;

public class OrderPlacedDto
{
    public int CustomerId { get; set; }
    public IEnumerable<OrderEntryPlacedDto>? OrderPlacedProducts { get; set; }
}

public class OrderEntryPlacedDto
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}