using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartVillages.Server.Migrations
{
    public partial class ProductRate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_User_PersonOneId",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_User_PersonTwoId",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_User_BuyerId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_User_UserId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_User_Places_PlaceId",
                table: "User");

            migrationBuilder.DropForeignKey(
                name: "FK_User_UserImage_UserImageId",
                table: "User");

            migrationBuilder.DropForeignKey(
                name: "FK_User_UserTypes_UserTypeId",
                table: "User");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserImage",
                table: "UserImage");

            migrationBuilder.DropPrimaryKey(
                name: "PK_User",
                table: "User");

            migrationBuilder.RenameTable(
                name: "UserImage",
                newName: "UserImages");

            migrationBuilder.RenameTable(
                name: "User",
                newName: "Users");

            migrationBuilder.RenameIndex(
                name: "IX_User_UserTypeId",
                table: "Users",
                newName: "IX_Users_UserTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_User_UserImageId",
                table: "Users",
                newName: "IX_Users_UserImageId");

            migrationBuilder.RenameIndex(
                name: "IX_User_PlaceId",
                table: "Users",
                newName: "IX_Users_PlaceId");

            migrationBuilder.RenameIndex(
                name: "IX_User_Email_OIB",
                table: "Users",
                newName: "IX_Users_Email_OIB");

            migrationBuilder.AddColumn<int>(
                name: "ProductRateId",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserImages",
                table: "UserImages",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "ProductRate",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    Rate = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductRate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductRate_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_ProductRateId",
                table: "Products",
                column: "ProductRateId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductRate_UserId",
                table: "ProductRate",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Users_PersonOneId",
                table: "Messages",
                column: "PersonOneId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Users_PersonTwoId",
                table: "Messages",
                column: "PersonTwoId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Users_BuyerId",
                table: "Orders",
                column: "BuyerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_ProductRate_ProductRateId",
                table: "Products",
                column: "ProductRateId",
                principalTable: "ProductRate",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Users_UserId",
                table: "Products",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Places_PlaceId",
                table: "Users",
                column: "PlaceId",
                principalTable: "Places",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_UserImages_UserImageId",
                table: "Users",
                column: "UserImageId",
                principalTable: "UserImages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_UserTypes_UserTypeId",
                table: "Users",
                column: "UserTypeId",
                principalTable: "UserTypes",
                principalColumn: "UserTypeId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Users_PersonOneId",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Users_PersonTwoId",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Users_BuyerId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_ProductRate_ProductRateId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Users_UserId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Places_PlaceId",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_UserImages_UserImageId",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_UserTypes_UserTypeId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "ProductRate");

            migrationBuilder.DropIndex(
                name: "IX_Products_ProductRateId",
                table: "Products");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserImages",
                table: "UserImages");

            migrationBuilder.DropColumn(
                name: "ProductRateId",
                table: "Products");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "User");

            migrationBuilder.RenameTable(
                name: "UserImages",
                newName: "UserImage");

            migrationBuilder.RenameIndex(
                name: "IX_Users_UserTypeId",
                table: "User",
                newName: "IX_User_UserTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_Users_UserImageId",
                table: "User",
                newName: "IX_User_UserImageId");

            migrationBuilder.RenameIndex(
                name: "IX_Users_PlaceId",
                table: "User",
                newName: "IX_User_PlaceId");

            migrationBuilder.RenameIndex(
                name: "IX_Users_Email_OIB",
                table: "User",
                newName: "IX_User_Email_OIB");

            migrationBuilder.AddPrimaryKey(
                name: "PK_User",
                table: "User",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserImage",
                table: "UserImage",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_User_PersonOneId",
                table: "Messages",
                column: "PersonOneId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_User_PersonTwoId",
                table: "Messages",
                column: "PersonTwoId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_User_BuyerId",
                table: "Orders",
                column: "BuyerId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_User_UserId",
                table: "Products",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_User_Places_PlaceId",
                table: "User",
                column: "PlaceId",
                principalTable: "Places",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_User_UserImage_UserImageId",
                table: "User",
                column: "UserImageId",
                principalTable: "UserImage",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_User_UserTypes_UserTypeId",
                table: "User",
                column: "UserTypeId",
                principalTable: "UserTypes",
                principalColumn: "UserTypeId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
