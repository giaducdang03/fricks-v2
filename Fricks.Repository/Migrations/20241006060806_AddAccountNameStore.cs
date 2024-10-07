using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fricks.Repository.Migrations
{
    /// <inheritdoc />
    public partial class AddAccountNameStore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccountName",
                table: "Store",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountName",
                table: "Store");
        }
    }
}
