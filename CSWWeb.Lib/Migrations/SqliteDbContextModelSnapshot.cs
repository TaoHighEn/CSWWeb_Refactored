﻿// <auto-generated />
using System;
using CSWWeb.Lib.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CSWWeb.Lib.Migrations
{
    [DbContext(typeof(SqliteDbContext))]
    partial class SqliteDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.14");

            modelBuilder.Entity("CSWWeb.Lib.Model.TbSysApiApplylog", b =>
                {
                    b.Property<int>("Seq")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("SEQ");

                    b.Property<int?>("Count")
                        .HasColumnType("INTEGER")
                        .HasColumnName("COUNT");

                    b.Property<DateTime?>("Lastdatetime")
                        .HasColumnType("datetime")
                        .HasColumnName("LASTDATETIME");

                    b.Property<string>("Method")
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("TEXT")
                        .HasColumnName("METHOD");

                    b.Property<string>("Module")
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("TEXT")
                        .HasColumnName("MODULE");

                    b.Property<string>("Sys")
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("TEXT")
                        .HasColumnName("SYS");

                    b.HasKey("Seq");

                    b.ToTable("TB_SYS_API_APPLYLOG", (string)null);
                });

            modelBuilder.Entity("CSWWeb.Lib.Model.TbSysWsAccount", b =>
                {
                    b.Property<int>("AccountId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("ACCOUNT_ID");

                    b.Property<DateTime?>("Datestamp")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasColumnName("DATESTAMP")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<string>("Remark")
                        .HasMaxLength(1000)
                        .HasColumnType("TEXT")
                        .HasColumnName("REMARK");

                    b.Property<string>("Status")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(1)
                        .IsUnicode(false)
                        .HasColumnType("TEXT")
                        .HasDefaultValue("N")
                        .HasColumnName("STATUS");

                    b.Property<string>("Sys")
                        .IsRequired()
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("TEXT")
                        .HasColumnName("SYS");

                    b.Property<string>("SysAllowIp")
                        .HasMaxLength(500)
                        .IsUnicode(false)
                        .HasColumnType("TEXT")
                        .HasColumnName("SYS_ALLOW_IP");

                    b.Property<string>("SysDesc")
                        .IsRequired()
                        .HasMaxLength(200)
                        .IsUnicode(false)
                        .HasColumnType("TEXT")
                        .HasColumnName("SYS_DESC");

                    b.Property<string>("SysOwner")
                        .IsRequired()
                        .HasMaxLength(200)
                        .IsUnicode(false)
                        .HasColumnType("TEXT")
                        .HasColumnName("SYS_OWNER");

                    b.Property<string>("SysOwnerEmpnum")
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("TEXT")
                        .HasColumnName("SYS_OWNER_EMPNUM");

                    b.Property<string>("SysPwd")
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("TEXT")
                        .HasColumnName("SYS_PWD");

                    b.Property<string>("Userstamp")
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("TEXT")
                        .HasColumnName("USERSTAMP");

                    b.Property<string>("UserstampNme")
                        .HasMaxLength(100)
                        .HasColumnType("TEXT")
                        .HasColumnName("USERSTAMP_NME");

                    b.Property<string>("WsAp")
                        .IsRequired()
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("TEXT")
                        .HasColumnName("WS_AP");

                    b.HasKey("AccountId");

                    b.ToTable("TB_SYS_WS_ACCOUNT", (string)null);
                });

            modelBuilder.Entity("CSWWeb.Lib.Model.TbSysWsAccountHist", b =>
                {
                    b.Property<int>("HistSeq")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("HIST_SEQ");

                    b.Property<int?>("AccountId")
                        .HasColumnType("INTEGER")
                        .HasColumnName("ACCOUNT_ID");

                    b.Property<DateTime?>("Datestamp")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasColumnName("DATESTAMP")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<string>("ModifyType")
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("TEXT")
                        .HasColumnName("MODIFY_TYPE");

                    b.Property<string>("Remark")
                        .HasMaxLength(1000)
                        .HasColumnType("TEXT")
                        .HasColumnName("REMARK");

                    b.Property<string>("Status")
                        .HasMaxLength(1)
                        .IsUnicode(false)
                        .HasColumnType("TEXT")
                        .HasColumnName("STATUS");

                    b.Property<string>("Sys")
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("TEXT")
                        .HasColumnName("SYS");

                    b.Property<string>("SysAllowIp")
                        .HasMaxLength(500)
                        .IsUnicode(false)
                        .HasColumnType("TEXT")
                        .HasColumnName("SYS_ALLOW_IP");

                    b.Property<string>("SysDesc")
                        .HasMaxLength(200)
                        .IsUnicode(false)
                        .HasColumnType("TEXT")
                        .HasColumnName("SYS_DESC");

                    b.Property<string>("SysOwner")
                        .HasMaxLength(200)
                        .IsUnicode(false)
                        .HasColumnType("TEXT")
                        .HasColumnName("SYS_OWNER");

                    b.Property<string>("SysOwnerEmpnum")
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("TEXT")
                        .HasColumnName("SYS_OWNER_EMPNUM");

                    b.Property<string>("SysPwd")
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("TEXT")
                        .HasColumnName("SYS_PWD");

                    b.Property<string>("Userstamp")
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("TEXT")
                        .HasColumnName("USERSTAMP");

                    b.Property<string>("UserstampNme")
                        .HasMaxLength(100)
                        .HasColumnType("TEXT")
                        .HasColumnName("USERSTAMP_NME");

                    b.Property<string>("WsAp")
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("TEXT")
                        .HasColumnName("WS_AP");

                    b.HasKey("HistSeq");

                    b.ToTable("TB_SYS_WS_ACCOUNT_HIST", (string)null);
                });

            modelBuilder.Entity("CSWWeb.Lib.Model.TbSysWsApServer", b =>
                {
                    b.Property<int>("ApServerId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("AP_SERVER_ID");

                    b.Property<DateTime?>("Datestamp")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasColumnName("DATESTAMP")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<string>("Status")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(1)
                        .IsUnicode(false)
                        .HasColumnType("TEXT")
                        .HasDefaultValue("N")
                        .HasColumnName("STATUS");

                    b.Property<string>("Userstamp")
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("TEXT")
                        .HasColumnName("USERSTAMP");

                    b.Property<string>("WsAp")
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("TEXT")
                        .HasColumnName("WS_AP");

                    b.Property<string>("WsApMonitorUrl")
                        .HasMaxLength(250)
                        .IsUnicode(false)
                        .HasColumnType("TEXT")
                        .HasColumnName("WS_AP_MONITOR_URL");

                    b.Property<string>("WsApServer")
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("TEXT")
                        .HasColumnName("WS_AP_SERVER");

                    b.HasKey("ApServerId");

                    b.ToTable("TB_SYS_WS_AP_SERVER", (string)null);
                });

            modelBuilder.Entity("CSWWeb.Lib.Model.TbSysWsLog", b =>
                {
                    b.Property<int>("Seq")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("SEQ");

                    b.Property<DateTime?>("Datestamp")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasColumnName("DATESTAMP")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<string>("LogMessage")
                        .HasMaxLength(1000)
                        .IsUnicode(false)
                        .HasColumnType("TEXT")
                        .HasColumnName("LOG_MESSAGE");

                    b.Property<string>("LogStatus")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(1)
                        .IsUnicode(false)
                        .HasColumnType("TEXT")
                        .HasDefaultValue("N")
                        .HasColumnName("LOG_STATUS");

                    b.Property<string>("LogType")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(10)
                        .IsUnicode(false)
                        .HasColumnType("TEXT")
                        .HasDefaultValue("NORMAL")
                        .HasColumnName("LOG_TYPE");

                    b.Property<string>("WsAp")
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("TEXT")
                        .HasColumnName("WS_AP");

                    b.Property<string>("WsApServer")
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("TEXT")
                        .HasColumnName("WS_AP_SERVER");

                    b.Property<string>("WsModule")
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("TEXT")
                        .HasColumnName("WS_MODULE");

                    b.HasKey("Seq");

                    b.ToTable("TB_SYS_WS_LOG", (string)null);
                });

            modelBuilder.Entity("CSWWeb.Lib.Model.TbSysWsModule", b =>
                {
                    b.Property<int>("ModuleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("MODULE_ID");

                    b.Property<DateTime?>("Datestamp")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasColumnName("DATESTAMP")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<string>("Userstamp")
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("TEXT")
                        .HasColumnName("USERSTAMP");

                    b.Property<string>("WsAp")
                        .IsRequired()
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("TEXT")
                        .HasColumnName("WS_AP");

                    b.Property<string>("WsMethod")
                        .IsRequired()
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("TEXT")
                        .HasColumnName("WS_METHOD");

                    b.Property<string>("WsModule")
                        .IsRequired()
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("TEXT")
                        .HasColumnName("WS_MODULE");

                    b.HasKey("ModuleId");

                    b.ToTable("TB_SYS_WS_MODULE", (string)null);
                });

            modelBuilder.Entity("CSWWeb.Lib.Model.TbSysWsPriv", b =>
                {
                    b.Property<int>("PrivSeq")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("PRIV_SEQ");

                    b.Property<int>("AccountId")
                        .HasColumnType("INTEGER")
                        .HasColumnName("ACCOUNT_ID");

                    b.Property<DateTime?>("Datestamp")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasColumnName("DATESTAMP")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<DateTime?>("DatestampWs")
                        .HasColumnType("datetime")
                        .HasColumnName("DATESTAMP_WS");

                    b.Property<DateTime?>("LastExecDate")
                        .HasColumnType("datetime");

                    b.Property<string>("Remark")
                        .HasMaxLength(1000)
                        .HasColumnType("TEXT")
                        .HasColumnName("REMARK");

                    b.Property<string>("Status")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(1)
                        .IsUnicode(false)
                        .HasColumnType("TEXT")
                        .HasDefaultValue("N")
                        .HasColumnName("STATUS");

                    b.Property<string>("Userstamp")
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("TEXT")
                        .HasColumnName("USERSTAMP");

                    b.Property<string>("UserstampNme")
                        .HasMaxLength(100)
                        .HasColumnType("TEXT")
                        .HasColumnName("USERSTAMP_NME");

                    b.Property<string>("WsMethod")
                        .IsRequired()
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("TEXT")
                        .HasColumnName("WS_METHOD");

                    b.Property<string>("WsModule")
                        .IsRequired()
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("TEXT")
                        .HasColumnName("WS_MODULE");

                    b.HasKey("PrivSeq");

                    b.ToTable("TB_SYS_WS_PRIV", (string)null);
                });

            modelBuilder.Entity("CSWWeb.Lib.Model.TbSysWsPrivHist", b =>
                {
                    b.Property<int>("HistSeq")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("HIST_SEQ");

                    b.Property<int?>("AccountId")
                        .HasColumnType("INTEGER")
                        .HasColumnName("ACCOUNT_ID");

                    b.Property<DateTime?>("Datestamp")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasColumnName("DATESTAMP")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<string>("ModifyType")
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("TEXT")
                        .HasColumnName("MODIFY_TYPE");

                    b.Property<int?>("PrivSeq")
                        .HasColumnType("INTEGER")
                        .HasColumnName("PRIV_SEQ");

                    b.Property<string>("Remark")
                        .HasMaxLength(1000)
                        .HasColumnType("TEXT")
                        .HasColumnName("REMARK");

                    b.Property<string>("Status")
                        .HasMaxLength(1)
                        .IsUnicode(false)
                        .HasColumnType("TEXT")
                        .HasColumnName("STATUS");

                    b.Property<string>("Userstamp")
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("TEXT")
                        .HasColumnName("USERSTAMP");

                    b.Property<string>("UserstampNme")
                        .HasMaxLength(100)
                        .HasColumnType("TEXT")
                        .HasColumnName("USERSTAMP_NME");

                    b.Property<string>("WsMethod")
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("TEXT")
                        .HasColumnName("WS_METHOD");

                    b.Property<string>("WsModule")
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("TEXT")
                        .HasColumnName("WS_MODULE");

                    b.HasKey("HistSeq");

                    b.ToTable("TB_SYS_WS_PRIV_HIST", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}
