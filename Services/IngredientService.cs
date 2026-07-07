using Microsoft.EntityFrameworkCore;
using SmartFoodPlanner.Data;
using SmartFoodPlanner.Models;

namespace SmartFoodPlanner.Services;

public sealed class IngredientService(ApplicationDbContext dbContext) : IIngredientService
{
    public Task<List<UserIngredient>> GetIngredientsAsync(string userId) =>
        dbContext.UserIngredients.AsNoTracking()
            .Where(item => item.UserId == userId)
            .OrderBy(item => item.Name)
            .ToListAsync();

    public async Task AddIngredientAsync(string userId, UserIngredient ingredient)
    {
        ingredient.UserId = userId;
        dbContext.UserIngredients.Add(ingredient);
        await dbContext.SaveChangesAsync();
    }

    public async Task<bool> UpdateIngredientAsync(string userId, UserIngredient ingredient)
    {
        var saved = await dbContext.UserIngredients
            .FirstOrDefaultAsync(item => item.Id == ingredient.Id && item.UserId == userId);
        if (saved is null) return false;

        saved.Name = ingredient.Name;
        saved.Quantity = ingredient.Quantity;
        saved.Unit = ingredient.Unit;
        await dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteIngredientAsync(string userId, int ingredientId)
    {
        var saved = await dbContext.UserIngredients
            .FirstOrDefaultAsync(item => item.Id == ingredientId && item.UserId == userId);
        if (saved is null) return false;

        dbContext.UserIngredients.Remove(saved);
        await dbContext.SaveChangesAsync();
        return true;
    }
}
