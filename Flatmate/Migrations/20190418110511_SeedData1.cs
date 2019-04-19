using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Flatmate.Migrations
{
    public partial class SeedData1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Teams",
                columns: new[] { "TeamId", "Name" },
                values: new object[] { 1, "Best Ekipa" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "EmailAddress", "FirstName", "LastName", "TeamId" },
                values: new object[,]
                {
                    { 1, "jan.kow@poczta.pl", "Jan", "Kowalski", 1 },
                    { 2, "grz.kacz@poczta.pl", "Grzegorz", "Kaczmarski", 1 },
                    { 3, "mac.now@poczta.pl", "Maciej", "Nowak", 1 },
                    { 4, "krys.adam@poczta.pl", "Krystian", "Adamowicz", 1 }
                });

            migrationBuilder.InsertData(
                table: "Expenses",
                columns: new[] { "ExpenseId", "Date", "ExpenseCategory", "ExpenseSubject", "InitiatorId", "Value" },
                values: new object[,]
                {
                    { 1, new DateTime(2019, 4, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "Expense 1", 1, 15.5 },
                    { 2, new DateTime(2019, 4, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, "Expense 2", 2, 12.0 },
                    { 3, new DateTime(2019, 4, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "Expense 3", 3, 125.0 },
                    { 4, new DateTime(2019, 4, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "Expense 4", 4, 78.2 }
                });

            migrationBuilder.InsertData(
                table: "ExpenseDebitor",
                columns: new[] { "ExpenseId", "DebitorId" },
                values: new object[,]
                {
                    { 1, 2 },
                    { 1, 3 },
                    { 1, 4 },
                    { 2, 1 },
                    { 2, 3 },
                    { 2, 4 },
                    { 3, 1 },
                    { 3, 2 },
                    { 3, 4 },
                    { 4, 1 },
                    { 4, 2 },
                    { 4, 3 }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ExpenseDebitor",
                keyColumns: new[] { "ExpenseId", "DebitorId" },
                keyValues: new object[] { 1, 2 });

            migrationBuilder.DeleteData(
                table: "ExpenseDebitor",
                keyColumns: new[] { "ExpenseId", "DebitorId" },
                keyValues: new object[] { 1, 3 });

            migrationBuilder.DeleteData(
                table: "ExpenseDebitor",
                keyColumns: new[] { "ExpenseId", "DebitorId" },
                keyValues: new object[] { 1, 4 });

            migrationBuilder.DeleteData(
                table: "ExpenseDebitor",
                keyColumns: new[] { "ExpenseId", "DebitorId" },
                keyValues: new object[] { 2, 1 });

            migrationBuilder.DeleteData(
                table: "ExpenseDebitor",
                keyColumns: new[] { "ExpenseId", "DebitorId" },
                keyValues: new object[] { 2, 3 });

            migrationBuilder.DeleteData(
                table: "ExpenseDebitor",
                keyColumns: new[] { "ExpenseId", "DebitorId" },
                keyValues: new object[] { 2, 4 });

            migrationBuilder.DeleteData(
                table: "ExpenseDebitor",
                keyColumns: new[] { "ExpenseId", "DebitorId" },
                keyValues: new object[] { 3, 1 });

            migrationBuilder.DeleteData(
                table: "ExpenseDebitor",
                keyColumns: new[] { "ExpenseId", "DebitorId" },
                keyValues: new object[] { 3, 2 });

            migrationBuilder.DeleteData(
                table: "ExpenseDebitor",
                keyColumns: new[] { "ExpenseId", "DebitorId" },
                keyValues: new object[] { 3, 4 });

            migrationBuilder.DeleteData(
                table: "ExpenseDebitor",
                keyColumns: new[] { "ExpenseId", "DebitorId" },
                keyValues: new object[] { 4, 1 });

            migrationBuilder.DeleteData(
                table: "ExpenseDebitor",
                keyColumns: new[] { "ExpenseId", "DebitorId" },
                keyValues: new object[] { 4, 2 });

            migrationBuilder.DeleteData(
                table: "ExpenseDebitor",
                keyColumns: new[] { "ExpenseId", "DebitorId" },
                keyValues: new object[] { 4, 3 });

            migrationBuilder.DeleteData(
                table: "Expenses",
                keyColumn: "ExpenseId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Expenses",
                keyColumn: "ExpenseId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Expenses",
                keyColumn: "ExpenseId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Expenses",
                keyColumn: "ExpenseId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Teams",
                keyColumn: "TeamId",
                keyValue: 1);
        }
    }
}
