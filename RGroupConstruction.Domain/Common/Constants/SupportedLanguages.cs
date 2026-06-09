namespace RGroupConstruction.Domain.Common.Constants;

public static class SupportedLanguages
{
    private static string _defaultCode = string.Empty;
    private static string[] _codes = [];

    public static string DefaultCode => _defaultCode;

    public static IReadOnlyCollection<string> Codes => _codes;

    public static void Configure(string? defaultCode, IEnumerable<string>? codes)
    {
        var normalizedCodes = (codes ?? [])
            .Where(code => !string.IsNullOrWhiteSpace(code))
            .Select(code => code.Trim().ToLowerInvariant())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (normalizedCodes.Length == 0)
            throw new InvalidOperationException("Localization:SupportedCultures must contain at least one culture code.");

        if (string.IsNullOrWhiteSpace(defaultCode))
            throw new InvalidOperationException("Localization:DefaultCulture is required.");

        var normalizedDefaultCode = defaultCode.Trim().ToLowerInvariant();

        if (!normalizedCodes.Contains(normalizedDefaultCode, StringComparer.OrdinalIgnoreCase))
            throw new InvalidOperationException("Localization:DefaultCulture must be included in Localization:SupportedCultures.");

        _defaultCode = normalizedDefaultCode;
        _codes = normalizedCodes;
    }

    public static bool IsSupported(string? code)
        => !string.IsNullOrWhiteSpace(code) &&
           _codes.Contains(code.Trim(), StringComparer.OrdinalIgnoreCase);
}

