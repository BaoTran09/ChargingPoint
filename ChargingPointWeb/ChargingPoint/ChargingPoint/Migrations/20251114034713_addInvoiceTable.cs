using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChargingPoint.Migrations
{
    /// <inheritdoc />
    public partial class addInvoiceTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           
     
            migrationBuilder.CreateTable(
                name: "Invoices",
                columns: table => new
                {
                    InvoiceId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SessionId = table.Column<long>(type: "bigint", nullable: false),
                    Customer_Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Customer_Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Customer_Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Vehicle_Info = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Vehicle_LicensePlate = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Station_Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Station_Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Station_Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    InvoiceNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    InvoiceTemplate = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    InvoiceSymbol = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpireDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TotalEnergyKWh = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    UnitPrice = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    TotalCost = table.Column<decimal>(type: "decimal(15,2)", nullable: true),
                    OverDueMinutes = table.Column<int>(type: "int", nullable: true),
                    IdleUnitPrice = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Total_IdleFee = table.Column<decimal>(type: "decimal(15,2)", nullable: true),
                    ExtraFee_Explain = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ExtraFee = table.Column<decimal>(type: "decimal(15,2)", nullable: true),
                    Tax = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    TaxAmount = table.Column<decimal>(type: "decimal(15,2)", nullable: true),
                    FinalCost = table.Column<decimal>(type: "decimal(15,2)", nullable: true),
                    PaymentLink = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    QRCodeData = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PaidAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PaymentMethod = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TransactionId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PdfFilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    EmailSent = table.Column<bool>(type: "bit", nullable: false),
                    EmailSentAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Customer_Signature = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Signature = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoices", x => x.InvoiceId);
                    table.ForeignKey(
                        name: "FK_Invoices_ChargingSession_SessionId",
                        column: x => x.SessionId,
                        principalTable: "ChargingSession",
                        principalColumn: "SessionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_SessionId",
                table: "Invoices",
                column: "SessionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Invoices");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Vehicle");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Vehicle");

            migrationBuilder.DropColumn(
                name: "Phone_Number",
                table: "Station");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Charger");
        }
    }
}
