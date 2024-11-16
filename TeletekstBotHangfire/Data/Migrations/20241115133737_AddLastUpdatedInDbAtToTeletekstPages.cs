using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeletekstBotHangfire.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddLastUpdatedInDbAtToTeletekstPages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdatedInDbAt",
                table: "TeletekstPages",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastUpdatedInDbAt",
                table: "TeletekstPages");
        }
    }
}
