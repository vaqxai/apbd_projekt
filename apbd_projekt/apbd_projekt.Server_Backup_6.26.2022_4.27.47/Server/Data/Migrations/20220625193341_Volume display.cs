using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace apbd_projekt.Server.Data.Migrations
{
    public partial class Volumedisplay : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Volume",
                table: "StockDays",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Volume",
                table: "StockDays");
        }
    }
}
