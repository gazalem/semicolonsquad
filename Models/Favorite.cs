using System.ComponentModel.DataAnnotations;

namespace SmartFoodPlanner.Models
{
    public class Favorite
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public int RecipeId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Recipe? Recipe { get; set; }
    }
}