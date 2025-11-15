namespace FitnessDuck.Models.Common;

public class DayPlan
{
    public DayOfWeek DayOfWeek { get; set; }
    public bool IsActive { get; set; }
    public List<HourPlan> Slots { get; set; }
    
    public DayPlan()
    {
        DayOfWeek = DayOfWeek.Monday;
        IsActive=false;
        Slots = new();

    }
    
    public DayPlan(DayOfWeek dayOfWeek)
    {
        DayOfWeek = dayOfWeek;
        IsActive=false;
        Slots = new();

    }

    public static List<DayPlan> BuildWeekPlan() => new List<DayPlan>()
    {
        new DayPlan(DayOfWeek.Monday),
        new DayPlan(DayOfWeek.Tuesday),
        new DayPlan(DayOfWeek.Wednesday),
        new DayPlan(DayOfWeek.Thursday),
        new DayPlan(DayOfWeek.Friday),
        new DayPlan(DayOfWeek.Saturday),
        new DayPlan(DayOfWeek.Sunday)
    };

}

public class HourPlan
{
    public int Hour { get; set; }
    public int Minute { get; set; }
}