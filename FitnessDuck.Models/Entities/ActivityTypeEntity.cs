using FitnessDuck.Models;

namespace FitnessDuck.Data.Entities;

public class ActivityTypeEntity:BaseEntity
{
    public string Name { get; set; } = "";
    public string? Icon { get; set; }
   
}
