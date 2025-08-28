using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HRMS.Migrations
{
    /// <inheritdoc />
    public partial class SeedDepartmentsAndPositions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "departments",
                columns: new[] { "id", "created_at", "description", "head_employee_id", "name" },
                values: new object[,]
                {
                    { new Guid("550e8400-e29b-41d4-a716-446655440000"), new DateTime(2025, 8, 23, 21, 42, 49, 475, DateTimeKind.Utc).AddTicks(3815), "Human Resource dept", null, "HR" },
                    { new Guid("550e8400-e29b-41d4-a716-446655440001"), new DateTime(2025, 8, 23, 21, 42, 49, 475, DateTimeKind.Utc).AddTicks(4199), "engineering dept", null, "Engineering" },
                    { new Guid("550e8400-e29b-41d4-a716-446655440002"), new DateTime(2025, 8, 23, 21, 42, 49, 475, DateTimeKind.Utc).AddTicks(4202), "Finance dept", null, "Finance" }
                });

            migrationBuilder.InsertData(
                table: "positions",
                columns: new[] { "id", "created_at", "department_id", "description", "salary_range_max", "salary_range_min", "title" },
                values: new object[,]
                {
                    { new Guid("550e8400-e29b-41d4-a716-446655440100"), new DateTime(2025, 8, 23, 21, 42, 49, 471, DateTimeKind.Utc).AddTicks(3066), new Guid("550e8400-e29b-41d4-a716-446655440000"), "human resource manager", 200000.00m, 320000.00m, "HR Manager" },
                    { new Guid("550e8400-e29b-41d4-a716-446655440101"), new DateTime(2025, 8, 23, 21, 42, 49, 471, DateTimeKind.Utc).AddTicks(3447), new Guid("550e8400-e29b-41d4-a716-446655440001"), "snr soft engr", 200000.00m, 150000.00m, "Software Engineer" },
                    { new Guid("550e8400-e29b-41d4-a716-446655440102"), new DateTime(2025, 8, 23, 21, 42, 49, 471, DateTimeKind.Utc).AddTicks(3451), new Guid("550e8400-e29b-41d4-a716-446655440002"), "snr soft engr", 200000.00m, 320000.00m, "Financial Analyst" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "positions",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440100"));

            migrationBuilder.DeleteData(
                table: "positions",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440101"));

            migrationBuilder.DeleteData(
                table: "positions",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440102"));

            migrationBuilder.DeleteData(
                table: "departments",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440000"));

            migrationBuilder.DeleteData(
                table: "departments",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440001"));

            migrationBuilder.DeleteData(
                table: "departments",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440002"));
        }
    }
}
