using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EventEasePOE.Migrations
{
    /// <inheritdoc />
    public partial class AddEventTypeSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Make this migration idempotent: only create/add objects if they do not already exist.
            migrationBuilder.Sql(@"
IF COL_LENGTH('dbo.Events','EventTypeId') IS NULL
BEGIN
    ALTER TABLE [Events] ADD [EventTypeId] int NOT NULL DEFAULT 1;
END

IF OBJECT_ID('dbo.EventTypes') IS NULL
BEGIN
    CREATE TABLE [EventTypes] (
        [EventTypeId] int NOT NULL IDENTITY,
        [CategoryName] nvarchar(100) NOT NULL,
        [Description] nvarchar(255) NULL,
        CONSTRAINT [PK_EventTypes] PRIMARY KEY ([EventTypeId])
    );

    SET IDENTITY_INSERT [EventTypes] ON;
    INSERT INTO [EventTypes] ([EventTypeId], [CategoryName], [Description]) VALUES
        (1, N'Conference', N'Business and tech conferences'),
        (2, N'Wedding', N'Wedding ceremonies and receptions'),
        (3, N'Concert', N'Live music performances'),
        (4, N'Corporate', N'Company meetings and galas'),
        (5, N'Private Party', N'Private celebrations and parties');
    SET IDENTITY_INSERT [EventTypes] OFF;
END
ELSE
BEGIN
    -- Insert any missing seed rows by EventTypeId
    IF NOT EXISTS(SELECT 1 FROM [EventTypes] WHERE [EventTypeId] = 1)
        INSERT INTO [EventTypes] ([CategoryName], [Description]) VALUES (N'Conference', N'Business and tech conferences');
    IF NOT EXISTS(SELECT 1 FROM [EventTypes] WHERE [EventTypeId] = 2)
        INSERT INTO [EventTypes] ([CategoryName], [Description]) VALUES (N'Wedding', N'Wedding ceremonies and receptions');
    IF NOT EXISTS(SELECT 1 FROM [EventTypes] WHERE [EventTypeId] = 3)
        INSERT INTO [EventTypes] ([CategoryName], [Description]) VALUES (N'Concert', N'Live music performances');
    IF NOT EXISTS(SELECT 1 FROM [EventTypes] WHERE [EventTypeId] = 4)
        INSERT INTO [EventTypes] ([CategoryName], [Description]) VALUES (N'Corporate', N'Company meetings and galas');
    IF NOT EXISTS(SELECT 1 FROM [EventTypes] WHERE [EventTypeId] = 5)
        INSERT INTO [EventTypes] ([CategoryName], [Description]) VALUES (N'Private Party', N'Private celebrations and parties');
END

-- Ensure existing events have a valid EventTypeId
UPDATE [Events] SET [EventTypeId] = 1 WHERE [EventTypeId] = 0 OR [EventTypeId] IS NULL;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Events_EventTypeId' AND object_id = OBJECT_ID('dbo.Events'))
    CREATE INDEX [IX_Events_EventTypeId] ON [Events] ([EventTypeId]);

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Events_EventTypes_EventTypeId')
BEGIN
    ALTER TABLE [Events] ADD CONSTRAINT [FK_Events_EventTypes_EventTypeId] FOREIGN KEY ([EventTypeId]) REFERENCES [EventTypes] ([EventTypeId]) ON DELETE CASCADE;
END
" );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_EventTypes_EventTypeId",
                table: "Events");

            migrationBuilder.DropTable(
                name: "EventTypes");

            migrationBuilder.DropIndex(
                name: "IX_Events_EventTypeId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "EventTypeId",
                table: "Events");
        }
    }
}
