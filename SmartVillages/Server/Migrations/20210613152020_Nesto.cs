using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartVillages.Server.Migrations
{
    public partial class Nesto : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_Orders_OrderId",
                table: "CartItems");

            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_User_SellerId",
                table: "CartItems");

            migrationBuilder.DropIndex(
                name: "IX_CartItems_OrderId",
                table: "CartItems");

            migrationBuilder.DropIndex(
                name: "IX_CartItems_SellerId",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "SellerId",
                table: "CartItems");

            migrationBuilder.AlterColumn<int>(
                name: "OrderId",
                table: "CartItems",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "OrderId",
                table: "CartItems",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "SellerId",
                table: "CartItems",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_OrderId",
                table: "CartItems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_SellerId",
                table: "CartItems",
                column: "SellerId");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_Orders_OrderId",
                table: "CartItems",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_User_SellerId",
                table: "CartItems",
                column: "SellerId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
