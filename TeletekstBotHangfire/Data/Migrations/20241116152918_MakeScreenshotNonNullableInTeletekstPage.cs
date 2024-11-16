using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeletekstBotHangfire.Data.Migrations
{
    /// <inheritdoc />
    public partial class MakeScreenshotNonNullableInTeletekstPage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte[]>(
                name: "Screenshot",
                table: "TeletekstPages",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0],
                oldClrType: typeof(byte[]),
                oldType: "bytea",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte[]>(
                name: "Screenshot",
                table: "TeletekstPages",
                type: "bytea",
                nullable: true,
                oldClrType: typeof(byte[]),
                oldType: "bytea");
        }
    }
}
