using infrastructure.QuerryModels;

namespace infrastructure.Repository;

public interface IRepository
{
    IEnumerable<PaperProperties> GetPaperProprieties();

    public IEnumerable<PaperToDisplay> GetPaperWithQuerries(int pageNumber, int itemsPerPage);

    Task<PaperProperties> CreatePaperProperty(string propertyName);

    /*Task<PaperProperties> EditPaperProperty(int propertyId, string? propName);*/
    Task<bool> DeletePaperProperty(int propertyId, string propertyName);

    Task<PaperProperties> EditPaperProperty(PaperProperties paperProperties);

    Task<bool> EditPaper(PaperToDisplay paperToBeEdited);
    Task<bool> ArePaperObjectsEqual(int requestId);
    public Task<IEnumerable<PaperProperties>> GetPaperById(int paperId);
    Task<bool> DeletePaperById(int paperId);

    Task<PriceRange> GetPriceRange();
    Task<IEnumerable<PaperToDisplay>> GetPapersByFilter(PaperFilterQuery filterPapers);
    public bool PaperExistsAsync(int paperId);


    public bool PropertyExistsAsync(int propertyId);
    Task<bool> RemovePropertyFromPaper(int paperId, int propertyId);
    Task<bool> AddPropertyToPaper(int paperId, int propertyId);
}