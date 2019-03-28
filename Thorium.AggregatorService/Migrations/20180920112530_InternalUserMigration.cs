using Microsoft.EntityFrameworkCore.Migrations;

namespace Thorium.Aggregator.Migrations
{
    public partial class InternalUserMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsInternal",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);

           
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsInternal",
                table: "AspNetUsers");

          
        }
    }
}
