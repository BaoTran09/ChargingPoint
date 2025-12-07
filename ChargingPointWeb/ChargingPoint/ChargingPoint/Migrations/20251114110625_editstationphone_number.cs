using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChargingPoint.Migrations
{
    /// <inheritdoc />
    public partial class editstationphone_number : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CustomerID",
                table: "Invoices",
                type: "bigint",
                nullable: true,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "StationId",
                table: "Invoices",
                type: "bigint",
                nullable: true,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_CustomerID",
                table: "Invoices",
                column: "CustomerID");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_StationId",
                table: "Invoices",
                column: "StationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Customer_CustomerID",
                table: "Invoices",
                column: "CustomerID",
                principalTable: "Customer",
                principalColumn: "CustomerID",
                onDelete: ReferentialAction.Restrict);

          
            migrationBuilder.AddForeignKey(
                 name: "FK_Invoices_Station_StationId",
                 table: "Invoices",
                 column: "StationId",
                 principalTable: "Station",
                 principalColumn: "StationId",
                 onDelete: ReferentialAction.Restrict // thay vì Cascade
);

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Customer_CustomerID",
                table: "Invoices");

            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Station_StationId",
                table: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_CustomerID",
                table: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_StationId",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "CustomerID",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "StationId",
                table: "Invoices");
        }
    }
}
