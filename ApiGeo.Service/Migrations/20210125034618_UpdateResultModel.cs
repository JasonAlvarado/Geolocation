using Microsoft.EntityFrameworkCore.Migrations;

namespace ApiGeo.Service.Migrations
{
    public partial class UpdateResultModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Long",
                table: "GeoResults");

            migrationBuilder.AddColumn<string>(
                name: "Lon",
                table: "GeoResults",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Lon",
                table: "GeoResults");

            migrationBuilder.AddColumn<string>(
                name: "Long",
                table: "GeoResults",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
