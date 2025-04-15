using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProductsManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class import_and_export_nullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentPrice",
                table: "Products");

            migrationBuilder.AddColumn<decimal>(
                name: "ExportPrice",
                table: "Products",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ImportPrice",
                table: "Products",
                type: "numeric",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExportPrice",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ImportPrice",
                table: "Products");

            migrationBuilder.AddColumn<decimal>(
                name: "CurrentPrice",
                table: "Products",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
