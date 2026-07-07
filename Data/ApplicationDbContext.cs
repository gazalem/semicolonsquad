using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartFoodPlanner.Models;

namespace SmartFoodPlanner.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<UserIngredient> UserIngredients => Set<UserIngredient>();

    public DbSet<FavoriteRecipe> FavoriteRecipes => Set<FavoriteRecipe>();

    public DbSet<MealPlan> MealPlans => Set<MealPlan>();

    public DbSet<Recipe> Recipes => Set<Recipe>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<UserIngredient>()
            .HasOne(ingredient => ingredient.User)
            .WithMany()
            .HasForeignKey(ingredient => ingredient.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<UserIngredient>()
            .HasIndex(ingredient => new { ingredient.UserId, ingredient.Name });

        builder.Entity<FavoriteRecipe>()
            .HasOne(recipe => recipe.User)
            .WithMany()
            .HasForeignKey(recipe => recipe.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<FavoriteRecipe>()
            .HasIndex(recipe => new { recipe.UserId, recipe.Title });

        builder.Entity<MealPlan>()
            .HasOne(plan => plan.User)
            .WithMany()
            .HasForeignKey(plan => plan.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<MealPlan>()
            .HasMany(plan => plan.Recipes)
            .WithOne(recipe => recipe.MealPlan)
            .HasForeignKey(recipe => recipe.MealPlanId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Recipe>()
            .HasIndex(recipe => recipe.ShareToken)
            .IsUnique();
    }
}
