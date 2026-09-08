using System.Text.Json;
using System.Text.RegularExpressions;

namespace Pdr.Api.Services.Templating;

public class TemplateRenderer
{
    public const string DeletedPlaceholder = "<DELETED>";
    private static readonly Regex PlaceholderRegex = new(@"\{([a-zA-Z0-9_]+)\}", RegexOptions.Compiled);

    public string Render(string template, IDictionary<string, object?> values) =>
        Render(template, values, null, null);

    public string Render(
        string template,
        IDictionary<string, object?> values,
        IDictionary<string, string?>? initialValues,
        string? staticFallback)
    {
        if (string.IsNullOrEmpty(template)) return string.Empty;

        var valuesLookup = new Dictionary<string, object?>(values, StringComparer.OrdinalIgnoreCase);
        var initialLookup = initialValues != null
            ? new Dictionary<string, string?>(initialValues, StringComparer.OrdinalIgnoreCase)
            : null;

        var matches = PlaceholderRegex.Matches(template);
        if (matches.Count == 0) return template;

        var missingKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var resolvedValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (Match match in matches)
        {
            var key = match.Groups[1].Value;
            if (resolvedValues.ContainsKey(key)) continue;

            if (valuesLookup.TryGetValue(key, out var val) && val != null && !string.IsNullOrEmpty(val.ToString()))
            {
                resolvedValues[key] = val.ToString()!;
            }
            else
            {
                missingKeys.Add(key);
            }
        }

        if (missingKeys.Count > 0)
        {
            if (initialLookup != null)
            {
                foreach (var key in missingKeys)
                {
                    if (initialLookup.TryGetValue(key, out var initVal) && !string.IsNullOrWhiteSpace(initVal))
                    {
                        resolvedValues[key] = initVal;
                    }
                }
            }

            // Fallback hierarchy:
            // 1. If ANY missing key lacks an initial value snapshot, check staticFallback.
            //    If staticFallback is provided, replace entire template with staticFallback.
            // 2. Otherwise (or if no staticFallback), replace any remaining unresolved missing keys with DeletedPlaceholder ("<DELETED>").
            var hasUnresolvedKeys = missingKeys.Any(k => !resolvedValues.ContainsKey(k));
            if (hasUnresolvedKeys && !string.IsNullOrWhiteSpace(staticFallback))
            {
                return staticFallback;
            }

            foreach (var key in missingKeys)
            {
                if (!resolvedValues.ContainsKey(key))
                {
                    resolvedValues[key] = DeletedPlaceholder;
                }
            }
        }

        var result = template;
        foreach (var (k, v) in resolvedValues)
        {
            result = result.Replace($"{{{k}}}", v, StringComparison.OrdinalIgnoreCase);
        }

        return result;
    }

    public static Dictionary<string, string?>? ExtractInitialValues(string? json)
    {
        if (string.IsNullOrWhiteSpace(json)) return null;

        try
        {
            using var doc = JsonDocument.Parse(json);
            if (doc.RootElement.ValueKind != JsonValueKind.Object) return null;

            foreach (var property in doc.RootElement.EnumerateObject())
            {
                if (property.NameEquals("_initialValues") || property.NameEquals("_initial_values"))
                {
                    if (property.Value.ValueKind != JsonValueKind.Object) return null;

                    var result = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
                    foreach (var inner in property.Value.EnumerateObject())
                    {
                        result[inner.Name] = inner.Value.ValueKind switch
                        {
                            JsonValueKind.Null => null,
                            JsonValueKind.String => inner.Value.GetString(),
                            _ => inner.Value.ToString()
                        };
                    }
                    return result;
                }
            }
        }
        catch
        {
            // Ignore JSON parsing errors for initial values extraction
        }

        return null;
    }
}
