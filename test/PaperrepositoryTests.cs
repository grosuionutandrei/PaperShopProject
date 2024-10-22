using infrastructure;
using infrastructure.Models;
using infrastructure.QuerryModels;
using infrastructure.Repository;
using Microsoft.EntityFrameworkCore;

namespace test;

public class PaperRepositoryTests : IDisposable
{
    private readonly DataBaseContext _dbContext;
    private readonly PaperRepository _paperRepository;

    public PaperRepositoryTests()
    {
        // Create an in-memory database for testing
        var options = new DbContextOptionsBuilder<DataBaseContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new DataBaseContext(options);
        _paperRepository = new PaperRepository(_dbContext);
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }

    [Fact]
    public async Task GetPapersByFilter_ReturnsPapersWithCorrectFilters()
    {
        // Arrange
        var papers = new List<Paper>
        {
            new()
            {
                Id = 1,
                Name = "Paper A",
                Price = 10,
                Stock = 100,
                Discontinued = false,
                Properties = new List<Property>
                {
                    new() { Id = 1, PropertyName = "Glossy" }
                }
            },
            new()
            {
                Id = 2,
                Name = "Paper B",
                Price = 20,
                Stock = 200,
                Discontinued = false,
                Properties = new List<Property>
                {
                    new() { Id = 2, PropertyName = "Matte" }
                }
            },
            new()
            {
                Id = 3,
                Name = "Paper C",
                Price = 30,
                Stock = 300,
                Discontinued = true,
                Properties = new List<Property>
                {
                    new() { Id = 3, PropertyName = "Recycled" }
                }
            }
        };

        _dbContext.Papers.AddRange(papers);
        _dbContext.SaveChanges();

        var filterQuery = new PaperFilterQuery
        {
            pageNumber = 0,
            pageItems = 10,
            searchFilter = "Paper",
            priceRange = new PriceRange { minimumRange = 10, maximumRange = 30 },
            paperPropertiesIds = new List<int> { 1, 2 }
        };

        // Act
        var result = await _paperRepository.GetPapersByFilter(filterQuery);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Contains(result, p => p.Name == "Paper A");
        Assert.Contains(result, p => p.Name == "Paper B");
    }

    [Fact]
    public async Task GetPapersByFilter_ReturnsPapersWithinPriceRange()
    {
        // Arrange
        var papers = new List<Paper>
        {
            new()
            {
                Id = 1,
                Name = "Paper A",
                Price = 15,
                Stock = 50,
                Discontinued = false,
                Properties = new List<Property>
                {
                    new() { Id = 1, PropertyName = "Glossy" }
                }
            },
            new()
            {
                Id = 2,
                Name = "Paper B",
                Price = 25,
                Stock = 60,
                Discontinued = false,
                Properties = new List<Property>
                {
                    new() { Id = 2, PropertyName = "Matte" }
                }
            },
            new()
            {
                Id = 3,
                Name = "Paper C",
                Price = 35,
                Stock = 70,
                Discontinued = true,
                Properties = new List<Property>
                {
                    new() { Id = 3, PropertyName = "Recycled" }
                }
            }
        };

        _dbContext.Papers.AddRange(papers);
        _dbContext.SaveChanges();

        var filterQuery = new PaperFilterQuery
        {
            pageNumber = 0,
            pageItems = 10,
            priceRange = new PriceRange { minimumRange = 10, maximumRange = 30 }
        };

        // Act
        var result = await _paperRepository.GetPapersByFilter(filterQuery);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Contains(result, p => p.Name == "Paper A");
        Assert.Contains(result, p => p.Name == "Paper B");
        Assert.DoesNotContain(result, p => p.Name == "Paper C");
    }

    [Fact]
    public async Task GetPapersByFilter_FiltersByPropertyIdsOnly()
    {
        // Arrange
        var papers = new List<Paper>
        {
            new()
            {
                Id = 1,
                Name = "Paper A",
                Price = 10,
                Stock = 100,
                Discontinued = false,
                Properties = new List<Property>
                {
                    new() { Id = 1, PropertyName = "Glossy" }
                }
            },
            new()
            {
                Id = 2,
                Name = "Paper B",
                Price = 20,
                Stock = 200,
                Discontinued = false,
                Properties = new List<Property>
                {
                    new() { Id = 2, PropertyName = "Matte" }
                }
            },
            new()
            {
                Id = 3,
                Name = "Paper C",
                Price = 30,
                Stock = 300,
                Discontinued = true,
                Properties = new List<Property>
                {
                    new() { Id = 3, PropertyName = "Recycled" }
                }
            }
        };

        _dbContext.Papers.AddRange(papers);
        _dbContext.SaveChanges();

        var filterQuery = new PaperFilterQuery
        {
            pageNumber = 0,
            pageItems = 10,
            paperPropertiesIds = new List<int> { 1, 3 }
        };

        // Act
        var result = await _paperRepository.GetPapersByFilter(filterQuery);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Contains(result, p => p.Name == "Paper A");
        Assert.Contains(result, p => p.Name == "Paper C");
        Assert.DoesNotContain(result, p => p.Name == "Paper B");
    }

    [Fact]
    public async Task GetPapersByFilter_ReturnsCorrectPageOfResults()
    {
        // Arrange
        var papers = new List<Paper>
        {
            new()
            {
                Id = 1, Name = "Paper A", Price = 10, Stock = 100, Discontinued = false,
                Properties = new List<Property> { new() { Id = 1, PropertyName = "Glossy" } }
            },
            new()
            {
                Id = 2, Name = "Paper B", Price = 20, Stock = 200, Discontinued = false,
                Properties = new List<Property> { new() { Id = 2, PropertyName = "Matte" } }
            },
            new()
            {
                Id = 3, Name = "Paper C", Price = 30, Stock = 300, Discontinued = true,
                Properties = new List<Property> { new() { Id = 3, PropertyName = "Recycled" } }
            },
            new()
            {
                Id = 4, Name = "Paper D", Price = 40, Stock = 400, Discontinued = true,
                Properties = new List<Property> { new() { Id = 4, PropertyName = "Textured" } }
            }
        };

        _dbContext.Papers.AddRange(papers);
        _dbContext.SaveChanges();

        var filterQuery = new PaperFilterQuery
        {
            pageNumber = 1,
            pageItems = 2
        };

        // Act
        var result = await _paperRepository.GetPapersByFilter(filterQuery);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Contains(result, p => p.Name == "Paper C");
        Assert.Contains(result, p => p.Name == "Paper D");
        Assert.DoesNotContain(result, p => p.Name == "Paper A");
        Assert.DoesNotContain(result, p => p.Name == "Paper B");
    }

    [Fact]
    public async Task GetPapersByFilter_ReturnsEmptyWhenNoMatches()
    {
        // Arrange
        var papers = new List<Paper>
        {
            new()
            {
                Id = 1, Name = "Paper A", Price = 10, Stock = 100, Discontinued = false,
                Properties = new List<Property> { new() { Id = 1, PropertyName = "Glossy" } }
            },
            new()
            {
                Id = 2, Name = "Paper B", Price = 20, Stock = 200, Discontinued = false,
                Properties = new List<Property> { new() { Id = 2, PropertyName = "Matte" } }
            },
            new()
            {
                Id = 3, Name = "Paper C", Price = 30, Stock = 300, Discontinued = true,
                Properties = new List<Property> { new() { Id = 3, PropertyName = "Recycled" } }
            }
        };

        _dbContext.Papers.AddRange(papers);
        _dbContext.SaveChanges();

        var filterQuery = new PaperFilterQuery
        {
            searchFilter = "Nonexistent Paper",
            priceRange = new PriceRange { minimumRange = 100, maximumRange = 200 },
            paperPropertiesIds = new List<int> { 99 }
        };

        // Act
        var result = await _paperRepository.GetPapersByFilter(filterQuery);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task CreatePaperProperty_ShouldCreateNewProperty()
    {
        // Arrange
        var propertyName = "Glossy";

        // Act
        var result = await _paperRepository.CreatePaperProperty(propertyName);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(propertyName, result.PropName);
        Assert.True(result.PropId > 0);
    }

    [Fact]
    public async Task CreatePaperProperty_ShouldSavePropertyToDatabase()
    {
        // Arrange
        var propertyName = "Matte";

        // Act
        var result = await _paperRepository.CreatePaperProperty(propertyName);

        // Assert
        var propertyInDb = await _dbContext.Properties.FindAsync(result.PropId);
        Assert.NotNull(propertyInDb);
        Assert.Equal(propertyName, propertyInDb.PropertyName);
    }

    [Fact]
    public async Task CreatePaperProperty_ShouldAllowDuplicateNames()
    {
        // Arrange
        var propertyName = "Recycled";

        await _paperRepository.CreatePaperProperty(propertyName);

        // Act
        var result = await _paperRepository.CreatePaperProperty(propertyName);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(propertyName, result.PropName);
        Assert.True(result.PropId > 0);
        var propertiesInDb = _dbContext.Properties.Where(p => p.PropertyName == propertyName).ToList();
        Assert.Equal(2, propertiesInDb.Count);
    }

    [Fact]
    public async Task EditPaperProperty_ShouldEditExistingProperty()
    {
        // Arrange
        var initialProperty = new Property { Id = 1, PropertyName = "Original Name" };
        _dbContext.Properties.Add(initialProperty);
        _dbContext.SaveChanges();

        var updatedPaperProperties = new PaperProperties
        {
            PropId = 1,
            PropName = "Updated Name"
        };

        // Act
        var result = await _paperRepository.EditPaperProperty(updatedPaperProperties);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(updatedPaperProperties.PropId, result.PropId);
        Assert.Equal(updatedPaperProperties.PropName, result.PropName);
    }

    [Fact]
    public async Task EditPaperProperty_ShouldThrowExceptionIfPropertyNotFound()
    {
        // Arrange
        var nonExistentPaperProperties = new PaperProperties
        {
            PropId = 5000,
            PropName = "Non-Existent Property"
        };

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _paperRepository.EditPaperProperty(nonExistentPaperProperties));
    }

    [Fact]
    public async Task EditPaperProperty_ShouldPersistChangesInDatabase()
    {
        // Arrange
        var initialProperty = new Property { Id = 2, PropertyName = "Old Name" };
        _dbContext.Properties.Add(initialProperty);
        _dbContext.SaveChanges();

        var updatedPaperProperties = new PaperProperties
        {
            PropId = 2,
            PropName = "New Name"
        };

        // Act
        var result = await _paperRepository.EditPaperProperty(updatedPaperProperties);

        // Assert
        var propertyInDb = await _dbContext.Properties.FindAsync(updatedPaperProperties.PropId);
        Assert.NotNull(propertyInDb);
        Assert.Equal(updatedPaperProperties.PropName, propertyInDb.PropertyName);
    }

    [Fact]
    public async Task ArePaperObjectsEqual_ShouldReturnTrue_WhenPaperExists()
    {
        // Arrange
        var paper = new Paper { Id = 1, Name = "Sample Paper", Price = 10, Stock = 100 };
        _dbContext.Papers.Add(paper);
        _dbContext.SaveChanges();

        // Act
        var result = await _paperRepository.ArePaperObjectsEqual(1);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ArePaperObjectsEqual_ShouldReturnFalse_WhenPaperDoesNotExist()
    {
        // Arrange
        var paper = new Paper { Id = 1, Name = "Sample Paper", Price = 10, Stock = 100 };
        _dbContext.Papers.Add(paper);
        _dbContext.SaveChanges();

        // Act
        var result = await _paperRepository.ArePaperObjectsEqual(99);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ArePaperObjectsEqual_ShouldReturnFalse_WhenDatabaseIsEmpty()
    {
        // Act
        var result = await _paperRepository.ArePaperObjectsEqual(1);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task GetPaperById_ShouldReturnProperties_WhenPaperExists()
    {
        // Arrange
        var paper = new Paper
        {
            Id = 1,
            Name = "Sample Paper",
            Properties = new List<Property>
            {
                new() { Id = 1, PropertyName = "Glossy" },
                new() { Id = 2, PropertyName = "Matte" }
            }
        };
        _dbContext.Papers.Add(paper);
        _dbContext.SaveChanges();

        // Act
        var result = await _paperRepository.GetPaperById(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Contains(result, p => p.PropName == "Glossy");
        Assert.Contains(result, p => p.PropName == "Matte");
    }

    [Fact]
    public async Task GetPaperById_ShouldReturnEmptyList_WhenPaperHasNoProperties()
    {
        // Arrange
        var paper = new Paper
        {
            Id = 2,
            Name = "Paper Without Properties",
            Properties = new List<Property>()
        };
        _dbContext.Papers.Add(paper);
        _dbContext.SaveChanges();

        // Act
        var result = await _paperRepository.GetPaperById(2);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task DeletePaperById_ShouldReturnTrue_WhenPaperExists()
    {
        // Arrange
        var paper = new Paper { Id = 1, Name = "Sample Paper", Price = 10, Stock = 100 };
        _dbContext.Papers.Add(paper);
        _dbContext.SaveChanges();

        // Act
        var result = await _paperRepository.DeletePaperById(1);

        // Assert
        Assert.True(result);

        var paperInDb = await _dbContext.Papers.FindAsync(1);
        Assert.Null(paperInDb);
    }

    [Fact]
    public async Task DeletePaperById_ShouldReturnFalse_WhenPaperDoesNotExist()
    {
        // Act
        var result = await _paperRepository.DeletePaperById(99);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task GetPriceRange_ShouldReturnCorrectPriceRange_WhenPapersExist()
    {
        // Arrange
        var papers = new List<Paper>
        {
            new() { Id = 1, Name = "Paper A", Price = 10, Stock = 100 },
            new() { Id = 2, Name = "Paper B", Price = 20, Stock = 200 },
            new() { Id = 3, Name = "Paper C", Price = 30, Stock = 300 }
        };
        _dbContext.Papers.AddRange(papers);
        _dbContext.SaveChanges();

        // Act
        var result = await _paperRepository.GetPriceRange();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10, result.minimumRange);
        Assert.Equal(30, result.maximumRange);
    }

    [Fact]
    public async Task DeletePaperProperty_ShouldReturnTrue_WhenPropertyExists()
    {
        // Arrange
        var property = new Property { Id = 1, PropertyName = "Glossy" };
        _dbContext.Properties.Add(property);
        _dbContext.SaveChanges();

        // Act
        var result = await _paperRepository.DeletePaperProperty(1, "Glossy");

        // Assert
        Assert.True(result);

        var propertyInDb = await _dbContext.Properties.FindAsync(1);
        Assert.Null(propertyInDb);
    }

    [Fact]
    public async Task DeletePaperProperty_ShouldReturnTrue_WhenPropertyIdExists_IgnoringName()
    {
        // Arrange
        var property = new Property { Id = 2, PropertyName = "Matte" };
        _dbContext.Properties.Add(property);
        _dbContext.SaveChanges();

        // Act
        var result = await _paperRepository.DeletePaperProperty(2, "Wrong");

        // Assert
        Assert.True(result);

        var propertyInDb = await _dbContext.Properties.FindAsync(2);
        Assert.Null(propertyInDb);
    }

    [Fact]
    public void GetPaperProprieties_ShouldReturnAllProperties_WhenPropertiesExist()
    {
        // Arrange
        var properties = new List<Property>
        {
            new() { Id = 1, PropertyName = "Glossy" },
            new() { Id = 2, PropertyName = "Matte" },
            new() { Id = 3, PropertyName = "Recycled" }
        };
        _dbContext.Properties.AddRange(properties);
        _dbContext.SaveChanges();

        // Act
        var result = _paperRepository.GetPaperProprieties();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count());
        Assert.Contains(result, p => p.PropName == "Glossy");
        Assert.Contains(result, p => p.PropName == "Matte");
        Assert.Contains(result, p => p.PropName == "Recycled");
    }
}