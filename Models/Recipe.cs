using System.ComponentModel.DataAnnotations;

namespace SmartFoodPlanner.Models;

public class Recipe
{
    public int Id { get; set; }
    public int MealPlanId { get; set; }
    public MealPlan? MealPlan { get; set; }

    [Required, StringLength(20)]
    public string Day { get; set; } = string.Empty;

    [Required, StringLength(150)]
    public string Name { get; set; } = string.Empty;

    [StringLength(30)]
    public string? PrepTime { get; set; }

    [StringLength(30)]
    public string? CookTime { get; set; }

    public string Ingredients { get; set; } = "[]";
    public string Instructions { get; set; } = string.Empty;

    [Required, StringLength(36)]
    public string ShareToken { get; set; } = Guid.NewGuid().ToString();
}
