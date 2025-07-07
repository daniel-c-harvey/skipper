using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AuthBlocksData.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "user");

            migrationBuilder.CreateTable(
                name: "roles",
                schema: "user",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    Modified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                schema: "user",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    Modified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "role_claims",
                schema: "user",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    Modified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    RoleId = table.Column<long>(type: "bigint", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_role_claims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_role_claims_roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "user",
                        principalTable: "roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_claims",
                schema: "user",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    Modified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_claims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_user_claims_users_UserId",
                        column: x => x.UserId,
                        principalSchema: "user",
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_logins",
                schema: "user",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    Modified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    LoginProvider = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_logins", x => x.Id);
                    table.ForeignKey(
                        name: "FK_user_logins_users_UserId",
                        column: x => x.UserId,
                        principalSchema: "user",
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_roles",
                schema: "user",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    Modified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    RoleId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_roles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_user_roles_roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "user",
                        principalTable: "roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_roles_users_UserId",
                        column: x => x.UserId,
                        principalSchema: "user",
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_tokens",
                schema: "user",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    Modified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    LoginProvider = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_tokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_user_tokens_users_UserId",
                        column: x => x.UserId,
                        principalSchema: "user",
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_role_claims_deleted",
                schema: "user",
                table: "role_claims",
                column: "Deleted");

            migrationBuilder.CreateIndex(
                name: "ix_role_claims_roleid",
                schema: "user",
                table: "role_claims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "ix_roles_deleted",
                schema: "user",
                table: "roles",
                column: "Deleted");

            migrationBuilder.CreateIndex(
                name: "ix_roles_normalizedname",
                schema: "user",
                table: "roles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_claims_deleted",
                schema: "user",
                table: "user_claims",
                column: "Deleted");

            migrationBuilder.CreateIndex(
                name: "ix_user_claims_userid",
                schema: "user",
                table: "user_claims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "ix_user_logins_deleted",
                schema: "user",
                table: "user_logins",
                column: "Deleted");

            migrationBuilder.CreateIndex(
                name: "ix_user_logins_loginprovider_providerkey",
                schema: "user",
                table: "user_logins",
                columns: new[] { "LoginProvider", "ProviderKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_logins_userid",
                schema: "user",
                table: "user_logins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "ix_user_roles_deleted",
                schema: "user",
                table: "user_roles",
                column: "Deleted");

            migrationBuilder.CreateIndex(
                name: "ix_user_roles_roleid",
                schema: "user",
                table: "user_roles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "ix_user_roles_userid",
                schema: "user",
                table: "user_roles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "ix_user_roles_userid_roleid",
                schema: "user",
                table: "user_roles",
                columns: new[] { "UserId", "RoleId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_tokens_deleted",
                schema: "user",
                table: "user_tokens",
                column: "Deleted");

            migrationBuilder.CreateIndex(
                name: "ix_user_tokens_userid",
                schema: "user",
                table: "user_tokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "ix_user_tokens_userid_loginprovider_name",
                schema: "user",
                table: "user_tokens",
                columns: new[] { "UserId", "LoginProvider", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_users_deleted",
                schema: "user",
                table: "users",
                column: "Deleted");

            migrationBuilder.CreateIndex(
                name: "ix_users_normalizedemail",
                schema: "user",
                table: "users",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "ix_users_normalizedusername",
                schema: "user",
                table: "users",
                column: "NormalizedUserName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "role_claims",
                schema: "user");

            migrationBuilder.DropTable(
                name: "user_claims",
                schema: "user");

            migrationBuilder.DropTable(
                name: "user_logins",
                schema: "user");

            migrationBuilder.DropTable(
                name: "user_roles",
                schema: "user");

            migrationBuilder.DropTable(
                name: "user_tokens",
                schema: "user");

            migrationBuilder.DropTable(
                name: "roles",
                schema: "user");

            migrationBuilder.DropTable(
                name: "users",
                schema: "user");
        }
    }
}
