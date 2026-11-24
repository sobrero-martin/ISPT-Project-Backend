using BD;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BD.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20261122170000_UpdateFileDivisionAdmissionStatus")]
public partial class UpdateFileDivisionAdmissionStatus : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<int>(
            name: "FileDivisionAdmitedStatus",
            table: "FileDivisions",
            type: "int",
            nullable: false,
            oldClrType: typeof(bool),
            oldType: "tinyint(1)");

        migrationBuilder.Sql("UPDATE `FileDivisions` SET `FileDivisionAdmitedStatus` = 2 WHERE `FileDivisionAdmitedStatus` = 0;");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("UPDATE `FileDivisions` SET `FileDivisionAdmitedStatus` = 0 WHERE `FileDivisionAdmitedStatus` IN (0, 2);");

        migrationBuilder.AlterColumn<bool>(
            name: "FileDivisionAdmitedStatus",
            table: "FileDivisions",
            type: "tinyint(1)",
            nullable: false,
            oldClrType: typeof(int),
            oldType: "int");
    }
}
