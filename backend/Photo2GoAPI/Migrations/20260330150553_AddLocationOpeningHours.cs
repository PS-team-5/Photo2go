using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Photo2GoAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddLocationOpeningHours : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<TimeOnly>(
                name: "ClosingTime",
                table: "Locations",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "OpeningTime",
                table: "Locations",
                type: "TEXT",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Locations",
                keyColumn: "id",
                keyValue: 1,
                columns: new[] { "ClosingTime", "OpeningTime" },
                values: new object[] { new TimeOnly(20, 0, 0), new TimeOnly(10, 0, 0) });

            migrationBuilder.UpdateData(
                table: "Locations",
                keyColumn: "id",
                keyValue: 2,
                columns: new[] { "ClosingTime", "OpeningTime" },
                values: new object[] { new TimeOnly(18, 0, 0), new TimeOnly(8, 0, 0) });

            migrationBuilder.UpdateData(
                table: "Locations",
                keyColumn: "id",
                keyValue: 3,
                columns: new[] { "ClosingTime", "OpeningTime" },
                values: new object[] { new TimeOnly(18, 0, 0), new TimeOnly(9, 0, 0) });

            migrationBuilder.UpdateData(
                table: "Locations",
                keyColumn: "id",
                keyValue: 4,
                columns: new[] { "ClosingTime", "OpeningTime" },
                values: new object[] { new TimeOnly(20, 0, 0), new TimeOnly(8, 0, 0) });

            migrationBuilder.UpdateData(
                table: "Locations",
                keyColumn: "id",
                keyValue: 5,
                columns: new[] { "ClosingTime", "OpeningTime" },
                values: new object[] { new TimeOnly(19, 0, 0), new TimeOnly(6, 0, 0) });

            migrationBuilder.UpdateData(
                table: "Locations",
                keyColumn: "id",
                keyValue: 6,
                columns: new[] { "ClosingTime", "OpeningTime" },
                values: new object[] { new TimeOnly(18, 0, 0), new TimeOnly(9, 0, 0) });

            migrationBuilder.UpdateData(
                table: "Locations",
                keyColumn: "id",
                keyValue: 7,
                columns: new[] { "ClosingTime", "OpeningTime" },
                values: new object[] { new TimeOnly(17, 0, 0), new TimeOnly(9, 0, 0) });

            migrationBuilder.UpdateData(
                table: "Locations",
                keyColumn: "id",
                keyValue: 8,
                columns: new[] { "ClosingTime", "OpeningTime" },
                values: new object[] { new TimeOnly(18, 0, 0), new TimeOnly(9, 0, 0) });

            migrationBuilder.UpdateData(
                table: "Locations",
                keyColumn: "id",
                keyValue: 9,
                columns: new[] { "ClosingTime", "OpeningTime" },
                values: new object[] { new TimeOnly(22, 0, 0), new TimeOnly(6, 0, 0) });

            migrationBuilder.UpdateData(
                table: "Locations",
                keyColumn: "id",
                keyValue: 10,
                columns: new[] { "ClosingTime", "OpeningTime" },
                values: new object[] { new TimeOnly(23, 59, 0), new TimeOnly(0, 0, 0) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClosingTime",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "OpeningTime",
                table: "Locations");
        }
    }
}
