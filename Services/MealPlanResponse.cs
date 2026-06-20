using System.Text.Json.Serialization;

namespace SmartFoodPlanner.Services {
  public class RecipeDto {
    [JsonPropertyName("day")]
    public string Day { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("prepTime")]
    public string PrepTime { get; set; } = string.Empty;

    [JsonPropertyName("cookTime")]
    public string CookTime { get; set; } = string.Empty;

    [JsonPropertyName("ingredients")]
    public List<string> Ingredients { get; set; } = new();

    [JsonPropertyName("instructions")]
    public string Instructions { get; set; } = string.Empty;
  }

  public class MealPlanResponse {
    [JsonPropertyName("recipes")]
    public List<RecipeDto> Recipes { get; set; } = new();
  }
}
