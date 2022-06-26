using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace apbd_projekt.Server.Data.Migrations
{
    public partial class stockandsearchcache : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CachedSearches",
                columns: table => new
                {
                    SearchId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SearchTerm = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CachedSearches", x => x.SearchId);
                });

            migrationBuilder.CreateTable(
                name: "Stocks",
                columns: table => new
                {
                    StockId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ticker = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stocks", x => x.StockId);
                });

            migrationBuilder.CreateTable(
                name: "StockStumps",
                columns: table => new
                {
                    Ticker = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PrimaryExchange = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SearchId = table.Column<int>(type: "int", nullable: true),
                    CachedStockSearchSearchId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockStumps", x => x.Ticker);
                    table.ForeignKey(
                        name: "FK_StockStumps_CachedSearches_CachedStockSearchSearchId",
                        column: x => x.CachedStockSearchSearchId,
                        principalTable: "CachedSearches",
                        principalColumn: "SearchId");
                });

            migrationBuilder.CreateTable(
                name: "CachedStocks",
                columns: table => new
                {
                    CachedStockId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StockId = table.Column<int>(type: "int", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CachedStocks", x => x.CachedStockId);
                    table.ForeignKey(
                        name: "FK_CachedStocks_Stocks_StockId",
                        column: x => x.StockId,
                        principalTable: "Stocks",
                        principalColumn: "StockId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CachedStocks_StockId",
                table: "CachedStocks",
                column: "StockId");

            migrationBuilder.CreateIndex(
                name: "IX_StockStumps_CachedStockSearchSearchId",
                table: "StockStumps",
                column: "CachedStockSearchSearchId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CachedStocks");

            migrationBuilder.DropTable(
                name: "StockStumps");

            migrationBuilder.DropTable(
                name: "Stocks");

            migrationBuilder.DropTable(
                name: "CachedSearches");
        }
    }
}
