using SmartFoodPlanner.Models;

namespace SmartFoodPlanner.Services;

public interface IIngredientService
{
    Task<List<UserIngredient>> GetIngredientsAsync(string userId);
    Task AddIngredientAsync(string userId, UserIngredient ingredient);
    Task<bool> UpdateIngredientAsync(string userId, UserIngredient ingredient);
    Task<bool> DeleteIngredientAsync(string userId, int ingredientId);
}
