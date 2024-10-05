using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fricks.Repository.Migrations
{
    /// <inheritdoc />
    public partial class UpdateStoreCate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Store",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Store",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Category",
                type: "nvarchar(5)",
                maxLength: 5,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Store");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Store");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "Category");
        }
    }
}
