using SmartFoodPlanner.Models;

namespace SmartFoodPlanner.Services;

public interface IMealPlanService
{
    Task<MealPlan> SaveMealPlanAsync(string userId, MealPlanResponse response);
    Task<List<MealPlan>> GetMealPlansAsync(string userId);
    Task<MealPlan?> GetMealPlanByIdAsync(int id, string userId);
    Task<Recipe?> GetRecipeByIdAsync(int recipeId, string userId);
}
