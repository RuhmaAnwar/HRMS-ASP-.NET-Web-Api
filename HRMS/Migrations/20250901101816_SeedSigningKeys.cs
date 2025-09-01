using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRMS.Migrations
{
    /// <inheritdoc />
    public partial class SeedSigningKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SigningKeys",
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
                    table.PrimaryKey("PK_SigningKeys", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "SigningKeys",
                columns: new[] { "Id", "CreatedAt", "ExpiresAt", "IsActive", "KeyId", "PrivateKey", "PublicKey" },
                values: new object[] { new Guid("130990e7-b4ca-464b-987c-a75de392cad2"), new DateTime(2025, 9, 1, 15, 18, 15, 403, DateTimeKind.Utc).AddTicks(6534), new DateTime(2026, 9, 1, 10, 18, 15, 403, DateTimeKind.Utc).AddTicks(6758), true, "e309ce68-e280-4fb2-a88f-d24ee37ddb51", "MIIEpAIBAAKCAQEAvwtEj2A6U6hsWueTq7wnQ11CcGm8qrgTGUKjHDdJwKyvXjJMWiXnR3DuBKpZndXOyQEvOtIcwRzhMmfFkBuBSE66QKhp9IU0mSpKvRLn5uPqWEPoJIRdadGat5KdjsmQT5oUhH8yvfgxq829hhZaxJfUDcTPuPVFwcnnV6/5qtxjF3DkptZ2wIVhOnUkpIZc3h6IpDaBku+j9cLw1ho7AAYpNLiE2SC8k0/9+4EAU1hGYXPJd7HFht3Cb584EnfO9wOYQy8BhqiCwImGOLhmNkYtPLhnyG79nu2iC9ztm3qqx75elYscmkyMERUe2s9TtEmnZpYFmX4V0pZR3t4X4QIDAQABAoIBABXVcdZ5BvF8tDWHdecQr+QRCoDSdM+GDhi69u1InFM66Sf406UR4+bUTz+VQHy7bNCoI/+y97pSVKJwHbIkkpfy93JgTQu3FZXTsYr+WCaZMbU4vZU+03Y+kFIenYcuHiIa1/F0oBX6AaNC9kAW2fTR7VhNf/gBgCFvs9Lft8cBCl7NsHlmOMkk7Li65pxm9rU9D8e4HrcpOELFO2At2wGDhAdWQEHrFlrvQ8zHLOXXkWztGyvxT2kCNCP9EnF61dcoRwrF11x3lSMO0vJYtwDTZ7Ve2Uv9FrNm/yjEGzKKDGYET7/CHoyO1jOy3HpdgWeG7emMvsk+1zAGtMyjwCkCgYEA7OZJzLaXFpgpl+NB5T5ExXY/K2KJwbHZTBxiwq57bnDG8WaUQvKYL2taCXPMaPSQcuP4QEDNRJD/TZLqdiX6bptL3IQYn8H20xERuBioS/rfpb3CXMJ/Wh1raadzTzKjMx8QkOIsCm0sTRbC6VZ/PIPB9eGafErILQzHr1D6fGsCgYEAznKAMDyUVop2PtwIRerRCVnvdj+yIotM6ZabgodHdzPEMIe5OROahShQxxIYvx6lIBo1d0TSNLsGrhIwVWvL8VhuuG3qoNogTYBwbCDlDyx8CzHnHkV+5KJUrL7OhCNY+THZSX6JCD3CBeF4jT8RJBLrclcpDRoE/AUcMWBGj+MCgYEA0/jnVh6XKKd0qCy33LkP7iP1OAvOuZs6lUjc8dgQJ+0PhhdEWHcbx2pmQQj1gYA79CGaE1woj86yMqZf1uH6qJealLE6G1ECWy/ty+18Qag0D+iwjjGWpHqojvw/SEFGGEsWlp1ZzOaM+lmAhCtaZDp9Blcc1VRQs0a5MlziFQsCgYBy5w+pyU3WjOIbQmPoZjVKOyAodHM0/sskB4suLxT7p3g+eowA1IRgMHwlEFFtriTBpVS0uT011BJ3t+/m20R7Y3jcdXDtbfokqKPpyrUyqNOxd2jnVE63+hGew+Q1b2cJ61DAswiZ6aWfmEp8cumbZlxdG13hyOWVT1LNuIQNeQKBgQDh5HEO2btWD0kQz6ad3wrexflV3p9i/pDsTQvoSpZ1+DwvwMnWC1mrdna/KlQGZg75UsByZ+KD43+v/OVA2M20o68TusIFibnIUvnEaLTUQhJsWgW4DLCTcFTlMH2xo/gCgv8/hYkhPh6J7nvQ9J8H8NU7LkuEOPue8xXD6zgSXA==", "MIIBCgKCAQEAvwtEj2A6U6hsWueTq7wnQ11CcGm8qrgTGUKjHDdJwKyvXjJMWiXnR3DuBKpZndXOyQEvOtIcwRzhMmfFkBuBSE66QKhp9IU0mSpKvRLn5uPqWEPoJIRdadGat5KdjsmQT5oUhH8yvfgxq829hhZaxJfUDcTPuPVFwcnnV6/5qtxjF3DkptZ2wIVhOnUkpIZc3h6IpDaBku+j9cLw1ho7AAYpNLiE2SC8k0/9+4EAU1hGYXPJd7HFht3Cb584EnfO9wOYQy8BhqiCwImGOLhmNkYtPLhnyG79nu2iC9ztm3qqx75elYscmkyMERUe2s9TtEmnZpYFmX4V0pZR3t4X4QIDAQAB" });

            migrationBuilder.UpdateData(
                table: "departments",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440000"),
                column: "created_at",
                value: new DateTime(2025, 9, 1, 10, 18, 15, 481, DateTimeKind.Utc).AddTicks(5329));

            migrationBuilder.UpdateData(
                table: "departments",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440001"),
                column: "created_at",
                value: new DateTime(2025, 9, 1, 10, 18, 15, 481, DateTimeKind.Utc).AddTicks(5717));

            migrationBuilder.UpdateData(
                table: "departments",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440002"),
                column: "created_at",
                value: new DateTime(2025, 9, 1, 10, 18, 15, 481, DateTimeKind.Utc).AddTicks(5719));

            migrationBuilder.UpdateData(
                table: "employees",
                keyColumn: "id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "ConcurrencyStamp", "created_at", "PasswordHash", "SecurityStamp" },
                values: new object[] { "50637a17-e898-41b9-9bf9-03588237e0ac", new DateTime(2025, 9, 1, 10, 18, 15, 422, DateTimeKind.Utc).AddTicks(9305), "AQAAAAIAAYagAAAAEM6b8Xvl9J4y1JCdyR11wOFFWOeXw4vq4p5Z3HwJMCWRs8tXgWG2oyzyGAobpqwfVg==", "59a3bb0b-8846-4058-99e7-7b532930f9b0" });

            migrationBuilder.UpdateData(
                table: "positions",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440100"),
                column: "created_at",
                value: new DateTime(2025, 9, 1, 10, 18, 15, 477, DateTimeKind.Utc).AddTicks(6450));

            migrationBuilder.UpdateData(
                table: "positions",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440101"),
                column: "created_at",
                value: new DateTime(2025, 9, 1, 10, 18, 15, 477, DateTimeKind.Utc).AddTicks(6867));

            migrationBuilder.UpdateData(
                table: "positions",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440102"),
                column: "created_at",
                value: new DateTime(2025, 9, 1, 10, 18, 15, 477, DateTimeKind.Utc).AddTicks(6872));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SigningKeys");

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

            migrationBuilder.UpdateData(
                table: "employees",
                keyColumn: "id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "ConcurrencyStamp", "created_at", "PasswordHash", "SecurityStamp" },
                values: new object[] { "2fa3eb2a-37ad-45f1-99a3-42df9a324632", new DateTime(2025, 8, 23, 21, 59, 54, 895, DateTimeKind.Utc).AddTicks(738), "AQAAAAIAAYagAAAAEFDPLWpCPf7nnCI9AW+ArhwTrzTHHAMjwijzk/KZksKDKW6MU+otNL5LPFiMRuFj/w==", "42b4633f-94c3-40df-93c1-45c94b86abda" });

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
    }
}
