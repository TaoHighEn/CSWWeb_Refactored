using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CSWWeb.Lib.Migrations
{
    /// <inheritdoc />
    public partial class updatetable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TB_SYS_API_APPLYLOG",
                columns: table => new
                {
                    SEQ = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SYS = table.Column<string>(type: "TEXT", unicode: false, maxLength: 50, nullable: true),
                    MODULE = table.Column<string>(type: "TEXT", unicode: false, maxLength: 100, nullable: true),
                    METHOD = table.Column<string>(type: "TEXT", unicode: false, maxLength: 100, nullable: true),
                    COUNT = table.Column<int>(type: "INTEGER", nullable: true),
                    LASTDATETIME = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_SYS_API_APPLYLOG", x => x.SEQ);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TB_SYS_API_APPLYLOG");
        }
    }
}
