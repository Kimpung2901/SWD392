using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DAL.Models;

public partial class DollDbContext : DbContext
{
    public DollDbContext(DbContextOptions<DollDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Character> Characters { get; set; }

    public virtual DbSet<CharacterOrder> CharacterOrders { get; set; }

    public virtual DbSet<CharacterPackage> CharacterPackages { get; set; }

    public virtual DbSet<Color> Colors { get; set; }

    public virtual DbSet<DollAvariant> DollAvariants { get; set; }

    public virtual DbSet<DollCharacterLink> DollCharacterLinks { get; set; }

    public virtual DbSet<DollModel> DollModels { get; set; }

    public virtual DbSet<DollType> DollTypes { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

    public virtual DbSet<OwnedDoll> OwnedDolls { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Size> Sizes { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserCharacter> UserCharacters { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Character>(entity =>
        {
            entity.HasKey(e => e.CharacterId).HasName("character_characterid_primary");

            entity.ToTable("Character");

            entity.Property(e => e.CharacterId).ValueGeneratedNever();
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Gender).HasMaxLength(255);
            entity.Property(e => e.Language).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.Personality).HasMaxLength(255);
        });

        modelBuilder.Entity<CharacterOrder>(entity =>
        {
            entity.HasKey(e => e.CharacterOrderID).HasName("characterorder_characterorderid_primary");

            entity.ToTable("CharacterOrder");

            entity.Property(e => e.CharacterOrderID).ValueGeneratedNever();
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(255);
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(8, 2)");
            entity.Property(e => e.end_date).HasColumnType("datetime");
            entity.Property(e => e.start_date).HasColumnType("datetime");
        });

        modelBuilder.Entity<CharacterPackage>(entity =>
        {
            entity.HasKey(e => e.PackageId).HasName("characterpackage_packageid_primary");

            entity.ToTable("CharacterPackage");

            entity.Property(e => e.PackageId).ValueGeneratedNever();
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.Price).HasColumnType("decimal(8, 2)");
            entity.Property(e => e.Status).HasMaxLength(255);
            entity.Property(e => e.billing_cycle).HasMaxLength(255);
        });

        modelBuilder.Entity<Color>(entity =>
        {
            entity.HasKey(e => e.ColorID).HasName("color_colorid_primary");

            entity.ToTable("Color");

            entity.Property(e => e.ColorID).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(255);
        });

        modelBuilder.Entity<DollAvariant>(entity =>
        {
            entity.HasKey(e => e.DollVariantID).HasName("dollavariant_dollvariantid_primary");

            entity.ToTable("DollAvariant");

            entity.Property(e => e.DollVariantID).ValueGeneratedNever();
            entity.Property(e => e.Image).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.Price).HasColumnType("decimal(8, 2)");
        });

        modelBuilder.Entity<DollCharacterLink>(entity =>
        {
            entity.HasKey(e => e.LinkID).HasName("dollcharacterlink_linkid_primary");

            entity.ToTable("DollCharacterLink");

            entity.Property(e => e.LinkID).ValueGeneratedNever();
            entity.Property(e => e.BoundAt).HasColumnType("datetime");
            entity.Property(e => e.Note).HasMaxLength(255);
            entity.Property(e => e.Status).HasMaxLength(255);
            entity.Property(e => e.UnBoundAtbigint).HasColumnType("datetime");
        });

        modelBuilder.Entity<DollModel>(entity =>
        {
            entity.HasKey(e => e.DollModelID).HasName("dollmodel_dollmodelid_primary");

            entity.ToTable("DollModel");

            entity.Property(e => e.DollModelID).ValueGeneratedNever();
            entity.Property(e => e.Create_at).HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Image).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(255);
        });

        modelBuilder.Entity<DollType>(entity =>
        {
            entity.HasKey(e => e.DollTypeID).HasName("dolltype_dolltypeid_primary");

            entity.ToTable("DollType");

            entity.Property(e => e.DollTypeID).ValueGeneratedNever();
            entity.Property(e => e.Create_at).HasColumnType("decimal(8, 2)");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Image).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(255);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderID).HasName("order_orderid_primary");

            entity.ToTable("Order");

            entity.Property(e => e.OrderID).ValueGeneratedNever();
            entity.Property(e => e.Currency).HasMaxLength(255);
            entity.Property(e => e.OrderDate).HasColumnType("datetime");
            entity.Property(e => e.ShippingAddress).HasColumnType("decimal(8, 2)");
            entity.Property(e => e.Status).HasMaxLength(255);
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(8, 2)");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.OrderItemID).HasName("orderitem_orderitemid_primary");

            entity.ToTable("OrderItem");

            entity.Property(e => e.OrderItemID).ValueGeneratedNever();
            entity.Property(e => e.LineTotal).HasColumnType("decimal(8, 2)");
            entity.Property(e => e.Status).HasMaxLength(255);
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(8, 2)");
        });

        modelBuilder.Entity<OwnedDoll>(entity =>
        {
            entity.HasKey(e => e.OwnedDollID).HasName("owneddoll_owneddollid_primary");

            entity.ToTable("OwnedDoll");

            entity.Property(e => e.OwnedDollID).ValueGeneratedNever();
            entity.Property(e => e.SerialCode).HasMaxLength(255);
            entity.Property(e => e.Status).HasMaxLength(255);
            entity.Property(e => e.acquired_at).HasColumnType("datetime");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentID).HasName("payment_paymentid_primary");

            entity.ToTable("Payment");

            entity.Property(e => e.PaymentID).ValueGeneratedNever();
            entity.Property(e => e.Amount).HasColumnType("decimal(8, 2)");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Method).HasMaxLength(255);
            entity.Property(e => e.Provider).HasMaxLength(255);
            entity.Property(e => e.Status).HasMaxLength(255);
            entity.Property(e => e.target_type).HasMaxLength(255);
        });

        modelBuilder.Entity<Size>(entity =>
        {
            entity.HasKey(e => e.SizeID).HasName("size_sizeid_primary");

            entity.ToTable("Size");

            entity.Property(e => e.SizeID).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(255);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserID).HasName("user_userid_primary");

            entity.ToTable("User");

            entity.Property(e => e.UserID).ValueGeneratedNever();
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.Phones).HasMaxLength(255);
            entity.Property(e => e.Role).HasMaxLength(255);
            entity.Property(e => e.Status).HasMaxLength(255);
            entity.Property(e => e.UserName).HasMaxLength(255);
        });

        modelBuilder.Entity<UserCharacter>(entity =>
        {
            entity.HasKey(e => e.UserCharacterID).HasName("usercharacter_usercharacterid_primary");

            entity.ToTable("UserCharacter");

            entity.Property(e => e.UserCharacterID).ValueGeneratedNever();
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.EndAt).HasColumnType("datetime");
            entity.Property(e => e.StartAt).HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(255);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
