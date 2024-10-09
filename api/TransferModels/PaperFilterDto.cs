namespace api.TransferModels;



    public class PaperFilterDto
    {
        public PaperPaginationQueryDto? pagination { get; set; }
        public PriceRangeDto? priceRange { get; set; }
        public string? paperPropertiesIds { get; set; }
        public string? searchFilter { get; set; }
        
        public List<int> GetParsedPaperPropertiesIds()
        {
            return paperPropertiesIds?
                .Split(',')
                .Select(id => int.TryParse(id, out var result) ? result : (int?)null)
                .Where(id => id.HasValue)
                .Select(id => id.Value)
                .ToList() ?? new List<int>();
        }
    }
    

    
