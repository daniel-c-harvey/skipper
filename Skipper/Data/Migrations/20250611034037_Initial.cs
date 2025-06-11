using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Skipper.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "skipper");

            migrationBuilder.CreateTable(
                name: "slip_classifications",
                schema: "skipper",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    MaxLength = table.Column<decimal>(type: "numeric(19,5)", precision: 19, scale: 5, nullable: false),
                    MaxBeam = table.Column<decimal>(type: "numeric(19,5)", precision: 19, scale: 5, nullable: false),
                    BasePrice = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_slip_classifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "vessels",
                schema: "skipper",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RegistrationNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Length = table.Column<decimal>(type: "numeric(19,5)", precision: 19, scale: 5, nullable: false),
                    Beam = table.Column<decimal>(type: "numeric(19,5)", precision: 19, scale: 5, nullable: false),
                    VesselType = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vessels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "slips",
                schema: "skipper",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SlipClassificationId = table.Column<long>(type: "bigint", nullable: false),
                    SlipNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    LocationCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_slips", x => x.Id);
                    table.ForeignKey(
                        name: "FK_slips_slip_classifications_SlipClassificationId",
                        column: x => x.SlipClassificationId,
                        principalSchema: "skipper",
                        principalTable: "slip_classifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "rental_agreements",
                schema: "skipper",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SlipId = table.Column<long>(type: "bigint", nullable: false),
                    VesselId = table.Column<long>(type: "bigint", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PriceRate = table.Column<int>(type: "integer", nullable: false),
                    PriceUnit = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rental_agreements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_rental_agreements_slips_SlipId",
                        column: x => x.SlipId,
                        principalSchema: "skipper",
                        principalTable: "slips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_rental_agreements_vessels_VesselId",
                        column: x => x.VesselId,
                        principalSchema: "skipper",
                        principalTable: "vessels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_rental_agreements_IsDeleted",
                schema: "skipper",
                table: "rental_agreements",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_rental_agreements_SlipId",
                schema: "skipper",
                table: "rental_agreements",
                column: "SlipId");

            migrationBuilder.CreateIndex(
                name: "IX_rental_agreements_Status",
                schema: "skipper",
                table: "rental_agreements",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_rental_agreements_VesselId",
                schema: "skipper",
                table: "rental_agreements",
                column: "VesselId");

            migrationBuilder.CreateIndex(
                name: "IX_slip_classifications_BasePrice",
                schema: "skipper",
                table: "slip_classifications",
                column: "BasePrice");

            migrationBuilder.CreateIndex(
                name: "IX_slip_classifications_IsDeleted",
                schema: "skipper",
                table: "slip_classifications",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_slip_classifications_Name",
                schema: "skipper",
                table: "slip_classifications",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_slips_IsDeleted",
                schema: "skipper",
                table: "slips",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_slips_SlipClassificationId",
                schema: "skipper",
                table: "slips",
                column: "SlipClassificationId");

            migrationBuilder.CreateIndex(
                name: "IX_slips_SlipNumber",
                schema: "skipper",
                table: "slips",
                column: "SlipNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_slips_Status",
                schema: "skipper",
                table: "slips",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_vessels_IsDeleted",
                schema: "skipper",
                table: "vessels",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_vessels_Name",
                schema: "skipper",
                table: "vessels",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_vessels_RegistrationNumber",
                schema: "skipper",
                table: "vessels",
                column: "RegistrationNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_vessels_VesselType",
                schema: "skipper",
                table: "vessels",
                column: "VesselType");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "rental_agreements",
                schema: "skipper");

            migrationBuilder.DropTable(
                name: "slips",
                schema: "skipper");

            migrationBuilder.DropTable(
                name: "vessels",
                schema: "skipper");

            migrationBuilder.DropTable(
                name: "slip_classifications",
                schema: "skipper");
        }
    }
}
