using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Photo2GoAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateLocationFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BuildingMaterials",
                table: "Locations",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Locations",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Province",
                table: "Locations",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UnescoStatus",
                table: "Locations",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BuildingMaterials",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "Province",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "UnescoStatus",
                table: "Locations");
        }
    }
}
