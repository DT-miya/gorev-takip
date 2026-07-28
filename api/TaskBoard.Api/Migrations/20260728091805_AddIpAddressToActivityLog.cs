using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskBoard.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddIpAddressToActivityLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IpAddress",
                table: "ActivityLogs",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IpAddress",
                table: "ActivityLogs");
        }
    }
}
