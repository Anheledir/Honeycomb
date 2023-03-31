using Fluent.Net;
using System.Globalization;

namespace BaseBotService.Core.Interfaces;
public interface ITranslationService
{
    CultureInfo Culture { get; }
    string PreferredLocale { get; }

    string GetString(string id, IDictionary<string, object>? args = null, ICollection<FluentError>? errors = null);
}