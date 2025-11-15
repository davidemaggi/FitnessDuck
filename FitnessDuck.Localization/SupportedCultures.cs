using System.Globalization;

namespace FitnessDuck.Localization;

public class SupportedCulture
{
    
    public string Flag { get; }
    public CultureInfo CultureInfo { get; }
    public bool IsDefault { get; }


    public SupportedCulture(CultureInfo cultureInfo, string flag, bool isDefault=false)
    {

        CultureInfo = cultureInfo;
        Flag = flag;
        IsDefault = isDefault;
        CultureInfo = cultureInfo;
    }


    public static List<SupportedCulture> GetAllSupported() => new List<SupportedCulture>()
    {
        new SupportedCulture(new CultureInfo("en-US"), "us"),
        new SupportedCulture(new CultureInfo("it-IT"), "it",true),


    };

}