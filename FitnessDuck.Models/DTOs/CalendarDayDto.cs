namespace FitnessDuck.Models.DTOs;

public class CalendarDayDto
{
    public int Number { get; set; }
    public string Name { get; set; }
    public bool HasLesson { get; set; } = false;
    public bool HasReservation { get; set; } = false;
    
    public DateTime raw { get; set; }
    
    
    public  CalendarDayDto(DateTime dt)
    {
        Number = dt.Day;
        Name = dt.DayOfWeek.ToString();
        raw = dt;
        
    }
    
    public bool IsInThePast()=> raw<DateTime.Now;

}