#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace EventEasePOE.Migrations
{
    public partial class CorrectEventTypeIds : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Ensure existing Events have a valid EventTypeId
            migrationBuilder.Sql("UPDATE Events SET EventTypeId = 1 WHERE EventTypeId = 0 OR EventTypeId IS NULL;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // No-op: we don't revert the correction automatically
        }
    }
}
