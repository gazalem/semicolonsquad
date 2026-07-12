using System.Text.Json;
using System.Text.RegularExpressions;
using SmartFoodPlanner.Models;

namespace SmartFoodPlanner.Services;

public static class RecipeTextFormatter
{
    public static IReadOnlyList<string> ParseIngredients(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return [];

        try
        {
            return JsonSerializer.Deserialize<List<string>>(value)
                ?.Where(item => !string.IsNullOrWhiteSpace(item))
                .Select(item => item.Trim())
                .ToList() ?? [];
        }
        catch (JsonException)
        {
            return SplitText(value);
        }
    }

    public static IReadOnlyList<string> ParseSteps(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return [];

        var lines = Regex.Split(value.Trim(), @"(?:\r?\n)+|(?=\s*\d+[.)]\s+)")
            .Select(line => Regex.Replace(line.Trim(), @"^\d+[.)]\s*", string.Empty))
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .ToList();

        return lines.Count > 1 ? lines : [value.Trim()];
    }

    public static string GetEstimatedTime(Recipe? recipe)
    {
        if (recipe is null) return "30 min";
        var minutes = ParseMinutes(recipe.PrepTime) + ParseMinutes(recipe.CookTime);
        return minutes > 0 ? $"{minutes} min" : recipe.CookTime ?? recipe.PrepTime ?? "30 min";
    }

    private static IReadOnlyList<string> SplitText(string value) => value
        .Split([',', '\n', '\r'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
        .ToList();

    private static int ParseMinutes(string? value)
    {
        var match = Regex.Match(value ?? string.Empty, @"\d+");
        return match.Success && int.TryParse(match.Value, out var minutes) ? minutes : 0;
    }
}
