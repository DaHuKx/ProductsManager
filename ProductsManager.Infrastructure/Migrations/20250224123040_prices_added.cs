using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProductsManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class prices_added : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Trades",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CurrentPrice",
                table: "Products",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "Trades");

            migrationBuilder.DropColumn(
                name: "CurrentPrice",
                table: "Products");
        }
    }
}
