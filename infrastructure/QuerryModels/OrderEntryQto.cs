namespace infrastructure.QuerryModels;

public class OrderEntryQto
{
    public int Id { get; set; }
    public string? PaperName { get; set; }
    public int OrderQuantity { get; set; }

    public double Price { get; set; }

    public IEnumerable<PaperProperties>? PaperProperties { get; set; }
}