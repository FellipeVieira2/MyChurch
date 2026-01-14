using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace MyChurch.Infrastructure;

public sealed class MyChurchDbContextFactory : IDesignTimeDbContextFactory<MyChurchDbContext>
{
    public MyChurchDbContext CreateDbContext(string[] args)
    {
        var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
            ?? Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")
            ?? "Development";

        // Quando rodado via `dotnet ef`, o diretório atual pode ser a raiz do repo.
        // Tentamos carregar primeiro o appsettings do projeto Web (onde está a connection string real).
        var currentDir = Directory.GetCurrentDirectory();
        var webProjectBasePath = Path.GetFullPath(Path.Combine(currentDir, "Web", "MyChurch.Api.Web"));
        var infraProjectBasePath = Path.GetFullPath(Path.Combine(currentDir, "Infrastructure", "MyChurch.Infrastructure"));

        var configuration = new ConfigurationBuilder()
            .SetBasePath(currentDir)
            .AddJsonFile(Path.Combine(webProjectBasePath, "appsettings.json"), optional: true)
            .AddJsonFile(Path.Combine(webProjectBasePath, $"appsettings.{environmentName}.json"), optional: true)
            .AddJsonFile(Path.Combine(infraProjectBasePath, "appsettings.json"), optional: true)
            .AddJsonFile(Path.Combine(infraProjectBasePath, $"appsettings.{environmentName}.json"), optional: true)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile($"appsettings.{environmentName}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString =
            configuration.GetConnectionString("MyChurchDb")
            ?? configuration["ConnectionStrings:MyChurchDb"]
            ?? configuration["DATABASE_CONNECTION_STRING"]
            ?? configuration.GetConnectionString("DefaultConnection")
            ?? configuration.GetConnectionString("Default")
            ?? configuration["ConnectionStrings:DefaultConnection"];

        // Design-time: permitir gerar migrations mesmo sem banco configurado.
        // (A conexão só é necessária ao aplicar migrations.)
        if (string.IsNullOrWhiteSpace(connectionString))
            connectionString = "Host=localhost;Port=5432;Database=mychurch_design;Username=postgres;Password=postgres";

        var optionsBuilder = new DbContextOptionsBuilder<MyChurchDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        optionsBuilder.ConfigureWarnings(w =>
            w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));

        return new MyChurchDbContext(optionsBuilder.Options);
    }
}
