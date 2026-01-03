using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoldPro.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BusinessProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BusinessName = table.Column<string>(type: "text", nullable: false),
                    Gstin = table.Column<string>(type: "text", nullable: false),
                    Pan = table.Column<string>(type: "text", nullable: false),
                    Phone = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false),
                    StateCode = table.Column<string>(type: "text", nullable: false),
                    StateName = table.Column<string>(type: "text", nullable: false),
                    Gold24KRate = table.Column<decimal>(type: "numeric", nullable: false),
                    Gold22KRate = table.Column<decimal>(type: "numeric", nullable: false),
                    Gold18KRate = table.Column<decimal>(type: "numeric", nullable: false),
                    SilverRateGram = table.Column<decimal>(type: "numeric", nullable: false),
                    SilverRateKg = table.Column<decimal>(type: "numeric", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessProfiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FullName = table.Column<string>(type: "text", nullable: false),
                    PhoneNumber = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false),
                    Gstin = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Estimates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: true),
                    CustomerName = table.Column<string>(type: "text", nullable: true),
                    IsInterState = table.Column<bool>(type: "boolean", nullable: false),
                    GoldValue = table.Column<decimal>(type: "numeric", nullable: false),
                    MakingCharges = table.Column<decimal>(type: "numeric", nullable: false),
                    Deduction = table.Column<decimal>(type: "numeric", nullable: false),
                    Subtotal = table.Column<decimal>(type: "numeric", nullable: false),
                    GstPercent = table.Column<decimal>(type: "numeric", nullable: false),
                    Cgst = table.Column<decimal>(type: "numeric", nullable: false),
                    Sgst = table.Column<decimal>(type: "numeric", nullable: false),
                    Igst = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalGstAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    EstimatedTotal = table.Column<decimal>(type: "numeric", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Estimates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InvoiceSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Prefix = table.Column<string>(type: "text", nullable: false),
                    StartingNumber = table.Column<int>(type: "integer", nullable: false),
                    ShowLogo = table.Column<bool>(type: "boolean", nullable: false),
                    ShowQr = table.Column<bool>(type: "boolean", nullable: false),
                    FooterText = table.Column<string>(type: "text", nullable: false),
                    Terms = table.Column<string>(type: "text", nullable: false),
                    Currency = table.Column<string>(type: "text", nullable: false),
                    DateFormat = table.Column<string>(type: "text", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NotificationSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmailEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    SmsEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    NewSale = table.Column<bool>(type: "boolean", nullable: false),
                    PaymentReminder = table.Column<bool>(type: "boolean", nullable: false),
                    LowStock = table.Column<bool>(type: "boolean", nullable: false),
                    MonthlyReport = table.Column<bool>(type: "boolean", nullable: false),
                    InvoiceGenerated = table.Column<bool>(type: "boolean", nullable: false),
                    NewCustomer = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OldGoldSlips",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: true),
                    CustomerName = table.Column<string>(type: "text", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    GoldValue = table.Column<decimal>(type: "numeric", nullable: false),
                    DeductionPercent = table.Column<decimal>(type: "numeric", nullable: false),
                    DeductionValue = table.Column<decimal>(type: "numeric", nullable: false),
                    NetPayable = table.Column<decimal>(type: "numeric", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OldGoldSlips", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: true),
                    CustomerName = table.Column<string>(type: "text", nullable: true),
                    DueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    GoldValue = table.Column<decimal>(type: "numeric", nullable: false),
                    MakingCharges = table.Column<decimal>(type: "numeric", nullable: false),
                    Deduction = table.Column<decimal>(type: "numeric", nullable: false),
                    Subtotal = table.Column<decimal>(type: "numeric", nullable: false),
                    GstPercent = table.Column<decimal>(type: "numeric", nullable: false),
                    Cgst = table.Column<decimal>(type: "numeric", nullable: false),
                    Sgst = table.Column<decimal>(type: "numeric", nullable: false),
                    Igst = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalGstAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    EstimatedTotal = table.Column<decimal>(type: "numeric", nullable: false),
                    AdvanceReceived = table.Column<decimal>(type: "numeric", nullable: false),
                    AmountPayable = table.Column<decimal>(type: "numeric", nullable: false),
                    PaymentMethod = table.Column<string>(type: "text", nullable: false),
                    PaymentStatus = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sales",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: true),
                    CustomerName = table.Column<string>(type: "text", nullable: true),
                    IsInterState = table.Column<bool>(type: "boolean", nullable: false),
                    GoldValue = table.Column<decimal>(type: "numeric", nullable: false),
                    MakingCharges = table.Column<decimal>(type: "numeric", nullable: false),
                    Deduction = table.Column<decimal>(type: "numeric", nullable: false),
                    Subtotal = table.Column<decimal>(type: "numeric", nullable: false),
                    GstPercent = table.Column<decimal>(type: "numeric", nullable: false),
                    Cgst = table.Column<decimal>(type: "numeric", nullable: false),
                    Sgst = table.Column<decimal>(type: "numeric", nullable: false),
                    Igst = table.Column<decimal>(type: "numeric", nullable: false),
                    GrandTotal = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalGstAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    PaymentMethod = table.Column<string>(type: "text", nullable: false),
                    PaymentStatus = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sales", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StockItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Purity = table.Column<string>(type: "text", nullable: false),
                    WeightGrams = table.Column<decimal>(type: "numeric", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    HsnCode = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tenants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FullName = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Phone = table.Column<string>(type: "text", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<string>(type: "text", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EstimateItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EstimateId = table.Column<Guid>(type: "uuid", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    WeightGrams = table.Column<decimal>(type: "numeric", nullable: false),
                    RatePerGram = table.Column<decimal>(type: "numeric", nullable: false),
                    MakingCharges = table.Column<decimal>(type: "numeric", nullable: false),
                    WastagePercent = table.Column<decimal>(type: "numeric", nullable: false),
                    Purity = table.Column<string>(type: "text", nullable: false),
                    GoldValue = table.Column<decimal>(type: "numeric", nullable: false),
                    DeductionValue = table.Column<decimal>(type: "numeric", nullable: false),
                    GstValue = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstimateItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EstimateItems_Estimates_EstimateId",
                        column: x => x.EstimateId,
                        principalTable: "Estimates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OldGoldItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SlipId = table.Column<Guid>(type: "uuid", nullable: false),
                    Purity = table.Column<string>(type: "text", nullable: false),
                    WeightGrams = table.Column<decimal>(type: "numeric", nullable: false),
                    RatePerGram = table.Column<decimal>(type: "numeric", nullable: false),
                    DeductionPercent = table.Column<decimal>(type: "numeric", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    GrossValue = table.Column<decimal>(type: "numeric", nullable: false),
                    DeductionValue = table.Column<decimal>(type: "numeric", nullable: false),
                    NetValue = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OldGoldItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OldGoldItems_OldGoldSlips_SlipId",
                        column: x => x.SlipId,
                        principalTable: "OldGoldSlips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Purity = table.Column<string>(type: "text", nullable: false),
                    WeightGrams = table.Column<decimal>(type: "numeric", nullable: false),
                    RatePerGram = table.Column<decimal>(type: "numeric", nullable: false),
                    MakingCharges = table.Column<decimal>(type: "numeric", nullable: false),
                    WastagePercent = table.Column<decimal>(type: "numeric", nullable: false),
                    GoldValue = table.Column<decimal>(type: "numeric", nullable: false),
                    DeductionValue = table.Column<decimal>(type: "numeric", nullable: false),
                    GstValue = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SaleItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SaleId = table.Column<Guid>(type: "uuid", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    WeightGrams = table.Column<decimal>(type: "numeric", nullable: false),
                    RatePerGram = table.Column<decimal>(type: "numeric", nullable: false),
                    WastagePercent = table.Column<decimal>(type: "numeric", nullable: false),
                    Purity = table.Column<string>(type: "text", nullable: false),
                    MakingCharges = table.Column<decimal>(type: "numeric", nullable: false),
                    GoldValue = table.Column<decimal>(type: "numeric", nullable: false),
                    DeductionValue = table.Column<decimal>(type: "numeric", nullable: false),
                    GstValue = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SaleItems_Sales_SaleId",
                        column: x => x.SaleId,
                        principalTable: "Sales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EstimateItems_EstimateId",
                table: "EstimateItems",
                column: "EstimateId");

            migrationBuilder.CreateIndex(
                name: "IX_OldGoldItems_SlipId",
                table: "OldGoldItems",
                column: "SlipId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId",
                table: "OrderItems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleItems_SaleId",
                table: "SaleItems",
                column: "SaleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BusinessProfiles");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "EstimateItems");

            migrationBuilder.DropTable(
                name: "InvoiceSettings");

            migrationBuilder.DropTable(
                name: "NotificationSettings");

            migrationBuilder.DropTable(
                name: "OldGoldItems");

            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "SaleItems");

            migrationBuilder.DropTable(
                name: "StockItems");

            migrationBuilder.DropTable(
                name: "Tenants");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Estimates");

            migrationBuilder.DropTable(
                name: "OldGoldSlips");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Sales");
        }
    }
}
