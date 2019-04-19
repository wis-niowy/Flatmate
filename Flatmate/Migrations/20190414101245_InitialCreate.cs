using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Flatmate.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Teams",
                columns: table => new
                {
                    TeamId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => x.TeamId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    EmailAddress = table.Column<string>(nullable: true),
                    FirstName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Expenses",
                columns: table => new
                {
                    ExpenseId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Date = table.Column<DateTime>(nullable: false),
                    ExpenseCategory = table.Column<int>(nullable: false),
                    ExpenseSubject = table.Column<string>(nullable: true),
                    InitiatorId = table.Column<int>(nullable: false),
                    TeamId = table.Column<int>(nullable: false),
                    Value = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Expenses", x => x.ExpenseId);
                    table.ForeignKey(
                        name: "FK_Expenses_Users_InitiatorId",
                        column: x => x.InitiatorId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Expenses_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "TeamId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    OrderId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Date = table.Column<DateTime>(nullable: false),
                    InitiatorId = table.Column<int>(nullable: false),
                    OrderSubject = table.Column<string>(nullable: true),
                    TeamId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.OrderId);
                    table.ForeignKey(
                        name: "FK_Orders_Users_InitiatorId",
                        column: x => x.InitiatorId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Orders_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "TeamId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RecurringBills",
                columns: table => new
                {
                    RecurringBillId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BillSubject = table.Column<string>(nullable: true),
                    Frequency = table.Column<double>(nullable: false),
                    InitiatorId = table.Column<int>(nullable: false),
                    LastEffectiveDate = table.Column<DateTime>(nullable: false),
                    NextEffectiveDate = table.Column<DateTime>(nullable: false),
                    TeamId = table.Column<int>(nullable: false),
                    Value = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecurringBills", x => x.RecurringBillId);
                    table.ForeignKey(
                        name: "FK_RecurringBills_Users_InitiatorId",
                        column: x => x.InitiatorId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RecurringBills_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "TeamId",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateTable(
                name: "ExpenseDebitor",
                columns: table => new
                {
                    ExpenseId = table.Column<int>(nullable: false),
                    DebitorId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpenseDebitor", x => new { x.ExpenseId, x.DebitorId });
                    table.ForeignKey(
                        name: "FK_ExpenseDebitor_Users_DebitorId",
                        column: x => x.DebitorId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExpenseDebitor_Expenses_ExpenseId",
                        column: x => x.ExpenseId,
                        principalTable: "Expenses",
                        principalColumn: "ExpenseId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrderDebitor",
                columns: table => new
                {
                    OrderId = table.Column<int>(nullable: false),
                    DebitorId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderDebitor", x => new { x.OrderId, x.DebitorId });
                    table.ForeignKey(
                        name: "FK_OrderDebitor_Users_DebitorId",
                        column: x => x.DebitorId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderDebitor_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RecurringBillDebitor",
                columns: table => new
                {
                    RecurringBillId = table.Column<int>(nullable: false),
                    DebitorId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecurringBillDebitor", x => new { x.RecurringBillId, x.DebitorId });
                    table.ForeignKey(
                        name: "FK_RecurringBillDebitor_Users_DebitorId",
                        column: x => x.DebitorId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RecurringBillDebitor_RecurringBills_RecurringBillId",
                        column: x => x.RecurringBillId,
                        principalTable: "RecurringBills",
                        principalColumn: "RecurringBillId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ScheduledEventUser",
                columns: table => new
                {
                    ScheduledEventId = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduledEventUser", x => new { x.ScheduledEventId, x.UserId });
                    table.ForeignKey(
                        name: "FK_ScheduledEventUser_ScheduledEvents_ScheduledEventId",
                        column: x => x.ScheduledEventId,
                        principalTable: "ScheduledEvents",
                        principalColumn: "ScheduledEventId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ScheduledEventUser_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseDebitor_DebitorId",
                table: "ExpenseDebitor",
                column: "DebitorId");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_InitiatorId",
                table: "Expenses",
                column: "InitiatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_TeamId",
                table: "Expenses",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDebitor_DebitorId",
                table: "OrderDebitor",
                column: "DebitorId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_InitiatorId",
                table: "Orders",
                column: "InitiatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_TeamId",
                table: "Orders",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_RecurringBillDebitor_DebitorId",
                table: "RecurringBillDebitor",
                column: "DebitorId");

            migrationBuilder.CreateIndex(
                name: "IX_RecurringBills_InitiatorId",
                table: "RecurringBills",
                column: "InitiatorId");

            migrationBuilder.CreateIndex(
                name: "IX_RecurringBills_TeamId",
                table: "RecurringBills",
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
                name: "IX_ScheduledEventUser_UserId",
                table: "ScheduledEventUser",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserTeam_TeamId",
                table: "UserTeam",
                column: "TeamId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExpenseDebitor");

            migrationBuilder.DropTable(
                name: "OrderDebitor");

            migrationBuilder.DropTable(
                name: "RecurringBillDebitor");

            migrationBuilder.DropTable(
                name: "ScheduledEventUser");

            migrationBuilder.DropTable(
                name: "UserTeam");

            migrationBuilder.DropTable(
                name: "Expenses");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "RecurringBills");

            migrationBuilder.DropTable(
                name: "ScheduledEvents");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Teams");
        }
    }
}
