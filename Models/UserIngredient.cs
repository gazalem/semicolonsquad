using System.ComponentModel.DataAnnotations;
using SmartFoodPlanner.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartFoodPlanner.Models;

public class UserIngredient
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;

    public ApplicationUser User { get; set; } = null!;

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(50)]
    public string? Quantity { get; set; }

    [StringLength(100)]
    public string? Category { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
