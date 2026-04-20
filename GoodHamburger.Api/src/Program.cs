using GoodHamburger.Application.Src;
using GoodHamburger.Infra.Src.Db;

var isDevelopment = string.Equals(
    Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
    Environments.Development,
    StringComparison.OrdinalIgnoreCase);

if (isDevelopment)
{
    var envPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", ".env"));

    if (File.Exists(envPath))
    {
        DotNetEnv.Env.Load(envPath);
    }
}

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddApplicationDependencies();
builder.Services.AddInfraDependencies(builder.Configuration);
builder.Services.AddHybridCache();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers();

app.UseHttpsRedirection();

app.Services.InitializeInfra();

app.Run();
