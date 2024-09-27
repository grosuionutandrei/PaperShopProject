
using infrastructure.QuerryModels;
using infrastructure.Repository;
using service.TransferModels.Request;

namespace service.Paper;

public class PaperService :IPaperService
{
    private readonly IRepository _repository;

    public PaperService(IRepository repository)
    {
        _repository = repository;
    }


    public IEnumerable<PaperToDisplay> GetPaperWithQuerries(int pageNumber, string searchTerm, int pageItems,
        string orderBy, string filter, int paperPropertyId)
    {
        return _repository.GetPaperWithQuerries(pageNumber,searchTerm,pageItems,orderBy,filter,paperPropertyId);
    }

    public IEnumerable<PaperProperties> GetPaperProprieties()
    {
        return _repository.GetPaperProprieties();
    }

    public Task<PaperToDisplay> EditPaper(PaperToEditDto paperToEditDto)
    {
        return 
    }

    public Task<PaperProperties> CreatePaperProperty(string propertyName)
    {
        return _repository.CreatePaperProperty(propertyName);
    }

    public Task<PaperProperties> EditPaperProperty(int propertyId, string? propName)
    {
        return _repository.EditPaperProperty(propertyId,propName);
    }

    public Task<bool> DeletePaperProperty(int propertyId,string propertyName)
    {
        return _repository.DeletePaperProperty(propertyId,propertyName);
    }
}
