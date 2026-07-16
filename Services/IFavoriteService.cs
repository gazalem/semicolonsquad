using SmartFoodPlanner.Models;

namespace SmartFoodPlanner.Services
{
    public interface IFavoriteService
    {
        Task ToggleFavoriteAsync(string userId, int recipeId);
        Task<bool> IsFavoriteAsync(string userId, int recipeId);
        Task<List<Recipe>> GetFavoriteRecipesAsync(string userId);
    }
}