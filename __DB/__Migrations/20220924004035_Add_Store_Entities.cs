using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tech_Store_API.__DB.__Migrations
{
    public partial class Add_Store_Entities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "purchases",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    purchased_at = table.Column<DateTime>(type: "datetime2(3)", nullable: false),
                    total_price = table.Column<double>(type: "float", nullable: false),
                    employee_id = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false)
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
                name: "sales",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    sold_at = table.Column<DateTime>(type: "datetime2(3)", nullable: false),
                    total_price = table.Column<double>(type: "float", nullable: false, defaultValue: 0.0),
                    employee_id = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    customer_id = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false)
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
                name: "stocks",
                columns: table => new
                {
                    model_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    total_purchases_price = table.Column<double>(type: "float", nullable: false, defaultValue: 0.0),
                    total_sales_price = table.Column<double>(type: "float", nullable: false, defaultValue: 0.0),
                    profit = table.Column<double>(type: "float", nullable: false, defaultValue: 0.0),
                    total_purchases_quantity = table.Column<long>(type: "bigint", nullable: false, defaultValue: 0L),
                    total_sales_quantity = table.Column<long>(type: "bigint", nullable: false, defaultValue: 0L),
                    total_in_stock = table.Column<long>(type: "bigint", nullable: false, defaultValue: 0L)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stocks", x => x.model_id);
                    table.ForeignKey(
                        name: "models_stocks_fk",
                        column: x => x.model_id,
                        principalTable: "models",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "purchases_items",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    quantity = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    unit_price = table.Column<double>(type: "float", nullable: false, defaultValue: 0.0),
                    total_price = table.Column<double>(type: "float", nullable: false, defaultValue: 0.0),
                    model_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    purchase_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_purchases_items", x => x.id);
                    table.ForeignKey(
                        name: "models_purchases_items_fk",
                        column: x => x.model_id,
                        principalTable: "models",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "purchases_purchases_items_fk",
                        column: x => x.purchase_id,
                        principalTable: "purchases",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "sales_items",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    quantity = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    unit_price = table.Column<double>(type: "float", nullable: false, defaultValue: 0.0),
                    total_price = table.Column<double>(type: "float", nullable: false, defaultValue: 0.0),
                    model_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    sale_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sales_items", x => x.id);
                    table.ForeignKey(
                        name: "models_sales_items_fk",
                        column: x => x.model_id,
                        principalTable: "models",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "sales_sales_items_fk",
                        column: x => x.sale_id,
                        principalTable: "sales",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "employee_id_fk_index",
                table: "purchases",
                column: "employee_id");

            migrationBuilder.CreateIndex(
                name: "model_id_fk_index",
                table: "purchases_items",
                column: "model_id");

            migrationBuilder.CreateIndex(
                name: "purchase_id_fk_index",
                table: "purchases_items",
                column: "purchase_id");

            migrationBuilder.CreateIndex(
                name: "customer_id_fk_index",
                table: "sales",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "employee_id_fk_index1",
                table: "sales",
                column: "employee_id");

            migrationBuilder.CreateIndex(
                name: "model_id_fk_index1",
                table: "sales_items",
                column: "model_id");

            migrationBuilder.CreateIndex(
                name: "sale_id_fk_index",
                table: "sales_items",
                column: "sale_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "purchases_items");

            migrationBuilder.DropTable(name: "sales_items");

            migrationBuilder.DropTable(name: "stocks");

            migrationBuilder.DropTable(name: "purchases");

            migrationBuilder.DropTable(name: "sales");
        }
    }
}
