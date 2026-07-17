using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace SmartFoodPlanner.Services {
  public class OllamaAIService : IAIService {
    private static readonly TimeSpan _requestTimeout = TimeSpan.FromSeconds(30);
    private static readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    private static readonly JsonElement _formatSchema = JsonDocument.Parse("""
      {
        "type": "object",
        "properties": {
          "recipes": {
            "type": "array",
            "minItems": 7,
            "maxItems": 7,
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
      var baseUrl = _configuration["OllamaSettings:BaseUrl"] ?? "https://ollama.com/";
      var model = _configuration["OllamaSettings:Model"] ?? "gemma4:cloud";
      var apiKey = _configuration["OllamaSettings:ApiKey"] ?? _configuration["OLLAMA_API_KEY"];
      var ingredientList = string.Join(", ", ingredients);

      var request = new GenerateRequest {
        Model = model,
        System = "You are a helpful meal planning assistant. Always respond with valid JSON only. No explanation, no markdown, just JSON.",
        Prompt = BuildPrompt(ingredientList),
        Stream = false,
        Options = new GenerateOptions { NumCtx = 4096 },
        Format = _formatSchema,
      };

      _logger.LogInformation(
          "Calling Ollama at {BaseUrl} with model {Model} (API key present: {HasApiKey})",
          baseUrl, model, !string.IsNullOrWhiteSpace(apiKey));

      var client = _httpClientFactory.CreateClient();
      client.Timeout = _requestTimeout;

      if (!string.IsNullOrWhiteSpace(apiKey)) {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
      }

      try {
        var httpResponse = await client.PostAsJsonAsync($"{baseUrl}/api/generate", request);
        httpResponse.EnsureSuccessStatusCode();

        var ollamaResponse = await httpResponse.Content.ReadFromJsonAsync<GenerateResponse>();
        if (ollamaResponse?.Response is null) {
          throw new JsonException("Ollama returned an empty response field");
        }

        _logger.LogInformation("Ollama response received, deserializing meal plan");
        _logger.LogDebug("Ollama raw response: {Response}", ollamaResponse.Response);

        var result = JsonSerializer.Deserialize<MealPlanResponse>(ollamaResponse.Response, _jsonOptions)
            ?? throw new JsonException("Failed to deserialize meal plan from Ollama response");

        if (result.Recipes.Count != 7) {
          _logger.LogWarning("Expected 7 recipes but received {Count}", result.Recipes.Count);
        }

        return result;
      } catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException) {
        _logger.LogError(ex, "Ollama request timed out after {TimeoutSeconds}s", _requestTimeout.TotalSeconds);
        throw new AIServiceException("The AI is taking too long. Please try again.", ex);
      } catch (HttpRequestException ex) {
        _logger.LogError(ex, "Ollama request failed with status {StatusCode}", ex.StatusCode);
        throw new AIServiceException("Service temporarily unavailable.", ex);
      } catch (JsonException ex) {
        _logger.LogError(ex, "Failed to parse Ollama response as meal plan JSON");
        throw new AIServiceException("Could not process the AI response. Please try again.", ex);
      }
    }

private static string BuildPrompt(string ingredientList) {
  return $$"""
  I have these ingredients available:
  {{ingredientList}}

  Create a 7-day meal plan with exactly one recipe for each day:
  Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday.

  Requirements:
  - Return exactly 7 recipes.
  - Do not return fewer than 7 recipes.
  - Do not combine multiple days into one recipe.
  - Each recipe must be 30 minutes or less total cook/prep time.
  - Use the available ingredients when possible.
  - You may add basic pantry items if needed.
  - Return JSON only.
  - Do not include markdown, comments, or explanation.

  The JSON must use this exact structure:

  {
    "recipes": [
      {
        "day": "Monday",
        "name": "Recipe Name",
        "prepTime": "10 min",
        "cookTime": "20 min",
        "ingredients": ["ingredient 1", "ingredient 2"],
        "instructions": "Step-by-step cooking instructions."
      },
      {
        "day": "Tuesday",
        "name": "Recipe Name",
        "prepTime": "10 min",
        "cookTime": "20 min",
        "ingredients": ["ingredient 1", "ingredient 2"],
        "instructions": "Step-by-step cooking instructions."
      },
      {
        "day": "Wednesday",
        "name": "Recipe Name",
        "prepTime": "10 min",
        "cookTime": "20 min",
        "ingredients": ["ingredient 1", "ingredient 2"],
        "instructions": "Step-by-step cooking instructions."
      },
      {
        "day": "Thursday",
        "name": "Recipe Name",
        "prepTime": "10 min",
        "cookTime": "20 min",
        "ingredients": ["ingredient 1", "ingredient 2"],
        "instructions": "Step-by-step cooking instructions."
      },
      {
        "day": "Friday",
        "name": "Recipe Name",
        "prepTime": "10 min",
        "cookTime": "20 min",
        "ingredients": ["ingredient 1", "ingredient 2"],
        "instructions": "Step-by-step cooking instructions."
      },
      {
        "day": "Saturday",
        "name": "Recipe Name",
        "prepTime": "10 min",
        "cookTime": "20 min",
        "ingredients": ["ingredient 1", "ingredient 2"],
        "instructions": "Step-by-step cooking instructions."
      },
      {
        "day": "Sunday",
        "name": "Recipe Name",
        "prepTime": "10 min",
        "cookTime": "20 min",
        "ingredients": ["ingredient 1", "ingredient 2"],
        "instructions": "Step-by-step cooking instructions."
      }
    ]
  }
  """;
}

    private sealed class GenerateRequest {
      [JsonPropertyName("model")]
      public string Model { get; set; } = string.Empty;

      [JsonPropertyName("system")]
      public string System { get; set; } = string.Empty;

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
