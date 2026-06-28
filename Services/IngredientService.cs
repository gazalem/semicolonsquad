using Microsoft.EntityFrameworkCore;
using SmartFoodPlanner.Data;
using SmartFoodPlanner.Models;

namespace SmartFoodPlanner.Services;

public class IngredientService : IIngredientService
{
    private readonly ApplicationDbContext _context;

    public IngredientService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<UserIngredient>> GetIngredientsAsync(string userId)
    {
        return await _context.UserIngredients
            .Where(i => i.UserId == userId)
            .OrderBy(i => i.Name)
            .ToListAsync();
    }

    public async Task AddIngredientAsync(UserIngredient ingredient)
    {
        _context.UserIngredients.Add(ingredient);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateIngredientAsync(UserIngredient ingredient)
    {
        var existingIngredient = await _context.UserIngredients
            .FirstOrDefaultAsync(i => i.Id == ingredient.Id && i.UserId == ingredient.UserId);

        if (existingIngredient is null)
            return;

        existingIngredient.Name = ingredient.Name;
        existingIngredient.Quantity = ingredient.Quantity;
        existingIngredient.Category = ingredient.Category;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteIngredientAsync(int id, string userId)
    {
        var ingredient = await _context.UserIngredients
            .FirstOrDefaultAsync(i => i.Id == id && i.UserId == userId);

        if (ingredient is null)
            return;

        _context.UserIngredients.Remove(ingredient);
        await _context.SaveChangesAsync();
    }
}