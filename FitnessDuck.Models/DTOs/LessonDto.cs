using System.Runtime.CompilerServices;

namespace FitnessDuck.Models.DTOs;

public class LessonDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public Guid? ScheduleId { get; set; }
    public DateTime StartDateUtc { get; set; }
    public DateTime EndDateUtc { get; set; }
    public ScheduleDto? Schedule { get; set; }
    public Guid? TrainerId { get; set; }
    public UserInfoDto? Trainer { get; set; }
    public int MinUnsubscribeHours { get; set; } = 24;
    public int AdvanceBookingDays { get; set; } = 90;
    public bool Manual { get; set; } = false;
    
    public int Seats { get; set; }
    


    public ICollection<BookingDto> Bookings { get; set; } = new List<BookingDto>();


    public bool IsUserRegistered(Guid userId)=>Bookings.Any(b => b.UserId == userId && b.Status!=BookingStatus.Deleted);
   
    
    public int DurationMinutes()=>(int)(EndDateUtc - StartDateUtc).TotalMinutes;
    
    
    public int AvailableSeats()=>Seats-Bookings.Count(b=>b.Status==BookingStatus.Confirmed);

    public LessonDto()
    {
       
   

    }


}