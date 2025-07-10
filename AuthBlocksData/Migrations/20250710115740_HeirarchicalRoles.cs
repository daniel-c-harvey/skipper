using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthBlocksData.Migrations
{
    /// <inheritdoc />
    public partial class HeirarchicalRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ParentRoleId",
                schema: "user",
                table: "roles",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_roles_parentroleid",
                schema: "user",
                table: "roles",
                column: "ParentRoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_roles_roles_ParentRoleId",
                schema: "user",
                table: "roles",
                column: "ParentRoleId",
                principalSchema: "user",
                principalTable: "roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_roles_roles_ParentRoleId",
                schema: "user",
                table: "roles");

            migrationBuilder.DropIndex(
                name: "ix_roles_parentroleid",
                schema: "user",
                table: "roles");

            migrationBuilder.DropColumn(
                name: "ParentRoleId",
                schema: "user",
                table: "roles");
        }
    }
}
