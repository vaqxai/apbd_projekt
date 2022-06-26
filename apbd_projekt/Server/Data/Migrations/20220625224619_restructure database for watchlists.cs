using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace apbd_projekt.Server.Data.Migrations
{
    public partial class restructuredatabaseforwatchlists : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CachedSimpleStockCachedStockSearch_CachedSearches_AppearsInSearchId",
                table: "CachedSimpleStockCachedStockSearch");

            migrationBuilder.DropForeignKey(
                name: "FK_CachedSimpleStockCachedStockSearch_StockStumps_SearchResultTicker",
                table: "CachedSimpleStockCachedStockSearch");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StockStumps",
                table: "StockStumps");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CachedSearches",
                table: "CachedSearches");

            migrationBuilder.RenameTable(
                name: "StockStumps",
                newName: "SimpleStocks");

            migrationBuilder.RenameTable(
                name: "CachedSearches",
                newName: "CachedStockSearches");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SimpleStocks",
                table: "SimpleStocks",
                column: "Ticker");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CachedStockSearches",
                table: "CachedStockSearches",
                column: "SearchId");

            migrationBuilder.CreateTable(
                name: "Watchlists",
                columns: table => new
                {
                    UserEmail = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Ticker = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Watchlists", x => new { x.Ticker, x.UserEmail });
                });

            migrationBuilder.AddForeignKey(
                name: "FK_CachedSimpleStockCachedStockSearch_CachedStockSearches_AppearsInSearchId",
                table: "CachedSimpleStockCachedStockSearch",
                column: "AppearsInSearchId",
                principalTable: "CachedStockSearches",
                principalColumn: "SearchId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CachedSimpleStockCachedStockSearch_SimpleStocks_SearchResultTicker",
                table: "CachedSimpleStockCachedStockSearch",
                column: "SearchResultTicker",
                principalTable: "SimpleStocks",
                principalColumn: "Ticker",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CachedSimpleStockCachedStockSearch_CachedStockSearches_AppearsInSearchId",
                table: "CachedSimpleStockCachedStockSearch");

            migrationBuilder.DropForeignKey(
                name: "FK_CachedSimpleStockCachedStockSearch_SimpleStocks_SearchResultTicker",
                table: "CachedSimpleStockCachedStockSearch");

            migrationBuilder.DropTable(
                name: "Watchlists");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SimpleStocks",
                table: "SimpleStocks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CachedStockSearches",
                table: "CachedStockSearches");

            migrationBuilder.RenameTable(
                name: "SimpleStocks",
                newName: "StockStumps");

            migrationBuilder.RenameTable(
                name: "CachedStockSearches",
                newName: "CachedSearches");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StockStumps",
                table: "StockStumps",
                column: "Ticker");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CachedSearches",
                table: "CachedSearches",
                column: "SearchId");

            migrationBuilder.AddForeignKey(
                name: "FK_CachedSimpleStockCachedStockSearch_CachedSearches_AppearsInSearchId",
                table: "CachedSimpleStockCachedStockSearch",
                column: "AppearsInSearchId",
                principalTable: "CachedSearches",
                principalColumn: "SearchId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CachedSimpleStockCachedStockSearch_StockStumps_SearchResultTicker",
                table: "CachedSimpleStockCachedStockSearch",
                column: "SearchResultTicker",
                principalTable: "StockStumps",
                principalColumn: "Ticker",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
