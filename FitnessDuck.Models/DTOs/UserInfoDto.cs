namespace FitnessDuck.Models.DTOs;

public class UserInfoDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public string Local { get; set; } = "it-IT";
    
    public UserPlan Plan { get; set; } = UserPlan.NoPlan;
    public int PlanAmount { get; set; } = 1;
    public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
    public DateTime? PlanExpiration { get; set; }

    
    public string GetFullName()=> $"{Name} {Surname}";
    
}