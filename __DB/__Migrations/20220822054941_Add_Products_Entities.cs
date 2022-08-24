using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tech_Store_API.__DB.__Migrations
{
    public partial class Add_Products_Entities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "brands",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    title = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "varchar(400)", maxLength: 400, nullable: false),
                    logo_url = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2(3)", nullable: false, defaultValueSql: "SYSDATETIME()"),
                    created_by = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, defaultValue: "app_dev"),
                    modified_at = table.Column<DateTime>(type: "datetime2(3)", nullable: true),
                    modified_by = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    is_deleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    deleted_at = table.Column<DateTime>(type: "datetime2(3)", nullable: true),
                    deleted_by = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    restored_at = table.Column<DateTime>(type: "datetime2(3)", nullable: true),
                    restored_by = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    is_active = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    activated_at = table.Column<DateTime>(type: "datetime2(3)", nullable: true),
                    activated_by = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    disabled_at = table.Column<DateTime>(type: "datetime2(3)", nullable: true),
                    disabled_by = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_brands", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "products",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    title = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "varchar(400)", maxLength: 400, nullable: false),
                    category = table.Column<byte>(type: "tinyint", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2(3)", nullable: false, defaultValueSql: "SYSDATETIME()"),
                    created_by = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, defaultValue: "app_dev"),
                    modified_at = table.Column<DateTime>(type: "datetime2(3)", nullable: true),
                    modified_by = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    is_deleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    deleted_at = table.Column<DateTime>(type: "datetime2(3)", nullable: true),
                    deleted_by = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    restored_at = table.Column<DateTime>(type: "datetime2(3)", nullable: true),
                    restored_by = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    is_active = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    activated_at = table.Column<DateTime>(type: "datetime2(3)", nullable: true),
                    activated_by = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    disabled_at = table.Column<DateTime>(type: "datetime2(3)", nullable: true),
                    disabled_by = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_products", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "models",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    title = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "varchar(400)", maxLength: 400, nullable: false),
                    thumb_url = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false),
                    photo_url = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false),
                    category = table.Column<byte>(type: "tinyint", nullable: false),
                    product_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    brand_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2(3)", nullable: false, defaultValueSql: "SYSDATETIME()"),
                    created_by = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, defaultValue: "app_dev"),
                    modified_at = table.Column<DateTime>(type: "datetime2(3)", nullable: true),
                    modified_by = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    is_deleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    deleted_at = table.Column<DateTime>(type: "datetime2(3)", nullable: true),
                    deleted_by = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    restored_at = table.Column<DateTime>(type: "datetime2(3)", nullable: true),
                    restored_by = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    is_active = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    activated_at = table.Column<DateTime>(type: "datetime2(3)", nullable: true),
                    activated_by = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    disabled_at = table.Column<DateTime>(type: "datetime2(3)", nullable: true),
                    disabled_by = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
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

            migrationBuilder.CreateIndex(
                name: "product_title_unique_index",
                table: "products",
                column: "title",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "brand_title_unique_index",
                table: "brands",
                column: "title",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "model_title_unique_index",
                table: "models",
                column: "title",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "product_id_fk_index",
                table: "models",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "brand_id_fk_index",
                table: "models",
                column: "brand_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "models");

            migrationBuilder.DropTable(name: "brands");

            migrationBuilder.DropTable(name: "products");
        }
    }
}
