using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartFoodPlanner.Migrations
{
    /// <inheritdoc />
    public partial class RenameIngredientCategoryToUnit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Category",
                table: "UserIngredients",
                newName: "Unit");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Unit",
                table: "UserIngredients",
                newName: "Category");
        }
    }
}
