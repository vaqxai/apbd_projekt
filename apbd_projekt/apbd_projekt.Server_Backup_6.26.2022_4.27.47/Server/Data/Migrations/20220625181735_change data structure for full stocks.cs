using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace apbd_projekt.Server.Data.Migrations
{
    public partial class changedatastructureforfullstocks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StockDays_Stocks_StockTicker",
                table: "StockDays");

            migrationBuilder.DropIndex(
                name: "IX_StockDays_StockTicker",
                table: "StockDays");

            migrationBuilder.DropColumn(
                name: "StockTicker",
                table: "StockDays");

            migrationBuilder.RenameColumn(
                name: "CeoName",
                table: "Stocks",
                newName: "LogoURL");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Stocks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_StockDays_Stocks_Ticker",
                table: "StockDays",
                column: "Ticker",
                principalTable: "Stocks",
                principalColumn: "Ticker",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StockDays_Stocks_Ticker",
                table: "StockDays");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Stocks");

            migrationBuilder.RenameColumn(
                name: "LogoURL",
                table: "Stocks",
                newName: "CeoName");

            migrationBuilder.AddColumn<string>(
                name: "StockTicker",
                table: "StockDays",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StockDays_StockTicker",
                table: "StockDays",
                column: "StockTicker");

            migrationBuilder.AddForeignKey(
                name: "FK_StockDays_Stocks_StockTicker",
                table: "StockDays",
                column: "StockTicker",
                principalTable: "Stocks",
                principalColumn: "Ticker");
        }
    }
}
