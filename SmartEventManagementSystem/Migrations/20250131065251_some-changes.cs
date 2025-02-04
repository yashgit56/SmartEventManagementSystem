using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Smart_Event_Management_System.Migrations
{
    /// <inheritdoc />
    public partial class somechanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "HashPassword",
                table: "Admins",
                newName: "Password");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Password",
                table: "Admins",
                newName: "HashPassword");
        }
    }
}
