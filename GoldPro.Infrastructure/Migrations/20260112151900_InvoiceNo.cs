using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoldPro.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InvoiceNo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InvoiceNo",
                table: "Sales",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "InvoiceSequence",
                table: "Sales",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "InvoiceNo",
                table: "SaleItems",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InvoiceNo",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "InvoiceSequence",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "InvoiceNo",
                table: "SaleItems");
        }
    }
}
