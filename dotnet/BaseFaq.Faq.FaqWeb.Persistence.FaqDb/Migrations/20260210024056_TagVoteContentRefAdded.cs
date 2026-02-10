using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BaseFaq.Faq.FaqWeb.Persistence.FaqDb.Migrations
{
    /// <inheritdoc />
    public partial class TagVoteContentRefAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CtaText",
                table: "FaqItems");

            migrationBuilder.DropColumn(
                name: "Origin",
                table: "FaqItems");

            migrationBuilder.RenameColumn(
                name: "SortType",
                table: "Faqs",
                newName: "SortStrategy");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Faqs",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Language",
                table: "Faqs",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<bool>(
                name: "CtaEnabled",
                table: "Faqs",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "CtaTarget",
                table: "Faqs",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Question",
                table: "FaqItems",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "CtaUrl",
                table: "FaqItems",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Answer",
                table: "FaqItems",
                type: "character varying(5000)",
                maxLength: 5000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "AdditionalInfo",
                table: "FaqItems",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ContentRefId",
                table: "FaqItems",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CtaTitle",
                table: "FaqItems",
                type: "character varying(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "FaqId1",
                table: "FaqItems",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "ShortAnswer",
                table: "FaqItems",
                type: "character varying(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "FaqItems",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "ContentRefs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Kind = table.Column<int>(type: "integer", nullable: false),
                    Locator = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Label = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Scope = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("PK_ContentRefs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Value = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("PK_Tags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Votes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Like = table.Column<bool>(type: "boolean", nullable: false),
                    UserPrint = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Ip = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    UserAgent = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    UnLikeReason = table.Column<int>(type: "integer", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    FaqItemId = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("PK_Votes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Votes_FaqItems_FaqItemId",
                        column: x => x.FaqItemId,
                        principalTable: "FaqItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FaqContentRefs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    FaqId = table.Column<Guid>(type: "uuid", nullable: false),
                    ContentRefId = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("PK_FaqContentRefs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FaqContentRefs_ContentRefs_ContentRefId",
                        column: x => x.ContentRefId,
                        principalTable: "ContentRefs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FaqContentRefs_Faqs_FaqId",
                        column: x => x.FaqId,
                        principalTable: "Faqs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FaqTags",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    FaqId = table.Column<Guid>(type: "uuid", nullable: false),
                    TagId = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("PK_FaqTags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FaqTags_Faqs_FaqId",
                        column: x => x.FaqId,
                        principalTable: "Faqs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FaqTags_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FaqItem_TenantId",
                table: "FaqItems",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_FaqItems_ContentRefId",
                table: "FaqItems",
                column: "ContentRefId");

            migrationBuilder.CreateIndex(
                name: "IX_FaqItems_FaqId1",
                table: "FaqItems",
                column: "FaqId1");

            migrationBuilder.CreateIndex(
                name: "IX_ContentRef_IsDeleted",
                table: "ContentRefs",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_ContentRef_Kind",
                table: "ContentRefs",
                column: "Kind");

            migrationBuilder.CreateIndex(
                name: "IX_ContentRef_TenantId",
                table: "ContentRefs",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_FaqContentRef_FaqId_ContentRefId",
                table: "FaqContentRefs",
                columns: new[] { "FaqId", "ContentRefId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FaqContentRef_IsDeleted",
                table: "FaqContentRefs",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_FaqContentRef_TenantId",
                table: "FaqContentRefs",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_FaqContentRefs_ContentRefId",
                table: "FaqContentRefs",
                column: "ContentRefId");

            migrationBuilder.CreateIndex(
                name: "IX_FaqTag_FaqId_TagId",
                table: "FaqTags",
                columns: new[] { "FaqId", "TagId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FaqTag_IsDeleted",
                table: "FaqTags",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_FaqTag_TenantId",
                table: "FaqTags",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_FaqTags_TagId",
                table: "FaqTags",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_Tag_IsDeleted",
                table: "Tags",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Tag_TenantId",
                table: "Tags",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Tag_Value",
                table: "Tags",
                column: "Value");

            migrationBuilder.CreateIndex(
                name: "IX_Vote_FaqItemId",
                table: "Votes",
                column: "FaqItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Vote_IsDeleted",
                table: "Votes",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Vote_TenantId",
                table: "Votes",
                column: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_FaqItems_ContentRefs_ContentRefId",
                table: "FaqItems",
                column: "ContentRefId",
                principalTable: "ContentRefs",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FaqItems_Faqs_FaqId1",
                table: "FaqItems",
                column: "FaqId1",
                principalTable: "Faqs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FaqItems_ContentRefs_ContentRefId",
                table: "FaqItems");

            migrationBuilder.DropForeignKey(
                name: "FK_FaqItems_Faqs_FaqId1",
                table: "FaqItems");

            migrationBuilder.DropTable(
                name: "FaqContentRefs");

            migrationBuilder.DropTable(
                name: "FaqTags");

            migrationBuilder.DropTable(
                name: "Votes");

            migrationBuilder.DropTable(
                name: "ContentRefs");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_FaqItem_TenantId",
                table: "FaqItems");

            migrationBuilder.DropIndex(
                name: "IX_FaqItems_ContentRefId",
                table: "FaqItems");

            migrationBuilder.DropIndex(
                name: "IX_FaqItems_FaqId1",
                table: "FaqItems");

            migrationBuilder.DropColumn(
                name: "CtaEnabled",
                table: "Faqs");

            migrationBuilder.DropColumn(
                name: "CtaTarget",
                table: "Faqs");

            migrationBuilder.DropColumn(
                name: "AdditionalInfo",
                table: "FaqItems");

            migrationBuilder.DropColumn(
                name: "ContentRefId",
                table: "FaqItems");

            migrationBuilder.DropColumn(
                name: "CtaTitle",
                table: "FaqItems");

            migrationBuilder.DropColumn(
                name: "FaqId1",
                table: "FaqItems");

            migrationBuilder.DropColumn(
                name: "ShortAnswer",
                table: "FaqItems");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "FaqItems");

            migrationBuilder.RenameColumn(
                name: "SortStrategy",
                table: "Faqs",
                newName: "SortType");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Faqs",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Language",
                table: "Faqs",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Question",
                table: "FaqItems",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<string>(
                name: "CtaUrl",
                table: "FaqItems",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Answer",
                table: "FaqItems",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(5000)",
                oldMaxLength: 5000,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CtaText",
                table: "FaqItems",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Origin",
                table: "FaqItems",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
