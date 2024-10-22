namespace infrastructure.Models;

public class OrderEntry
{
    public int Id { get; set; }

    public int Quantity { get; set; }

    public int? ProductId { get; set; }

    public int? OrderId { get; set; }

    public virtual Order? Order { get; set; }

    public virtual Paper? Product { get; set; }
}