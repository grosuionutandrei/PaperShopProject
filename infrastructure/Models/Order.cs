﻿namespace infrastructure.Models;

public class Order
{
    public int Id { get; set; }

    public DateTime OrderDate { get; set; }

    public DateOnly? DeliveryDate { get; set; }

    public string Status { get; set; } = null!;

    public double TotalAmount { get; set; }

    public int? CustomerId { get; set; }

    public bool Deleted { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual ICollection<OrderEntry> OrderEntries { get; set; } = new List<OrderEntry>();
}