namespace infrastructure.QuerryModels;

public class PaperFilterQuery
{
    public string? searchFilter { get; set; }
    public int pageNumber { get; set; }
    public int pageItems { get; set; }
    public PriceRange? priceRange { get; set; }
    public IEnumerable<int>? paperPropertiesIds { get; set; }
}