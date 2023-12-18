using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    public partial class _1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Genres",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ParentGenreId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genres", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Genres_Genres_ParentGenreId",
                        column: x => x.ParentGenreId,
                        principalTable: "Genres",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CustomerId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Sum = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Discount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaidDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Platforms",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Platforms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Publishers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HomePage = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Publishers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrderDetails",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProductId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Discount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OrderId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderDetails_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    Key = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Id = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UnitInStock = table.Column<int>(type: "int", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    Discontinued = table.Column<int>(type: "int", nullable: true),
                    PublisherId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Key);
                    table.ForeignKey(
                        name: "FK_Games_Publishers_PublisherId",
                        column: x => x.PublisherId,
                        principalTable: "Publishers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "GameGenres",
                columns: table => new
                {
                    GamesKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    GenresId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameGenres", x => new { x.GamesKey, x.GenresId });
                    table.ForeignKey(
                        name: "FK_GameGenres_Games_GamesKey",
                        column: x => x.GamesKey,
                        principalTable: "Games",
                        principalColumn: "Key",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameGenres_Genres_GenresId",
                        column: x => x.GenresId,
                        principalTable: "Genres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GamePlatforms",
                columns: table => new
                {
                    GamesKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PlatformsId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GamePlatforms", x => new { x.GamesKey, x.PlatformsId });
                    table.ForeignKey(
                        name: "FK_GamePlatforms_Games_GamesKey",
                        column: x => x.GamesKey,
                        principalTable: "Games",
                        principalColumn: "Key",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GamePlatforms_Platforms_PlatformsId",
                        column: x => x.PlatformsId,
                        principalTable: "Platforms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GameGenres_GenresId",
                table: "GameGenres",
                column: "GenresId");

            migrationBuilder.CreateIndex(
                name: "IX_GamePlatforms_PlatformsId",
                table: "GamePlatforms",
                column: "PlatformsId");

            migrationBuilder.CreateIndex(
                name: "IX_Games_PublisherId",
                table: "Games",
                column: "PublisherId");

            migrationBuilder.CreateIndex(
                name: "IX_Genres_ParentGenreId",
                table: "Genres",
                column: "ParentGenreId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_OrderId",
                table: "OrderDetails",
                column: "OrderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "GameGenres");

            migrationBuilder.DropTable(
                name: "GamePlatforms");

            migrationBuilder.DropTable(
                name: "OrderDetails");

            migrationBuilder.DropTable(
                name: "Genres");

            migrationBuilder.DropTable(
                name: "Games");

            migrationBuilder.DropTable(
                name: "Platforms");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Publishers");
        }
    }
}
