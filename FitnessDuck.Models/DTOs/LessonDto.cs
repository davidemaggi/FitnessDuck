using System.Runtime.CompilerServices;

namespace FitnessDuck.Models.DTOs;

public class LessonDto
{
    public Guid Id { get; set; }
    public Guid? ScheduleId { get; set; }
    public DateTime StartDateUtc { get; set; }
    public DateTime EndDateUtc { get; set; }
    public ScheduleDto? Schedule { get; set; }
    public string? Name { get; set; }

    
    public int Seats { get; set; }
    


    public ICollection<BookingDto> Bookings { get; set; } = new List<BookingDto>();


    public bool IsUserRegistered(Guid userId)=>Bookings.Any(b => b.UserId == userId && b.Status!=BookingStatus.Deleted);
   
    
    public int DurationMinutes()=>(EndDateUtc - StartDateUtc).Minutes;
    
    
    public int AvailableSeats()=>Seats-Bookings.Count;

    public LessonDto()
    {
       
   

    }


}