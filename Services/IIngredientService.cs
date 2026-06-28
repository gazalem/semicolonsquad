using SmartFoodPlanner.Models;

namespace SmartFoodPlanner.Services;

public interface IIngredientService
{
    Task<List<UserIngredient>> GetIngredientsAsync(string userId);
    Task AddIngredientAsync(UserIngredient ingredient);
    Task UpdateIngredientAsync(UserIngredient ingredient);
    Task DeleteIngredientAsync(int id, string userId);
}