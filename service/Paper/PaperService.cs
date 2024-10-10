
using infrastructure.QuerryModels;
using infrastructure.Repository;


namespace service.Paper;

public class PaperService :IPaperService
{
    private readonly IRepository _repository;

    public PaperService(IRepository repository)
    {
        _repository = repository;
    }


    public IEnumerable<PaperToDisplay> GetPaperWithQuerries(int pageNumber,int pageItems)
    {
        return _repository.GetPaperWithQuerries(pageNumber,pageItems);
    }

    public IEnumerable<PaperProperties> GetPaperProprieties()
    {
        return _repository.GetPaperProprieties();
    }

    public  Task<bool> EditPaper(PaperToDisplay paperToDisplay)
    {
        return _repository.EditPaper(paperToDisplay);
    }



    public Task<PaperProperties> CreatePaperProperty(string propertyName)
    {
        return _repository.CreatePaperProperty(propertyName);
    }

    public Task<PaperProperties> EditPaperProperty(PaperProperties paperProperties)
    {
        return _repository.EditPaperProperty(paperProperties);
    }

    public Task<bool> DeletePaperProperty(int propertyId,string propertyName)
    {
        return _repository.DeletePaperProperty(propertyId,propertyName);
    }

    public Task<bool> ArePaperObjectsEqual(int requestId)
    {
        return _repository.ArePaperObjectsEqual(requestId);
    }

    public Task<IEnumerable<PaperProperties>> GetPaperById(int paperId)
    {
        return _repository.GetPaperById(paperId);
    }

    public Task<bool> DeletePaperById(int paperId)
    {
        return _repository.DeletePaperById(paperId);
    }

    public Task<PriceRange> GetPriceRange()
    {
        return _repository.GetPriceRange();
    }

    public Task<IEnumerable<PaperToDisplay>> GetPapersByFilter(PaperFilterQuery filterPapers)
    {
        return _repository.GetPapersByFilter(filterPapers);
    }

    public bool PaperExistsAsync(int paperId)
    {
        return _repository.PaperExistsAsync(paperId);
    }

    public bool PropertyExistsAsync(int propertyId)
    {
        return _repository.PropertyExistsAsync(propertyId);
    }

    public Task<bool> RemovePropertyFromPaper(int paperId, int propertyId)
    {
        return _repository.RemovePropertyFromPaper(paperId,propertyId);
    }

    public Task<bool> AddPropertyToPaper(int paperId, int propertyId)
    {
        return _repository.AddPropertyToPaper(paperId, propertyId);
    }

    public Task<PaperToDisplay> CreatePaperProduct(PaperToAdd paperToAdd)
    {
        return _repository.CreatePaperProduct(paperToAdd);
    }
}
