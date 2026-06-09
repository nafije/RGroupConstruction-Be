using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RGroupConstruction.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class Add_Migration_UpdateUnit_Table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LogoUrl",
                table: "CompanyInfos",
                newName: "SalesPhoneNumber");

            migrationBuilder.AddColumn<string>(
                name: "FlorPlanFileName",
                table: "Units",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "FlorPlanFileUrl",
                table: "Units",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<double>(
                name: "TerraceAre",
                table: "Units",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "ProjectPlanFileName",
                table: "Projects",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ProjectPlanFileUrl",
                table: "Projects",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FlorPlanFileName",
                table: "Units");

            migrationBuilder.DropColumn(
                name: "FlorPlanFileUrl",
                table: "Units");

            migrationBuilder.DropColumn(
                name: "TerraceAre",
                table: "Units");

            migrationBuilder.DropColumn(
                name: "ProjectPlanFileName",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "ProjectPlanFileUrl",
                table: "Projects");

            migrationBuilder.RenameColumn(
                name: "SalesPhoneNumber",
                table: "CompanyInfos",
                newName: "LogoUrl");
        }
    }
}

