﻿using CSWWeb.Lib.Model;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CSWWeb.Lib.Data
{
    public partial class SqliteDbContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public SqliteDbContext(DbContextOptions<SqliteDbContext> options, IConfiguration configuration)
            : base(options)
        {
            _configuration = configuration;
        }

        public virtual DbSet<TbSysApiApplylog> TbSysApiApplylog { get; set; }
        public virtual DbSet<TbSysWsAccount> TbSysWsAccounts { get; set; }

        public virtual DbSet<TbSysWsAccountHist> TbSysWsAccountHists { get; set; }

        public virtual DbSet<TbSysWsApServer> TbSysWsApServers { get; set; }

        public virtual DbSet<TbSysWsLog> TbSysWsLogs { get; set; }

        public virtual DbSet<TbSysWsModule> TbSysWsModules { get; set; }

        public virtual DbSet<TbSysWsPriv> TbSysWsPrivs { get; set; }

        public virtual DbSet<TbSysWsPrivHist> TbSysWsPrivHists { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var sqliteSettings = _configuration.GetSection("SqliteSettings").Get<SqliteSettings>();
            if (sqliteSettings == null || string.IsNullOrEmpty(sqliteSettings.DbPath))
            {
                throw new Exception("SQLite 資料庫設定錯誤，請檢查 appsettings.json");
            }

            if (!optionsBuilder.IsConfigured)
            {
                var connectionString = $"Data Source={sqliteSettings.DbPath};Pooling=False;";
                var connection = new SqliteConnection(connectionString);
                connection.Open();

                // 啟用 SQLCipher 擴展功能（若有加密需求）
                connection.EnableExtensions(true);

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"PRAGMA key = '{sqliteSettings.Password}'";
                    command.ExecuteNonQuery();
                }

                optionsBuilder.UseSqlite(connection);
            }
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TbSysApiApplylog>(entity =>
            {
                entity
                    //.HasNoKey()
                    .ToTable("TB_SYS_API_APPLYLOG");

                entity.Property(e => e.Count).HasColumnName("COUNT");
                entity.Property(e => e.Lastdatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("LASTDATETIME");
                entity.Property(e => e.Method)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("METHOD");
                entity.Property(e => e.Module)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("MODULE");
                entity.Property(e => e.Seq)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("SEQ");
                entity.Property(e => e.Sys)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("SYS");
            });
            modelBuilder.Entity<TbSysWsAccount>(entity =>
            {
                entity
                    //.HasNoKey()
                    .ToTable("TB_SYS_WS_ACCOUNT");

                entity.Property(e => e.AccountId).HasColumnName("ACCOUNT_ID");
                entity.Property(e => e.Datestamp)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime")
                    .HasColumnName("DATESTAMP");
                entity.Property(e => e.Remark)
                    .HasMaxLength(1000)
                    .HasColumnName("REMARK");
                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasDefaultValue("N")
                    .HasColumnName("STATUS");
                entity.Property(e => e.Sys)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("SYS");
                entity.Property(e => e.SysAllowIp)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("SYS_ALLOW_IP");
                entity.Property(e => e.SysDesc)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("SYS_DESC");
                entity.Property(e => e.SysOwner)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("SYS_OWNER");
                entity.Property(e => e.SysOwnerEmpnum)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("SYS_OWNER_EMPNUM");
                entity.Property(e => e.SysPwd)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("SYS_PWD");
                entity.Property(e => e.Userstamp)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("USERSTAMP");
                entity.Property(e => e.UserstampNme)
                    .HasMaxLength(100)
                    .HasColumnName("USERSTAMP_NME");
                entity.Property(e => e.WsAp)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("WS_AP");
            });

            modelBuilder.Entity<TbSysWsAccountHist>(entity =>
            {
                entity
                    //.HasNoKey()
                    .ToTable("TB_SYS_WS_ACCOUNT_HIST");

                entity.Property(e => e.AccountId).HasColumnName("ACCOUNT_ID");
                entity.Property(e => e.Datestamp)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime")
                    .HasColumnName("DATESTAMP");
                entity.Property(e => e.HistSeq).HasColumnName("HIST_SEQ");
                entity.Property(e => e.ModifyType)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("MODIFY_TYPE");
                entity.Property(e => e.Remark)
                    .HasMaxLength(1000)
                    .HasColumnName("REMARK");
                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("STATUS");
                entity.Property(e => e.Sys)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("SYS");
                entity.Property(e => e.SysAllowIp)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("SYS_ALLOW_IP");
                entity.Property(e => e.SysDesc)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("SYS_DESC");
                entity.Property(e => e.SysOwner)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("SYS_OWNER");
                entity.Property(e => e.SysOwnerEmpnum)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("SYS_OWNER_EMPNUM");
                entity.Property(e => e.SysPwd)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("SYS_PWD");
                entity.Property(e => e.Userstamp)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("USERSTAMP");
                entity.Property(e => e.UserstampNme)
                    .HasMaxLength(100)
                    .HasColumnName("USERSTAMP_NME");
                entity.Property(e => e.WsAp)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("WS_AP");
            });

            modelBuilder.Entity<TbSysWsApServer>(entity =>
            {
                entity
                    //.HasNoKey()
                    .ToTable("TB_SYS_WS_AP_SERVER");

                entity.Property(e => e.ApServerId).HasColumnName("AP_SERVER_ID");
                entity.Property(e => e.Datestamp)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime")
                    .HasColumnName("DATESTAMP");
                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasDefaultValue("N")
                    .HasColumnName("STATUS");
                entity.Property(e => e.Userstamp)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("USERSTAMP");
                entity.Property(e => e.WsAp)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("WS_AP");
                entity.Property(e => e.WsApMonitorUrl)
                    .HasMaxLength(250)
                    .IsUnicode(false)
                    .HasColumnName("WS_AP_MONITOR_URL");
                entity.Property(e => e.WsApServer)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("WS_AP_SERVER");
            });

            modelBuilder.Entity<TbSysWsLog>(entity =>
            {
                entity
                    //.HasNoKey()
                    .ToTable("TB_SYS_WS_LOG");

                entity.Property(e => e.Datestamp)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime")
                    .HasColumnName("DATESTAMP");
                entity.Property(e => e.LogMessage)
                    .HasMaxLength(1000)
                    .IsUnicode(false)
                    .HasColumnName("LOG_MESSAGE");
                entity.Property(e => e.LogStatus)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasDefaultValue("N")
                    .HasColumnName("LOG_STATUS");
                entity.Property(e => e.LogType)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasDefaultValue("NORMAL")
                    .HasColumnName("LOG_TYPE");
                entity.Property(e => e.Seq)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("SEQ");
                entity.Property(e => e.WsAp)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("WS_AP");
                entity.Property(e => e.WsApServer)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("WS_AP_SERVER");
                entity.Property(e => e.WsModule)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("WS_MODULE");
            });

            modelBuilder.Entity<TbSysWsModule>(entity =>
            {
                entity
                    //.HasNoKey()
                    .ToTable("TB_SYS_WS_MODULE");

                entity.Property(e => e.Datestamp)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime")
                    .HasColumnName("DATESTAMP");
                entity.Property(e => e.ModuleId).HasColumnName("MODULE_ID");
                entity.Property(e => e.Userstamp)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("USERSTAMP");
                entity.Property(e => e.WsAp)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("WS_AP");
                entity.Property(e => e.WsMethod)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("WS_METHOD");
                entity.Property(e => e.WsModule)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("WS_MODULE");
            });

            modelBuilder.Entity<TbSysWsPriv>(entity =>
            {
                entity
                    //.HasNoKey()
                    .ToTable("TB_SYS_WS_PRIV");

                entity.Property(e => e.AccountId).HasColumnName("ACCOUNT_ID");
                entity.Property(e => e.Datestamp)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime")
                    .HasColumnName("DATESTAMP");
                entity.Property(e => e.DatestampWs)
                    .HasColumnType("datetime")
                    .HasColumnName("DATESTAMP_WS");
                entity.Property(e => e.LastExecDate).HasColumnType("datetime");
                entity.Property(e => e.PrivSeq).HasColumnName("PRIV_SEQ");
                entity.Property(e => e.Remark)
                    .HasMaxLength(1000)
                    .HasColumnName("REMARK");
                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasDefaultValue("N")
                    .HasColumnName("STATUS");
                entity.Property(e => e.Userstamp)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("USERSTAMP");
                entity.Property(e => e.UserstampNme)
                    .HasMaxLength(100)
                    .HasColumnName("USERSTAMP_NME");
                entity.Property(e => e.WsMethod)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("WS_METHOD");
                entity.Property(e => e.WsModule)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("WS_MODULE");
            });

            modelBuilder.Entity<TbSysWsPrivHist>(entity =>
            {
                entity
                    //.HasNoKey()
                    .ToTable("TB_SYS_WS_PRIV_HIST");

                entity.Property(e => e.AccountId).HasColumnName("ACCOUNT_ID");
                entity.Property(e => e.Datestamp)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime")
                    .HasColumnName("DATESTAMP");
                entity.Property(e => e.HistSeq).HasColumnName("HIST_SEQ");
                entity.Property(e => e.ModifyType)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("MODIFY_TYPE");
                entity.Property(e => e.PrivSeq).HasColumnName("PRIV_SEQ");
                entity.Property(e => e.Remark)
                    .HasMaxLength(1000)
                    .HasColumnName("REMARK");
                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("STATUS");
                entity.Property(e => e.Userstamp)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("USERSTAMP");
                entity.Property(e => e.UserstampNme)
                    .HasMaxLength(100)
                    .HasColumnName("USERSTAMP_NME");
                entity.Property(e => e.WsMethod)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("WS_METHOD");
                entity.Property(e => e.WsModule)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("WS_MODULE");
            });
            OnModelCreatingPartial(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
