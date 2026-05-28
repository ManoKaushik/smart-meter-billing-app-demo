using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartMeterWeb.Migrations
{
    /// <inheritdoc />
    public partial class LoginLockConsumer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FailedLoginAttempts",
                table: "Consumers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "LoginLockEnd",
                table: "Consumers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Consumers",
                keyColumn: "ConsumerId",
                keyValue: 1L,
                columns: new[] { "FailedLoginAttempts", "LoginLockEnd" },
                values: new object[] { 0, null });

            migrationBuilder.UpdateData(
                table: "Consumers",
                keyColumn: "ConsumerId",
                keyValue: 2L,
                columns: new[] { "FailedLoginAttempts", "LoginLockEnd" },
                values: new object[] { 0, null });

            migrationBuilder.UpdateData(
                table: "Consumers",
                keyColumn: "ConsumerId",
                keyValue: 3L,
                columns: new[] { "FailedLoginAttempts", "LoginLockEnd" },
                values: new object[] { 0, null });

            migrationBuilder.UpdateData(
                table: "Consumers",
                keyColumn: "ConsumerId",
                keyValue: 4L,
                columns: new[] { "FailedLoginAttempts", "LoginLockEnd" },
                values: new object[] { 0, null });

            migrationBuilder.UpdateData(
                table: "Consumers",
                keyColumn: "ConsumerId",
                keyValue: 5L,
                columns: new[] { "FailedLoginAttempts", "LoginLockEnd" },
                values: new object[] { 0, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FailedLoginAttempts",
                table: "Consumers");

            migrationBuilder.DropColumn(
                name: "LoginLockEnd",
                table: "Consumers");
        }
    }
}
