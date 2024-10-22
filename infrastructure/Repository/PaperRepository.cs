using infrastructure.Models;
using infrastructure.QuerryModels;
using Microsoft.EntityFrameworkCore;

namespace infrastructure.Repository;

public class PaperRepository : IRepository
{
    private readonly DataBaseContext _dataBaseContext;

    public PaperRepository(DataBaseContext db)
    {
        _dataBaseContext = db;
    }

    public async Task<IEnumerable<PaperToDisplay>> GetPapersByFilter(PaperFilterQuery filterPapers)
    {
        var query = _dataBaseContext.Papers.AsQueryable();
        if (filterPapers.pageNumber > 0) query = query.Skip(filterPapers.pageNumber * filterPapers.pageItems);

        if (!string.IsNullOrEmpty(filterPapers.searchFilter))
        {
            var search = filterPapers.searchFilter.TrimStart().TrimEnd();
            query = query.Where(p => p.Name.ToLower().Contains(search.ToLower()));
        }

        if (filterPapers.priceRange != null)
        {
            if (filterPapers.priceRange.minimumRange.HasValue)
                query = query.Where(p => p.Price >= filterPapers.priceRange.minimumRange.Value);

            if (filterPapers.priceRange.maximumRange.HasValue)
                query = query.Where(p => p.Price <= filterPapers.priceRange.maximumRange.Value);
        }

        if (filterPapers.paperPropertiesIds != null && filterPapers.paperPropertiesIds.Any())
            query = query.Where(p => p.Properties.Any(prop => filterPapers.paperPropertiesIds.Contains(prop.Id)));

        query = query.Where(p => p.Discontinued != true);
        var result = await query
            .Take(filterPapers.pageItems)
            .Select(e => new PaperToDisplay
            {
                Id = e.Id,
                Discontinued = e.Discontinued,
                Name = e.Name,
                Price = e.Price,
                Stock = e.Stock
            }).ToListAsync();
        Console.WriteLine(result);
        return result;
    }


    public IEnumerable<PaperToDisplay> GetPaperWithQuerries(int pageNumber, int pageItems)
    {
        var query = _dataBaseContext.Papers.AsQueryable();

        if (pageNumber > 0) query = query.Skip(pageNumber * pageItems);


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
        var newProperty = new Property { PropertyName = propertyName };
        _dataBaseContext.Properties.Add(newProperty);
        await _dataBaseContext.SaveChangesAsync();
        return new PaperProperties { PropId = newProperty.Id, PropName = newProperty.PropertyName };
    }

    public async Task<PaperToDisplay> CreatePaperProduct(PaperToAdd paperToAdd)
    {
        var newPaper = new Paper
        {
            Name = paperToAdd.Name,
            Discontinued = paperToAdd.Discontinued,
            Stock = paperToAdd.Stock,
            Price = paperToAdd.Price,
            Properties = paperToAdd.PaperPropertiesList?.Select(p => new Property
            {
                Id = p.PropId,
                PropertyName = p.PropName
            }).ToList() ?? new List<Property>()
        };

        _dataBaseContext.Papers.Add(newPaper);
        await _dataBaseContext.SaveChangesAsync();


        var paperToDisplay = new PaperToDisplay
        {
            Id = newPaper.Id,
            Name = newPaper.Name,
            Discontinued = newPaper.Discontinued,
            Stock = newPaper.Stock,
            Price = newPaper.Price
        };

        paperToDisplay.IncludeProperties(newPaper.Properties.Select(p => new PaperProperties
        {
            PropId = p.Id,
            PropName = p.PropertyName
        }));

        return paperToDisplay;
    }


    //EDIT PAPER PROPERTY
    public async Task<PaperProperties> EditPaperProperty(PaperProperties paperProperties)
    {
        var property = await _dataBaseContext.Properties.FindAsync(paperProperties.PropId);
        if (property == null) throw new KeyNotFoundException("Property not found");

        property.PropertyName = paperProperties.PropName!;
        _dataBaseContext.Properties.Update(property);
        await _dataBaseContext.SaveChangesAsync();

        return new PaperProperties
        {
            PropId = property.Id,
            PropName = property.PropertyName
        };
    }


    public bool PaperExistsAsync(int paperId)
    {
        return _dataBaseContext.Papers.Any(p => p.Id == paperId);
    }

    public bool PropertyExistsAsync(int propertyId)
    {
        return _dataBaseContext.Properties.Any(p => p.Id == propertyId);
    }

    public async Task<bool> RemovePropertyFromPaper(int paperId, int propertyId)
    {
        // Use FirstOrDefault to avoid exceptions if paper is not found
        var paper = _dataBaseContext.Papers
            .Include(p => p.Properties) // Make sure properties are included
            .FirstOrDefault(p => p.Id == paperId);

        if (paper == null) return false; // Paper not found

        // Find the property in the paper's properties
        var propertyToRemove = paper.Properties.FirstOrDefault(p => p.Id == propertyId);
        if (propertyToRemove == null) return false; // Property not found

        // Remove the property from the paper
        paper.Properties.Remove(propertyToRemove);

        // Save changes to the database
        await _dataBaseContext.SaveChangesAsync();

        return true;
    }

    public async Task<bool> AddPropertyToPaper(int paperId, int propertyId)
    {
        var paperToEdit = await _dataBaseContext.Papers
            .Include(p => p.Properties)
            .FirstOrDefaultAsync(p => p.Id == paperId);

        if (paperToEdit == null) return false;

        var property = await _dataBaseContext.Properties.FindAsync(propertyId);
        if (property == null) return false;

        if (paperToEdit.Properties.Any(p => p.Id == propertyId)) return false;

        paperToEdit.Properties.Add(property);
        await _dataBaseContext.SaveChangesAsync();
        return true;
    }


    public async Task<bool> EditPaper(PaperToDisplay paperToBeEdited)
    {
        var paperToEdit = await _dataBaseContext.Papers.FindAsync(paperToBeEdited.Id);
        if (paperToEdit == null) return false;

        paperToEdit.Name = paperToBeEdited.Name;
        paperToEdit.Discontinued = paperToBeEdited.Discontinued;
        paperToEdit.Stock = paperToBeEdited.Stock;
        paperToEdit.Price = paperToBeEdited.Price;
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
            .Select(p => new PaperProperties
            {
                PropId = p.Id,
                PropName = p.PropertyName
            }).ToListAsync();
        return paperProperties;
    }

    public async Task<bool> DeletePaperById(int paperId)
    {
        var requestPaper = await _dataBaseContext.Papers.Where(e => e.Id == paperId).FirstOrDefaultAsync();
        if (requestPaper == null) return false;

        _dataBaseContext.Papers.Remove(requestPaper);
        await _dataBaseContext.SaveChangesAsync();
        return true;
    }

    public async Task<PriceRange> GetPriceRange()
    {
        var priceMin = await _dataBaseContext.Papers.MinAsync(p => p.Price);
        var priceMax = await _dataBaseContext.Papers.MaxAsync(p => p.Price);
        return new PriceRange { minimumRange = priceMin, maximumRange = priceMax };
    }


    public async Task<bool> DeletePaperProperty(int propertyId, string propertyName)
    {
        var property = await _dataBaseContext.Properties.FindAsync(propertyId);
        if (property == null) return false;
        var papers = await _dataBaseContext.Papers
            .Include(p => p.Properties)
            .Where(p => p.Properties.Any(prop => prop.Id == propertyId))
            .ToListAsync();
        foreach (var paper in papers)
        {
            paper.Properties.Remove(property);
        }
        
        _dataBaseContext.Properties.Remove(property);
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