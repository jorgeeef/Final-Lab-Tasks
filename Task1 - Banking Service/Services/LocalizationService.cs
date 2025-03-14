using System.Globalization;
using Microsoft.Extensions.Localization;

namespace Task1___Banking_Service.Services;

public class LocalizationService
{
    private readonly IStringLocalizer _localizer;

    public LocalizationService(IStringLocalizer<LocalizationService> localizer)
    {
        _localizer = localizer;
    }

    public string GetLocalizedString(string key, string language)
    {
        CultureInfo.CurrentUICulture = new CultureInfo(language);
        return _localizer[key] ?? key; 
    }
}