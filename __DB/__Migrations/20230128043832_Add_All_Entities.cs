using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TechStoreAPI.DB.Migrations
{
    public partial class AddAllEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "contact_us_messages",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    email = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    mobile = table.Column<string>(type: "varchar(15)", maxLength: 15, nullable: true),
                    title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    content = table.Column<string>(type: "nvarchar(800)", maxLength: 800, nullable: false),
                    type_id = table.Column<byte>(name: "type_id", type: "tinyint", nullable: false),
                    reply_type_id = table.Column<byte>(name: "reply_type_id", type: "tinyint", nullable: false, defaultValue: (byte)0),
                    reply = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    replied_by = table.Column<string>(name: "replied_by", type: "varchar(100)", maxLength: 100, nullable: true),
                    replied_at = table.Column<DateTime>(name: "replied_at", type: "datetime2(3)", nullable: true),
                    created_at = table.Column<DateTime>(name: "created_at", type: "datetime2(3)", nullable: false, defaultValueSql: "SYSDATETIME()"),
                    created_by = table.Column<string>(name: "created_by", type: "varchar(100)", maxLength: 100, nullable: false, defaultValue: "app_dev"),
                    modified_at = table.Column<DateTime>(name: "modified_at", type: "datetime2(3)", nullable: true),
                    modified_by = table.Column<string>(name: "modified_by", type: "varchar(100)", maxLength: 100, nullable: true),
                    is_deleted = table.Column<bool>(name: "is_deleted", type: "bit", nullable: false, defaultValue: false),
                    deleted_at = table.Column<DateTime>(name: "deleted_at", type: "datetime2(3)", nullable: true),
                    deleted_by = table.Column<string>(name: "deleted_by", type: "varchar(100)", maxLength: 100, nullable: true),
                    restored_at = table.Column<DateTime>(name: "restored_at", type: "datetime2(3)", nullable: true),
                    restored_by = table.Column<string>(name: "restored_by", type: "varchar(100)", maxLength: 100, nullable: true),
                    is_active = table.Column<bool>(name: "is_active", type: "bit", nullable: false, defaultValue: false),
                    activated_at = table.Column<DateTime>(name: "activated_at", type: "datetime2(3)", nullable: true),
                    activated_by = table.Column<string>(name: "activated_by", type: "varchar(100)", maxLength: 100, nullable: true),
                    disabled_at = table.Column<DateTime>(name: "disabled_at", type: "datetime2(3)", nullable: true),
                    disabled_by = table.Column<string>(name: "disabled_by", type: "varchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contact_us_messages", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "countries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_countries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "products",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    title = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "varchar(400)", maxLength: 400, nullable: false),
                    photo_url = table.Column<string>(name: "photo_url", type: "varchar(50)", maxLength: 50, nullable: false),
                    thumb_url = table.Column<string>(name: "thumb_url", type: "varchar(50)", maxLength: 50, nullable: false),
                    category_id = table.Column<byte>(name: "category_id", type: "tinyint", nullable: false),
                    created_at = table.Column<DateTime>(name: "created_at", type: "datetime2(3)", nullable: false, defaultValueSql: "SYSDATETIME()"),
                    created_by = table.Column<string>(name: "created_by", type: "varchar(100)", maxLength: 100, nullable: false, defaultValue: "app_dev"),
                    modified_at = table.Column<DateTime>(name: "modified_at", type: "datetime2(3)", nullable: true),
                    modified_by = table.Column<string>(name: "modified_by", type: "varchar(100)", maxLength: 100, nullable: true),
                    is_deleted = table.Column<bool>(name: "is_deleted", type: "bit", nullable: false, defaultValue: false),
                    deleted_at = table.Column<DateTime>(name: "deleted_at", type: "datetime2(3)", nullable: true),
                    deleted_by = table.Column<string>(name: "deleted_by", type: "varchar(100)", maxLength: 100, nullable: true),
                    restored_at = table.Column<DateTime>(name: "restored_at", type: "datetime2(3)", nullable: true),
                    restored_by = table.Column<string>(name: "restored_by", type: "varchar(100)", maxLength: 100, nullable: true),
                    is_active = table.Column<bool>(name: "is_active", type: "bit", nullable: false, defaultValue: false),
                    activated_at = table.Column<DateTime>(name: "activated_at", type: "datetime2(3)", nullable: true),
                    activated_by = table.Column<string>(name: "activated_by", type: "varchar(100)", maxLength: 100, nullable: true),
                    disabled_at = table.Column<DateTime>(name: "disabled_at", type: "datetime2(3)", nullable: true),
                    disabled_by = table.Column<string>(name: "disabled_by", type: "varchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_products", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RolesClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolesClaims", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    GenderId = table.Column<byte>(type: "tinyint", nullable: false),
                    BirthDate = table.Column<DateTime>(type: "datetime2(3)", nullable: true),
                    StatusId = table.Column<byte>(type: "tinyint", nullable: false),
                    LastLoggedAt = table.Column<DateTime>(type: "datetime2(3)", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UsersClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersClaims", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UsersLogins",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersLogins", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UsersTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersTokens", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "brands",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    title = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "varchar(400)", maxLength: 400, nullable: false),
                    logo_name = table.Column<string>(name: "logo_name", type: "varchar(500)", maxLength: 500, nullable: false),
                    country_id = table.Column<Guid>(name: "country_id", type: "uniqueidentifier", nullable: false),
                    created_at = table.Column<DateTime>(name: "created_at", type: "datetime2(3)", nullable: false, defaultValueSql: "SYSDATETIME()"),
                    created_by = table.Column<string>(name: "created_by", type: "varchar(100)", maxLength: 100, nullable: false, defaultValue: "app_dev"),
                    modified_at = table.Column<DateTime>(name: "modified_at", type: "datetime2(3)", nullable: true),
                    modified_by = table.Column<string>(name: "modified_by", type: "varchar(100)", maxLength: 100, nullable: true),
                    is_deleted = table.Column<bool>(name: "is_deleted", type: "bit", nullable: false, defaultValue: false),
                    deleted_at = table.Column<DateTime>(name: "deleted_at", type: "datetime2(3)", nullable: true),
                    deleted_by = table.Column<string>(name: "deleted_by", type: "varchar(100)", maxLength: 100, nullable: true),
                    restored_at = table.Column<DateTime>(name: "restored_at", type: "datetime2(3)", nullable: true),
                    restored_by = table.Column<string>(name: "restored_by", type: "varchar(100)", maxLength: 100, nullable: true),
                    is_active = table.Column<bool>(name: "is_active", type: "bit", nullable: false, defaultValue: false),
                    activated_at = table.Column<DateTime>(name: "activated_at", type: "datetime2(3)", nullable: true),
                    activated_by = table.Column<string>(name: "activated_by", type: "varchar(100)", maxLength: 100, nullable: true),
                    disabled_at = table.Column<DateTime>(name: "disabled_at", type: "datetime2(3)", nullable: true),
                    disabled_by = table.Column<string>(name: "disabled_by", type: "varchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_brands", x => x.id);
                    table.ForeignKey(
                        name: "brands_countries_fk",
                        column: x => x.country_id,
                        principalTable: "countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "cities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    country_id = table.Column<Guid>(name: "country_id", type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cities", x => x.Id);
                    table.ForeignKey(
                        name: "countries_cities_fk",
                        column: x => x.country_id,
                        principalTable: "countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "purchases",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    purchased_at = table.Column<DateTime>(name: "purchased_at", type: "datetime2(3)", nullable: false),
                    total_price = table.Column<double>(name: "total_price", type: "float", nullable: false),
                    employee_id = table.Column<Guid>(name: "employee_id", type: "uniqueidentifier", maxLength: 450, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_purchases", x => x.id);
                    table.ForeignKey(
                        name: "users_purchases_fk",
                        column: x => x.employee_id,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Value = table.Column<string>(type: "varchar(128)", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2(3)", nullable: false),
                    RevokedAt = table.Column<DateTime>(type: "datetime2(3)", nullable: true),
                    RevokedReason = table.Column<string>(type: "varchar(450)", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sales",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    sold_at = table.Column<DateTime>(name: "sold_at", type: "datetime2(3)", nullable: false),
                    total_price = table.Column<double>(name: "total_price", type: "float", nullable: false, defaultValue: 0.0),
                    employee_id = table.Column<Guid>(name: "employee_id", type: "uniqueidentifier", maxLength: 450, nullable: false),
                    customer_id = table.Column<Guid>(name: "customer_id", type: "uniqueidentifier", maxLength: 450, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sales", x => x.id);
                    table.ForeignKey(
                        name: "users_customer_sales_fk",
                        column: x => x.customer_id,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "users_employee_sales_fk",
                        column: x => x.employee_id,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UsersRoles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UsersRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsersRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "models",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    title = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "varchar(400)", maxLength: 400, nullable: false),
                    photo_url = table.Column<string>(name: "photo_url", type: "varchar(50)", maxLength: 50, nullable: false),
                    thumb_url = table.Column<string>(name: "thumb_url", type: "varchar(50)", maxLength: 50, nullable: false),
                    category_id = table.Column<byte>(name: "category_id", type: "tinyint", nullable: false),
                    product_id = table.Column<Guid>(name: "product_id", type: "uniqueidentifier", nullable: false),
                    brand_id = table.Column<Guid>(name: "brand_id", type: "uniqueidentifier", nullable: false),
                    rating_count = table.Column<long>(name: "rating_count", type: "bigint", nullable: false, defaultValue: 0L),
                    rating_average = table.Column<double>(name: "rating_average", type: "float", nullable: false, defaultValue: 0.0),
                    created_at = table.Column<DateTime>(name: "created_at", type: "datetime2(3)", nullable: false, defaultValueSql: "SYSDATETIME()"),
                    created_by = table.Column<string>(name: "created_by", type: "varchar(100)", maxLength: 100, nullable: false, defaultValue: "app_dev"),
                    modified_at = table.Column<DateTime>(name: "modified_at", type: "datetime2(3)", nullable: true),
                    modified_by = table.Column<string>(name: "modified_by", type: "varchar(100)", maxLength: 100, nullable: true),
                    is_deleted = table.Column<bool>(name: "is_deleted", type: "bit", nullable: false, defaultValue: false),
                    deleted_at = table.Column<DateTime>(name: "deleted_at", type: "datetime2(3)", nullable: true),
                    deleted_by = table.Column<string>(name: "deleted_by", type: "varchar(100)", maxLength: 100, nullable: true),
                    restored_at = table.Column<DateTime>(name: "restored_at", type: "datetime2(3)", nullable: true),
                    restored_by = table.Column<string>(name: "restored_by", type: "varchar(100)", maxLength: 100, nullable: true),
                    is_active = table.Column<bool>(name: "is_active", type: "bit", nullable: false, defaultValue: false),
                    activated_at = table.Column<DateTime>(name: "activated_at", type: "datetime2(3)", nullable: true),
                    activated_by = table.Column<string>(name: "activated_by", type: "varchar(100)", maxLength: 100, nullable: true),
                    disabled_at = table.Column<DateTime>(name: "disabled_at", type: "datetime2(3)", nullable: true),
                    disabled_by = table.Column<string>(name: "disabled_by", type: "varchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_models", x => x.id);
                    table.ForeignKey(
                        name: "brands_models_fk",
                        column: x => x.brand_id,
                        principalTable: "brands",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "products_models_fk",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "suppliers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    company_name = table.Column<string>(name: "company_name", type: "nvarchar(100)", maxLength: 100, nullable: false),
                    company_address = table.Column<string>(name: "company_address", type: "nvarchar(500)", maxLength: 500, nullable: false),
                    contact_name = table.Column<string>(name: "contact_name", type: "nvarchar(100)", maxLength: 100, nullable: false),
                    contact_title = table.Column<string>(name: "contact_title", type: "nvarchar(100)", maxLength: 100, nullable: false),
                    mobile = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true),
                    phone = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true),
                    fax = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true),
                    city_id = table.Column<Guid>(name: "city_id", type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_suppliers", x => x.Id);
                    table.ForeignKey(
                        name: "cities_suppliers_fk",
                        column: x => x.city_id,
                        principalTable: "cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "comments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    text = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    customer_id = table.Column<Guid>(name: "customer_id", type: "uniqueidentifier", nullable: false),
                    model_id = table.Column<Guid>(name: "model_id", type: "uniqueidentifier", nullable: false),
                    is_approved = table.Column<bool>(name: "is_approved", type: "bit", nullable: false, defaultValue: false),
                    approved_by = table.Column<string>(name: "approved_by", type: "varchar(100)", maxLength: 100, nullable: true),
                    approved_at = table.Column<DateTime>(name: "approved_at", type: "datetime2(3)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_comments", x => x.Id);
                    table.ForeignKey(
                        name: "models_comments_fk",
                        column: x => x.model_id,
                        principalTable: "models",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "users_comments_fk",
                        column: x => x.customer_id,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "customers_favorites_models",
                columns: table => new
                {
                    customerid = table.Column<Guid>(name: "customer_id", type: "uniqueidentifier", nullable: false),
                    modelid = table.Column<Guid>(name: "model_id", type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_customers_favorites_models", x => new { x.customerid, x.modelid });
                    table.ForeignKey(
                        name: "models_customers_favorites_models_fk",
                        column: x => x.modelid,
                        principalTable: "models",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "users_customers_favorites_models_fk",
                        column: x => x.customerid,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "purchases_items",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    quantity = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    unitprice = table.Column<double>(name: "unit_price", type: "float", nullable: false, defaultValue: 0.0),
                    totalprice = table.Column<double>(name: "total_price", type: "float", nullable: false, defaultValue: 0.0),
                    modelid = table.Column<Guid>(name: "model_id", type: "uniqueidentifier", nullable: false),
                    purchaseid = table.Column<Guid>(name: "purchase_id", type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_purchases_items", x => x.id);
                    table.ForeignKey(
                        name: "models_purchases_items_fk",
                        column: x => x.modelid,
                        principalTable: "models",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "purchases_purchases_items_fk",
                        column: x => x.purchaseid,
                        principalTable: "purchases",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ratings",
                columns: table => new
                {
                    customerid = table.Column<Guid>(name: "customer_id", type: "uniqueidentifier", nullable: false),
                    modelid = table.Column<Guid>(name: "model_id", type: "uniqueidentifier", nullable: false),
                    value = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ratings", x => new { x.customerid, x.modelid });
                    table.ForeignKey(
                        name: "models_ratings_fk",
                        column: x => x.modelid,
                        principalTable: "models",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "users_ratings_fk",
                        column: x => x.customerid,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "sales_items",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    quantity = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    unitprice = table.Column<double>(name: "unit_price", type: "float", nullable: false, defaultValue: 0.0),
                    totalprice = table.Column<double>(name: "total_price", type: "float", nullable: false, defaultValue: 0.0),
                    modelid = table.Column<Guid>(name: "model_id", type: "uniqueidentifier", nullable: false),
                    saleid = table.Column<Guid>(name: "sale_id", type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sales_items", x => x.id);
                    table.ForeignKey(
                        name: "models_sales_items_fk",
                        column: x => x.modelid,
                        principalTable: "models",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "sales_sales_items_fk",
                        column: x => x.saleid,
                        principalTable: "sales",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "stocks",
                columns: table => new
                {
                    modelid = table.Column<Guid>(name: "model_id", type: "uniqueidentifier", nullable: false),
                    totalpurchasesprice = table.Column<double>(name: "total_purchases_price", type: "float", nullable: false, defaultValue: 0.0),
                    totalsalesprice = table.Column<double>(name: "total_sales_price", type: "float", nullable: false, defaultValue: 0.0),
                    profit = table.Column<double>(type: "float", nullable: false, defaultValue: 0.0),
                    totalpurchasesquantity = table.Column<long>(name: "total_purchases_quantity", type: "bigint", nullable: false, defaultValue: 0L),
                    totalsalesquantity = table.Column<long>(name: "total_sales_quantity", type: "bigint", nullable: false, defaultValue: 0L),
                    totalinstock = table.Column<long>(name: "total_in_stock", type: "bigint", nullable: false, defaultValue: 0L)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stocks", x => x.modelid);
                    table.ForeignKey(
                        name: "models_stocks_fk",
                        column: x => x.modelid,
                        principalTable: "models",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "replies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    text = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    customerid = table.Column<Guid>(name: "customer_id", type: "uniqueidentifier", nullable: false),
                    commentid = table.Column<Guid>(name: "comment_id", type: "uniqueidentifier", nullable: false),
                    isapproved = table.Column<bool>(name: "is_approved", type: "bit", nullable: false, defaultValue: false),
                    approvedby = table.Column<string>(name: "approved_by", type: "varchar(100)", maxLength: 100, nullable: true),
                    approvedat = table.Column<DateTime>(name: "approved_at", type: "datetime2(3)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_replies", x => x.Id);
                    table.ForeignKey(
                        name: "comments_replies_fk",
                        column: x => x.commentid,
                        principalTable: "comments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "users_replies_fk",
                        column: x => x.customerid,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "brand_title_unique_index",
                table: "brands",
                column: "title",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "brands_country_id_fk_index",
                table: "brands",
                column: "country_id");

            migrationBuilder.CreateIndex(
                name: "cities_country_id_fk_index",
                table: "cities",
                column: "country_id");

            migrationBuilder.CreateIndex(
                name: "city_name_unique_index",
                table: "cities",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "comments_customer_id_fk_index",
                table: "comments",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "comments_model_id_fk_index",
                table: "comments",
                column: "model_id");

            migrationBuilder.CreateIndex(
                name: "country_name_unique_index",
                table: "countries",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "customers_favorites_models_customer_id_fk_index",
                table: "customers_favorites_models",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "customers_favorites_models_model_id_fk_index",
                table: "customers_favorites_models",
                column: "model_id");

            migrationBuilder.CreateIndex(
                name: "model_title_unique_index",
                table: "models",
                column: "title",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "models_brand_id_fk_index",
                table: "models",
                column: "brand_id");

            migrationBuilder.CreateIndex(
                name: "models_product_id_fk_index",
                table: "models",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "product_title_unique_index",
                table: "products",
                column: "title",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "employee_id_fk_index",
                table: "purchases",
                column: "employee_id");

            migrationBuilder.CreateIndex(
                name: "purchases_items_model_id_fk_index",
                table: "purchases_items",
                column: "model_id");

            migrationBuilder.CreateIndex(
                name: "purchases_items_purchase_id_fk_index",
                table: "purchases_items",
                column: "purchase_id");

            migrationBuilder.CreateIndex(
                name: "ratings_customer_id_fk_index",
                table: "ratings",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "ratings_model_id_fk_index",
                table: "ratings",
                column: "model_id");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                table: "RefreshTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_Value",
                table: "RefreshTokens",
                column: "Value",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "replies_comment_id_fk_index",
                table: "replies",
                column: "comment_id");

            migrationBuilder.CreateIndex(
                name: "replies_customer_id_fk_index",
                table: "replies",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "customer_id_fk_index",
                table: "sales",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "employee_id_fk_index",
                table: "sales",
                column: "employee_id");

            migrationBuilder.CreateIndex(
                name: "sales_items_model_id_fk_index",
                table: "sales_items",
                column: "model_id");

            migrationBuilder.CreateIndex(
                name: "sales_items_sale_id_fk_index",
                table: "sales_items",
                column: "sale_id");

            migrationBuilder.CreateIndex(
                name: "cities_country_id_fk_index",
                table: "suppliers",
                column: "city_id");

            migrationBuilder.CreateIndex(
                name: "supplier_company_name_unique_index",
                table: "suppliers",
                column: "company_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UsersRoles_RoleId",
                table: "UsersRoles",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "contact_us_messages");
            migrationBuilder.DropTable(name: "customers_favorites_models");
            migrationBuilder.DropTable(name: "ratings");
            migrationBuilder.DropTable(name: "replies");
            migrationBuilder.DropTable(name: "comments");

            migrationBuilder.DropTable(name: "purchases_items");
            migrationBuilder.DropTable(name: "sales_items");
            migrationBuilder.DropTable(name: "purchases");
            migrationBuilder.DropTable(name: "sales");
            migrationBuilder.DropTable(name: "stocks");

            migrationBuilder.DropTable(name: "models");
            migrationBuilder.DropTable(name: "brands");
            migrationBuilder.DropTable(name: "products");

            migrationBuilder.DropTable(name: "suppliers");
            migrationBuilder.DropTable(name: "cities");
            migrationBuilder.DropTable(name: "countries");

            migrationBuilder.DropTable(name: "RefreshTokens");
            migrationBuilder.DropTable(name: "RolesClaims");
            migrationBuilder.DropTable(name: "UsersClaims");
            migrationBuilder.DropTable(name: "UsersLogins");
            migrationBuilder.DropTable(name: "UsersRoles");
            migrationBuilder.DropTable(name: "UsersTokens");
            migrationBuilder.DropTable(name: "Roles");
            migrationBuilder.DropTable(name: "Users");
        }
    }
}
