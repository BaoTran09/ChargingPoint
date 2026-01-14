using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChargingPoint.Migrations
{
    /// <inheritdoc />
    public partial class UpdateVehicleStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChargingSession_Connector_ConnectorId",
                table: "ChargingSession");

            migrationBuilder.DropForeignKey(
                name: "FK_ChargingSession_IndividualVehicle_IndividualVehicleVIN",
                table: "ChargingSession");

            migrationBuilder.DropForeignKey(
                name: "FK_ChargingSession_Vehicle_VehicleId",
                table: "ChargingSession");

            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceDetail_Invoices_InvoiceId",
                table: "InvoiceDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_ChargingSession_SessionId",
                table: "Invoices");

            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Customer_CustomerId",
                table: "Invoices");

            migrationBuilder.DropForeignKey(
                name: "FK_Receipt_Invoices_InvoiceId",
                table: "Receipt");

            migrationBuilder.DropForeignKey(
                name: "FK_ReceiptDetail_Invoices_RefInvoiceId",
                table: "ReceiptDetail");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Invoices",
                table: "Invoices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChargingSession",
                table: "ChargingSession");

            migrationBuilder.DropColumn(
                name: "LicensePlate",
                table: "Vehicle");

            migrationBuilder.DropColumn(
                name: "ProductionDate",
                table: "Vehicle");

            migrationBuilder.RenameTable(
                name: "Invoices",
                newName: "Invoice");

            migrationBuilder.RenameTable(
                name: "ChargingSession",
                newName: "ChargingSessions");

            migrationBuilder.RenameColumn(
                name: "VIN",
                table: "Vehicle",
                newName: "ConnectorType");

            migrationBuilder.RenameIndex(
                name: "IX_Invoices_SessionId",
                table: "Invoice",
                newName: "IX_Invoice_SessionId");

            migrationBuilder.RenameIndex(
                name: "IX_Invoices_CustomerId",
                table: "Invoice",
                newName: "IX_Invoice_CustomerId");

            migrationBuilder.RenameIndex(
                name: "IX_ChargingSession_VehicleId",
                table: "ChargingSessions",
                newName: "IX_ChargingSessions_VehicleId");

            migrationBuilder.RenameIndex(
                name: "IX_ChargingSession_IndividualVehicleVIN",
                table: "ChargingSessions",
                newName: "IX_ChargingSessions_IndividualVehicleVIN");

            migrationBuilder.RenameIndex(
                name: "IX_ChargingSession_ConnectorId",
                table: "ChargingSessions",
                newName: "IX_ChargingSessions_ConnectorId");

            migrationBuilder.AddColumn<int>(
                name: "NominalVoltage",
                table: "Vehicle",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BatterySOC",
                table: "IndividualVehicle",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BatterySOH",
                table: "IndividualVehicle",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "IndividualVehicle",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ProductionDate",
                table: "IndividualVehicle",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Invoice",
                table: "Invoice",
                column: "InvoiceId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChargingSessions",
                table: "ChargingSessions",
                column: "SessionId");

            migrationBuilder.CreateTable(
                name: "ChargingCurve",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VehicleId = table.Column<long>(type: "bigint", nullable: false),
                    SocFrom = table.Column<int>(type: "int", nullable: false),
                    SocTo = table.Column<int>(type: "int", nullable: false),
                    MaxPowerKW = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsDcFastCharge = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChargingCurve", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChargingCurve_Vehicle_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "Vehicle",
                        principalColumn: "VehicleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Images",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntityType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EntityId = table.Column<long>(type: "bigint", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PublicId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Caption = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Images", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChargingCurve_VehicleId",
                table: "ChargingCurve",
                column: "VehicleId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChargingSessions_Connector_ConnectorId",
                table: "ChargingSessions",
                column: "ConnectorId",
                principalTable: "Connector",
                principalColumn: "ConnectorId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChargingSessions_IndividualVehicle_IndividualVehicleVIN",
                table: "ChargingSessions",
                column: "IndividualVehicleVIN",
                principalTable: "IndividualVehicle",
                principalColumn: "VIN");

            migrationBuilder.AddForeignKey(
                name: "FK_ChargingSessions_Vehicle_VehicleId",
                table: "ChargingSessions",
                column: "VehicleId",
                principalTable: "Vehicle",
                principalColumn: "VehicleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoice_ChargingSessions_SessionId",
                table: "Invoice",
                column: "SessionId",
                principalTable: "ChargingSessions",
                principalColumn: "SessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoice_Customer_CustomerId",
                table: "Invoice",
                column: "CustomerId",
                principalTable: "Customer",
                principalColumn: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceDetail_Invoice_InvoiceId",
                table: "InvoiceDetail",
                column: "InvoiceId",
                principalTable: "Invoice",
                principalColumn: "InvoiceId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Receipt_Invoice_InvoiceId",
                table: "Receipt",
                column: "InvoiceId",
                principalTable: "Invoice",
                principalColumn: "InvoiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReceiptDetail_Invoice_RefInvoiceId",
                table: "ReceiptDetail",
                column: "RefInvoiceId",
                principalTable: "Invoice",
                principalColumn: "InvoiceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChargingSessions_Connector_ConnectorId",
                table: "ChargingSessions");

            migrationBuilder.DropForeignKey(
                name: "FK_ChargingSessions_IndividualVehicle_IndividualVehicleVIN",
                table: "ChargingSessions");

            migrationBuilder.DropForeignKey(
                name: "FK_ChargingSessions_Vehicle_VehicleId",
                table: "ChargingSessions");

            migrationBuilder.DropForeignKey(
                name: "FK_Invoice_ChargingSessions_SessionId",
                table: "Invoice");

            migrationBuilder.DropForeignKey(
                name: "FK_Invoice_Customer_CustomerId",
                table: "Invoice");

            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceDetail_Invoice_InvoiceId",
                table: "InvoiceDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_Receipt_Invoice_InvoiceId",
                table: "Receipt");

            migrationBuilder.DropForeignKey(
                name: "FK_ReceiptDetail_Invoice_RefInvoiceId",
                table: "ReceiptDetail");

            migrationBuilder.DropTable(
                name: "ChargingCurve");

            migrationBuilder.DropTable(
                name: "Images");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Invoice",
                table: "Invoice");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChargingSessions",
                table: "ChargingSessions");

            migrationBuilder.DropColumn(
                name: "NominalVoltage",
                table: "Vehicle");

            migrationBuilder.DropColumn(
                name: "BatterySOC",
                table: "IndividualVehicle");

            migrationBuilder.DropColumn(
                name: "BatterySOH",
                table: "IndividualVehicle");

            migrationBuilder.DropColumn(
                name: "Color",
                table: "IndividualVehicle");

            migrationBuilder.DropColumn(
                name: "ProductionDate",
                table: "IndividualVehicle");

            migrationBuilder.RenameTable(
                name: "Invoice",
                newName: "Invoices");

            migrationBuilder.RenameTable(
                name: "ChargingSessions",
                newName: "ChargingSession");

            migrationBuilder.RenameColumn(
                name: "ConnectorType",
                table: "Vehicle",
                newName: "VIN");

            migrationBuilder.RenameIndex(
                name: "IX_Invoice_SessionId",
                table: "Invoices",
                newName: "IX_Invoices_SessionId");

            migrationBuilder.RenameIndex(
                name: "IX_Invoice_CustomerId",
                table: "Invoices",
                newName: "IX_Invoices_CustomerId");

            migrationBuilder.RenameIndex(
                name: "IX_ChargingSessions_VehicleId",
                table: "ChargingSession",
                newName: "IX_ChargingSession_VehicleId");

            migrationBuilder.RenameIndex(
                name: "IX_ChargingSessions_IndividualVehicleVIN",
                table: "ChargingSession",
                newName: "IX_ChargingSession_IndividualVehicleVIN");

            migrationBuilder.RenameIndex(
                name: "IX_ChargingSessions_ConnectorId",
                table: "ChargingSession",
                newName: "IX_ChargingSession_ConnectorId");

            migrationBuilder.AddColumn<string>(
                name: "LicensePlate",
                table: "Vehicle",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ProductionDate",
                table: "Vehicle",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Invoices",
                table: "Invoices",
                column: "InvoiceId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChargingSession",
                table: "ChargingSession",
                column: "SessionId");

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    TransactionId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    InvoiceId = table.Column<long>(type: "bigint", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Content = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    FromAccountName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FromBankCode = table.Column<int>(type: "int", nullable: true),
                    FromBankName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    InvoiceNumber = table.Column<long>(type: "bigint", nullable: true),
                    ToAccountName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ToBankCode = table.Column<int>(type: "int", nullable: true),
                    ToBankName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TransactionCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TransactionType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.TransactionId);
                    table.ForeignKey(
                        name: "FK_Transactions_Invoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoices",
                        principalColumn: "InvoiceId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_InvoiceId",
                table: "Transactions",
                column: "InvoiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChargingSession_Connector_ConnectorId",
                table: "ChargingSession",
                column: "ConnectorId",
                principalTable: "Connector",
                principalColumn: "ConnectorId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChargingSession_IndividualVehicle_IndividualVehicleVIN",
                table: "ChargingSession",
                column: "IndividualVehicleVIN",
                principalTable: "IndividualVehicle",
                principalColumn: "VIN");

            migrationBuilder.AddForeignKey(
                name: "FK_ChargingSession_Vehicle_VehicleId",
                table: "ChargingSession",
                column: "VehicleId",
                principalTable: "Vehicle",
                principalColumn: "VehicleId");

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceDetail_Invoices_InvoiceId",
                table: "InvoiceDetail",
                column: "InvoiceId",
                principalTable: "Invoices",
                principalColumn: "InvoiceId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_ChargingSession_SessionId",
                table: "Invoices",
                column: "SessionId",
                principalTable: "ChargingSession",
                principalColumn: "SessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Customer_CustomerId",
                table: "Invoices",
                column: "CustomerId",
                principalTable: "Customer",
                principalColumn: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Receipt_Invoices_InvoiceId",
                table: "Receipt",
                column: "InvoiceId",
                principalTable: "Invoices",
                principalColumn: "InvoiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReceiptDetail_Invoices_RefInvoiceId",
                table: "ReceiptDetail",
                column: "RefInvoiceId",
                principalTable: "Invoices",
                principalColumn: "InvoiceId");
        }
    }
}
