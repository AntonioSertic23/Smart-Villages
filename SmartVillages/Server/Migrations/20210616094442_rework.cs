using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartVillages.Server.Migrations
{
    public partial class rework : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_ProductRate_ProductRateId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_ProductRateId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ProductRateId",
                table: "Products");

            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "ProductRate",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductRate_ProductId",
                table: "ProductRate",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductRate_Products_ProductId",
                table: "ProductRate",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductRate_Products_ProductId",
                table: "ProductRate");

            migrationBuilder.DropIndex(
                name: "IX_ProductRate_ProductId",
                table: "ProductRate");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "ProductRate");

            migrationBuilder.AddColumn<int>(
                name: "ProductRateId",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_ProductRateId",
                table: "Products",
                column: "ProductRateId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_ProductRate_ProductRateId",
                table: "Products",
                column: "ProductRateId",
                principalTable: "ProductRate",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
