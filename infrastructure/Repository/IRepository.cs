
using infrastructure.QuerryModels;
namespace infrastructure.Repository;

public interface IRepository
{
    IEnumerable<PaperProperties> GetPaperProprieties();

    public IEnumerable<PaperToDisplay> GetPaperWithQuerries(int pageNumber, string searchTerm, int pageItems,
        string orderBy, string filter, int paperPropertyId);

  Task<PaperProperties> CreatePaperProperty(string propertyName);
  Task<PaperProperties> EditPaperProperty(int propertyId, string? propName);
  Task<bool> DeletePaperProperty(int propertyId,string propertyName);
}