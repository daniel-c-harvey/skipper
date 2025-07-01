using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkipperData.Data.Migrations
{
    /// <inheritdoc />
    public partial class stringsinsensitive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "RegistrationNumber",
                schema: "skipper",
                table: "vessels",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                collation: "en-US-x-icu",
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "skipper",
                table: "vessels",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                collation: "en-US-x-icu",
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "SlipNumber",
                schema: "skipper",
                table: "slips",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                collation: "en-US-x-icu",
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "LocationCode",
                schema: "skipper",
                table: "slips",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                collation: "en-US-x-icu",
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "skipper",
                table: "slip_classifications",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                collation: "en-US-x-icu",
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "skipper",
                table: "slip_classifications",
                type: "text",
                nullable: false,
                collation: "en-US-x-icu",
                oldClrType: typeof(string),
                oldType: "text");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "RegistrationNumber",
                schema: "skipper",
                table: "vessels",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldCollation: "en-US-x-icu");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "skipper",
                table: "vessels",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255,
                oldCollation: "en-US-x-icu");

            migrationBuilder.AlterColumn<string>(
                name: "SlipNumber",
                schema: "skipper",
                table: "slips",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldCollation: "en-US-x-icu");

            migrationBuilder.AlterColumn<string>(
                name: "LocationCode",
                schema: "skipper",
                table: "slips",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldCollation: "en-US-x-icu");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "skipper",
                table: "slip_classifications",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255,
                oldCollation: "en-US-x-icu");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "skipper",
                table: "slip_classifications",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldCollation: "en-US-x-icu");
        }
    }
}
