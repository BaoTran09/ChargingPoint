using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChargingPoint.Migrations
{
    /// <inheritdoc />
    public partial class Addsessionkey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Charger",
                columns: table => new
                {
                    ChargerId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StationId = table.Column<long>(type: "bigint", nullable: false),
                    SerialNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Model = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Manufacturer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChargerType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaxPowerKW = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Phases = table.Column<int>(type: "int", nullable: true),
                    OutputVoltageMin = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    OutputVoltageMax = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PortCount = table.Column<int>(type: "int", nullable: false),
                    Design = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Protections = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirmwareVersion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InstalledAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PicturePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UseFor = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Charger", x => x.ChargerId);
                    table.ForeignKey(
                        name: "FK_Charger_Station_StationId",
                        column: x => x.StationId,
                        principalTable: "Station",
                        principalColumn: "StationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Vehicle",
                columns: table => new
                {
                    VehicleId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<long>(type: "bigint", nullable: false),
                    VehicleType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Model = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BatteryType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Version = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BatteryGrossKWh = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    BatteryUsableKWh = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    AcChargingSupport = table.Column<bool>(type: "bit", nullable: false),
                    DcChargingSupport = table.Column<bool>(type: "bit", nullable: false),
                    MaxAcChargeKW = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MaxDcChargeKW = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    LicensePlate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VIN = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehicle", x => x.VehicleId);
                });

            migrationBuilder.CreateTable(
                name: "Connector",
                columns: table => new
                {
                    ConnectorId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChargerId = table.Column<long>(type: "bigint", nullable: false),
                    ConnectorIndex = table.Column<int>(type: "int", nullable: false),
                    ConnectorType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Connector", x => x.ConnectorId);
                    table.ForeignKey(
                        name: "FK_Connector_Charger_ChargerId",
                        column: x => x.ChargerId,
                        principalTable: "Charger",
                        principalColumn: "ChargerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChargingSession",
                columns: table => new
                {
                    SessionId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConnectorId = table.Column<long>(type: "bigint", nullable: false),
                    VehicleId = table.Column<long>(type: "bigint", nullable: true),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MeterStartKWh = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MeterStopKWh = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    StartSOC = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    EndSOC = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TargetSOC = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FullTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChargingSession", x => x.SessionId);
                    table.ForeignKey(
                        name: "FK_ChargingSession_Connector_ConnectorId",
                        column: x => x.ConnectorId,
                        principalTable: "Connector",
                        principalColumn: "ConnectorId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChargingSession_Vehicle_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "Vehicle",
                        principalColumn: "VehicleId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Charger_StationId",
                table: "Charger",
                column: "StationId");

            migrationBuilder.CreateIndex(
                name: "IX_ChargingSession_ConnectorId",
                table: "ChargingSession",
                column: "ConnectorId");

            migrationBuilder.CreateIndex(
                name: "IX_ChargingSession_VehicleId",
                table: "ChargingSession",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_Connector_ChargerId",
                table: "Connector",
                column: "ChargerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChargingSession");

            migrationBuilder.DropTable(
                name: "Connector");

            migrationBuilder.DropTable(
                name: "Vehicle");

            migrationBuilder.DropTable(
                name: "Charger");
        }
    }
}
