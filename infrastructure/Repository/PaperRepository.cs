
using infrastructure.Models;
using infrastructure.QuerryModels;
using Microsoft.EntityFrameworkCore;

namespace infrastructure.Repository;

public class PaperRepository: IRepository
{
    private readonly DataBaseContext _dataBaseContext;

    public PaperRepository(DataBaseContext db)
    {
        _dataBaseContext = db;
    }


    public IEnumerable<PaperToDisplay>  GetPaper()
    {
        return _dataBaseContext.Papers.Select(e =>new PaperToDisplay
        {
         Id   = e.Id,
         Discontinued = e.Discontinued,
         Name=e.Name,
         Price=e.Price,
         Stock = e.Stock
        }).ToList();
    }

    public IEnumerable<PaperToDisplay> GetPaperWithQuerries(int pageNumber, int pageItems)
    {
        var query = _dataBaseContext.Papers.AsQueryable();

        // // Apply search term filter
        // if (!string.IsNullOrEmpty(searchTerm))
        // {
        //     query = query.Where(e => e.Name.ToLower().Contains(searchTerm.ToLower()));
        // }
        //
        // // Apply paper property filter
        // if (paperPropertyId != 0)
        // {
        //     query = query.Where(e => e.Properties.Any(p => p.Id == paperPropertyId));
        // }
        //
        // // Apply pagination
        if (pageNumber > 0)
        {
            query = query.Skip(pageNumber * pageItems);
        }
        //
        // // Apply ordering
        // switch (orderBy.ToLower())
        // {
        //     case "name":
        //         query = query.OrderBy(e => e.Name);
        //         break;
        //     case "price":
        //         query = query.OrderBy(e => e.Price);
        //         break;
        //     case "stock":
        //         query = query.OrderBy(e => e.Stock);
        //         break;
        //     default:
        //         query = query.OrderBy(e => e.Id);
        //         break;
        // }
        //
        // // Apply filter
        // if (!string.IsNullOrEmpty(filter))
        // {
        //     // Add filter logic here if needed
        // }

        return query
            .Take(pageItems)
            .Select(e => new PaperToDisplay
            {
                Id = e.Id,
                Discontinued = e.Discontinued,
                Name = e.Name,
                Price = e.Price,
                Stock = e.Stock
            }).ToList();
    }

    public async Task<PaperProperties> CreatePaperProperty(string propertyName)
    {
        var newProperty = new Property{ PropertyName = propertyName};
        _dataBaseContext.Properties.Add(newProperty);
        await _dataBaseContext.SaveChangesAsync();
        return new PaperProperties{PropId = newProperty.Id,PropName = newProperty.PropertyName};
    }

    //EDIT PAPER PROPERTY
    public async Task<PaperProperties> EditPaperProperty(PaperProperties paperProperties)
    {
        var property = await _dataBaseContext.Properties.FindAsync(paperProperties.PropId);
        if (property == null)
        {
            throw new KeyNotFoundException("Property not found");
        }

        property.PropertyName = paperProperties.PropName!;
        _dataBaseContext.Properties.Update(property);
        await _dataBaseContext.SaveChangesAsync();

        return new PaperProperties
        {
            PropId = property.Id,
            PropName = property.PropertyName
        };
    }

    public async Task<bool> EditPaper(PaperToDisplay paperToDisplay)
    {
        var paperToEdit = await _dataBaseContext.Papers.FindAsync(paperToDisplay.Id);

        if (paperToEdit == null)
        {
            return false;
        }
        
        paperToEdit.Name = paperToDisplay.Name;
        paperToDisplay.Discontinued = paperToDisplay.Discontinued;
        paperToDisplay.Stock = paperToDisplay.Stock;
        paperToDisplay.Price = paperToDisplay.Price;
        _dataBaseContext.Papers.Update(paperToEdit);
       await _dataBaseContext.SaveChangesAsync();
       return true;
    }

    public async Task<bool> ArePaperObjectsEqual(int requestId)
    {
        var paper = await _dataBaseContext.Papers
            .Where(e => e.Id == requestId)
            .FirstOrDefaultAsync();

        return paper != null;
    }

    public async Task<IEnumerable<PaperProperties>> GetPaperById(int paperId)
    { 
        
        
        var paperProperties = await _dataBaseContext.Papers
            .Where(e => e.Id == paperId)
            .SelectMany(p => p.Properties)
            .Select(p => new PaperProperties()
            {
                PropId = p.Id,
                PropName = p.PropertyName
            }).ToListAsync();
        return paperProperties;
    }

    public async Task<bool> DeletePaperById(int paperId)
    {
        var requestPaper = await _dataBaseContext.Papers.Where(e => e.Id==paperId).FirstOrDefaultAsync();
        if (requestPaper==null)
        {
            return false;
          
        }

        _dataBaseContext.Papers.Remove(requestPaper);
        await _dataBaseContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeletePaperProperty(int propertyId,string propertyName)
    {
        var property = _dataBaseContext.Properties.FindAsync(propertyId);
        if (property.Result == null)
        {
            return false;
        }
        _dataBaseContext.Properties.Remove(property.Result!);
        await _dataBaseContext.SaveChangesAsync();
        return true;
    }

    public IEnumerable<PaperProperties> GetPaperProprieties()
    {
        return _dataBaseContext.Properties.Select(p => new PaperProperties
        {
            PropId = p.Id,
            PropName = p.PropertyName

        }).ToList();
    }
}

