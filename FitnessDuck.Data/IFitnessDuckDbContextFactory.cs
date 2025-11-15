namespace FitnessDuck.Data;

public interface IFitnessDuckDbContextFactory
{
    FitnessDuckDbContext CreateDbContext(string? provider=null);
}