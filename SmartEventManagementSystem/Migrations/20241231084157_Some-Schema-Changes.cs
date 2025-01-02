using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Smart_Event_Management_System.Migrations
{
    /// <inheritdoc />
    public partial class SomeSchemaChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CheckInLogs_Attendees_AttendeeId",
                table: "CheckInLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_CheckInLogs_Events_EventId",
                table: "CheckInLogs");

            migrationBuilder.DropIndex(
                name: "IX_CheckInLogs_AttendeeId",
                table: "CheckInLogs");

            migrationBuilder.DropIndex(
                name: "IX_CheckInLogs_EventId",
                table: "CheckInLogs");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Attendees",
                newName: "Username");

            migrationBuilder.AddColumn<string>(
                name: "HashPassword",
                table: "Attendees",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Admins",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Username = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    HashPassword = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admins", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Admins");

            migrationBuilder.DropColumn(
                name: "HashPassword",
                table: "Attendees");

            migrationBuilder.RenameColumn(
                name: "Username",
                table: "Attendees",
                newName: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_CheckInLogs_AttendeeId",
                table: "CheckInLogs",
                column: "AttendeeId");

            migrationBuilder.CreateIndex(
                name: "IX_CheckInLogs_EventId",
                table: "CheckInLogs",
                column: "EventId");

            migrationBuilder.AddForeignKey(
                name: "FK_CheckInLogs_Attendees_AttendeeId",
                table: "CheckInLogs",
                column: "AttendeeId",
                principalTable: "Attendees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CheckInLogs_Events_EventId",
                table: "CheckInLogs",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
