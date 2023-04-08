using Fluent.Net;
using System.Globalization;

namespace BaseBotService.Core.Interfaces;
public interface ITranslationService
{
    CultureInfo Culture { get; }
    string PreferredLocale { get; }

    string GetAttrString(string id, string attribute, IDictionary<string, object>? args = null, ICollection<FluentError>? errors = null);
    string GetString(string id, IDictionary<string, object>? args = null, ICollection<FluentError>? errors = null);
    string GetString(string id, string locale, IDictionary<string, object>? args = null, ICollection<FluentError>? errors = null);
    string GetString(string id, string? attribute, string locale, IDictionary<string, object>? args = null, ICollection<FluentError>? errors = null);
}