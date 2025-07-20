using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthBlocksData.Migrations
{
    /// <inheritdoc />
    public partial class PendingRegistrationsRoleIds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_pending_registrations_PendingUserEmail_IsConsumed",
                schema: "user",
                table: "pending_registrations");

            migrationBuilder.AddColumn<long[]>(
                name: "RoleIds",
                schema: "user",
                table: "pending_registrations",
                type: "bigint[]",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_pending_registrations_PendingUserEmail_IsConsumed",
                schema: "user",
                table: "pending_registrations",
                columns: new[] { "PendingUserEmail", "IsConsumed" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_pending_registrations_PendingUserEmail_IsConsumed",
                schema: "user",
                table: "pending_registrations");

            migrationBuilder.DropColumn(
                name: "RoleIds",
                schema: "user",
                table: "pending_registrations");

            migrationBuilder.CreateIndex(
                name: "IX_pending_registrations_PendingUserEmail_IsConsumed",
                schema: "user",
                table: "pending_registrations",
                columns: new[] { "PendingUserEmail", "IsConsumed" })
                .Annotation("Npgsql:IndexInclude", new[] { "TokenHash", "ExpiresAt" });
        }
    }
}
