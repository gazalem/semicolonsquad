using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartFoodPlanner.Models
{
    public class Ingredient
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal? Quantity { get; set; }

        [StringLength(50)]
        public string? Unit { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}