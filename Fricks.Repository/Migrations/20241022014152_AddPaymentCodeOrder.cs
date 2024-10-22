using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fricks.Repository.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentCodeOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "PaymentCode",
                table: "Order",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentCode",
                table: "Order");
        }
    }
}
