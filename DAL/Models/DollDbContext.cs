using Microsoft.EntityFrameworkCore;

namespace DAL.Models;

public partial class DollDbContext : DbContext
{
    public DollDbContext(DbContextOptions<DollDbContext> options) : base(options) { }

    public virtual DbSet<DollType> DollTypes { get; set; }
    public virtual DbSet<DollModel> DollModels { get; set; }
    public virtual DbSet<DollVariant> DollVariants { get; set; }
    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<Order> Orders { get; set; }
    public virtual DbSet<OrderItem> OrderItems { get; set; }
    public virtual DbSet<OwnedDoll> OwnedDolls { get; set; }
    public virtual DbSet<Character> Characters { get; set; }
    public virtual DbSet<CharacterPackage> CharacterPackages { get; set; }
    public virtual DbSet<UserCharacter> UserCharacters { get; set; }
    public virtual DbSet<DollCharacterLink> DollCharacterLinks { get; set; }
    public virtual DbSet<CharacterOrder> CharacterOrders { get; set; }
    public virtual DbSet<Payment> Payments { get; set; }
    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }
    public virtual DbSet<PasswordReset> PasswordResets { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // DollType
        modelBuilder.Entity<DollType>(entity =>
        {
            entity.HasKey(e => e.DollTypeID);
            entity.ToTable("DollType");
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Image).HasMaxLength(255);
            entity.Property(e => e.Create_at).HasColumnType("datetime");
        });

        // DollModel
        modelBuilder.Entity<DollModel>(entity =>
        {
            entity.HasKey(e => e.DollModelID);
            entity.ToTable("DollModel");
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Image).HasMaxLength(255);
            entity.Property(e => e.Create_at).HasColumnType("datetime");
            entity.HasOne<DollType>()
                .WithMany()
                .HasForeignKey(e => e.DollTypeID)
                .HasConstraintName("FK_DollModel_DollType");
        });

        // DollVariant
        modelBuilder.Entity<DollVariant>(entity =>
        {
            entity.HasKey(e => e.DollVariantID);
            entity.ToTable("DollVariant");
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.Image).HasMaxLength(255);
            entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Color).HasMaxLength(20);
            entity.Property(e => e.Size).HasMaxLength(5);
            entity.HasOne<DollModel>()
                .WithMany()
                .HasForeignKey(e => e.DollModelID)
                .HasConstraintName("FK_DollVariant_DollModel");
        });

        // User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserID);
            entity.ToTable("User");
            entity.Property(e => e.UserName).HasMaxLength(255);
            entity.Property(e => e.Phones).HasMaxLength(255);
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.Status).HasDefaultValue("Active");
            entity.Property(e => e.Role).HasDefaultValue("user");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
            entity.Property(u => u.Status)
                     .HasMaxLength(255)
                     .IsRequired()
                     .HasDefaultValue("Active");
        });


        // Order
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderID);
            entity.ToTable("Order");
            entity.Property(e => e.OrderDate).HasColumnType("datetime");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Currency).HasMaxLength(10);
            entity.Property(e => e.ShippingAddress).HasMaxLength(500).IsRequired(); ;
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.HasOne<User>()
                .WithMany()
                .HasForeignKey(e => e.UserID)
                .HasConstraintName("FK_Order_User");
            entity.HasOne<Payment>()
                .WithMany()
                .HasForeignKey(e => e.PaymentID)
                .HasConstraintName("FK_Order_Payment");
        });

        // OrderItem
        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.OrderItemID);
            entity.ToTable("OrderItem");
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18,2)");
            entity.Property(e => e.LineTotal).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.HasOne<Order>()
                .WithMany()
                .HasForeignKey(e => e.OrderID)
                .HasConstraintName("FK_OrderItem_Order");
            entity.HasOne<DollVariant>()
                .WithMany()
                .HasForeignKey(e => e.DollVariantID)
                .HasConstraintName("FK_OrderItem_DollVariant");
        });

        // OwnedDoll
        modelBuilder.Entity<OwnedDoll>(entity =>
        {
            entity.HasKey(e => e.OwnedDollID);
            entity.ToTable("OwnedDoll");
            entity.Property(e => e.SerialCode).HasMaxLength(255);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.Acquired_at).HasColumnType("datetime");
            entity.Property(e => e.Expired_at).HasColumnType("datetime");
            entity.HasOne<User>()
                .WithMany()
                .HasForeignKey(e => e.UserID)
                .HasConstraintName("FK_OwnedDoll_User");
            entity.HasOne<DollVariant>()
                .WithMany()
                .HasForeignKey(e => e.DollVariantID)
                .HasConstraintName("FK_OwnedDoll_DollVariant");
        });

        // Character
        modelBuilder.Entity<Character>(entity =>
        {
            entity.HasKey(e => e.CharacterId);
            entity.ToTable("Character");
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.Gender).HasMaxLength(50);
            entity.Property(e => e.Language).HasMaxLength(50);
            entity.Property(e => e.Personality).HasMaxLength(255);
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
        });

        // CharacterPackage
        modelBuilder.Entity<CharacterPackage>(entity =>
        {
            entity.HasKey(e => e.PackageId);
            entity.ToTable("CharacterPackage");
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.Billing_Cycle).HasMaxLength(50);
            entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.HasOne<Character>()
                .WithMany()
                .HasForeignKey(e => e.CharacterId)
                .HasConstraintName("FK_CharacterPackage_Character");
        });

        // UserCharacter
        modelBuilder.Entity<UserCharacter>(entity =>
        {
            entity.HasKey(e => e.UserCharacterID);
            entity.ToTable("UserCharacter");
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.StartAt).HasColumnType("datetime");
            entity.Property(e => e.EndAt).HasColumnType("datetime");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.HasOne<User>()
                .WithMany()
                .HasForeignKey(e => e.UserID)
                .HasConstraintName("FK_UserCharacter_User");
            entity.HasOne<Character>()
                .WithMany()
                .HasForeignKey(e => e.CharacterID)
                .HasConstraintName("FK_UserCharacter_Character");
            entity.HasOne<CharacterPackage>()
                .WithMany()
                .HasForeignKey(e => e.PackageId)
                .HasConstraintName("FK_UserCharacter_Package");
        });

        // DollCharacterLink
        modelBuilder.Entity<DollCharacterLink>(entity =>
        {
            entity.HasKey(e => e.LinkID);
            entity.ToTable("DollCharacterLink");
            entity.Property(e => e.BoundAt).HasColumnType("datetime");
            entity.Property(e => e.UnBoundAt).HasColumnType("datetime");
            entity.Property(e => e.Note).HasMaxLength(255);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.HasOne<OwnedDoll>()
                .WithMany()
                .HasForeignKey(e => e.OwnedDollID)
                .HasConstraintName("FK_DollCharacterLink_OwnedDoll");
            entity.HasOne<UserCharacter>()
                .WithMany()
                .HasForeignKey(e => e.UserCharacterID)
                .HasConstraintName("FK_DollCharacterLink_UserCharacter");
        });

        // CharacterOrder
        modelBuilder.Entity<CharacterOrder>(entity =>
        {
            entity.HasKey(e => e.CharacterOrderID);
            entity.ToTable("CharacterOrder");
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.Start_Date).HasColumnType("datetime");
            entity.Property(e => e.End_Date).HasColumnType("datetime");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.HasOne<Character>()
                .WithMany()
                .HasForeignKey(e => e.CharacterID)
                .HasConstraintName("FK_CharacterOrder_Character");
            entity.HasOne<CharacterPackage>()
                .WithMany()
                .HasForeignKey(e => e.PackageID)
                .HasConstraintName("FK_CharacterOrder_Package");
            entity.HasOne<UserCharacter>()
                .WithMany()
                .HasForeignKey(e => e.UserCharacterID)
                .HasConstraintName("FK_CharacterOrder_UserCharacter");
        });

        // Payment
        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentID);
            entity.ToTable("Payment");
            entity.Property(e => e.Provider).HasMaxLength(255);
            entity.Property(e => e.Method).HasMaxLength(255);
            entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Currency).HasMaxLength(10);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Target_Type).HasMaxLength(50);
            entity.HasOne<CharacterOrder>()
                .WithMany()
                .HasForeignKey(e => e.CharacterOrderID)
                .HasConstraintName("FK_Payment_CharacterOrder");
            entity.HasOne<Order>()
                .WithMany()
                .HasForeignKey(e => e.OrderID)
                .HasConstraintName("FK_Payment_Order");
        });
        modelBuilder.Entity<PasswordReset>(e =>
        {
            e.ToTable("PasswordResets");
            e.HasKey(x => x.PasswordResetsID);
            e.Property(x => x.UserID).HasColumnName("UserID");
            e.Property(x => x.Code).HasMaxLength(10);
            e.Property(x => x.CreatedByIp).HasMaxLength(50);
            e.Property(x => x.Created).HasColumnType("datetime");
            e.Property(x => x.Expires).HasColumnType("datetime");

            e.HasOne(x => x.User)
             .WithMany(u => u.PasswordResets)   // nhớ thêm ICollection<PasswordReset> trong User
             .HasForeignKey(x => x.UserID)
             .HasConstraintName("FK_PasswordResets_User");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(u => u.UserName).IsUnique();
            entity.HasIndex(u => u.Email).IsUnique();
        });


        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
