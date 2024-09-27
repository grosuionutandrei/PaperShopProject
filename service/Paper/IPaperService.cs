
using infrastructure.QuerryModels;
using service.TransferModels.Request;

namespace service.Paper;

public interface IPaperService
{
    public IEnumerable<PaperToDisplay> GetPaperWithQuerries(int pageNumber, string searchTerm, int pageItems,
        string orderBy, string filter, int paperPropertyId);
    IEnumerable<PaperProperties> GetPaperProprieties();

    Task<PaperToDisplay> EditPaper(PaperToEditDto paperToEditDto);

    Task<PaperProperties> CreatePaperProperty(string propertyName);


    Task<PaperProperties> EditPaperProperty(int propertyId, string? propName);
    Task <bool>DeletePaperProperty(int propertyId,string propertyName);
    
    
    
}