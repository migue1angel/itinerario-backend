using System.Globalization;
using System.Text.RegularExpressions;

namespace Api.Validators;

public static class OffsetDateTimeFormat
{
    private static readonly Regex Regex = new(
        @"^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}(?:\.\d+)?(?:Z|[+-]\d{2}:\d{2})$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public static bool IsValid(string? raw)
    {
        return !string.IsNullOrWhiteSpace(raw) &&
               Regex.IsMatch(raw) &&
               DateTimeOffset.TryParse(
                   raw,
                   CultureInfo.InvariantCulture,
                   DateTimeStyles.None,
                   out _);
    }
}