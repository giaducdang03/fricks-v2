using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fricks.Repository.Migrations
{
    /// <inheritdoc />
    public partial class AddCodeProductUnit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "ProductUnit",
                type: "nvarchar(4)",
                maxLength: 4,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Code",
                table: "ProductUnit");
        }
    }
}
