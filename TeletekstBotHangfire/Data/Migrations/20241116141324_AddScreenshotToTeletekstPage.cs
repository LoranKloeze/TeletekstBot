using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeletekstBotHangfire.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddScreenshotToTeletekstPage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "Screenshot",
                table: "TeletekstPages",
                type: "bytea",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Screenshot",
                table: "TeletekstPages");
        }
    }
}
