using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fricks.Repository.Migrations
{
    /// <inheritdoc />
    public partial class AddBankInfoStore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccountNumber",
                table: "Store",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BankCode",
                table: "Store",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountNumber",
                table: "Store");

            migrationBuilder.DropColumn(
                name: "BankCode",
                table: "Store");
        }
    }
}
