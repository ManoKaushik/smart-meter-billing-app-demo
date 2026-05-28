using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartMeterWeb.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedPaymentStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Billings_PaidStatus",
                table: "Billings");

            migrationBuilder.AddColumn<double>(
                name: "AmountPaid",
                table: "Billings",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.UpdateData(
                table: "Billings",
                keyColumn: "BillId",
                keyValue: 1L,
                column: "AmountPaid",
                value: 0.0);

            migrationBuilder.UpdateData(
                table: "Billings",
                keyColumn: "BillId",
                keyValue: 2L,
                column: "AmountPaid",
                value: 0.0);

            migrationBuilder.UpdateData(
                table: "Billings",
                keyColumn: "BillId",
                keyValue: 3L,
                column: "AmountPaid",
                value: 0.0);

            migrationBuilder.UpdateData(
                table: "Billings",
                keyColumn: "BillId",
                keyValue: 4L,
                column: "AmountPaid",
                value: 0.0);

            migrationBuilder.UpdateData(
                table: "Billings",
                keyColumn: "BillId",
                keyValue: 5L,
                column: "AmountPaid",
                value: 0.0);

            migrationBuilder.AddCheckConstraint(
                name: "CK_Billings_PaidStatus",
                table: "Billings",
                sql: "\"PaymentStatus\" IN ('Paid','Unpaid','Overdue','Cancelled','Partially-Paid')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Billings_PaidStatus",
                table: "Billings");

            migrationBuilder.DropColumn(
                name: "AmountPaid",
                table: "Billings");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Billings_PaidStatus",
                table: "Billings",
                sql: "\"PaymentStatus\" IN ('Paid','Unpaid','Overdue','Cancelled')");
        }
    }
}
