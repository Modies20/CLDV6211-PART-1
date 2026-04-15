using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EventEasePOE.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    EventId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    EventDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EventDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageURL = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.EventId);
                });

            migrationBuilder.CreateTable(
                name: "Venues",
                columns: table => new
                {
                    VenueId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VenueName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    VenueLocation = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    ImageURL = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Venues", x => x.VenueId);
                });

            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    BookingId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventId = table.Column<int>(type: "int", nullable: false),
                    VenueId = table.Column<int>(type: "int", nullable: false),
                    BookingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CustomerName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CustomerEmail = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    BookingStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.BookingId);
                    table.ForeignKey(
                        name: "FK_Bookings_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "EventId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Bookings_Venues_VenueId",
                        column: x => x.VenueId,
                        principalTable: "Venues",
                        principalColumn: "VenueId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Events",
                columns: new[] { "EventId", "EventDate", "EventDescription", "EventName", "ImageURL" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Annual technology conference featuring industry leaders", "Annual Tech Summit", "https://picsum.photos/id/0/400/300" },
                    { 2, new DateTime(2026, 6, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Showcase of wedding vendors and services", "Wedding Expo", "https://picsum.photos/id/26/400/300" },
                    { 3, new DateTime(2026, 7, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Annual charity fundraising gala", "Corporate Gala", "https://picsum.photos/id/29/400/300" }
                });

            migrationBuilder.InsertData(
                table: "Venues",
                columns: new[] { "VenueId", "Capacity", "ImageURL", "VenueLocation", "VenueName" },
                values: new object[,]
                {
                    { 1, 500, "https://picsum.photos/id/20/400/300", "123 Main St, Downtown", "Grand Conference Hall" },
                    { 2, 200, "https://picsum.photos/id/15/400/300", "456 Park Ave, Uptown", "Sunset Garden" },
                    { 3, 150, "https://picsum.photos/id/30/400/300", "789 High St, City Center", "Skyline Rooftop" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_EventId",
                table: "Bookings",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueVenueBooking",
                table: "Bookings",
                columns: new[] { "VenueId", "StartDateTime", "EndDateTime" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bookings");

            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropTable(
                name: "Venues");
        }
    }
}
