namespace infrastructure.QuerryModels;

public class PaperToAdd
{
    public string Name { get; set; } = null!;

    public bool Discontinued { get; set; }

    public int Stock { get; set; }

    public double Price { get; set; }

    public IEnumerable<PaperProperties>? PaperPropertiesList { get; set; }
}