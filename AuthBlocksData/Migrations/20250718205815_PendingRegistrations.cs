using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AuthBlocksData.Migrations
{
    /// <inheritdoc />
    public partial class PendingRegistrations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "pending_registrations",
                schema: "user",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PendingUserEmail = table.Column<string>(type: "text", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TokenHash = table.Column<string>(type: "text", nullable: false),
                    IsConsumed = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    ConsumedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pending_registrations", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_pending_registrations_IsDeleted",
                schema: "user",
                table: "pending_registrations",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_pending_registrations_PendingUserEmail_IsConsumed",
                schema: "user",
                table: "pending_registrations",
                columns: new[] { "PendingUserEmail", "IsConsumed" })
                .Annotation("Npgsql:IndexInclude", new[] { "TokenHash", "ExpiresAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "pending_registrations",
                schema: "user");
        }
    }
}
