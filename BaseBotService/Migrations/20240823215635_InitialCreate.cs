using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BaseBotService.Migrations;

/// <inheritdoc />
public partial class InitialCreate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Guilds",
            columns: table => new
            {
                GuildId = table.Column<ulong>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                Settings = table.Column<int>(type: "INTEGER", nullable: false),
                ActivityPointsName = table.Column<string>(type: "TEXT", nullable: true),
                ActivityPointsSymbol = table.Column<string>(type: "TEXT", nullable: true),
                ActivityPointsAverageActiveHours = table.Column<double>(type: "REAL", nullable: false),
                ModeratorRoles = table.Column<string>(type: "TEXT", nullable: false),
                ArtistRoles = table.Column<string>(type: "TEXT", nullable: false),
                InternalNotificationChannel = table.Column<ulong>(type: "INTEGER", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Guilds", x => x.GuildId);
            });

        migrationBuilder.CreateTable(
            name: "Members",
            columns: table => new
            {
                MemberId = table.Column<ulong>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                Birthday = table.Column<DateTime>(type: "TEXT", nullable: true),
                Country = table.Column<int>(type: "INTEGER", nullable: false),
                Languages = table.Column<int>(type: "INTEGER", nullable: false),
                Timezone = table.Column<int>(type: "INTEGER", nullable: false),
                GenderIdentity = table.Column<int>(type: "INTEGER", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Members", x => x.MemberId);
            });

        migrationBuilder.CreateTable(
            name: "Achievements",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                MemberId = table.Column<ulong>(type: "INTEGER", nullable: false),
                GuildId = table.Column<ulong>(type: "INTEGER", nullable: true),
                CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                Name = table.Column<string>(type: "TEXT", nullable: false),
                Description = table.Column<string>(type: "TEXT", nullable: true),
                Emoji = table.Column<string>(type: "TEXT", nullable: false),
                Points = table.Column<int>(type: "INTEGER", nullable: false),
                ImageUrl = table.Column<string>(type: "TEXT", nullable: true),
                SourceIdentifier = table.Column<Guid>(type: "TEXT", nullable: false),
                AchievementType = table.Column<string>(type: "TEXT", maxLength: 21, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Achievements", x => x.Id);
                table.ForeignKey(
                    name: "FK_Achievements_Guilds_GuildId",
                    column: x => x.GuildId,
                    principalTable: "Guilds",
                    principalColumn: "GuildId",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_Achievements_Members_MemberId",
                    column: x => x.MemberId,
                    principalTable: "Members",
                    principalColumn: "MemberId",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "GuildMembers",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                GuildId = table.Column<ulong>(type: "INTEGER", nullable: false),
                MemberId = table.Column<ulong>(type: "INTEGER", nullable: false),
                LastActive = table.Column<DateTime>(type: "TEXT", nullable: false),
                ActivityPoints = table.Column<uint>(type: "INTEGER", nullable: false),
                LastActivityPoint = table.Column<DateTime>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_GuildMembers", x => x.Id);
                table.ForeignKey(
                    name: "FK_GuildMembers_Guilds_GuildId",
                    column: x => x.GuildId,
                    principalTable: "Guilds",
                    principalColumn: "GuildId",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_GuildMembers_Members_MemberId",
                    column: x => x.MemberId,
                    principalTable: "Members",
                    principalColumn: "MemberId",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Achievements_GuildId",
            table: "Achievements",
            column: "GuildId");

        migrationBuilder.CreateIndex(
            name: "IX_Achievements_GuildId_MemberId",
            table: "Achievements",
            columns: new[] { "GuildId", "MemberId" });

        migrationBuilder.CreateIndex(
            name: "IX_Achievements_MemberId",
            table: "Achievements",
            column: "MemberId");

        migrationBuilder.CreateIndex(
            name: "IX_Achievements_SourceIdentifier",
            table: "Achievements",
            column: "SourceIdentifier");

        migrationBuilder.CreateIndex(
            name: "IX_GuildMembers_GuildId",
            table: "GuildMembers",
            column: "GuildId");

        migrationBuilder.CreateIndex(
            name: "IX_GuildMembers_GuildId_MemberId",
            table: "GuildMembers",
            columns: new[] { "GuildId", "MemberId" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_GuildMembers_MemberId",
            table: "GuildMembers",
            column: "MemberId");

        migrationBuilder.CreateIndex(
            name: "IX_Guilds_GuildId",
            table: "Guilds",
            column: "GuildId",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Members_MemberId",
            table: "Members",
            column: "MemberId",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Achievements");

        migrationBuilder.DropTable(
            name: "GuildMembers");

        migrationBuilder.DropTable(
            name: "Guilds");

        migrationBuilder.DropTable(
            name: "Members");
    }
}
