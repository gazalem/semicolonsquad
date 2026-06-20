using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace SmartFoodPlanner.Services {
  public class OllamaAIService : IAIService {
    private static readonly JsonElement _formatSchema = JsonDocument.Parse("""
      {
        "type": "object",
        "properties": {
          "recipes": {
            "type": "array",
            "items": {
              "type": "object",
              "properties": {
                "day":          { "type": "string" },
                "name":         { "type": "string" },
                "prepTime":     { "type": "string" },
                "cookTime":     { "type": "string" },
                "ingredients":  { "type": "array", "items": { "type": "string" } },
                "instructions": { "type": "string" }
              },
              "required": ["day", "name", "prepTime", "cookTime", "ingredients", "instructions"]
            }
          }
        },
        "required": ["recipes"]
      }
      """).RootElement.Clone();

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<OllamaAIService> _logger;

    public OllamaAIService(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<OllamaAIService> logger) {
      _httpClientFactory = httpClientFactory;
      _configuration = configuration;
      _logger = logger;
    }

    public async Task<MealPlanResponse> GenerateMealPlanAsync(IReadOnlyList<string> ingredients) {
      var baseUrl = _configuration["OllamaSettings:BaseUrl"] ?? "http://localhost:11434";
      var model = _configuration["OllamaSettings:Model"] ?? "gemma3:4b";
      var ingredientList = string.Join(", ", ingredients);

      var request = new GenerateRequest {
        Model = model,
        Prompt = BuildPrompt(ingredientList),
        Stream = false,
        Options = new GenerateOptions { NumCtx = 4096 },
        Format = _formatSchema,
      };

      _logger.LogInformation("Calling Ollama at {BaseUrl} with model {Model}", baseUrl, model);

      var client = _httpClientFactory.CreateClient();
      var httpResponse = await client.PostAsJsonAsync($"{baseUrl}/api/generate", request);
      httpResponse.EnsureSuccessStatusCode();

      var ollamaResponse = await httpResponse.Content.ReadFromJsonAsync<GenerateResponse>();
      if (ollamaResponse?.Response is null) {
        throw new InvalidOperationException("Ollama returned an empty response field");
      }

      _logger.LogInformation("Ollama response received, deserializing meal plan");

      var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
      return JsonSerializer.Deserialize<MealPlanResponse>(ollamaResponse.Response, options)
          ?? throw new JsonException("Failed to deserialize meal plan from Ollama response");
    }

    private static string BuildPrompt(string ingredientList) {
      return $"Create a one-week menu. Using the provided following ingredients: {ingredientList}. " +
             "Return JSON only. Use this schema: " +
             "{\"recipes\":[{\"day\":\"Monday\",\"name\":\"Recipe Name\"," +
             "\"prepTime\":\"15 min\",\"cookTime\":\"30 min\"," +
             "\"ingredients\":[\"ingredient1\",\"ingredient2\"]," +
             "\"instructions\":\"Cooking instructions here.\"}]}";
    }

    private sealed class GenerateRequest {
      [JsonPropertyName("model")]
      public string Model { get; set; } = string.Empty;

      [JsonPropertyName("prompt")]
      public string Prompt { get; set; } = string.Empty;

      [JsonPropertyName("stream")]
      public bool Stream { get; set; }

      [JsonPropertyName("options")]
      public GenerateOptions? Options { get; set; }

      [JsonPropertyName("format")]
      public JsonElement Format { get; set; }
    }

    private sealed class GenerateOptions {
      [JsonPropertyName("num_ctx")]
      public int NumCtx { get; set; }
    }

    private sealed class GenerateResponse {
      [JsonPropertyName("response")]
      public string? Response { get; set; }

      [JsonPropertyName("done")]
      public bool Done { get; set; }
    }
  }
}
