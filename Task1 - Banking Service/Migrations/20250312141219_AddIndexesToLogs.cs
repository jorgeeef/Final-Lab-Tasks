using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Task1___Banking_Service.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexesToLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_RequestId",
                table: "Logs",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_Timestamp",
                table: "Logs",
                column: "Timestamp");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RequestId",
                table: "Logs");

            migrationBuilder.DropIndex(
                name: "IX_Timestamp",
                table: "Logs");
        }
    }
}
