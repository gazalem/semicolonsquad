using System.ComponentModel.DataAnnotations;
using SmartFoodPlanner.Data;

namespace SmartFoodPlanner.Models;

public class MealPlan
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;

    public ApplicationUser? User { get; set; }

    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;

    public List<Recipe> Recipes { get; set; } = [];
}
