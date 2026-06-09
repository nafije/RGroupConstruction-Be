using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RGroupConstruction.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class RestoreCompanyInfoLogoUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LogoUrl",
                table: "CompanyInfos",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LogoUrl",
                table: "CompanyInfos");
        }
    }
}

