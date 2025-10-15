using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class FixDatabaseSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CharacterOrder_Package",
                table: "CharacterOrder");

            migrationBuilder.DropForeignKey(
                name: "FK_CharacterOrder_UserCharacter",
                table: "CharacterOrder");

            migrationBuilder.DropForeignKey(
                name: "FK_DollCharacterLink_UserCharacter",
                table: "DollCharacterLink");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_Payment",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_Payment_CharacterOrder",
                table: "Payment");

            migrationBuilder.DropForeignKey(
                name: "FK_Payment_Order",
                table: "Payment");

            migrationBuilder.DropTable(
                name: "PasswordResets");

            migrationBuilder.DropColumn(
                name: "ColorID",
                table: "DollVariant");

            migrationBuilder.DropColumn(
                name: "SizeID",
                table: "DollVariant");

            migrationBuilder.RenameColumn(
                name: "target_id",
                table: "Payment",
                newName: "Target_Id");

            migrationBuilder.AlterColumn<string>(
                name: "Phones",
                table: "User",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "User",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedByIp",
                table: "RefreshTokens",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Payment",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "Pending",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Provider",
                table: "Payment",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<int>(
                name: "OrderID",
                table: "Payment",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Method",
                table: "Payment",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "Currency",
                table: "Payment",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Payment",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "SYSUTCDATETIME()",
                oldClrType: typeof(DateTime),
                oldType: "datetime");

            migrationBuilder.AlterColumn<int>(
                name: "CharacterOrderID",
                table: "Payment",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedAt",
                table: "Payment",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MoMoOrderId",
                table: "Payment",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrderInfo",
                table: "Payment",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PayUrl",
                table: "Payment",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RawResponse",
                table: "Payment",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TransactionId",
                table: "Payment",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CharacterOrder_Package",
                table: "CharacterOrder",
                column: "PackageID",
                principalTable: "CharacterPackage",
                principalColumn: "PackageId");

            migrationBuilder.AddForeignKey(
                name: "FK_CharacterOrder_UserCharacter",
                table: "CharacterOrder",
                column: "UserCharacterID",
                principalTable: "UserCharacter",
                principalColumn: "UserCharacterID");

            migrationBuilder.AddForeignKey(
                name: "FK_DollCharacterLink_UserCharacter",
                table: "DollCharacterLink",
                column: "UserCharacterID",
                principalTable: "UserCharacter",
                principalColumn: "UserCharacterID");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_Payment",
                table: "Order",
                column: "PaymentID",
                principalTable: "Payment",
                principalColumn: "PaymentID");

            migrationBuilder.AddForeignKey(
                name: "FK_Payment_CharacterOrder",
                table: "Payment",
                column: "CharacterOrderID",
                principalTable: "CharacterOrder",
                principalColumn: "CharacterOrderID",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Payment_Order",
                table: "Payment",
                column: "OrderID",
                principalTable: "Order",
                principalColumn: "OrderID",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CharacterOrder_Package",
                table: "CharacterOrder");

            migrationBuilder.DropForeignKey(
                name: "FK_CharacterOrder_UserCharacter",
                table: "CharacterOrder");

            migrationBuilder.DropForeignKey(
                name: "FK_DollCharacterLink_UserCharacter",
                table: "DollCharacterLink");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_Payment",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_Payment_CharacterOrder",
                table: "Payment");

            migrationBuilder.DropForeignKey(
                name: "FK_Payment_Order",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "CompletedAt",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "MoMoOrderId",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "OrderInfo",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "PayUrl",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "RawResponse",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "TransactionId",
                table: "Payment");

            migrationBuilder.RenameColumn(
                name: "Target_Id",
                table: "Payment",
                newName: "target_id");

            migrationBuilder.AlterColumn<string>(
                name: "Phones",
                table: "User",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "User",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedByIp",
                table: "RefreshTokens",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Payment",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldDefaultValue: "Pending");

            migrationBuilder.AlterColumn<string>(
                name: "Provider",
                table: "Payment",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<int>(
                name: "OrderID",
                table: "Payment",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Method",
                table: "Payment",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Currency",
                table: "Payment",
                type: "int",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Payment",
                type: "datetime",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "SYSUTCDATETIME()");

            migrationBuilder.AlterColumn<int>(
                name: "CharacterOrderID",
                table: "Payment",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ColorID",
                table: "DollVariant",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SizeID",
                table: "DollVariant",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "PasswordResets",
                columns: table => new
                {
                    PasswordResetsID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Created = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreatedByIp = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Expires = table.Column<DateTime>(type: "datetime", nullable: false),
                    Used = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PasswordResets", x => x.PasswordResetsID);
                    table.ForeignKey(
                        name: "FK_PasswordResets_User",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PasswordResets_UserID",
                table: "PasswordResets",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_CharacterOrder_Package",
                table: "CharacterOrder",
                column: "PackageID",
                principalTable: "CharacterPackage",
                principalColumn: "PackageId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CharacterOrder_UserCharacter",
                table: "CharacterOrder",
                column: "UserCharacterID",
                principalTable: "UserCharacter",
                principalColumn: "UserCharacterID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DollCharacterLink_UserCharacter",
                table: "DollCharacterLink",
                column: "UserCharacterID",
                principalTable: "UserCharacter",
                principalColumn: "UserCharacterID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Order_Payment",
                table: "Order",
                column: "PaymentID",
                principalTable: "Payment",
                principalColumn: "PaymentID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Payment_CharacterOrder",
                table: "Payment",
                column: "CharacterOrderID",
                principalTable: "CharacterOrder",
                principalColumn: "CharacterOrderID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Payment_Order",
                table: "Payment",
                column: "OrderID",
                principalTable: "Order",
                principalColumn: "OrderID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
