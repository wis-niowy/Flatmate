using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Flatmate.Migrations
{
    public partial class OneToManyUserTeamRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_Teams_TeamId",
                table: "Expenses");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Teams_TeamId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_RecurringBills_Teams_TeamId",
                table: "RecurringBills");

            migrationBuilder.DropForeignKey(
                name: "FK_ScheduledEventUser_ScheduledEvents_ScheduledEventId",
                table: "ScheduledEventUser");

            migrationBuilder.DropTable(
                name: "ScheduledEvents");

            migrationBuilder.DropTable(
                name: "UserTeam");

            migrationBuilder.DropIndex(
                name: "IX_RecurringBills_TeamId",
                table: "RecurringBills");

            migrationBuilder.DropIndex(
                name: "IX_Orders_TeamId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Expenses_TeamId",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "RecurringBills");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "Expenses");

            migrationBuilder.AddColumn<int>(
                name: "TeamId",
                table: "Users",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ScheduledEvent",
                columns: table => new
                {
                    ScheduledEventId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    OwnerId = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    Title = table.Column<string>(nullable: true),
                    Desription = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduledEvent", x => x.ScheduledEventId);
                    table.ForeignKey(
                        name: "FK_ScheduledEvent_Users_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_TeamId",
                table: "Users",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledEvent_OwnerId",
                table: "ScheduledEvent",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduledEventUser_ScheduledEvent_ScheduledEventId",
                table: "ScheduledEventUser",
                column: "ScheduledEventId",
                principalTable: "ScheduledEvent",
                principalColumn: "ScheduledEventId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Teams_TeamId",
                table: "Users",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "TeamId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScheduledEventUser_ScheduledEvent_ScheduledEventId",
                table: "ScheduledEventUser");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Teams_TeamId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "ScheduledEvent");

            migrationBuilder.DropIndex(
                name: "IX_Users_TeamId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "Users");

            migrationBuilder.AddColumn<int>(
                name: "TeamId",
                table: "RecurringBills",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TeamId",
                table: "Orders",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TeamId",
                table: "Expenses",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ScheduledEvents",
                columns: table => new
                {
                    ScheduledEventId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Date = table.Column<DateTime>(nullable: false),
                    Desription = table.Column<string>(nullable: true),
                    OwnerId = table.Column<int>(nullable: false),
                    TeamId = table.Column<int>(nullable: false),
                    Title = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduledEvents", x => x.ScheduledEventId);
                    table.ForeignKey(
                        name: "FK_ScheduledEvents_Users_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScheduledEvents_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "TeamId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserTeam",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false),
                    TeamId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTeam", x => new { x.UserId, x.TeamId });
                    table.ForeignKey(
                        name: "FK_UserTeam_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "TeamId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserTeam_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RecurringBills_TeamId",
                table: "RecurringBills",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_TeamId",
                table: "Orders",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_TeamId",
                table: "Expenses",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledEvents_OwnerId",
                table: "ScheduledEvents",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledEvents_TeamId",
                table: "ScheduledEvents",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_UserTeam_TeamId",
                table: "UserTeam",
                column: "TeamId");

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_Teams_TeamId",
                table: "Expenses",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "TeamId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Teams_TeamId",
                table: "Orders",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "TeamId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RecurringBills_Teams_TeamId",
                table: "RecurringBills",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "TeamId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduledEventUser_ScheduledEvents_ScheduledEventId",
                table: "ScheduledEventUser",
                column: "ScheduledEventId",
                principalTable: "ScheduledEvents",
                principalColumn: "ScheduledEventId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
