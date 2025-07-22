using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SkipperData.Data.Migrations
{
    /// <inheritdoc />
    public partial class SlipReservations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "rental_agreements",
                schema: "skipper",
                newName: "slip_reservations",
                newSchema: "skipper"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "slip_reservations",
                schema: "skipper",
                newName: "rental_agreements",
                newSchema: "skipper"
            );
        }
    }
}
