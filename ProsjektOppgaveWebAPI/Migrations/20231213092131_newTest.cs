using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProsjektOppgaveWebAPI.Migrations
{
    public partial class newTest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "71d12dd0-d81b-4b0e-b51b-1a9deb3a0527");

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "4a2a9c3c-3d92-419b-9fe1-bc52131f11e8", 0, "da4a271c-cbe8-4e91-9fae-d84ca01b93c5", "admin@example.com", true, false, null, "ADMIN@EXAMPLE.COM", "ADMIN@EXAMPLE.COM", "AQAAAAIAAYagAAAAEMSiURkqtxY48kfcDy+9uYvmtYsXTAvf9MsJaJrIK9S7E95fQmixbG2f21SUh3H0mQ==", null, false, "", false, "admin@example.com" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "4a2a9c3c-3d92-419b-9fe1-bc52131f11e8");

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "71d12dd0-d81b-4b0e-b51b-1a9deb3a0527", 0, "ee079c12-00e0-4ec2-85d8-e0b552720d88", "admin@example.com", true, false, null, "ADMIN@EXAMPLE.COM", "ADMIN", "AQAAAAIAAYagAAAAEPFAbfaWxWUEG60yse2wLNHmvofOtZHr+tEiIupTUsCHhX476jq3YWEPfjNdFSncUA==", null, false, "", false, "admin" });
        }
    }
}
