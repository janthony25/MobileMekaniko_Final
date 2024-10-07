using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MobileMekaniko_Final.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixQuotationandQuotationIteminDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Quotation_Cars_CarId",
                table: "Quotation");

            migrationBuilder.DropForeignKey(
                name: "FK_QuotationItem_Quotation_QuotationId",
                table: "QuotationItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_QuotationItem",
                table: "QuotationItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Quotation",
                table: "Quotation");

            migrationBuilder.RenameTable(
                name: "QuotationItem",
                newName: "QuotationItems");

            migrationBuilder.RenameTable(
                name: "Quotation",
                newName: "Quotations");

            migrationBuilder.RenameIndex(
                name: "IX_QuotationItem_QuotationId",
                table: "QuotationItems",
                newName: "IX_QuotationItems_QuotationId");

            migrationBuilder.RenameIndex(
                name: "IX_Quotation_CarId",
                table: "Quotations",
                newName: "IX_Quotations_CarId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_QuotationItems",
                table: "QuotationItems",
                column: "QuotationItemId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Quotations",
                table: "Quotations",
                column: "QuotationId");

            migrationBuilder.AddForeignKey(
                name: "FK_QuotationItems_Quotations_QuotationId",
                table: "QuotationItems",
                column: "QuotationId",
                principalTable: "Quotations",
                principalColumn: "QuotationId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Quotations_Cars_CarId",
                table: "Quotations",
                column: "CarId",
                principalTable: "Cars",
                principalColumn: "CarId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuotationItems_Quotations_QuotationId",
                table: "QuotationItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Quotations_Cars_CarId",
                table: "Quotations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Quotations",
                table: "Quotations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_QuotationItems",
                table: "QuotationItems");

            migrationBuilder.RenameTable(
                name: "Quotations",
                newName: "Quotation");

            migrationBuilder.RenameTable(
                name: "QuotationItems",
                newName: "QuotationItem");

            migrationBuilder.RenameIndex(
                name: "IX_Quotations_CarId",
                table: "Quotation",
                newName: "IX_Quotation_CarId");

            migrationBuilder.RenameIndex(
                name: "IX_QuotationItems_QuotationId",
                table: "QuotationItem",
                newName: "IX_QuotationItem_QuotationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Quotation",
                table: "Quotation",
                column: "QuotationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_QuotationItem",
                table: "QuotationItem",
                column: "QuotationItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_Quotation_Cars_CarId",
                table: "Quotation",
                column: "CarId",
                principalTable: "Cars",
                principalColumn: "CarId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuotationItem_Quotation_QuotationId",
                table: "QuotationItem",
                column: "QuotationId",
                principalTable: "Quotation",
                principalColumn: "QuotationId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
