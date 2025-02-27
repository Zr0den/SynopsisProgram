using CatalogService.DB;
using Microsoft.EntityFrameworkCore;

var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

if (environment == "Test")
{
    builder.Services.AddDbContext<CatalogDbContext>(options =>
        options.UseInMemoryDatabase("CatalogTestDb")); // Use in-memory database for tests
}
else
{
    builder.Services.AddDbContext<CatalogDbContext>(options =>
        options.UseMySql(builder.Configuration.GetConnectionString("CatalogDatabase"),
            new MySqlServerVersion(new Version(8, 0, 29)),
            options => options.EnableRetryOnFailure()));
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

var port = builder.Configuration.GetValue<string>("PORT") ?? "6000";
app.Urls.Add($"http://*:{port}");

app.Run();
