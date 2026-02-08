using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BaseFaq.Faq.FaqWeb.Persistence.FaqDb.Migrations
{
    /// <inheritdoc />
    public partial class FaqItemCtaFieldsAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CtaText",
                table: "FaqItems",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CtaUrl",
                table: "FaqItems",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CtaText",
                table: "FaqItems");

            migrationBuilder.DropColumn(
                name: "CtaUrl",
                table: "FaqItems");
        }
    }
}
