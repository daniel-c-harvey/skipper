using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SkipperData.Data.Migrations
{
    /// <inheritdoc />
    public partial class OrdersCustomersTPH : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_business_customer_contacts_business_customer_profiles_Busin~",
                schema: "skipper",
                table: "business_customer_contacts");

            migrationBuilder.DropForeignKey(
                name: "FK_business_customer_contacts_business_customer_profiles_Busi~1",
                schema: "skipper",
                table: "business_customer_contacts");

            migrationBuilder.DropForeignKey(
                name: "FK_vessel_owner_vessels_vessel_owner_profiles_VesselOwnerProfi~",
                schema: "skipper",
                table: "vessel_owner_vessels");

            migrationBuilder.DropTable(
                name: "business_customer_profiles",
                schema: "skipper");

            migrationBuilder.DropTable(
                name: "individual_customer_profiles",
                schema: "skipper");

            migrationBuilder.DropTable(
                name: "MemberCustomerProfiles",
                schema: "skipper");

            migrationBuilder.DropTable(
                name: "slip_reservations",
                schema: "skipper");

            migrationBuilder.DropTable(
                name: "vessel_owner_profiles",
                schema: "skipper");

            migrationBuilder.DropIndex(
                name: "IX_customers_CustomerProfileId_CustomerProfileType",
                schema: "skipper",
                table: "customers");

            migrationBuilder.DropIndex(
                name: "IX_business_customer_contacts_BusinessCustomerProfileEntityId",
                schema: "skipper",
                table: "business_customer_contacts");

            migrationBuilder.DropColumn(
                name: "CustomerProfileId",
                schema: "skipper",
                table: "customers");

            migrationBuilder.DropColumn(
                name: "BusinessCustomerProfileEntityId",
                schema: "skipper",
                table: "business_customer_contacts");

            migrationBuilder.RenameColumn(
                name: "VesselOwnerProfileId",
                schema: "skipper",
                table: "vessel_owner_vessels",
                newName: "VesselOwnerCustomerId");

            migrationBuilder.RenameColumn(
                name: "BusinessCustomerProfileId",
                schema: "skipper",
                table: "business_customer_contacts",
                newName: "BusinessCustomerId");

            migrationBuilder.AlterColumn<int>(
                name: "CustomerProfileType",
                schema: "skipper",
                table: "customers",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "BusinessName",
                schema: "skipper",
                table: "customers",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                collation: "en-US-x-icu");

            migrationBuilder.AddColumn<long>(
                name: "ContactId",
                schema: "skipper",
                table: "customers",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfBirth",
                schema: "skipper",
                table: "customers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LicenseExpiryDate",
                schema: "skipper",
                table: "customers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LicenseNumber",
                schema: "skipper",
                table: "customers",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                collation: "en-US-x-icu");

            migrationBuilder.AddColumn<DateTime>(
                name: "MemberSince",
                schema: "skipper",
                table: "customers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MembershipLevel",
                schema: "skipper",
                table: "customers",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                collation: "en-US-x-icu");

            migrationBuilder.AddColumn<string>(
                name: "MembershipNumber",
                schema: "skipper",
                table: "customers",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                collation: "en-US-x-icu");

            migrationBuilder.AddColumn<string>(
                name: "PreferredContactMethod",
                schema: "skipper",
                table: "customers",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                collation: "en-US-x-icu");

            migrationBuilder.AddColumn<string>(
                name: "TaxId",
                schema: "skipper",
                table: "customers",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                collation: "en-US-x-icu");

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

            migrationBuilder.CreateIndex(
                name: "IX_customers_ContactId",
                schema: "skipper",
                table: "customers",
                column: "ContactId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_business_customer_contacts_customers_BusinessCustomerId",
                schema: "skipper",
                table: "business_customer_contacts",
                column: "BusinessCustomerId",
                principalSchema: "skipper",
                principalTable: "customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_customers_contacts_ContactId",
                schema: "skipper",
                table: "customers",
                column: "ContactId",
                principalSchema: "skipper",
                principalTable: "contacts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_vessel_owner_vessels_customers_VesselOwnerCustomerId",
                schema: "skipper",
                table: "vessel_owner_vessels",
                column: "VesselOwnerCustomerId",
                principalSchema: "skipper",
                principalTable: "customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_business_customer_contacts_customers_BusinessCustomerId",
                schema: "skipper",
                table: "business_customer_contacts");

            migrationBuilder.DropForeignKey(
                name: "FK_customers_contacts_ContactId",
                schema: "skipper",
                table: "customers");

            migrationBuilder.DropForeignKey(
                name: "FK_vessel_owner_vessels_customers_VesselOwnerCustomerId",
                schema: "skipper",
                table: "vessel_owner_vessels");

            migrationBuilder.DropTable(
                name: "orders",
                schema: "skipper");

            migrationBuilder.DropIndex(
                name: "IX_customers_ContactId",
                schema: "skipper",
                table: "customers");

            migrationBuilder.DropColumn(
                name: "BusinessName",
                schema: "skipper",
                table: "customers");

            migrationBuilder.DropColumn(
                name: "ContactId",
                schema: "skipper",
                table: "customers");

            migrationBuilder.DropColumn(
                name: "DateOfBirth",
                schema: "skipper",
                table: "customers");

            migrationBuilder.DropColumn(
                name: "LicenseExpiryDate",
                schema: "skipper",
                table: "customers");

            migrationBuilder.DropColumn(
                name: "LicenseNumber",
                schema: "skipper",
                table: "customers");

            migrationBuilder.DropColumn(
                name: "MemberSince",
                schema: "skipper",
                table: "customers");

            migrationBuilder.DropColumn(
                name: "MembershipLevel",
                schema: "skipper",
                table: "customers");

            migrationBuilder.DropColumn(
                name: "MembershipNumber",
                schema: "skipper",
                table: "customers");

            migrationBuilder.DropColumn(
                name: "PreferredContactMethod",
                schema: "skipper",
                table: "customers");

            migrationBuilder.DropColumn(
                name: "TaxId",
                schema: "skipper",
                table: "customers");

            migrationBuilder.RenameColumn(
                name: "VesselOwnerCustomerId",
                schema: "skipper",
                table: "vessel_owner_vessels",
                newName: "VesselOwnerProfileId");

            migrationBuilder.RenameColumn(
                name: "BusinessCustomerId",
                schema: "skipper",
                table: "business_customer_contacts",
                newName: "BusinessCustomerProfileId");

            migrationBuilder.AlterColumn<string>(
                name: "CustomerProfileType",
                schema: "skipper",
                table: "customers",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<long>(
                name: "CustomerProfileId",
                schema: "skipper",
                table: "customers",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "BusinessCustomerProfileEntityId",
                schema: "skipper",
                table: "business_customer_contacts",
                type: "bigint",
                nullable: true);

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

            migrationBuilder.CreateTable(
                name: "business_customer_profiles",
                schema: "skipper",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BusinessName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false, collation: "en-US-x-icu"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    TaxId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true, collation: "en-US-x-icu"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_business_customer_profiles", x => x.Id);
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
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
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
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    MembershipEndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    MembershipLevel = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true, collation: "en-US-x-icu"),
                    MembershipStartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
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
                name: "slip_reservations",
                schema: "skipper",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SlipId = table.Column<long>(type: "bigint", nullable: false),
                    VesselId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    PriceRate = table.Column<int>(type: "integer", nullable: false),
                    PriceUnit = table.Column<string>(type: "text", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_slip_reservations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_slip_reservations_slips_SlipId",
                        column: x => x.SlipId,
                        principalSchema: "skipper",
                        principalTable: "slips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_slip_reservations_vessels_VesselId",
                        column: x => x.VesselId,
                        principalSchema: "skipper",
                        principalTable: "vessels",
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
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_customers_CustomerProfileId_CustomerProfileType",
                schema: "skipper",
                table: "customers",
                columns: new[] { "CustomerProfileId", "CustomerProfileType" });

            migrationBuilder.CreateIndex(
                name: "IX_business_customer_contacts_BusinessCustomerProfileEntityId",
                schema: "skipper",
                table: "business_customer_contacts",
                column: "BusinessCustomerProfileEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_business_customer_profiles_IsDeleted",
                schema: "skipper",
                table: "business_customer_profiles",
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
                name: "IX_slip_reservations_IsDeleted",
                schema: "skipper",
                table: "slip_reservations",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_slip_reservations_SlipId",
                schema: "skipper",
                table: "slip_reservations",
                column: "SlipId");

            migrationBuilder.CreateIndex(
                name: "IX_slip_reservations_Status",
                schema: "skipper",
                table: "slip_reservations",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_slip_reservations_VesselId",
                schema: "skipper",
                table: "slip_reservations",
                column: "VesselId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_business_customer_contacts_business_customer_profiles_Busin~",
                schema: "skipper",
                table: "business_customer_contacts",
                column: "BusinessCustomerProfileEntityId",
                principalSchema: "skipper",
                principalTable: "business_customer_profiles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_business_customer_contacts_business_customer_profiles_Busi~1",
                schema: "skipper",
                table: "business_customer_contacts",
                column: "BusinessCustomerProfileId",
                principalSchema: "skipper",
                principalTable: "business_customer_profiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_vessel_owner_vessels_vessel_owner_profiles_VesselOwnerProfi~",
                schema: "skipper",
                table: "vessel_owner_vessels",
                column: "VesselOwnerProfileId",
                principalSchema: "skipper",
                principalTable: "vessel_owner_profiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
