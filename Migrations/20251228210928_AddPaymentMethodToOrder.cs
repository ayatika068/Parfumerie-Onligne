using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ParfumerieOnline.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentMethodToOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "mode_paiement",
                table: "Commandes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "mode_paiement",
                table: "Commandes");
        }
    }
}
