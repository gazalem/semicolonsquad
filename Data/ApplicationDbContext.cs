using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartFoodPlanner.Models;

namespace SmartFoodPlanner.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<UserIngredient> UserIngredients => Set<UserIngredient>();

    public DbSet<FavoriteRecipe> FavoriteRecipes => Set<FavoriteRecipe>();

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
    }
}
