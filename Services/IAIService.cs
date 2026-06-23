namespace SmartFoodPlanner.Services {
  public interface IAIService {
    Task<MealPlanResponse> GenerateMealPlanAsync(IReadOnlyList<string> ingredients);
  }
}
