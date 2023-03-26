namespace BaseBotService.Commands.Enums;

[Flags]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Critical Code Smell", "S2346:Flags enumerations zero-value members should be named \"None\"", Justification = "Used in GUI right now, other is a better choice.")]
public enum Languages
{
    Other = 0,
    English = 1 << 0,           // United States, Canada, United Kingdom, Australia
    German = 1 << 1,            // Germany, Austria, Switzerland
    French = 1 << 2,            // France
    Portuguese = 1 << 3,        // Brazil
    Spanish = 1 << 4,           // Mexico, Spain
    Dutch = 1 << 5,             // Netherlands
    Swedish = 1 << 6,           // Sweden
    Norwegian = 1 << 7,         // Norway
    Danish = 1 << 8,            // Denmark
    Finnish = 1 << 9,           // Finland
    Polish = 1 << 10,           // Poland
    Russian = 1 << 11,          // Russia
    Japanese = 1 << 12,         // Japan
    Korean = 1 << 13,           // South Korea
    Chinese = 1 << 14,          // China
    Hindi = 1 << 15,            // India
    Turkish = 1 << 16,          // Turkey
    Indonesian = 1 << 17,       // Indonesia
    Filipino_Tagalog = 1 << 18, // Philippines
    Italian = 1 << 19,          // Italy
    Arabic = 1 << 20,           // Saudi Arabia, United Arab Emirates, Egypt, Iraq, Jordan, Kuwait, Lebanon, Libya, Morocco, Oman, Qatar, Sudan, Syria, Tunisia, Yemen
    Thai = 1 << 21,             // Thailand
    Vietnamese = 1 << 22,       // Vietnam
    Greek = 1 << 23,            // Greece
}
