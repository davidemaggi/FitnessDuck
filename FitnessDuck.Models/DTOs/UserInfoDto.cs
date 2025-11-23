namespace FitnessDuck.Models.DTOs;

public class UserInfoDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public string Local { get; set; } = "it-IT";
    
    
    
    
    public string GetFullName()=> $"{Name} {Surname}";
    
}