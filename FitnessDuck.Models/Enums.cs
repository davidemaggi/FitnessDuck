namespace FitnessDuck.Models;

public enum BookingStatus
{
    Deleted = -1,
    Pending = 0,
    Confirmed = 1,
    
}

public enum ContactMethod
{
    Telegram = 1,
    Email = 0,
    WebPush = 2,
    Mobile = 2,

    
}





public enum RecurrenceType
{
    None = 0,
    Daily = 1,
    Weekly = 2,
    Monthly = 3,
    WeekPlan = 4,

    
}

public enum UserPlan
{
    NoPlan = 0,
    Weekly = 1,
    Monthly = 2,
    Carnet = 3,


    
}

public enum UserRole
{
    Visitor = 0,
    Trainee = 1,
    Trainer = 2,
    Admin = 3,


    
}


public enum FormMode
{
    View = 0,
    Edit = 1,
    Create = 2


    
}