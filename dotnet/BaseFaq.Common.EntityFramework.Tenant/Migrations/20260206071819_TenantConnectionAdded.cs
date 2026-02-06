using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BaseFaq.Common.EntityFramework.Tenant.Migrations
{
    /// <inheritdoc />
    public partial class TenantConnectionAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TenantConnections",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    ConnectionString = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    IsCurrent = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantConnections", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TenantConnection_IsDeleted",
                table: "TenantConnections",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_TenantConnection_TenantId",
                table: "TenantConnections",
                column: "TenantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TenantConnections");
        }
    }
}
