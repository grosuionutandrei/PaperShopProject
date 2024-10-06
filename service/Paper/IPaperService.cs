
using infrastructure.QuerryModels;

namespace service.Paper;

public interface IPaperService
{
    public IEnumerable<PaperToDisplay> GetPaperWithQuerries(int pageNumber, string searchTerm, int pageItems,
        string orderBy, string filter, int paperPropertyId);
    IEnumerable<PaperProperties> GetPaperProprieties();

    Task<bool> EditPaper(PaperToDisplay paperToDisplay);

    Task<PaperProperties> CreatePaperProperty(string propertyName);


    Task<PaperProperties> EditPaperProperty(PaperProperties paperProperties);
    Task <bool>DeletePaperProperty(int propertyId,string propertyName);


    Task<bool> ArePaperObjectsEqual(int requestId);
    Task<PaperToDisplay> GetPaperById(int paperId);
    Task<bool> DeletePaperById(int paperId);
}