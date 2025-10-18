using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class FixUserCharacterRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItem_DollVariant_DollVariantID1",
                table: "OrderItem");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItem_Order_OrderID1",
                table: "OrderItem");

            migrationBuilder.DropIndex(
                name: "IX_OrderItem_DollVariantID1",
                table: "OrderItem");

            migrationBuilder.DropIndex(
                name: "IX_OrderItem_OrderID1",
                table: "OrderItem");

            migrationBuilder.DropColumn(
                name: "DollVariantID1",
                table: "OrderItem");

            migrationBuilder.DropColumn(
                name: "OrderID1",
                table: "OrderItem");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DollVariantID1",
                table: "OrderItem",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrderID1",
                table: "OrderItem",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderItem_DollVariantID1",
                table: "OrderItem",
                column: "DollVariantID1");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItem_OrderID1",
                table: "OrderItem",
                column: "OrderID1");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItem_DollVariant_DollVariantID1",
                table: "OrderItem",
                column: "DollVariantID1",
                principalTable: "DollVariant",
                principalColumn: "DollVariantID");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItem_Order_OrderID1",
                table: "OrderItem",
                column: "OrderID1",
                principalTable: "Order",
                principalColumn: "OrderID");
        }
    }
}
