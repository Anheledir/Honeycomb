using Fluent.Net;
using System.Globalization;

namespace BaseBotService.Core.Interfaces;
public interface ITranslationService
{
    CultureInfo Culture { get; }
    string PreferredLocale { get; }

    Dictionary<string, object> Arguments(string? name, object value, params object[] args);
    string GetString(string id, IDictionary<string, object>? args = null, ICollection<FluentError>? errors = null);
    string GetString(string id, string locale, IDictionary<string, object>? args = null, ICollection<FluentError>? errors = null);
}