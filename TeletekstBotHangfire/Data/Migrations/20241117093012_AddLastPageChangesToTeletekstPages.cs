using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeletekstBotHangfire.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddLastPageChangesToTeletekstPages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LastPageChanges",
                table: "TeletekstPages",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastPageChanges",
                table: "TeletekstPages");
        }
    }
}
