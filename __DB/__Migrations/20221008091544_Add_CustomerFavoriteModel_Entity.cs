using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tech_Store_API.__DB.__Migrations
{
    public partial class Add_CustomerFavoriteModel_Entity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "customers_favorites_models",
                columns: table => new
                {
                    customer_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    model_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_customers_favorites_models", x => new { x.customer_id, x.model_id });
                    table.ForeignKey(
                        name: "models_customers_favorites_models_fk",
                        column: x => x.model_id,
                        principalTable: "models",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "users_customers_favorites_models_fk",
                        column: x => x.customer_id,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "customers_favorites_models_customer_id_fk_index",
                table: "customers_favorites_models",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "customers_favorites_models_model_id_fk_index",
                table: "customers_favorites_models",
                column: "model_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "customers_favorites_models");
        }
    }
}
