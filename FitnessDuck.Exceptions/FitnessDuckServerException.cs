namespace FitnessDuck.Exceptions;

public class FitnessDuckServerException:Exception
{
    public string ErrorCode { get; }
    public object? AdditionalData { get; }

    public FitnessDuckServerException(string errorCode, string message, object? additionalData = null)
        : base(message)
    {
        ErrorCode = errorCode;
        AdditionalData = additionalData;
    }
}