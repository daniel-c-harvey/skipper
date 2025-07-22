using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SkipperData.Data.Migrations
{
    /// <inheritdoc />
    public partial class CustomerModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "addresses",
                schema: "skipper",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Address1 = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false, collation: "en-US-x-icu"),
                    Address2 = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false, collation: "en-US-x-icu"),
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
                name: "business_customer_profiles",
                schema: "skipper",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BusinessName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false, collation: "en-US-x-icu"),
                    TaxId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true, collation: "en-US-x-icu"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_business_customer_profiles", x => x.Id);
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
                    CustomerProfileType = table.Column<string>(type: "text", nullable: false),
                    CustomerProfileId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_customers", x => x.Id);
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
                name: "business_customer_contacts",
                schema: "skipper",
                columns: table => new
                {
                    BusinessCustomerProfileId = table.Column<long>(type: "bigint", nullable: false),
                    ContactId = table.Column<long>(type: "bigint", nullable: false),
                    IsPrimary = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    IsEmergency = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    BusinessCustomerProfileEntityId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_business_customer_contacts", x => new { x.BusinessCustomerProfileId, x.ContactId });
                    table.ForeignKey(
                        name: "FK_business_customer_contacts_business_customer_profiles_Busin~",
                        column: x => x.BusinessCustomerProfileEntityId,
                        principalSchema: "skipper",
                        principalTable: "business_customer_profiles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_business_customer_contacts_business_customer_profiles_Busi~1",
                        column: x => x.BusinessCustomerProfileId,
                        principalSchema: "skipper",
                        principalTable: "business_customer_profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_business_customer_contacts_contacts_ContactId",
                        column: x => x.ContactId,
                        principalSchema: "skipper",
                        principalTable: "contacts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "individual_customer_profiles",
                schema: "skipper",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ContactId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_individual_customer_profiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_individual_customer_profiles_contacts_ContactId",
                        column: x => x.ContactId,
                        principalSchema: "skipper",
                        principalTable: "contacts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MemberCustomerProfiles",
                schema: "skipper",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ContactId = table.Column<long>(type: "bigint", nullable: false),
                    MembershipStartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    MembershipEndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    MembershipLevel = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true, collation: "en-US-x-icu"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberCustomerProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MemberCustomerProfiles_contacts_ContactId",
                        column: x => x.ContactId,
                        principalSchema: "skipper",
                        principalTable: "contacts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "vessel_owner_profiles",
                schema: "skipper",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ContactId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vessel_owner_profiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_vessel_owner_profiles_contacts_ContactId",
                        column: x => x.ContactId,
                        principalSchema: "skipper",
                        principalTable: "contacts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "vessel_owner_vessels",
                schema: "skipper",
                columns: table => new
                {
                    VesselOwnerProfileId = table.Column<long>(type: "bigint", nullable: false),
                    VesselId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vessel_owner_vessels", x => new { x.VesselOwnerProfileId, x.VesselId });
                    table.ForeignKey(
                        name: "FK_vessel_owner_vessels_vessel_owner_profiles_VesselOwnerProfi~",
                        column: x => x.VesselOwnerProfileId,
                        principalSchema: "skipper",
                        principalTable: "vessel_owner_profiles",
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
                name: "IX_business_customer_contacts_BusinessCustomerProfileEntityId",
                schema: "skipper",
                table: "business_customer_contacts",
                column: "BusinessCustomerProfileEntityId");

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
                name: "IX_business_customer_profiles_IsDeleted",
                schema: "skipper",
                table: "business_customer_profiles",
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
                name: "IX_customers_CustomerProfileId_CustomerProfileType",
                schema: "skipper",
                table: "customers",
                columns: new[] { "CustomerProfileId", "CustomerProfileType" });

            migrationBuilder.CreateIndex(
                name: "IX_customers_IsDeleted",
                schema: "skipper",
                table: "customers",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_individual_customer_profiles_ContactId",
                schema: "skipper",
                table: "individual_customer_profiles",
                column: "ContactId");

            migrationBuilder.CreateIndex(
                name: "IX_individual_customer_profiles_IsDeleted",
                schema: "skipper",
                table: "individual_customer_profiles",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_MemberCustomerProfiles_ContactId",
                schema: "skipper",
                table: "MemberCustomerProfiles",
                column: "ContactId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberCustomerProfiles_IsDeleted",
                schema: "skipper",
                table: "MemberCustomerProfiles",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_MemberCustomerProfiles_MembershipEndDate",
                schema: "skipper",
                table: "MemberCustomerProfiles",
                column: "MembershipEndDate");

            migrationBuilder.CreateIndex(
                name: "IX_MemberCustomerProfiles_MembershipLevel",
                schema: "skipper",
                table: "MemberCustomerProfiles",
                column: "MembershipLevel");

            migrationBuilder.CreateIndex(
                name: "IX_MemberCustomerProfiles_MembershipStartDate",
                schema: "skipper",
                table: "MemberCustomerProfiles",
                column: "MembershipStartDate");

            migrationBuilder.CreateIndex(
                name: "IX_vessel_owner_profiles_ContactId",
                schema: "skipper",
                table: "vessel_owner_profiles",
                column: "ContactId");

            migrationBuilder.CreateIndex(
                name: "IX_vessel_owner_profiles_IsDeleted",
                schema: "skipper",
                table: "vessel_owner_profiles",
                column: "IsDeleted");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "business_customer_contacts",
                schema: "skipper");

            migrationBuilder.DropTable(
                name: "customers",
                schema: "skipper");

            migrationBuilder.DropTable(
                name: "individual_customer_profiles",
                schema: "skipper");

            migrationBuilder.DropTable(
                name: "MemberCustomerProfiles",
                schema: "skipper");

            migrationBuilder.DropTable(
                name: "vessel_owner_vessels",
                schema: "skipper");

            migrationBuilder.DropTable(
                name: "business_customer_profiles",
                schema: "skipper");

            migrationBuilder.DropTable(
                name: "vessel_owner_profiles",
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
