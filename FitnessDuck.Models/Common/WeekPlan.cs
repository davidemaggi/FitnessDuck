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

    public static bool AreWeekPlansEqual(List<DayPlan> wp1, List<DayPlan> wp2)
    {
        if (wp1 == null && wp2 == null)
            return true;
        if (wp1 == null || wp2 == null)
            return false;
        if (wp1.Count != wp2.Count)
            return false;

        // Sort both by DayOfWeek to compare DayPlans in matching order
        var orderedWp1 = wp1.OrderBy(dp => dp.DayOfWeek);
        var orderedWp2 = wp2.OrderBy(dp => dp.DayOfWeek);

        foreach (var (dp1, dp2) in orderedWp1.Zip(orderedWp2))
        {
            // Compare slots only, ignoring IsActive and other properties
            if (!AreSlotsEqual(dp1.Slots, dp2.Slots))
                return false;
        }

        return true;
    }

    static bool AreSlotsEqual(List<HourPlan>? slots1, List<HourPlan>? slots2)
    {
        if (slots1 == null && slots2 == null) return true;
        if (slots1 == null || slots2 == null) return false;
        if (slots1.Count != slots2.Count) return false;

        // If order matters:
        //return slots1.SequenceEqual(slots2);

        // Or ignore order if you prefer:
        
        var ordered1 = slots1.OrderBy(s => s.Hour).ThenBy(s => s.Minute);
        var ordered2 = slots2.OrderBy(s => s.Hour).ThenBy(s => s.Minute);
        return ordered1.SequenceEqual(ordered2);
        
    }
    
    
}

public class HourPlan
{
    public int Hour { get; set; }
    public int Minute { get; set; }

    public string? GetTimeString()
    {
       
        var min=   Minute < 10 ? $"0{Minute}" : Minute.ToString();
        return $"{Hour}:{min}";
    }
    
   
    
}