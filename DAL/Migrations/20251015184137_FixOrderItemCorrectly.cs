using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class FixOrderItemCorrectly : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Character",
                columns: table => new
                {
                    CharacterId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Language = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AgeRange = table.Column<int>(type: "int", nullable: false),
                    Personality = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Character", x => x.CharacterId);
                });

            migrationBuilder.CreateTable(
                name: "DollType",
                columns: table => new
                {
                    DollTypeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Create_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    Image = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DollType", x => x.DollTypeID);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    UserID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Phones = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false, defaultValue: "Active"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "user"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.UserID);
                });

            migrationBuilder.CreateTable(
                name: "CharacterPackage",
                columns: table => new
                {
                    PackageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CharacterId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DurationDays = table.Column<int>(type: "int", nullable: false),
                    Billing_Cycle = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterPackage", x => x.PackageId);
                    table.ForeignKey(
                        name: "FK_CharacterPackage_Character",
                        column: x => x.CharacterId,
                        principalTable: "Character",
                        principalColumn: "CharacterId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DollModel",
                columns: table => new
                {
                    DollModelID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DollTypeID = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Create_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    Image = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DollModel", x => x.DollModelID);
                    table.ForeignKey(
                        name: "FK_DollModel_DollType",
                        column: x => x.DollTypeID,
                        principalTable: "DollType",
                        principalColumn: "DollTypeID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    RefreshTokenID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Expires = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Revoked = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByIp = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.RefreshTokenID);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_User_UserID",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserCharacter",
                columns: table => new
                {
                    UserCharacterID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    CharacterID = table.Column<int>(type: "int", nullable: false),
                    PackageId = table.Column<int>(type: "int", nullable: false),
                    StartAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    EndAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    AutoRenew = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCharacter", x => x.UserCharacterID);
                    table.ForeignKey(
                        name: "FK_UserCharacter_Character",
                        column: x => x.CharacterID,
                        principalTable: "Character",
                        principalColumn: "CharacterId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserCharacter_Package",
                        column: x => x.PackageId,
                        principalTable: "CharacterPackage",
                        principalColumn: "PackageId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserCharacter_User",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DollVariant",
                columns: table => new
                {
                    DollVariantID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DollModelID = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Color = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Size = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    Image = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DollVariant", x => x.DollVariantID);
                    table.ForeignKey(
                        name: "FK_DollVariant_DollModel",
                        column: x => x.DollModelID,
                        principalTable: "DollModel",
                        principalColumn: "DollModelID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharacterOrder",
                columns: table => new
                {
                    CharacterOrderID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PackageID = table.Column<int>(type: "int", nullable: false),
                    CharacterID = table.Column<int>(type: "int", nullable: false),
                    UserCharacterID = table.Column<int>(type: "int", nullable: false),
                    QuantityMonths = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Start_Date = table.Column<DateTime>(type: "datetime", nullable: false),
                    End_Date = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterOrder", x => x.CharacterOrderID);
                    table.ForeignKey(
                        name: "FK_CharacterOrder_Character",
                        column: x => x.CharacterID,
                        principalTable: "Character",
                        principalColumn: "CharacterId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CharacterOrder_Package",
                        column: x => x.PackageID,
                        principalTable: "CharacterPackage",
                        principalColumn: "PackageId");
                    table.ForeignKey(
                        name: "FK_CharacterOrder_UserCharacter",
                        column: x => x.UserCharacterID,
                        principalTable: "UserCharacter",
                        principalColumn: "UserCharacterID");
                });

            migrationBuilder.CreateTable(
                name: "OwnedDoll",
                columns: table => new
                {
                    OwnedDollID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    DollVariantID = table.Column<int>(type: "int", nullable: false),
                    SerialCode = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Acquired_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    Expired_at = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OwnedDoll", x => x.OwnedDollID);
                    table.ForeignKey(
                        name: "FK_OwnedDoll_DollVariant",
                        column: x => x.DollVariantID,
                        principalTable: "DollVariant",
                        principalColumn: "DollVariantID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OwnedDoll_User",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DollCharacterLink",
                columns: table => new
                {
                    LinkID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OwnedDollID = table.Column<int>(type: "int", nullable: false),
                    UserCharacterID = table.Column<int>(type: "int", nullable: false),
                    BoundAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UnBoundAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DollCharacterLink", x => x.LinkID);
                    table.ForeignKey(
                        name: "FK_DollCharacterLink_OwnedDoll",
                        column: x => x.OwnedDollID,
                        principalTable: "OwnedDoll",
                        principalColumn: "OwnedDollID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DollCharacterLink_UserCharacter",
                        column: x => x.UserCharacterID,
                        principalTable: "UserCharacter",
                        principalColumn: "UserCharacterID");
                });

            migrationBuilder.CreateTable(
                name: "Order",
                columns: table => new
                {
                    OrderID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: true),
                    PaymentID = table.Column<int>(type: "int", nullable: true),
                    OrderDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    ShippingAddress = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order", x => x.OrderID);
                    table.ForeignKey(
                        name: "FK_Order_User",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderItem",
                columns: table => new
                {
                    OrderItemID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderID = table.Column<int>(type: "int", nullable: false),
                    DollVariantID = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LineTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    OrderID1 = table.Column<int>(type: "int", nullable: true),
                    DollVariantID1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItem", x => x.OrderItemID);
                    table.ForeignKey(
                        name: "FK_OrderItem_DollVariant",
                        column: x => x.DollVariantID,
                        principalTable: "DollVariant",
                        principalColumn: "DollVariantID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderItem_DollVariant_DollVariantID1",
                        column: x => x.DollVariantID1,
                        principalTable: "DollVariant",
                        principalColumn: "DollVariantID");
                    table.ForeignKey(
                        name: "FK_OrderItem_Order",
                        column: x => x.OrderID,
                        principalTable: "Order",
                        principalColumn: "OrderID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderItem_Order_OrderID1",
                        column: x => x.OrderID1,
                        principalTable: "Order",
                        principalColumn: "OrderID");
                });

            migrationBuilder.CreateTable(
                name: "Payment",
                columns: table => new
                {
                    PaymentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Provider = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Method = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Pending"),
                    TransactionId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MoMoOrderId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PayUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    OrderInfo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    RawResponse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Target_Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Target_Id = table.Column<int>(type: "int", nullable: false),
                    OrderID = table.Column<int>(type: "int", nullable: true),
                    CharacterOrderID = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payment", x => x.PaymentID);
                    table.ForeignKey(
                        name: "FK_Payment_CharacterOrder",
                        column: x => x.CharacterOrderID,
                        principalTable: "CharacterOrder",
                        principalColumn: "CharacterOrderID",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Payment_Order",
                        column: x => x.OrderID,
                        principalTable: "Order",
                        principalColumn: "OrderID",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CharacterOrder_CharacterID",
                table: "CharacterOrder",
                column: "CharacterID");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterOrder_PackageID",
                table: "CharacterOrder",
                column: "PackageID");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterOrder_UserCharacterID",
                table: "CharacterOrder",
                column: "UserCharacterID");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterPackage_CharacterId",
                table: "CharacterPackage",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_DollCharacterLink_OwnedDollID",
                table: "DollCharacterLink",
                column: "OwnedDollID");

            migrationBuilder.CreateIndex(
                name: "IX_DollCharacterLink_UserCharacterID",
                table: "DollCharacterLink",
                column: "UserCharacterID");

            migrationBuilder.CreateIndex(
                name: "IX_DollModel_DollTypeID",
                table: "DollModel",
                column: "DollTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_DollVariant_DollModelID",
                table: "DollVariant",
                column: "DollModelID");

            migrationBuilder.CreateIndex(
                name: "IX_Order_PaymentID",
                table: "Order",
                column: "PaymentID");

            migrationBuilder.CreateIndex(
                name: "IX_Order_UserID",
                table: "Order",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItem_DollVariantID",
                table: "OrderItem",
                column: "DollVariantID");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItem_DollVariantID1",
                table: "OrderItem",
                column: "DollVariantID1");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItem_OrderID",
                table: "OrderItem",
                column: "OrderID");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItem_OrderID1",
                table: "OrderItem",
                column: "OrderID1");

            migrationBuilder.CreateIndex(
                name: "IX_OwnedDoll_DollVariantID",
                table: "OwnedDoll",
                column: "DollVariantID");

            migrationBuilder.CreateIndex(
                name: "IX_OwnedDoll_UserID",
                table: "OwnedDoll",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_CharacterOrderID",
                table: "Payment",
                column: "CharacterOrderID");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_OrderID",
                table: "Payment",
                column: "OrderID");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserID",
                table: "RefreshTokens",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_User_Email",
                table: "User",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_UserName",
                table: "User",
                column: "UserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserCharacter_CharacterID",
                table: "UserCharacter",
                column: "CharacterID");

            migrationBuilder.CreateIndex(
                name: "IX_UserCharacter_PackageId",
                table: "UserCharacter",
                column: "PackageId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCharacter_UserID",
                table: "UserCharacter",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_Payment",
                table: "Order",
                column: "PaymentID",
                principalTable: "Payment",
                principalColumn: "PaymentID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CharacterOrder_Character",
                table: "CharacterOrder");

            migrationBuilder.DropForeignKey(
                name: "FK_CharacterPackage_Character",
                table: "CharacterPackage");

            migrationBuilder.DropForeignKey(
                name: "FK_UserCharacter_Character",
                table: "UserCharacter");

            migrationBuilder.DropForeignKey(
                name: "FK_CharacterOrder_Package",
                table: "CharacterOrder");

            migrationBuilder.DropForeignKey(
                name: "FK_UserCharacter_Package",
                table: "UserCharacter");

            migrationBuilder.DropForeignKey(
                name: "FK_CharacterOrder_UserCharacter",
                table: "CharacterOrder");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_Payment",
                table: "Order");

            migrationBuilder.DropTable(
                name: "DollCharacterLink");

            migrationBuilder.DropTable(
                name: "OrderItem");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "OwnedDoll");

            migrationBuilder.DropTable(
                name: "DollVariant");

            migrationBuilder.DropTable(
                name: "DollModel");

            migrationBuilder.DropTable(
                name: "DollType");

            migrationBuilder.DropTable(
                name: "Character");

            migrationBuilder.DropTable(
                name: "CharacterPackage");

            migrationBuilder.DropTable(
                name: "UserCharacter");

            migrationBuilder.DropTable(
                name: "Payment");

            migrationBuilder.DropTable(
                name: "CharacterOrder");

            migrationBuilder.DropTable(
                name: "Order");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
