using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkipperData.Data.Migrations
{
    /// <inheritdoc />
    public partial class SlipJoins : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_slips_slip_classifications_SlipClassificationId",
                schema: "skipper",
                table: "slips");

            migrationBuilder.AddForeignKey(
                name: "FK_slips_slip_classifications_SlipClassificationId",
                schema: "skipper",
                table: "slips",
                column: "SlipClassificationId",
                principalSchema: "skipper",
                principalTable: "slip_classifications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_slips_slip_classifications_SlipClassificationId",
                schema: "skipper",
                table: "slips");

            migrationBuilder.AddForeignKey(
                name: "FK_slips_slip_classifications_SlipClassificationId",
                schema: "skipper",
                table: "slips",
                column: "SlipClassificationId",
                principalSchema: "skipper",
                principalTable: "slip_classifications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
