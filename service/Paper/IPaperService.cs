using infrastructure.QuerryModels;

namespace service.Paper;

public interface IPaperService
{
    public IEnumerable<PaperToDisplay> GetPaperWithQuerries(int pageNumber, int itemsPerPage);
    IEnumerable<PaperProperties> GetPaperProprieties();

    Task<bool> EditPaper(PaperToDisplay paperToDisplay);

    Task<PaperProperties> CreatePaperProperty(string propertyName);


    Task<PaperProperties> EditPaperProperty(PaperProperties paperProperties);
    Task<bool> DeletePaperProperty(int propertyId, string propertyName);


    Task<bool> ArePaperObjectsEqual(int requestId);
    Task<IEnumerable<PaperProperties>> GetPaperById(int paperId);
    Task<bool> DeletePaperById(int paperId);
    Task<PriceRange> GetPriceRange();
    Task<IEnumerable<PaperToDisplay>> GetPapersByFilter(PaperFilterQuery filterPapers);
    bool PaperExistsAsync(int paperId);
    bool PropertyExistsAsync(int propertyId);
    Task<bool> RemovePropertyFromPaper(int paperId, int propertyId);
    Task<bool> AddPropertyToPaper(int paperId, int propertyId);
    public Task<PaperToDisplay> CreatePaperProduct(PaperToAdd paperToAdd);
}