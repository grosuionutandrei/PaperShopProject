namespace infrastructure.QuerryModels;

public class OrderMain
{
    public int Id { get; set; }

    public DateTime OrderDate { get; set; }

    public DateOnly? DeliveryDate { get; set; }

    public string Status { get; set; } = null!;

    public double TotalAmount { get; set; }
}