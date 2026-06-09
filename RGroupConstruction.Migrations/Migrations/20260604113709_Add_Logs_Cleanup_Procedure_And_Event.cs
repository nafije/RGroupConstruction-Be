using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RGroupConstruction.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class Add_Logs_Cleanup_Procedure_And_Event : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Create Stored Procedure
            migrationBuilder.Sql(@"
                CREATE PROCEDURE IF NOT EXISTS `ABVConsctuction`.`sp_DeleteAllLogs`()
                BEGIN
                    TRUNCATE TABLE `ABVConsctuction`.`Logs`;
                END
            ");

            // 2. Create Scheduled Event
            migrationBuilder.Sql(@"
                CREATE EVENT IF NOT EXISTS `ABVConsctuction`.`evt_DeleteAllLogs`
                ON SCHEDULE EVERY 1 WEEK
                STARTS NOW()
                DO
                    CALL `ABVConsctuction`.`sp_DeleteAllLogs`();
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP EVENT IF EXISTS `ABVConsctuction`.`evt_DeleteAllLogs`;");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS `ABVConsctuction`.`sp_DeleteAllLogs`;");
        }
    }
}

