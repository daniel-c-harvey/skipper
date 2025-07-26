using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SkipperData.Data.Migrations
{
    /// <inheritdoc />
    public partial class OrdersCustomers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "customers",
                schema: "skipper");

            migrationBuilder.CreateTable(
                name: "vessel_owner_customers",
                schema: "skipper",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LicenseNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, collation: "en-US-x-icu"),
                    LicenseExpiryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AccountNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, collation: "en-US-x-icu"),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false, collation: "en-US-x-icu"),
                    CustomerProfileType = table.Column<string>(type: "text", nullable: false),
                    CustomerProfileId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vessel_owner_customers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_vessel_owner_customers_vessel_owner_profiles_CustomerProfil~",
                        column: x => x.CustomerProfileId,
                        principalSchema: "skipper",
                        principalTable: "vessel_owner_profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "vessel_owner_orders",
                schema: "skipper",
                columns: table => new
                {
                    OrderType = table.Column<string>(type: "text", nullable: false),
                    OrderTypeId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    OrderNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, collation: "en-US-x-icu"),
                    CustomerId = table.Column<long>(type: "bigint", nullable: false),
                    OrderDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TotalAmount = table.Column<int>(type: "integer", nullable: false),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true, collation: "en-US-x-icu"),
                    Status = table.Column<string>(type: "text", nullable: false),
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Discriminator = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vessel_owner_orders", x => new { x.OrderTypeId, x.OrderType });
                    table.ForeignKey(
                        name: "FK_vessel_owner_orders_vessel_owner_profiles_CustomerId",
                        column: x => x.CustomerId,
                        principalSchema: "skipper",
                        principalTable: "vessel_owner_profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_vessel_owner_customers_AccountNumber",
                schema: "skipper",
                table: "vessel_owner_customers",
                column: "AccountNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_vessel_owner_customers_CustomerProfileId_CustomerProfileType",
                schema: "skipper",
                table: "vessel_owner_customers",
                columns: new[] { "CustomerProfileId", "CustomerProfileType" });

            migrationBuilder.CreateIndex(
                name: "IX_vessel_owner_customers_IsDeleted",
                schema: "skipper",
                table: "vessel_owner_customers",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_vessel_owner_orders_CustomerId",
                schema: "skipper",
                table: "vessel_owner_orders",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_vessel_owner_orders_IsDeleted",
                schema: "skipper",
                table: "vessel_owner_orders",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_vessel_owner_orders_OrderNumber",
                schema: "skipper",
                table: "vessel_owner_orders",
                column: "OrderNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_vessel_owner_orders_Status_OrderDate",
                schema: "skipper",
                table: "vessel_owner_orders",
                columns: new[] { "Status", "OrderDate" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "vessel_owner_customers",
                schema: "skipper");

            migrationBuilder.DropTable(
                name: "vessel_owner_orders",
                schema: "skipper");

            migrationBuilder.CreateTable(
                name: "customers",
                schema: "skipper",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AccountNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, collation: "en-US-x-icu"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CustomerProfileId = table.Column<long>(type: "bigint", nullable: false),
                    CustomerProfileType = table.Column<string>(type: "text", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false, collation: "en-US-x-icu"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_customers", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_customers_AccountNumber",
                schema: "skipper",
                table: "customers",
                column: "AccountNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_customers_CustomerProfileId_CustomerProfileType",
                schema: "skipper",
                table: "customers",
                columns: new[] { "CustomerProfileId", "CustomerProfileType" });

            migrationBuilder.CreateIndex(
                name: "IX_customers_IsDeleted",
                schema: "skipper",
                table: "customers",
                column: "IsDeleted");
        }
    }
}
