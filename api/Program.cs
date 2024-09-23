using api.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//MIDDLEWARE
builder.Services.AddCors();

//SETUP OTHER SERVICES
builder.Services.AddControllers();

if (builder.Environment.IsDevelopment())
{
    //FOR SWAGGER / OPENAPI IN DEV MODE
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
}

app.UseSession();

app.UseCors(options =>
{
    options.SetIsOriginAllowed(origin => true)
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
});


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.UseMiddleware<Middleware>();
app.Run();

