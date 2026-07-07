using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SmartFoodPlanner.Data;
using SmartFoodPlanner.Models;

namespace SmartFoodPlanner.Services;

public sealed class MealPlanService(ApplicationDbContext dbContext) : IMealPlanService
{
    public async Task<MealPlan> SaveMealPlanAsync(string userId, MealPlanResponse response)
    {
        var plan = new MealPlan
        {
            UserId = userId,
            Recipes = response.Recipes.Select(item => new Recipe
            {
                Day = item.Day.Trim(),
                Name = item.Name.Trim(),
                PrepTime = Normalize(item.PrepTime),
                CookTime = Normalize(item.CookTime),
                Ingredients = JsonSerializer.Serialize(item.Ingredients),
                Instructions = item.Instructions.Trim(),
                ShareToken = Guid.NewGuid().ToString()
            }).ToList()
        };

        dbContext.MealPlans.Add(plan);
        await dbContext.SaveChangesAsync();
        return plan;
    }

    public Task<List<MealPlan>> GetMealPlansAsync(string userId) =>
        dbContext.MealPlans.AsNoTracking()
            .Where(plan => plan.UserId == userId)
            .Include(plan => plan.Recipes)
            .OrderByDescending(plan => plan.GeneratedAt)
            .ToListAsync();

    public Task<MealPlan?> GetMealPlanByIdAsync(int id, string userId) =>
        dbContext.MealPlans.AsNoTracking()
            .Include(plan => plan.Recipes)
            .FirstOrDefaultAsync(plan => plan.Id == id && plan.UserId == userId);

    public Task<Recipe?> GetRecipeByIdAsync(int recipeId, string userId) =>
        dbContext.Recipes.AsNoTracking()
            .FirstOrDefaultAsync(recipe => recipe.Id == recipeId && recipe.MealPlan!.UserId == userId);

    private static string? Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
