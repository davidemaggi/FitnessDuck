using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace FitnessDuck.Data;

public class FitnessDuckDbContextFactory : IFitnessDuckDbContextFactory,IDesignTimeDbContextFactory<FitnessDuckDbContext>
{
    private readonly IConfiguration _configuration;

    public FitnessDuckDbContextFactory(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public FitnessDuckDbContextFactory()
    {
        
        
        
        
        _configuration= new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();
      
    }
    
    public FitnessDuckDbContext CreateDbContext(string? provider = null)
    {
        if (!string.IsNullOrWhiteSpace(_configuration["DB_PROVIDER"]))
            provider = _configuration["DB_PROVIDER"];

        provider ??= "sqlite";
        var connString = _configuration.GetConnectionString(provider);
        
        var optionsBuilder = new DbContextOptionsBuilder<FitnessDuckDbContext>();

        if (provider.Equals("postgres", StringComparison.OrdinalIgnoreCase))
            optionsBuilder.UseNpgsql(connString);
        else
            optionsBuilder.UseSqlite(connString);

        return new FitnessDuckDbContext(optionsBuilder.Options);
    }
    
    
    public FitnessDuckDbContext CreateDbContext(string[] args)
    {
        
        
        
        var optionsBuilder = new DbContextOptionsBuilder<FitnessDuckDbContext>();

        // Read provider from environment variable or defaults
        var provider = Environment.GetEnvironmentVariable("DB_PROVIDER") ?? "sqlite";

        // Set connection string
        string connString;
        if (provider.Equals("postgres", StringComparison.OrdinalIgnoreCase))
        {
            connString = "Host=localhost;Database=FitnessDuck;Username=postgres;Password=password";
            optionsBuilder.UseNpgsql(connString);
        }
        else
        {
            connString = "Data Source=FitnessDuck.db";
            optionsBuilder.UseSqlite(connString);
        }

        return new FitnessDuckDbContext(optionsBuilder.Options);
    }
    
    
    
}