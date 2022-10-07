using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tech_Store_API.__DB.__Migrations
{
    public partial class Add_Comment_Entities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "sale_id_fk_index",
                table: "sales_items",
                newName: "sales_items_sale_id_fk_index");

            migrationBuilder.RenameIndex(
                name: "model_id_fk_index1",
                table: "sales_items",
                newName: "sales_items_model_id_fk_index");

            migrationBuilder.RenameIndex(
                name: "purchase_id_fk_index",
                table: "purchases_items",
                newName: "purchases_items_purchase_id_fk_index");

            migrationBuilder.RenameIndex(
                name: "model_id_fk_index",
                table: "purchases_items",
                newName: "purchases_items_model_id_fk_index");

            migrationBuilder.RenameIndex(
                name: "product_id_fk_index",
                table: "models",
                newName: "models_product_id_fk_index");

            migrationBuilder.RenameIndex(
                name: "brand_id_fk_index",
                table: "models",
                newName: "models_brand_id_fk_index");

            migrationBuilder.CreateTable(
                name: "comments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    text = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    is_approved = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    approved_by = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    approved_at = table.Column<DateTime>(type: "datetime2(3)", nullable: true),
                    model_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    customer_id = table.Column<string>(type: "nvarchar(450)", nullable: true)
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
                name: "replies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    text = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    is_approved = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    approved_by = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    approved_at = table.Column<DateTime>(type: "datetime2(3)", nullable: true),
                    comment_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    customer_id = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_replies", x => x.Id);
                    table.ForeignKey(
                        name: "comments_replies_fk",
                        column: x => x.comment_id,
                        principalTable: "comments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "users_replies_fk",
                        column: x => x.customer_id,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "comments_customer_id_fk_index",
                table: "comments",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "comments_model_id_fk_index",
                table: "comments",
                column: "model_id");

            migrationBuilder.CreateIndex(
                name: "replies_comment_id_fk_index",
                table: "replies",
                column: "comment_id");

            migrationBuilder.CreateIndex(
                name: "replies_customer_id_fk_index",
                table: "replies",
                column: "customer_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "replies");

            migrationBuilder.DropTable(name: "comments");

            migrationBuilder.RenameIndex(
                name: "sales_items_sale_id_fk_index",
                table: "sales_items",
                newName: "sale_id_fk_index");

            migrationBuilder.RenameIndex(
                name: "sales_items_model_id_fk_index",
                table: "sales_items",
                newName: "model_id_fk_index1");

            migrationBuilder.RenameIndex(
                name: "purchases_items_purchase_id_fk_index",
                table: "purchases_items",
                newName: "purchase_id_fk_index");

            migrationBuilder.RenameIndex(
                name: "purchases_items_model_id_fk_index",
                table: "purchases_items",
                newName: "model_id_fk_index");

            migrationBuilder.RenameIndex(
                name: "models_product_id_fk_index",
                table: "models",
                newName: "product_id_fk_index");

            migrationBuilder.RenameIndex(
                name: "models_brand_id_fk_index",
                table: "models",
                newName: "brand_id_fk_index");
        }
    }
}
