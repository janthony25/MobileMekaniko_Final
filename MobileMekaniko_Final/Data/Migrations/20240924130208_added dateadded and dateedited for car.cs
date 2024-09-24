using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MobileMekaniko_Final.Data.Migrations
{
    /// <inheritdoc />
    public partial class addeddateaddedanddateeditedforcar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CarAdded",
                table: "Cars",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateEdited",
                table: "Cars",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CarAdded",
                table: "Cars");

            migrationBuilder.DropColumn(
                name: "DateEdited",
                table: "Cars");
        }
    }
}
