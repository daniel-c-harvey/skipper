using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthBlocksData.Migrations
{
    /// <inheritdoc />
    public partial class UserRoleLinkage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Modified",
                schema: "user",
                table: "user_roles",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "Deleted",
                schema: "user",
                table: "user_roles",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "Created",
                schema: "user",
                table: "user_roles",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "Modified",
                schema: "user",
                table: "roles",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "Deleted",
                schema: "user",
                table: "roles",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "Created",
                schema: "user",
                table: "roles",
                newName: "CreatedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                schema: "user",
                table: "user_roles",
                newName: "Modified");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                schema: "user",
                table: "user_roles",
                newName: "Deleted");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                schema: "user",
                table: "user_roles",
                newName: "Created");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                schema: "user",
                table: "roles",
                newName: "Modified");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                schema: "user",
                table: "roles",
                newName: "Deleted");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                schema: "user",
                table: "roles",
                newName: "Created");
        }
    }
}
