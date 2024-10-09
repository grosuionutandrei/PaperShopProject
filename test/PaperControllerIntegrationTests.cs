using api.Controllers;
using infrastructure;
using infrastructure.Models;
using infrastructure.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using service.Paper;


namespace test;

public class PaperControllerIntegrationTests
{
    private readonly PaperController _controller;
    private readonly PaperService _paperService;
    private readonly Mock<IRepository> _mockRepository;
    private readonly DbContextOptions<DataBaseContext> _dbOptions;
    private readonly DataBaseContext _dbContext;

    public PaperControllerIntegrationTests()
    {
        // Setup in-memory database
        _dbOptions = new DbContextOptionsBuilder<DataBaseContext>()
            .UseInMemoryDatabase(databaseName: "PaperTestDb")
            .Options;
        _dbContext = new DataBaseContext(_dbOptions);
        
        _mockRepository = new Mock<IRepository>();

        SeedDatabase(_dbContext);
        _paperService = new PaperService(_mockRepository.Object);

        var mockLogger = new Mock<ILogger<PaperController>>();
        _controller = new PaperController(_paperService, mockLogger.Object);
    }

    private void SeedDatabase(DataBaseContext context)
    {
        var papers = new List<Paper>
        {
            new Paper { Id = 9, Name = "Paper A", Price = 90.0, Stock = 100, Discontinued = false },
            new Paper { Id = 8, Name = "Paper B", Price = 19.0, Stock = 200, Discontinued = true }
        };

        context.Papers.AddRange(papers);
        context.SaveChanges();
    }
    [Fact]
    public void GetProprieties_ShouldReturnOk_WhenPropertiesExist()
    {
        // Arrange
        _mockRepository.Setup(repo => repo.GetPaperProprieties()).Returns(new List<PaperProperties>
        {
            new PaperProperties { PropId = 1, PropName = "Glossy" },
            new PaperProperties { PropId = 2, PropName = "Matte" }
        });

        // Act
        var result = _controller.GetProprieties();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var properties = Assert.IsAssignableFrom<IEnumerable<PaperProperties>>(okResult.Value);
        Assert.Equal(2, properties.Count());
    }
}