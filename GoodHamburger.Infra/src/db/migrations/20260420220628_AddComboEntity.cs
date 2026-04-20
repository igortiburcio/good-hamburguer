using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoodHamburger.Infra.Src.db.migrations
{
    /// <inheritdoc />
    public partial class AddComboEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "products");

            migrationBuilder.AddColumn<Guid>(
                name: "category_id",
                table: "products",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "combos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    discount_percent = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_combos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "product_categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product_categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "combo_categories",
                columns: table => new
                {
                    combo_id = table.Column<Guid>(type: "uuid", nullable: false),
                    category_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_combo_categories", x => new { x.combo_id, x.category_id });
                    table.ForeignKey(
                        name: "FK_combo_categories_combos_combo_id",
                        column: x => x.combo_id,
                        principalTable: "combos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_combo_categories_product_categories_category_id",
                        column: x => x.category_id,
                        principalTable: "product_categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_products_category_id",
                table: "products",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "IX_combo_categories_category_id",
                table: "combo_categories",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "IX_product_categories_Name",
                table: "product_categories",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_products_product_categories_category_id",
                table: "products",
                column: "category_id",
                principalTable: "product_categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_products_product_categories_category_id",
                table: "products");

            migrationBuilder.DropTable(
                name: "combo_categories");

            migrationBuilder.DropTable(
                name: "combos");

            migrationBuilder.DropTable(
                name: "product_categories");

            migrationBuilder.DropIndex(
                name: "IX_products_category_id",
                table: "products");

            migrationBuilder.DropColumn(
                name: "category_id",
                table: "products");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "products",
                type: "character varying(60)",
                maxLength: 60,
                nullable: false,
                defaultValue: "");
        }
    }
}
