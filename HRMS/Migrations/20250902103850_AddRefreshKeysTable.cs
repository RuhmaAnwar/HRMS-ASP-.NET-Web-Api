using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HRMS.Migrations
{
    /// <inheritdoc />
    public partial class AddRefreshKeysTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.CreateTable(
            //    name: "roles",
            //    columns: table => new
            //    {
            //        Id = table.Column<Guid>(type: "uuid", nullable: false),
            //        Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
            //        NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
            //        ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_roles", x => x.Id);
            //    });

            migrationBuilder.CreateTable(
                name: "signing_keys",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    KeyId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PrivateKey = table.Column<string>(type: "text", nullable: false),
                    PublicKey = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_signing_keys", x => x.Id);
                });

            //migrationBuilder.CreateTable(
                //name: "role_claims",
                //columns: table => new
                //{
                //    Id = table.Column<int>(type: "integer", nullable: false)
                //        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                //    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                //    ClaimType = table.Column<string>(type: "text", nullable: true),
                //    ClaimValue = table.Column<string>(type: "text", nullable: true)
                //},
                //constraints: table =>
                //{
                //    table.PrimaryKey("PK_role_claims", x => x.Id);
                //    table.ForeignKey(
                //        name: "FK_role_claims_roles_RoleId",
                //        column: x => x.RoleId,
                //        principalTable: "roles",
                //        principalColumn: "Id",
                //        onDelete: ReferentialAction.Cascade);
                //});

            //migrationBuilder.CreateTable(
            //    name: "attendances",
            //    columns: table => new
            //    {
            //        id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
            //        employee_id = table.Column<Guid>(type: "uuid", nullable: false),
            //        date = table.Column<DateOnly>(type: "date", nullable: false),
            //        check_in_time = table.Column<TimeOnly>(type: "time", nullable: true),
            //        check_out_time = table.Column<TimeOnly>(type: "time", nullable: true),
            //        status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Pending"),
            //        created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
            //        updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
            //        is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_attendances", x => x.id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "departments",
            //    columns: table => new
            //    {
            //        id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
            //        name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
            //        description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
            //        head_employee_id = table.Column<Guid>(type: "uuid", nullable: true),
            //        created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
            //        updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
            //        is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_departments", x => x.id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "positions",
            //    columns: table => new
            //    {
            //        id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
            //        title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
            //        description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
            //        salary_range_min = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
            //        salary_range_max = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
            //        department_id = table.Column<Guid>(type: "uuid", nullable: true),
            //        created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
            //        updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
            //        is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_positions", x => x.id);
            //        table.ForeignKey(
            //            name: "FK_positions_departments_department_id",
            //            column: x => x.department_id,
            //            principalTable: "departments",
            //            principalColumn: "id",
            //            onDelete: ReferentialAction.SetNull);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "employees",
            //    columns: table => new
            //    {
            //        id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
            //        first_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
            //        last_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
            //        dob = table.Column<DateTime>(type: "date", nullable: false),
            //        address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
            //        PhoneNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
            //        hire_date = table.Column<DateTime>(type: "date", nullable: false),
            //        salary = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
            //        created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
            //        updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
            //        is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
            //        department_id = table.Column<Guid>(type: "uuid", nullable: false),
            //        position_id = table.Column<Guid>(type: "uuid", nullable: false),
            //        manager_id = table.Column<Guid>(type: "uuid", nullable: true),
            //        total_leaves = table.Column<int>(type: "integer", nullable: false, defaultValue: 25),
            //        leaves_used = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
            //        UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
            //        NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
            //        Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
            //        NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
            //        EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
            //        PasswordHash = table.Column<string>(type: "text", nullable: true),
            //        SecurityStamp = table.Column<string>(type: "text", nullable: true),
            //        ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
            //        PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
            //        TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
            //        LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
            //        LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
            //        AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_employees", x => x.id);
            //        table.ForeignKey(
            //            name: "FK_employees_departments_department_id",
            //            column: x => x.department_id,
            //            principalTable: "departments",
            //            principalColumn: "id",
            //            onDelete: ReferentialAction.Restrict);
            //        table.ForeignKey(
            //            name: "FK_employees_employees_manager_id",
            //            column: x => x.manager_id,
            //            principalTable: "employees",
            //            principalColumn: "id",
            //            onDelete: ReferentialAction.SetNull);
            //        table.ForeignKey(
            //            name: "FK_employees_positions_position_id",
            //            column: x => x.position_id,
            //            principalTable: "positions",
            //            principalColumn: "id",
            //            onDelete: ReferentialAction.Restrict);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "leaves",
            //    columns: table => new
            //    {
            //        id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
            //        employee_id = table.Column<Guid>(type: "uuid", nullable: false),
            //        start_date = table.Column<DateOnly>(type: "date", nullable: false),
            //        end_date = table.Column<DateOnly>(type: "date", nullable: false),
            //        type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Casual"),
            //        status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Pending"),
            //        approver_id = table.Column<Guid>(type: "uuid", nullable: true),
            //        reason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
            //        created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
            //        updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
            //        is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_leaves", x => x.id);
            //        table.ForeignKey(
            //            name: "FK_leaves_employees_approver_id",
            //            column: x => x.approver_id,
            //            principalTable: "employees",
            //            principalColumn: "id",
            //            onDelete: ReferentialAction.SetNull);
            //        table.ForeignKey(
            //            name: "FK_leaves_employees_employee_id",
            //            column: x => x.employee_id,
            //            principalTable: "employees",
            //            principalColumn: "id",
            //            onDelete: ReferentialAction.Restrict);
            //    });

            migrationBuilder.CreateTable(
                name: "refresh_tokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Token = table.Column<string>(type: "text", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsRevoked = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RevokedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_refresh_tokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_refresh_tokens_employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "employees",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            //migrationBuilder.CreateTable(
            //    name: "user_claims",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "integer", nullable: false)
            //            .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
            //        UserId = table.Column<Guid>(type: "uuid", nullable: false),
            //        ClaimType = table.Column<string>(type: "text", nullable: true),
            //        ClaimValue = table.Column<string>(type: "text", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_user_claims", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_user_claims_employees_UserId",
            //            column: x => x.UserId,
            //            principalTable: "employees",
            //            principalColumn: "id",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "user_logins",
            //    columns: table => new
            //    {
            //        LoginProvider = table.Column<string>(type: "text", nullable: false),
            //        ProviderKey = table.Column<string>(type: "text", nullable: false),
            //        ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
            //        UserId = table.Column<Guid>(type: "uuid", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_user_logins", x => new { x.LoginProvider, x.ProviderKey });
            //        table.ForeignKey(
            //            name: "FK_user_logins_employees_UserId",
            //            column: x => x.UserId,
            //            principalTable: "employees",
            //            principalColumn: "id",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "user_roles",
            //    columns: table => new
            //    {
            //        UserId = table.Column<Guid>(type: "uuid", nullable: false),
            //        RoleId = table.Column<Guid>(type: "uuid", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_user_roles", x => new { x.UserId, x.RoleId });
            //        table.ForeignKey(
            //            name: "FK_user_roles_employees_UserId",
            //            column: x => x.UserId,
            //            principalTable: "employees",
            //            principalColumn: "id",
            //            onDelete: ReferentialAction.Cascade);
            //        table.ForeignKey(
            //            name: "FK_user_roles_roles_RoleId",
            //            column: x => x.RoleId,
            //            principalTable: "roles",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "user_tokens",
            //    columns: table => new
            //    {
            //        UserId = table.Column<Guid>(type: "uuid", nullable: false),
            //        LoginProvider = table.Column<string>(type: "text", nullable: false),
            //        Name = table.Column<string>(type: "text", nullable: false),
            //        Value = table.Column<string>(type: "text", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_user_tokens", x => new { x.UserId, x.LoginProvider, x.Name });
            //        table.ForeignKey(
            //            name: "FK_user_tokens_employees_UserId",
            //            column: x => x.UserId,
            //            principalTable: "employees",
            //            principalColumn: "id",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.InsertData(
            //    table: "departments",
            //    columns: new[] { "id", "created_at", "description", "head_employee_id", "name" },
            //    values: new object[,]
            //    {
            //        { new Guid("550e8400-e29b-41d4-a716-446655440000"), new DateTime(2025, 9, 2, 10, 38, 50, 121, DateTimeKind.Utc).AddTicks(2928), "Human Resource dept", null, "HR" },
            //        { new Guid("550e8400-e29b-41d4-a716-446655440001"), new DateTime(2025, 9, 2, 10, 38, 50, 121, DateTimeKind.Utc).AddTicks(3324), "engineering dept", null, "Engineering" },
            //        { new Guid("550e8400-e29b-41d4-a716-446655440002"), new DateTime(2025, 9, 2, 10, 38, 50, 121, DateTimeKind.Utc).AddTicks(3326), "Finance dept", null, "Finance" }
            //    });

            migrationBuilder.InsertData(
                table: "signing_keys",
                columns: new[] { "Id", "CreatedAt", "ExpiresAt", "IsActive", "KeyId", "PrivateKey", "PublicKey" },
                values: new object[] { new Guid("d15a1571-e276-4f8c-87de-ff3848349c92"), new DateTime(2025, 9, 2, 15, 38, 50, 38, DateTimeKind.Utc).AddTicks(1721), new DateTime(2026, 9, 2, 10, 38, 50, 38, DateTimeKind.Utc).AddTicks(1947), true, "1c6abb0c-533f-47d0-a78b-4f7357b05532", "MIIEpAIBAAKCAQEAxgV3trGyIqKf4p0Y0I9UKEBkn7i875IaPxx3lF6PqIYg/3TNz9yAXIYEwQhgOLTj1KFvLKbXIBT3DYuhkckStDGaN+YMnnbWDW0agU0D1H8j5Jgt+GuaPOu1z1vibDmvElKtfbjcYkwGZyz6+WQf9eFEgc0adxlBVAYgIiXcnUTMS1/vwXix5gMdw1zFGWqp1ThGGVsu1v6byrSdtYKiqc1etyyWDuakYR9yhCIArofPm8s27tRLzRswUaj85tPnUQHNuLGU+3Zzypka8arxQp3Ii63A1dEc+iPahe9pvPvPht/O7aKDuODNIDHLMkqrqTltXNTX0c1wj3x/lsVdSQIDAQABAoIBAF/2pl1TIBDI3tLjbvMjgpU/H4SH8ofnNfD/yrOMX/I7jiI+aL8jDNHJ4OPrOzY/82lIEqiA8iAKCO/3iwUrfHT21NXiFpt026cGCKJAwGpqcuqFFUIeY1DCqgkjbkckbUNj7c/Pln3Meg3mLnpNm299C8Ybr4eoJZCip0/lKVTM9wkqLm2lorbMhAVXo1Q/qhmqHsIzo7CPI713P0ze7++l2jZGFVx8PUqWKXevpAWi2NWFFnTecngopp7OV9w9vX6lzAW/foKuRZiOisb6N5CqjdLjUfVf9apYaYh3sGl0LymEcLxJU/7t+mEXGq31p0Yl9XbdjlANACl2T6PriUECgYEAys/0lQxOiIvznwhW3SW3IqweDv2qJl9Y1Ezgei1NiauF84QWDzxbHB/0KVy323p+/+YYBZU2VoTZAedx/x6in9p3eEt23534DmCA1DglqRXxoba2k//TtWe1GaMbRU41nDFb8JvhgZVxxypQ54OikKRiyRyqMbMJUk8rHitzet8CgYEA+fPdbdxWhfqhhxk0Q65K8ubofrby40p8eZ9miu48Fmzi37AvEMv0bssB2sZ8n0rvaso49tj/RO5HWY8PNzdRLz8O03auGY4j7GNa7OtDASegK37OYeZ7chBy+mX6gMOY/hbO2ECFKk9EQpUkHYw4P63v01lrekplEU1rndKIVNcCgYAkAYVnjLbIkSggPMfHHTIFKs9vU+OISncnwbo8lpxka5otYG7WJ535QUcErNxLT9UKM1xiaVorRAyOxPs9EsBGZu+84JDrJE1sp/9XLdypxyaN0jVZ0xpP42iIc6ecx0THRRAQiGwhqFl5xW51m64ZxcVv9PHWPvXRshJymb43uwKBgQDTDuu796EL8u1wMYUjXUomP3q0fPEtodjnrgSdbbtJSFpufcvQSBg3ca44OQBEVbdCmk1tiyBJ6VZQNm+ntUsVNQ7k2sL6Cz8kPWUqxyFGGVqzP5kvq7ozP9aGv5O+JOvMKiAxFJIdft9pxZvAY4Wfp9TpxwjtM2KSMKidlIfP8wKBgQDHh20Ug2Qdh6GDYWW975avc9ZtzybkR3H9+JYeJ+9tUGjrmCmhTxhsr6jB39U+PDmHDJYsKT98UUz0oaZ8R1OAiQpJD2dotSzjPtUOjMbG0vLowiyq4uX8v4W0FtApu9Pj8Y8ZvuwaUygS7XoRtKF/U1hRJq3dRlqiiS3iBV3k0w==", "MIIBCgKCAQEAxgV3trGyIqKf4p0Y0I9UKEBkn7i875IaPxx3lF6PqIYg/3TNz9yAXIYEwQhgOLTj1KFvLKbXIBT3DYuhkckStDGaN+YMnnbWDW0agU0D1H8j5Jgt+GuaPOu1z1vibDmvElKtfbjcYkwGZyz6+WQf9eFEgc0adxlBVAYgIiXcnUTMS1/vwXix5gMdw1zFGWqp1ThGGVsu1v6byrSdtYKiqc1etyyWDuakYR9yhCIArofPm8s27tRLzRswUaj85tPnUQHNuLGU+3Zzypka8arxQp3Ii63A1dEc+iPahe9pvPvPht/O7aKDuODNIDHLMkqrqTltXNTX0c1wj3x/lsVdSQIDAQAB" });

            //migrationBuilder.InsertData(
            //    table: "positions",
            //    columns: new[] { "id", "created_at", "department_id", "description", "salary_range_max", "salary_range_min", "title" },
            //    values: new object[,]
            //    {
            //        { new Guid("550e8400-e29b-41d4-a716-446655440100"), new DateTime(2025, 9, 2, 10, 38, 50, 117, DateTimeKind.Utc).AddTicks(5289), new Guid("550e8400-e29b-41d4-a716-446655440000"), "human resource manager", 200000.00m, 320000.00m, "HR Manager" },
            //        { new Guid("550e8400-e29b-41d4-a716-446655440101"), new DateTime(2025, 9, 2, 10, 38, 50, 117, DateTimeKind.Utc).AddTicks(5757), new Guid("550e8400-e29b-41d4-a716-446655440001"), "snr soft engr", 200000.00m, 150000.00m, "Software Engineer" },
            //        { new Guid("550e8400-e29b-41d4-a716-446655440102"), new DateTime(2025, 9, 2, 10, 38, 50, 117, DateTimeKind.Utc).AddTicks(5760), new Guid("550e8400-e29b-41d4-a716-446655440002"), "snr soft engr", 200000.00m, 320000.00m, "Financial Analyst" }
            //    });

            //migrationBuilder.InsertData(
            //    table: "employees",
            //    columns: new[] { "id", "AccessFailedCount", "address", "ConcurrencyStamp", "created_at", "dob", "department_id", "Email", "EmailConfirmed", "first_name", "hire_date", "last_name", "LockoutEnabled", "LockoutEnd", "manager_id", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "position_id", "salary", "SecurityStamp", "total_leaves", "TwoFactorEnabled", "UserName" },
            //    values: new object[] { new Guid("11111111-1111-1111-1111-111111111111"), 0, "HQ", "f1948295-43db-4caa-92b3-2e3a638b2d39", new DateTime(2025, 9, 2, 10, 38, 50, 54, DateTimeKind.Utc).AddTicks(1641), new DateTime(2003, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("550e8400-e29b-41d4-a716-446655440001"), "ruhma@email.com", true, "Ruhma", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Anwar", false, null, null, "RUHMA@EMAIL.COM", "RUHMA@EMAIL.COM", "AQAAAAIAAYagAAAAECjOtwAmJ6isqDL6segRL4sF5zfiyfw6EB9tbv8ctixJWOewcWNo9WzANd2mb5bmJg==", "03077147775", true, new Guid("550e8400-e29b-41d4-a716-446655440101"), 1000m, "b03e1d0d-2049-4f72-92db-8c8482b590b4", 25, false, "ruhma@email.com" });

            //migrationBuilder.CreateIndex(
            //    name: "IX_attendances_date",
            //    table: "attendances",
            //    column: "date");

            //migrationBuilder.CreateIndex(
            //    name: "IX_attendances_employee_id",
            //    table: "attendances",
            //    column: "employee_id");

            //migrationBuilder.CreateIndex(
            //    name: "IX_attendances_employee_id_date",
            //    table: "attendances",
            //    columns: new[] { "employee_id", "date" },
            //    unique: true);

            //migrationBuilder.CreateIndex(
            //    name: "IX_attendances_is_deleted",
            //    table: "attendances",
            //    column: "is_deleted");

            //migrationBuilder.CreateIndex(
            //    name: "IX_departments_head_employee_id",
            //    table: "departments",
            //    column: "head_employee_id",
            //    unique: true,
            //    filter: "\"head_employee_id\" IS NOT NULL");

            //migrationBuilder.CreateIndex(
            //    name: "IX_departments_is_deleted",
            //    table: "departments",
            //    column: "is_deleted");

            //migrationBuilder.CreateIndex(
            //    name: "IX_departments_name",
            //    table: "departments",
            //    column: "name",
            //    unique: true);

            //migrationBuilder.CreateIndex(
            //    name: "EmailIndex",
            //    table: "employees",
            //    column: "NormalizedEmail");

            //migrationBuilder.CreateIndex(
            //    name: "IX_employees_department_id",
            //    table: "employees",
            //    column: "department_id");

            //migrationBuilder.CreateIndex(
            //    name: "IX_employees_Email",
            //    table: "employees",
            //    column: "Email",
            //    unique: true);

            //migrationBuilder.CreateIndex(
            //    name: "IX_employees_is_deleted",
            //    table: "employees",
            //    column: "is_deleted");

            //migrationBuilder.CreateIndex(
            //    name: "IX_employees_manager_id",
            //    table: "employees",
            //    column: "manager_id");

            //migrationBuilder.CreateIndex(
            //    name: "IX_employees_position_id",
            //    table: "employees",
            //    column: "position_id");

            //migrationBuilder.CreateIndex(
            //    name: "UserNameIndex",
            //    table: "employees",
            //    column: "NormalizedUserName",
            //    unique: true);

            //migrationBuilder.CreateIndex(
            //    name: "IX_leaves_approver_id",
            //    table: "leaves",
            //    column: "approver_id");

            //migrationBuilder.CreateIndex(
            //    name: "IX_leaves_employee_id",
            //    table: "leaves",
            //    column: "employee_id");

            //migrationBuilder.CreateIndex(
            //    name: "IX_leaves_end_date",
            //    table: "leaves",
            //    column: "end_date");

            //migrationBuilder.CreateIndex(
            //    name: "IX_leaves_is_deleted",
            //    table: "leaves",
            //    column: "is_deleted");

            //migrationBuilder.CreateIndex(
            //    name: "IX_leaves_start_date",
            //    table: "leaves",
            //    column: "start_date");

            //migrationBuilder.CreateIndex(
            //    name: "IX_leaves_status",
            //    table: "leaves",
            //    column: "status");

            //migrationBuilder.CreateIndex(
            //    name: "IX_positions_department_id",
            //    table: "positions",
            //    column: "department_id");

            //migrationBuilder.CreateIndex(
            //    name: "IX_positions_is_deleted",
            //    table: "positions",
            //    column: "is_deleted");

            //migrationBuilder.CreateIndex(
            //    name: "IX_positions_title",
            //    table: "positions",
            //    column: "title",
            //    unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_refresh_tokens_EmployeeId",
                table: "refresh_tokens",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_refresh_tokens_Token",
                table: "refresh_tokens",
                column: "Token",
                unique: true);

            //migrationBuilder.CreateIndex(
            //    name: "IX_role_claims_RoleId",
            //    table: "role_claims",
            //    column: "RoleId");

            //migrationBuilder.CreateIndex(
            //    name: "RoleNameIndex",
            //    table: "roles",
            //    column: "NormalizedName",
            //    unique: true);

            //migrationBuilder.CreateIndex(
            //    name: "IX_user_claims_UserId",
            //    table: "user_claims",
            //    column: "UserId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_user_logins_UserId",
            //    table: "user_logins",
            //    column: "UserId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_user_roles_RoleId",
            //    table: "user_roles",
            //    column: "RoleId");

            //migrationBuilder.AddForeignKey(
            //    name: "FK_attendances_employees_employee_id",
            //    table: "attendances",
            //    column: "employee_id",
            //    principalTable: "employees",
            //    principalColumn: "id",
            //    onDelete: ReferentialAction.Restrict);

            //migrationBuilder.AddForeignKey(
            //    name: "FK_departments_employees_head_employee_id",
            //    table: "departments",
            //    column: "head_employee_id",
            //    principalTable: "employees",
            //    principalColumn: "id",
            //    onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_departments_employees_head_employee_id",
                table: "departments");

            migrationBuilder.DropTable(
                name: "attendances");

            migrationBuilder.DropTable(
                name: "leaves");

            migrationBuilder.DropTable(
                name: "refresh_tokens");

            migrationBuilder.DropTable(
                name: "role_claims");

            migrationBuilder.DropTable(
                name: "signing_keys");

            migrationBuilder.DropTable(
                name: "user_claims");

            migrationBuilder.DropTable(
                name: "user_logins");

            migrationBuilder.DropTable(
                name: "user_roles");

            migrationBuilder.DropTable(
                name: "user_tokens");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropTable(
                name: "employees");

            migrationBuilder.DropTable(
                name: "positions");

            migrationBuilder.DropTable(
                name: "departments");
        }
    }
}
