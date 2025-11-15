using FitnessDuck.Client;
using FitnessDuck.Client.Localization;
using Microsoft.Extensions.Localization;
using MudBlazor;

namespace FitnessDuck.Client.Services;

public class ResXMudLocalizer : MudLocalizer
{
    private readonly IStringLocalizer<LocalizationsMarker> _localizer;

    public ResXMudLocalizer(IStringLocalizer<LocalizationsMarker> localizer)
    {
        _localizer = localizer;
    }

    public override LocalizedString this[string key] => _localizer[key];
}