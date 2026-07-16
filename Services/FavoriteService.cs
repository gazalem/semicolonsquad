using Microsoft.EntityFrameworkCore;
using SmartFoodPlanner.Data;
using SmartFoodPlanner.Models;

namespace SmartFoodPlanner.Services
{
    public class FavoriteService : IFavoriteService
    {
        private readonly ApplicationDbContext _context;

        public FavoriteService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task ToggleFavoriteAsync(string userId, int recipeId)
        {
            var existingFavorite = await _context.Favorites
                .FirstOrDefaultAsync(f => f.UserId == userId && f.RecipeId == recipeId);

            if (existingFavorite is not null)
            {
                _context.Favorites.Remove(existingFavorite);
            }
            else
            {
                var favorite = new Favorite
                {
                    UserId = userId,
                    RecipeId = recipeId,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Favorites.Add(favorite);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsFavoriteAsync(string userId, int recipeId)
        {
            return await _context.Favorites
                .AnyAsync(f => f.UserId == userId && f.RecipeId == recipeId);
        }

        public async Task<List<Recipe>> GetFavoriteRecipesAsync(string userId)
        {
            return await _context.Favorites
                .Where(f => f.UserId == userId)
                .Include(f => f.Recipe)
                .Select(f => f.Recipe!)
                .ToListAsync();
        }
    }
}