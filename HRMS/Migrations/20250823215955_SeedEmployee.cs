using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRMS.Migrations
{
    /// <inheritdoc />
    public partial class SeedEmployee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "departments",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440000"),
                column: "created_at",
                value: new DateTime(2025, 8, 23, 21, 59, 54, 969, DateTimeKind.Utc).AddTicks(6075));

            migrationBuilder.UpdateData(
                table: "departments",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440001"),
                column: "created_at",
                value: new DateTime(2025, 8, 23, 21, 59, 54, 969, DateTimeKind.Utc).AddTicks(6441));

            migrationBuilder.UpdateData(
                table: "departments",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440002"),
                column: "created_at",
                value: new DateTime(2025, 8, 23, 21, 59, 54, 969, DateTimeKind.Utc).AddTicks(6444));

            migrationBuilder.InsertData(
                table: "employees",
                columns: new[] { "id", "AccessFailedCount", "address", "ConcurrencyStamp", "created_at", "dob", "department_id", "Email", "EmailConfirmed", "first_name", "hire_date", "last_name", "LockoutEnabled", "LockoutEnd", "manager_id", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "position_id", "salary", "SecurityStamp", "total_leaves", "TwoFactorEnabled", "UserName" },
                values: new object[] { new Guid("11111111-1111-1111-1111-111111111111"), 0, "HQ", "2fa3eb2a-37ad-45f1-99a3-42df9a324632", new DateTime(2025, 8, 23, 21, 59, 54, 895, DateTimeKind.Utc).AddTicks(738), new DateTime(2003, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("550e8400-e29b-41d4-a716-446655440001"), "ruhma@email.com", true, "Ruhma", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Anwar", false, null, null, "RUHMA@EMAIL.COM", "RUHMA@EMAIL.COM", "AQAAAAIAAYagAAAAEFDPLWpCPf7nnCI9AW+ArhwTrzTHHAMjwijzk/KZksKDKW6MU+otNL5LPFiMRuFj/w==", "03077147775", true, new Guid("550e8400-e29b-41d4-a716-446655440101"), 1000m, "42b4633f-94c3-40df-93c1-45c94b86abda", 25, false, "ruhma@email.com" });

            migrationBuilder.UpdateData(
                table: "positions",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440100"),
                column: "created_at",
                value: new DateTime(2025, 8, 23, 21, 59, 54, 965, DateTimeKind.Utc).AddTicks(6908));

            migrationBuilder.UpdateData(
                table: "positions",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440101"),
                column: "created_at",
                value: new DateTime(2025, 8, 23, 21, 59, 54, 965, DateTimeKind.Utc).AddTicks(7309));

            migrationBuilder.UpdateData(
                table: "positions",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440102"),
                column: "created_at",
                value: new DateTime(2025, 8, 23, 21, 59, 54, 965, DateTimeKind.Utc).AddTicks(7313));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "employees",
                keyColumn: "id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.UpdateData(
                table: "departments",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440000"),
                column: "created_at",
                value: new DateTime(2025, 8, 23, 21, 42, 49, 475, DateTimeKind.Utc).AddTicks(3815));

            migrationBuilder.UpdateData(
                table: "departments",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440001"),
                column: "created_at",
                value: new DateTime(2025, 8, 23, 21, 42, 49, 475, DateTimeKind.Utc).AddTicks(4199));

            migrationBuilder.UpdateData(
                table: "departments",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440002"),
                column: "created_at",
                value: new DateTime(2025, 8, 23, 21, 42, 49, 475, DateTimeKind.Utc).AddTicks(4202));

            migrationBuilder.UpdateData(
                table: "positions",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440100"),
                column: "created_at",
                value: new DateTime(2025, 8, 23, 21, 42, 49, 471, DateTimeKind.Utc).AddTicks(3066));

            migrationBuilder.UpdateData(
                table: "positions",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440101"),
                column: "created_at",
                value: new DateTime(2025, 8, 23, 21, 42, 49, 471, DateTimeKind.Utc).AddTicks(3447));

            migrationBuilder.UpdateData(
                table: "positions",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440102"),
                column: "created_at",
                value: new DateTime(2025, 8, 23, 21, 42, 49, 471, DateTimeKind.Utc).AddTicks(3451));
        }
    }
}
