namespace infrastructure.QuerryModels;

public class OrderPlaced:OrderMain
{
    public IEnumerable<OrderEntryPlaced>? OrderProducts { get; set; }
    public bool Deleted { get; set; } = false;
}