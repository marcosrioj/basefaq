using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BaseFaq.Common.EntityFramework.Tenant.Migrations
{
    /// <inheritdoc />
    public partial class TenantsClientKeyAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ClientKey",
                table: "Tenants",
                type: "character varying(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tenant_ClientKey",
                table: "Tenants",
                column: "ClientKey",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tenant_ClientKey",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "ClientKey",
                table: "Tenants");
        }
    }
}
