using System.ComponentModel.DataAnnotations;
using SmartFoodPlanner.Data;

namespace SmartFoodPlanner.Models;

public class FavoriteRecipe
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;

    public ApplicationUser? User { get; set; }

    [Required]
    [StringLength(150)]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Ingredients { get; set; }

    [StringLength(2000)]
    public string? Instructions { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
