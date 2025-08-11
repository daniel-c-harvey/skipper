using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SkipperData.Data.Migrations
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
                name: "addresses",
                schema: "skipper",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Address1 = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false, collation: "en-US-x-icu"),
                    Address2 = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true, collation: "en-US-x-icu"),
                    City = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false, collation: "en-US-x-icu"),
                    State = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false, collation: "en-US-x-icu"),
                    ZipCode = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false, collation: "en-US-x-icu"),
                    Country = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false, collation: "en-US-x-icu"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_addresses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "slip_classifications",
                schema: "skipper",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false, collation: "en-US-x-icu"),
                    MaxLength = table.Column<decimal>(type: "numeric(19,5)", precision: 19, scale: 5, nullable: false),
                    MaxBeam = table.Column<decimal>(type: "numeric(19,5)", precision: 19, scale: 5, nullable: false),
                    BasePrice = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false, collation: "en-US-x-icu"),
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
                    RegistrationNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, collation: "en-US-x-icu"),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false, collation: "en-US-x-icu"),
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
                name: "contacts",
                schema: "skipper",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FirstName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false, collation: "en-US-x-icu"),
                    LastName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false, collation: "en-US-x-icu"),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false, collation: "en-US-x-icu"),
                    PhoneNumber = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true, collation: "en-US-x-icu"),
                    AddressId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contacts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_contacts_addresses_AddressId",
                        column: x => x.AddressId,
                        principalSchema: "skipper",
                        principalTable: "addresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "slips",
                schema: "skipper",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SlipClassificationId = table.Column<long>(type: "bigint", nullable: false),
                    SlipNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, collation: "en-US-x-icu"),
                    LocationCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, collation: "en-US-x-icu"),
                    Status = table.Column<string>(type: "text", nullable: false),
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
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "customers",
                schema: "skipper",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AccountNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, collation: "en-US-x-icu"),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false, collation: "en-US-x-icu"),
                    CustomerProfileType = table.Column<int>(type: "integer", nullable: false),
                    BusinessName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true, collation: "en-US-x-icu"),
                    TaxId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true, collation: "en-US-x-icu"),
                    DateOfBirth = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PreferredContactMethod = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true, collation: "en-US-x-icu"),
                    MembershipNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true, collation: "en-US-x-icu"),
                    MemberSince = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    MembershipLevel = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true, collation: "en-US-x-icu"),
                    LicenseNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true, collation: "en-US-x-icu"),
                    LicenseExpiryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ContactId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_customers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_customers_contacts_ContactId",
                        column: x => x.ContactId,
                        principalSchema: "skipper",
                        principalTable: "contacts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "business_customer_contacts",
                schema: "skipper",
                columns: table => new
                {
                    BusinessCustomerId = table.Column<long>(type: "bigint", nullable: false),
                    ContactId = table.Column<long>(type: "bigint", nullable: false),
                    IsPrimary = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    IsEmergency = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_business_customer_contacts", x => new { x.BusinessCustomerId, x.ContactId });
                    table.ForeignKey(
                        name: "FK_business_customer_contacts_contacts_ContactId",
                        column: x => x.ContactId,
                        principalSchema: "skipper",
                        principalTable: "contacts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_business_customer_contacts_customers_BusinessCustomerId",
                        column: x => x.BusinessCustomerId,
                        principalSchema: "skipper",
                        principalTable: "customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "orders",
                schema: "skipper",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrderNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, collation: "en-US-x-icu"),
                    CustomerId = table.Column<long>(type: "bigint", nullable: false),
                    OrderDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    OrderType = table.Column<string>(type: "text", nullable: false),
                    TotalAmount = table.Column<int>(type: "integer", nullable: false),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true, collation: "en-US-x-icu"),
                    Status = table.Column<string>(type: "text", nullable: false),
                    SlipId = table.Column<long>(type: "bigint", nullable: true),
                    VesselId = table.Column<long>(type: "bigint", nullable: true),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PriceRate = table.Column<int>(type: "integer", nullable: true),
                    PriceUnit = table.Column<string>(type: "text", nullable: true),
                    RentalStatus = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_orders_customers_CustomerId",
                        column: x => x.CustomerId,
                        principalSchema: "skipper",
                        principalTable: "customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_orders_slips_SlipId",
                        column: x => x.SlipId,
                        principalSchema: "skipper",
                        principalTable: "slips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_orders_vessels_VesselId",
                        column: x => x.VesselId,
                        principalSchema: "skipper",
                        principalTable: "vessels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "vessel_owner_vessels",
                schema: "skipper",
                columns: table => new
                {
                    VesselOwnerCustomerId = table.Column<long>(type: "bigint", nullable: false),
                    VesselId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vessel_owner_vessels", x => new { x.VesselOwnerCustomerId, x.VesselId });
                    table.ForeignKey(
                        name: "FK_vessel_owner_vessels_customers_VesselOwnerCustomerId",
                        column: x => x.VesselOwnerCustomerId,
                        principalSchema: "skipper",
                        principalTable: "customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_vessel_owner_vessels_vessels_VesselId",
                        column: x => x.VesselId,
                        principalSchema: "skipper",
                        principalTable: "vessels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_addresses_IsDeleted",
                schema: "skipper",
                table: "addresses",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_business_customer_contacts_ContactId",
                schema: "skipper",
                table: "business_customer_contacts",
                column: "ContactId");

            migrationBuilder.CreateIndex(
                name: "IX_business_customer_contacts_IsDeleted",
                schema: "skipper",
                table: "business_customer_contacts",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_contacts_AddressId",
                schema: "skipper",
                table: "contacts",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_contacts_IsDeleted",
                schema: "skipper",
                table: "contacts",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_customers_AccountNumber",
                schema: "skipper",
                table: "customers",
                column: "AccountNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_customers_ContactId",
                schema: "skipper",
                table: "customers",
                column: "ContactId");

            migrationBuilder.CreateIndex(
                name: "IX_customers_IsDeleted",
                schema: "skipper",
                table: "customers",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_orders_CustomerId",
                schema: "skipper",
                table: "orders",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_orders_IsDeleted",
                schema: "skipper",
                table: "orders",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_orders_OrderNumber",
                schema: "skipper",
                table: "orders",
                column: "OrderNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_orders_OrderType",
                schema: "skipper",
                table: "orders",
                column: "OrderType");

            migrationBuilder.CreateIndex(
                name: "IX_orders_RentalStatus",
                schema: "skipper",
                table: "orders",
                column: "RentalStatus");

            migrationBuilder.CreateIndex(
                name: "IX_orders_SlipId",
                schema: "skipper",
                table: "orders",
                column: "SlipId");

            migrationBuilder.CreateIndex(
                name: "IX_orders_SlipId_StartDate_EndDate",
                schema: "skipper",
                table: "orders",
                columns: new[] { "SlipId", "StartDate", "EndDate" });

            migrationBuilder.CreateIndex(
                name: "IX_orders_Status_OrderDate",
                schema: "skipper",
                table: "orders",
                columns: new[] { "Status", "OrderDate" });

            migrationBuilder.CreateIndex(
                name: "IX_orders_VesselId",
                schema: "skipper",
                table: "orders",
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
                name: "IX_vessel_owner_vessels_IsDeleted",
                schema: "skipper",
                table: "vessel_owner_vessels",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_vessel_owner_vessels_VesselId",
                schema: "skipper",
                table: "vessel_owner_vessels",
                column: "VesselId",
                unique: true);

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
                name: "business_customer_contacts",
                schema: "skipper");

            migrationBuilder.DropTable(
                name: "orders",
                schema: "skipper");

            migrationBuilder.DropTable(
                name: "vessel_owner_vessels",
                schema: "skipper");

            migrationBuilder.DropTable(
                name: "slips",
                schema: "skipper");

            migrationBuilder.DropTable(
                name: "customers",
                schema: "skipper");

            migrationBuilder.DropTable(
                name: "vessels",
                schema: "skipper");

            migrationBuilder.DropTable(
                name: "slip_classifications",
                schema: "skipper");

            migrationBuilder.DropTable(
                name: "contacts",
                schema: "skipper");

            migrationBuilder.DropTable(
                name: "addresses",
                schema: "skipper");
        }
    }
}
