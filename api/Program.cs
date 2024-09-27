using System.Text.Json;
using api.Middleware;
using api.Validators;
using FluentValidation;
using FluentValidation.AspNetCore;
using infrastructure;
using infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using service.Paper;


var builder = WebApplication.CreateBuilder(args);

// Configure AppOptions with validation
builder.Services.AddOptionsWithValidateOnStart<AppOptions>()
    .Bind(builder.Configuration.GetSection(nameof(AppOptions)))
    .ValidateDataAnnotations()
    .Validate(options => new AppOptionsValidator().Validate(options).IsValid,
        $"{nameof(AppOptions)} validation failed");

// Configure DbContext
builder.Services.AddDbContext<DataBaseContext>((serviceProvider, options) =>
{
    var appOptions = serviceProvider.GetRequiredService<IOptions<AppOptions>>().Value;
    options.UseNpgsql(Environment.GetEnvironmentVariable("DbConnectionString") 
                      ?? appOptions.DbConnectionString);
    options.EnableSensitiveDataLogging();
});


// Add repositories and services
builder.Services.AddScoped<IRepository, PaperRepository>();
builder.Services.AddScoped<IPaperService, PaperService>();


builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<AppOptionsValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreatePropertyValidator>();

// Add services for Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS setup (register CORS service)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowOrigin",
        builder => builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());
});
var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

// Retrieve and log AppOptions
var options = app.Services.GetRequiredService<IOptions<AppOptions>>().Value;
Console.WriteLine("APP OPTIONS: " + JsonSerializer.Serialize(options));


app.MapControllers();

// Use custom middleware
app.UseMiddleware<Middleware>();

// Run the application
app.Run();
