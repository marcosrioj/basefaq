using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BaseFaq.Faq.Common.Persistence.FaqDb.Migrations
{
    /// <inheritdoc />
    public partial class VoteTenantIdAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "Votes",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Vote_TenantId",
                table: "Votes",
                column: "TenantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Vote_TenantId",
                table: "Votes");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Votes");
        }
    }
}
