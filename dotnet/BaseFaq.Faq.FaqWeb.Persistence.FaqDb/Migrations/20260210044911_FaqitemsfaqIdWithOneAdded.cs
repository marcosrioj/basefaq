using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BaseFaq.Faq.FaqWeb.Persistence.FaqDb.Migrations
{
    /// <inheritdoc />
    public partial class FaqitemsfaqIdWithOneAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FaqItems_Faqs_FaqId1",
                table: "FaqItems");

            migrationBuilder.DropIndex(
                name: "IX_FaqItems_FaqId1",
                table: "FaqItems");

            migrationBuilder.DropColumn(
                name: "FaqId1",
                table: "FaqItems");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "FaqId1",
                table: "FaqItems",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_FaqItems_FaqId1",
                table: "FaqItems",
                column: "FaqId1");

            migrationBuilder.AddForeignKey(
                name: "FK_FaqItems_Faqs_FaqId1",
                table: "FaqItems",
                column: "FaqId1",
                principalTable: "Faqs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
