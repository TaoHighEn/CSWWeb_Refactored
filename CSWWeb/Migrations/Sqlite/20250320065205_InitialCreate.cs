using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CSWWeb.Migrations.Sqlite
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccountInfos",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ACCOUNT = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NAME = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    PASSWORD = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountInfos", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ModuleInfos",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActionName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ControllerName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModuleInfos", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "PRIVInfos",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ACCOUNT_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MODULE_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PRIVInfos", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "SyncLogs",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SyncTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SyncSource = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SyncLogs", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "SystemLogs",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LogLevel = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    Exception = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    RequestPath = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    LogDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserIP = table.Column<string>(type: "nvarchar(200)", nullable: false, defaultValue: "")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemLogs", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "TB_SYS_WS_ACCOUNT",
                columns: table => new
                {
                    ACCOUNT_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WS_AP = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    SYS = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    SYS_PWD = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    SYS_DESC = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: false),
                    SYS_OWNER = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: false),
                    STATUS = table.Column<string>(type: "varchar(1)", unicode: false, maxLength: 1, nullable: false, defaultValue: "N"),
                    DATESTAMP = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    USERSTAMP = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    SYS_ALLOW_IP = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: true),
                    SYS_OWNER_EMPNUM = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    USERSTAMP_NME = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    REMARK = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "TB_SYS_WS_ACCOUNT_HIST",
                columns: table => new
                {
                    HIST_SEQ = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ACCOUNT_ID = table.Column<int>(type: "int", nullable: true),
                    WS_AP = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    SYS = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    SYS_PWD = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    SYS_DESC = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: true),
                    SYS_OWNER = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: true),
                    STATUS = table.Column<string>(type: "varchar(1)", unicode: false, maxLength: 1, nullable: true),
                    DATESTAMP = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    USERSTAMP = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    SYS_ALLOW_IP = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: true),
                    SYS_OWNER_EMPNUM = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    USERSTAMP_NME = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    REMARK = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    MODIFY_TYPE = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "TB_SYS_WS_AP_SERVER",
                columns: table => new
                {
                    AP_SERVER_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WS_AP = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    WS_AP_SERVER = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    STATUS = table.Column<string>(type: "varchar(1)", unicode: false, maxLength: 1, nullable: false, defaultValue: "N"),
                    DATESTAMP = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    USERSTAMP = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    WS_AP_MONITOR_URL = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "TB_SYS_WS_LOG",
                columns: table => new
                {
                    SEQ = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WS_AP = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    WS_MODULE = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    WS_AP_SERVER = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    LOG_MESSAGE = table.Column<string>(type: "varchar(1000)", unicode: false, maxLength: 1000, nullable: true),
                    LOG_TYPE = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: true, defaultValue: "NORMAL"),
                    LOG_STATUS = table.Column<string>(type: "varchar(1)", unicode: false, maxLength: 1, nullable: false, defaultValue: "N"),
                    DATESTAMP = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "TB_SYS_WS_MODULE",
                columns: table => new
                {
                    MODULE_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WS_AP = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    WS_MODULE = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    WS_METHOD = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    DATESTAMP = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    USERSTAMP = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "TB_SYS_WS_PRIV",
                columns: table => new
                {
                    PRIV_SEQ = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ACCOUNT_ID = table.Column<int>(type: "int", nullable: false),
                    WS_MODULE = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    WS_METHOD = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    STATUS = table.Column<string>(type: "varchar(1)", unicode: false, maxLength: 1, nullable: false, defaultValue: "N"),
                    DATESTAMP = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    USERSTAMP = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    LastExecDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    DATESTAMP_WS = table.Column<DateTime>(type: "datetime", nullable: true),
                    USERSTAMP_NME = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    REMARK = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "TB_SYS_WS_PRIV_HIST",
                columns: table => new
                {
                    HIST_SEQ = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PRIV_SEQ = table.Column<int>(type: "int", nullable: true),
                    ACCOUNT_ID = table.Column<int>(type: "int", nullable: true),
                    WS_MODULE = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    WS_METHOD = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    STATUS = table.Column<string>(type: "varchar(1)", unicode: false, maxLength: 1, nullable: true),
                    DATESTAMP = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    USERSTAMP = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    USERSTAMP_NME = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    REMARK = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    MODIFY_TYPE = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountInfos");

            migrationBuilder.DropTable(
                name: "ModuleInfos");

            migrationBuilder.DropTable(
                name: "PRIVInfos");

            migrationBuilder.DropTable(
                name: "SyncLogs");

            migrationBuilder.DropTable(
                name: "SystemLogs");

            migrationBuilder.DropTable(
                name: "TB_SYS_WS_ACCOUNT");

            migrationBuilder.DropTable(
                name: "TB_SYS_WS_ACCOUNT_HIST");

            migrationBuilder.DropTable(
                name: "TB_SYS_WS_AP_SERVER");

            migrationBuilder.DropTable(
                name: "TB_SYS_WS_LOG");

            migrationBuilder.DropTable(
                name: "TB_SYS_WS_MODULE");

            migrationBuilder.DropTable(
                name: "TB_SYS_WS_PRIV");

            migrationBuilder.DropTable(
                name: "TB_SYS_WS_PRIV_HIST");
        }
    }
}
