using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace apbd_projekt.Server.Data.Migrations
{
    public partial class addmanytomanytocachedstocksearchresults : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StockStumps_CachedSearches_CachedStockSearchSearchId",
                table: "StockStumps");

            migrationBuilder.DropIndex(
                name: "IX_StockStumps_CachedStockSearchSearchId",
                table: "StockStumps");

            migrationBuilder.DropColumn(
                name: "CachedStockSearchSearchId",
                table: "StockStumps");

            migrationBuilder.DropColumn(
                name: "SearchId",
                table: "StockStumps");

            migrationBuilder.CreateTable(
                name: "CachedSimpleStockCachedStockSearch",
                columns: table => new
                {
                    AppearsInSearchId = table.Column<int>(type: "int", nullable: false),
                    SearchResultTicker = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CachedSimpleStockCachedStockSearch", x => new { x.AppearsInSearchId, x.SearchResultTicker });
                    table.ForeignKey(
                        name: "FK_CachedSimpleStockCachedStockSearch_CachedSearches_AppearsInSearchId",
                        column: x => x.AppearsInSearchId,
                        principalTable: "CachedSearches",
                        principalColumn: "SearchId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CachedSimpleStockCachedStockSearch_StockStumps_SearchResultTicker",
                        column: x => x.SearchResultTicker,
                        principalTable: "StockStumps",
                        principalColumn: "Ticker",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CachedSimpleStockCachedStockSearch_SearchResultTicker",
                table: "CachedSimpleStockCachedStockSearch",
                column: "SearchResultTicker");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CachedSimpleStockCachedStockSearch");

            migrationBuilder.AddColumn<int>(
                name: "CachedStockSearchSearchId",
                table: "StockStumps",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SearchId",
                table: "StockStumps",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StockStumps_CachedStockSearchSearchId",
                table: "StockStumps",
                column: "CachedStockSearchSearchId");

            migrationBuilder.AddForeignKey(
                name: "FK_StockStumps_CachedSearches_CachedStockSearchSearchId",
                table: "StockStumps",
                column: "CachedStockSearchSearchId",
                principalTable: "CachedSearches",
                principalColumn: "SearchId");
        }
    }
}
