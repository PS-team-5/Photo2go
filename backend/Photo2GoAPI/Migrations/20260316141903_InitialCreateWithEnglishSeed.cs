using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Photo2GoAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreateWithEnglishSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UnescoStatus",
                table: "Locations",
                newName: "IsUnescoProtected");

            migrationBuilder.InsertData(
                table: "Locations",
                columns: new[] { "id", "ArchitectureStyle", "BuildingMaterials", "Category", "City", "IsUnescoProtected", "Name", "ObjectType", "Period", "Province" },
                values: new object[,]
                {
                    { 1, "Gothic", "Bricks, Stone", "Historical Monument", "Vilnius", "Yes", "Gediminas Tower", "Castle", "14th century", "Vilnius County" },
                    { 2, "Classicism", "Plaster, Bricks", "Religious Object", "Vilnius", "Yes", "Vilnius Cathedral", "Church", "18th century", "Vilnius County" },
                    { 3, "Flamboyant Gothic", "Clay, Bricks", "Religious Object", "Vilnius", "Yes", "St. Anne's Church", "Church", "15th century", "Vilnius County" },
                    { 4, "Baroque/Renaissance", "Masonry", "Educational Institution", "Vilnius", "Yes", "Vilnius University", "University", "16th century", "Vilnius County" },
                    { 5, "Renaissance", "Stone, Bricks", "Religious/Defensive", "Vilnius", "Yes", "Gate of Dawn", "City Gate", "16th century", "Vilnius County" },
                    { 6, "Baroque", "Masonry, Stucco", "Religious Object", "Vilnius", "No", "St. Peter and St. Paul's Church", "Church", "17th century", "Vilnius County" },
                    { 7, "Classicism", "Masonry, Plaster", "Government Building", "Vilnius", "Yes", "Presidential Palace", "Palace", "19th century", "Vilnius County" },
                    { 8, "Classicism", "Masonry", "Local Government", "Vilnius", "Yes", "Vilnius Town Hall", "Town Hall", "18th century", "Vilnius County" },
                    { 9, "None", "Nature", "Recreational Area", "Vilnius", "Yes", "Bernardinai Garden", "Park", "15th century", "Vilnius County" },
                    { 10, "Modernism", "Bronze", "Artistic Object", "Vilnius", "Yes", "Angel of Uzupis", "Sculpture", "21st century", "Vilnius County" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "id",
                keyValue: 10);

            migrationBuilder.RenameColumn(
                name: "IsUnescoProtected",
                table: "Locations",
                newName: "UnescoStatus");
        }
    }
}
