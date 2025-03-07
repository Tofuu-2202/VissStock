using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
// using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
namespace VisssStock.Domain.DataObjects;

public partial class VissstocksDbContext : DbContext
{
    public VissstocksDbContext()
    {
    }

    public VissstocksDbContext(DbContextOptions<VissstocksDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AlertLog> AlertLogs { get; set; }

    public virtual DbSet<ConditionGroup> ConditionGroups { get; set; }

    public virtual DbSet<Exchange> Exchanges { get; set; }

    public virtual DbSet<Indicator> Indicators { get; set; }

    public virtual DbSet<IndicatorDraft> IndicatorDrafts { get; set; }

    public virtual DbSet<Interval> Intervals { get; set; }

    public virtual DbSet<Menu> Menus { get; set; }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<RoleMenu> RoleMenus { get; set; }

    public virtual DbSet<RolePermission> RolePermissions { get; set; }

    public virtual DbSet<Screener> Screeners { get; set; }

    public virtual DbSet<Stock> Stocks { get; set; }

    public virtual DbSet<StockGroup> StockGroups { get; set; }

    public virtual DbSet<StockGroupIndicator> StockGroupIndicators { get; set; }

    public virtual DbSet<StockGroupStock> StockGroupStocks { get; set; }

    public virtual DbSet<TelegramBot> TelegramBots { get; set; }

    public virtual DbSet<Token> Tokens { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<TransactionType> TransactionTypes { get; set; }

    public virtual DbSet<Type> Types { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    public virtual DbSet<UserTelegram> UserTelegrams { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySQL("server=160.30.113.15;port=3306;user=loc_user;password=VissSoft@a123;database=vissstocks_db");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AlertLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("alert_log");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ChatId)
                .HasColumnType("text")
                .HasColumnName("chatId");
            entity.Property(e => e.CreateAt).HasColumnName("createAt");
            entity.Property(e => e.DataJson)
                .HasColumnType("text")
                .HasColumnName("dataJson");
            entity.Property(e => e.Guid)
                .HasMaxLength(45)
                .HasColumnName("GUID");
        });

        modelBuilder.Entity<ConditionGroup>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("condition_group");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreateBy).HasColumnName("createBy");
            entity.Property(e => e.CreateDate)
                .HasColumnType("date")
                .HasColumnName("createDate");
            entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");
            entity.Property(e => e.Name)
                .HasColumnType("text")
                .HasColumnName("name");
            entity.Property(e => e.UpdateBy).HasColumnName("updateBy");
            entity.Property(e => e.UpdateDate)
                .HasColumnType("date")
                .HasColumnName("updateDate");
        });

        modelBuilder.Entity<Exchange>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("exchange");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreateBy).HasColumnName("createBy");
            entity.Property(e => e.CreateDate)
                .HasColumnType("date")
                .HasColumnName("createDate");
            entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");
            entity.Property(e => e.Name)
                .HasMaxLength(45)
                .HasColumnName("name");
            entity.Property(e => e.UpdateBy).HasColumnName("updateBy");
            entity.Property(e => e.UpdateDate)
                .HasColumnType("date")
                .HasColumnName("updateDate");
        });

        modelBuilder.Entity<Indicator>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("indicator");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreateBy).HasColumnName("createBy");
            entity.Property(e => e.CreateDate)
                .HasColumnType("date")
                .HasColumnName("createDate");
            entity.Property(e => e.Formula)
                .HasMaxLength(255)
                .HasColumnName("formula");
            entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");
            entity.Property(e => e.Name)
                .HasMaxLength(45)
                .HasColumnName("name");
            entity.Property(e => e.UpdateBy).HasColumnName("updateBy");
            entity.Property(e => e.UpdateDate)
                .HasColumnType("date")
                .HasColumnName("updateDate");
        });

        modelBuilder.Entity<IndicatorDraft>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("indicator_draft");

            entity.HasIndex(e => e.StockGroupId, "id1_idx");

            entity.HasIndex(e => e.IndicatorId1, "id2_idx");

            entity.HasIndex(e => e.IndicatorId2, "id3_idx");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IndicatorId1).HasColumnName("indicatorId1");
            entity.Property(e => e.IndicatorId2).HasColumnName("indicatorId2");
            entity.Property(e => e.StockGroupId).HasColumnName("stockGroupId");
            entity.Property(e => e.Type)
                .HasColumnType("text")
                .HasColumnName("type");

            entity.HasOne(d => d.IndicatorId1Navigation).WithMany(p => p.IndicatorDraftIndicatorId1Navigations)
                .HasForeignKey(d => d.IndicatorId1)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("id2");

            entity.HasOne(d => d.IndicatorId2Navigation).WithMany(p => p.IndicatorDraftIndicatorId2Navigations)
                .HasForeignKey(d => d.IndicatorId2)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("id3");

            entity.HasOne(d => d.StockGroup).WithMany(p => p.IndicatorDrafts)
                .HasForeignKey(d => d.StockGroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("id1");
        });

        modelBuilder.Entity<Interval>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("interval");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreateBy).HasColumnName("createBy");
            entity.Property(e => e.CreateDate)
                .HasColumnType("date")
                .HasColumnName("createDate");
            entity.Property(e => e.Description)
                .HasMaxLength(45)
                .HasColumnName("description");
            entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");
            entity.Property(e => e.Symbol)
                .HasMaxLength(45)
                .HasColumnName("symbol");
            entity.Property(e => e.UpdateBy).HasColumnName("updateBy");
            entity.Property(e => e.UpdateDate)
                .HasColumnType("date")
                .HasColumnName("updateDate");
        });

        modelBuilder.Entity<Menu>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("menu");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreateBy).HasColumnName("createBy");
            entity.Property(e => e.CreateDate)
                .HasColumnType("date")
                .HasColumnName("createDate");
            entity.Property(e => e.Icon)
                .HasMaxLength(255)
                .HasColumnName("icon");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValueSql("'0'")
                .HasColumnName("isDeleted");
            entity.Property(e => e.Orderno)
                .HasDefaultValueSql("'0'")
                .HasColumnName("orderno");
            entity.Property(e => e.ParentId).HasColumnName("parentId");
            entity.Property(e => e.Path)
                .HasMaxLength(255)
                .HasColumnName("path");
            entity.Property(e => e.Subheader)
                .HasMaxLength(255)
                .HasColumnName("subheader");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
            entity.Property(e => e.UpdateBy).HasColumnName("updateBy");
            entity.Property(e => e.UpdateDate)
                .HasColumnType("date")
                .HasColumnName("updateDate");
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("permission");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreateBy).HasColumnName("createBy");
            entity.Property(e => e.CreateDate)
                .HasColumnType("date")
                .HasColumnName("createDate");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.UpdateBy).HasColumnName("updateBy");
            entity.Property(e => e.UpdateDate)
                .HasColumnType("date")
                .HasColumnName("updateDate");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("role");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreateBy).HasColumnName("createBy");
            entity.Property(e => e.CreateDate)
                .HasColumnType("date")
                .HasColumnName("createDate");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.UpdateBy).HasColumnName("updateBy");
            entity.Property(e => e.UpdateDate)
                .HasColumnType("date")
                .HasColumnName("updateDate");
        });

        modelBuilder.Entity<RoleMenu>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("role_menu");

            entity.HasIndex(e => e.RoleId, "FK_roleMenu_1");

            entity.HasIndex(e => e.MenuId, "FK_roleMenu_2");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreateBy).HasColumnName("createBy");
            entity.Property(e => e.CreateDate)
                .HasColumnType("date")
                .HasColumnName("createDate");
            entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");
            entity.Property(e => e.MenuId).HasColumnName("menuId");
            entity.Property(e => e.RoleId).HasColumnName("roleId");
            entity.Property(e => e.UpdateBy).HasColumnName("updateBy");
            entity.Property(e => e.UpdateDate)
                .HasColumnType("date")
                .HasColumnName("updateDate");

            entity.HasOne(d => d.Menu).WithMany(p => p.RoleMenus)
                .HasForeignKey(d => d.MenuId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_roleMenu_2");

            entity.HasOne(d => d.Role).WithMany(p => p.RoleMenus)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_roleMenu_1");
        });

        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("role_permission");

            entity.HasIndex(e => e.RoleId, "FKRole_Permi239516");

            entity.HasIndex(e => e.PermissionId, "FKRole_Permi568197");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreateBy).HasColumnName("createBy");
            entity.Property(e => e.CreateDate)
                .HasColumnType("date")
                .HasColumnName("createDate");
            entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");
            entity.Property(e => e.PermissionId).HasColumnName("permissionID");
            entity.Property(e => e.RoleId).HasColumnName("roleID");
            entity.Property(e => e.UpdateBy).HasColumnName("updateBy");
            entity.Property(e => e.UpdateDate)
                .HasColumnType("date")
                .HasColumnName("updateDate");

            entity.HasOne(d => d.Permission).WithMany(p => p.RolePermissions)
                .HasForeignKey(d => d.PermissionId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FKRole_Permi568197");

            entity.HasOne(d => d.Role).WithMany(p => p.RolePermissions)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FKRole_Permi239516");
        });

        modelBuilder.Entity<Screener>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("screener");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreateBy).HasColumnName("createBy");
            entity.Property(e => e.CreateDate)
                .HasColumnType("date")
                .HasColumnName("createDate");
            entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");
            entity.Property(e => e.Name)
                .HasMaxLength(45)
                .HasColumnName("name");
            entity.Property(e => e.UpdateBy).HasColumnName("updateBy");
            entity.Property(e => e.UpdateDate)
                .HasColumnType("date")
                .HasColumnName("updateDate");
        });

        modelBuilder.Entity<Stock>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("stock");

            entity.HasIndex(e => e.TypeId, "s1_idx");

            entity.HasIndex(e => e.ExchangeId, "s2_idx");

            entity.HasIndex(e => e.ScreenerId, "s3_idx");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreateBy).HasColumnName("createBy");
            entity.Property(e => e.CreateDate)
                .HasColumnType("date")
                .HasColumnName("createDate");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.ExchangeId).HasColumnName("exchangeId");
            entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");
            entity.Property(e => e.ScreenerId).HasColumnName("screenerId");
            entity.Property(e => e.Symbol)
                .HasMaxLength(45)
                .HasColumnName("symbol");
            entity.Property(e => e.TypeId).HasColumnName("typeId");
            entity.Property(e => e.UpdateBy).HasColumnName("updateBy");
            entity.Property(e => e.UpdateDate)
                .HasColumnType("date")
                .HasColumnName("updateDate");

            entity.HasOne(d => d.Exchange).WithMany(p => p.Stocks)
                .HasForeignKey(d => d.ExchangeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("s2");

            entity.HasOne(d => d.Screener).WithMany(p => p.Stocks)
                .HasForeignKey(d => d.ScreenerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("s32");

            entity.HasOne(d => d.Type).WithMany(p => p.Stocks)
                .HasForeignKey(d => d.TypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("s1");
        });

        modelBuilder.Entity<StockGroup>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("stock_group");

            entity.HasIndex(e => e.IntervalId, "sg1_idx");

            entity.HasIndex(e => e.ConditionGroupId, "sg2_idx");

            entity.HasIndex(e => e.TelegramBotId, "sg3_idx");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ConditionGroupId).HasColumnName("conditionGroupId");
            entity.Property(e => e.CreateBy).HasColumnName("createBy");
            entity.Property(e => e.CreateDate)
                .HasColumnType("date")
                .HasColumnName("createDate");
            entity.Property(e => e.IntervalId)
                .HasDefaultValueSql("'6'")
                .HasColumnName("intervalId");
            entity.Property(e => e.IsActive)
                .HasDefaultValueSql("'1'")
                .HasColumnName("isActive");
            entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");
            entity.Property(e => e.Name)
                .HasColumnType("text")
                .HasColumnName("name");
            entity.Property(e => e.TelegramBotId).HasColumnName("telegramBotId");
            entity.Property(e => e.UpdateBy).HasColumnName("updateBy");
            entity.Property(e => e.UpdateDate)
                .HasColumnType("date")
                .HasColumnName("updateDate");

            entity.HasOne(d => d.ConditionGroup).WithMany(p => p.StockGroups)
                .HasForeignKey(d => d.ConditionGroupId)
                .HasConstraintName("sg2");

            entity.HasOne(d => d.Interval).WithMany(p => p.StockGroups)
                .HasForeignKey(d => d.IntervalId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("sg1");

            entity.HasOne(d => d.TelegramBot).WithMany(p => p.StockGroups)
                .HasForeignKey(d => d.TelegramBotId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("sg3");
        });

        modelBuilder.Entity<StockGroupIndicator>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("stock_group_indicator");

            entity.HasIndex(e => e.StockGroupId, "si1_idx");

            entity.HasIndex(e => e.IndicatorId, "si2_idx");

            entity.HasIndex(e => e.UserId, "si4_idx");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreateBy).HasColumnName("createBy");
            entity.Property(e => e.CreateDate)
                .HasColumnType("date")
                .HasColumnName("createDate");
            entity.Property(e => e.Formula)
                .HasColumnType("text")
                .HasColumnName("formula");
            entity.Property(e => e.IndicatorId).HasColumnName("indicatorId");
            entity.Property(e => e.IsActive)
                .HasDefaultValueSql("'1'")
                .HasColumnName("isActive");
            entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");
            entity.Property(e => e.StockGroupId).HasColumnName("stockGroupId");
            entity.Property(e => e.UpdateBy).HasColumnName("updateBy");
            entity.Property(e => e.UpdateDate)
                .HasColumnType("date")
                .HasColumnName("updateDate");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.Indicator).WithMany(p => p.StockGroupIndicators)
                .HasForeignKey(d => d.IndicatorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("si2");

            entity.HasOne(d => d.StockGroup).WithMany(p => p.StockGroupIndicators)
                .HasForeignKey(d => d.StockGroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("si1");

            entity.HasOne(d => d.User).WithMany(p => p.StockGroupIndicators)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("si4");
        });

        modelBuilder.Entity<StockGroupStock>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("stock_group_stocks");

            entity.HasIndex(e => e.StockId, "sgs1_idx");

            entity.HasIndex(e => e.StockGroupId, "sgs2_idx");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreateBy).HasColumnName("createBy");
            entity.Property(e => e.CreateDate)
                .HasColumnType("date")
                .HasColumnName("createDate");
            entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");
            entity.Property(e => e.IsLike).HasColumnName("isLike");
            entity.Property(e => e.StockGroupId).HasColumnName("stockGroupId");
            entity.Property(e => e.StockId).HasColumnName("stockId");
            entity.Property(e => e.UpdateBy).HasColumnName("updateBy");
            entity.Property(e => e.UpdateDate)
                .HasColumnType("date")
                .HasColumnName("updateDate");

            entity.HasOne(d => d.StockGroup).WithMany(p => p.StockGroupStocks)
                .HasForeignKey(d => d.StockGroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("sgs2");

            entity.HasOne(d => d.Stock).WithMany(p => p.StockGroupStocks)
                .HasForeignKey(d => d.StockId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("sgs1");
        });

        modelBuilder.Entity<TelegramBot>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("telegram_bot");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.TelegramBotName)
                .HasMaxLength(45)
                .HasColumnName("telegramBotName");
            entity.Property(e => e.Token)
                .HasColumnType("text")
                .HasColumnName("token");
        });

        modelBuilder.Entity<Token>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("tokens");

            entity.HasIndex(e => e.UserId, "FKTokens266803");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreateBy).HasColumnName("createBy");
            entity.Property(e => e.CreateDate)
                .HasColumnType("date")
                .HasColumnName("createDate");
            entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");
            entity.Property(e => e.Issue)
                .HasMaxLength(255)
                .HasColumnName("issue");
            entity.Property(e => e.Token1)
                .HasMaxLength(255)
                .HasColumnName("token");
            entity.Property(e => e.UpdateBy).HasColumnName("updateBy");
            entity.Property(e => e.UpdateDate)
                .HasColumnType("date")
                .HasColumnName("updateDate");
            entity.Property(e => e.UserId).HasColumnName("userID");
            entity.Property(e => e.UserId1)
                .HasDefaultValueSql("'0'")
                .HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Tokens)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FKTokens266803");
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("transaction");

            entity.HasIndex(e => e.UserId, "t_1_idx");

            entity.HasIndex(e => e.TypeId, "t_2_idx");

            entity.HasIndex(e => e.StockId, "t_3_idx");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Price).HasColumnName("price");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.StockId).HasColumnName("stockId");
            entity.Property(e => e.Time).HasColumnName("time");
            entity.Property(e => e.TypeId)
                .HasDefaultValueSql("'1'")
                .HasColumnName("typeId");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.Stock).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.StockId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("t_3");

            entity.HasOne(d => d.Type).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.TypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("t_2");

            entity.HasOne(d => d.User).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("t_1");
        });

        modelBuilder.Entity<TransactionType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("transaction_type");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasColumnType("text")
                .HasColumnName("name");
        });

        modelBuilder.Entity<Type>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("type");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreateBy).HasColumnName("createBy");
            entity.Property(e => e.CreateDate)
                .HasColumnType("date")
                .HasColumnName("createDate");
            entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");
            entity.Property(e => e.Name)
                .HasMaxLength(45)
                .HasColumnName("name");
            entity.Property(e => e.UpdateBy).HasColumnName("updateBy");
            entity.Property(e => e.UpdateDate)
                .HasColumnType("date")
                .HasColumnName("updateDate");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("user");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .HasColumnName("address");
            entity.Property(e => e.BirthDate)
                .HasColumnType("date")
                .HasColumnName("birthDate");
            entity.Property(e => e.CreateBy).HasColumnName("createBy");
            entity.Property(e => e.CreateDate)
                .HasColumnType("date")
                .HasColumnName("createDate");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.Enable).HasColumnName("enable");
            entity.Property(e => e.FirstName)
                .HasMaxLength(255)
                .HasColumnName("firstName");
            entity.Property(e => e.Gender).HasColumnName("gender");
            entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");
            entity.Property(e => e.IsTeacher).HasColumnName("isTeacher");
            entity.Property(e => e.LastName)
                .HasMaxLength(255)
                .HasColumnName("lastName");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.Phone)
                .HasMaxLength(255)
                .HasColumnName("phone");
            entity.Property(e => e.TelegramChatId)
                .HasColumnType("text")
                .HasColumnName("telegramChatId");
            entity.Property(e => e.TelegramUsername)
                .HasColumnType("text")
                .HasColumnName("telegramUsername");
            entity.Property(e => e.UpdateBy).HasColumnName("updateBy");
            entity.Property(e => e.UpdateDate)
                .HasColumnType("date")
                .HasColumnName("updateDate");
            entity.Property(e => e.UserName)
                .HasMaxLength(255)
                .HasColumnName("userName");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("user_role");

            entity.HasIndex(e => e.UserId, "FKUser_Role107067");

            entity.HasIndex(e => e.RoleId, "FKUser_Role628471");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreateBy).HasColumnName("createBy");
            entity.Property(e => e.CreateDate)
                .HasColumnType("date")
                .HasColumnName("createDate");
            entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");
            entity.Property(e => e.RoleId).HasColumnName("roleID");
            entity.Property(e => e.UpdateBy).HasColumnName("updateBy");
            entity.Property(e => e.UpdateDate)
                .HasColumnType("date")
                .HasColumnName("updateDate");
            entity.Property(e => e.UserId).HasColumnName("userID");

            entity.HasOne(d => d.Role).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FKUser_Role628471");

            entity.HasOne(d => d.User).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FKUser_Role107067");
        });

        modelBuilder.Entity<UserTelegram>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("user_telegram");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ChatId)
                .HasMaxLength(45)
                .HasColumnName("chatId");
            entity.Property(e => e.TelegramBotId).HasColumnName("telegramBotId");
            entity.Property(e => e.UserId).HasColumnName("userId");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
