using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkipperData.Data.Migrations
{
    /// <inheritdoc />
    public partial class Address2Null : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Address2",
                schema: "skipper",
                table: "addresses",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true,
                collation: "en-US-x-icu",
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255,
                oldCollation: "en-US-x-icu");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Address2",
                schema: "skipper",
                table: "addresses",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                collation: "en-US-x-icu",
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255,
                oldNullable: true,
                oldCollation: "en-US-x-icu");
        }
    }
}
