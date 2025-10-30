namespace FitnessDuck.Data.Entities;

public class OtpCacheEntity
{
    public int Id { get; set; }
    public string Contact { get; set; } = "";
    public string Method { get; set; } = "";
    public string HashedCode { get; set; } = "";
    public DateTime Expiry { get; set; }
}