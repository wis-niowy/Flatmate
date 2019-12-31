using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Flatmate.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ComplexOrders",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Subject = table.Column<string>(nullable: true),
                    ExpenseCategory = table.Column<string>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComplexOrders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RecurringBills",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Subject = table.Column<string>(nullable: true),
                    Value = table.Column<double>(nullable: false),
                    Frequency = table.Column<string>(nullable: false),
                    ExpenseCategory = table.Column<string>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: false),
                    LastOccurenceDate = table.Column<DateTime>(nullable: true),
                    ExpirationDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecurringBills", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ScheduledEvents",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false),
                    Title = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    IsBlocking = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduledEvents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Teams",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FirstName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    EmailAddress = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrderElements",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SCOId = table.Column<int>(nullable: false),
                    Title = table.Column<string>(nullable: true),
                    Amount = table.Column<double>(nullable: false),
                    Unit = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderElements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderElements_ComplexOrders_SCOId",
                        column: x => x.SCOId,
                        principalTable: "ComplexOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TotalExpenses",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Subject = table.Column<string>(nullable: true),
                    FinalizationDate = table.Column<DateTime>(nullable: false),
                    Value = table.Column<double>(nullable: false),
                    Covered = table.Column<bool>(nullable: false),
                    ExpenseCategory = table.Column<string>(nullable: false),
                    OwnerId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TotalExpenses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TotalExpenses_Users_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserPerTeams",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false),
                    TeamId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPerTeams", x => new { x.UserId, x.TeamId });
                    table.ForeignKey(
                        name: "FK_UserPerTeams_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserPerTeams_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrdersAssignments",
                columns: table => new
                {
                    SCOId = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: false),
                    TeamId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdersAssignments", x => new { x.SCOId, x.UserId, x.TeamId });
                    table.ForeignKey(
                        name: "FK_OrdersAssignments_ComplexOrders_SCOId",
                        column: x => x.SCOId,
                        principalTable: "ComplexOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrdersAssignments_UserPerTeams_UserId_TeamId",
                        columns: x => new { x.UserId, x.TeamId },
                        principalTable: "UserPerTeams",
                        principalColumns: new[] { "UserId", "TeamId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PartialExpenses",
                columns: table => new
                {
                    TotalExpenseId = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: false),
                    TeamId = table.Column<int>(nullable: false),
                    Value = table.Column<double>(nullable: false),
                    SettlementDate = table.Column<DateTime>(nullable: true),
                    Covered = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartialExpenses", x => new { x.TotalExpenseId, x.UserId, x.TeamId });
                    table.ForeignKey(
                        name: "FK_PartialExpenses_TotalExpenses_TotalExpenseId",
                        column: x => x.TotalExpenseId,
                        principalTable: "TotalExpenses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PartialExpenses_UserPerTeams_UserId_TeamId",
                        columns: x => new { x.UserId, x.TeamId },
                        principalTable: "UserPerTeams",
                        principalColumns: new[] { "UserId", "TeamId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RecurringBillAssignments",
                columns: table => new
                {
                    RecurringBillId = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: false),
                    TeamId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecurringBillAssignments", x => new { x.RecurringBillId, x.UserId, x.TeamId });
                    table.ForeignKey(
                        name: "FK_RecurringBillAssignments_RecurringBills_RecurringBillId",
                        column: x => x.RecurringBillId,
                        principalTable: "RecurringBills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RecurringBillAssignments_UserPerTeams_UserId_TeamId",
                        columns: x => new { x.UserId, x.TeamId },
                        principalTable: "UserPerTeams",
                        principalColumns: new[] { "UserId", "TeamId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScheduledEventUsers",
                columns: table => new
                {
                    ScheduledEventId = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: false),
                    TeamId = table.Column<int>(nullable: false),
                    IsOwner = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduledEventUsers", x => new { x.ScheduledEventId, x.UserId, x.TeamId });
                    table.ForeignKey(
                        name: "FK_ScheduledEventUsers_ScheduledEvents_ScheduledEventId",
                        column: x => x.ScheduledEventId,
                        principalTable: "ScheduledEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScheduledEventUsers_UserPerTeams_UserId_TeamId",
                        columns: x => new { x.UserId, x.TeamId },
                        principalTable: "UserPerTeams",
                        principalColumns: new[] { "UserId", "TeamId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderElements_SCOId",
                table: "OrderElements",
                column: "SCOId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdersAssignments_UserId_TeamId",
                table: "OrdersAssignments",
                columns: new[] { "UserId", "TeamId" });

            migrationBuilder.CreateIndex(
                name: "IX_PartialExpenses_UserId_TeamId",
                table: "PartialExpenses",
                columns: new[] { "UserId", "TeamId" });

            migrationBuilder.CreateIndex(
                name: "IX_RecurringBillAssignments_UserId_TeamId",
                table: "RecurringBillAssignments",
                columns: new[] { "UserId", "TeamId" });

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledEventUsers_UserId_TeamId",
                table: "ScheduledEventUsers",
                columns: new[] { "UserId", "TeamId" });

            migrationBuilder.CreateIndex(
                name: "IX_TotalExpenses_OwnerId",
                table: "TotalExpenses",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPerTeams_TeamId",
                table: "UserPerTeams",
                column: "TeamId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderElements");

            migrationBuilder.DropTable(
                name: "OrdersAssignments");

            migrationBuilder.DropTable(
                name: "PartialExpenses");

            migrationBuilder.DropTable(
                name: "RecurringBillAssignments");

            migrationBuilder.DropTable(
                name: "ScheduledEventUsers");

            migrationBuilder.DropTable(
                name: "ComplexOrders");

            migrationBuilder.DropTable(
                name: "TotalExpenses");

            migrationBuilder.DropTable(
                name: "RecurringBills");

            migrationBuilder.DropTable(
                name: "ScheduledEvents");

            migrationBuilder.DropTable(
                name: "UserPerTeams");

            migrationBuilder.DropTable(
                name: "Teams");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
