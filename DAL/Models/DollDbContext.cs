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


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // DollType
        modelBuilder.Entity<DollType>(entity =>
        {
            entity.HasKey(e => e.DollTypeID);
            entity.ToTable("DollType");
            entity.Property(e => e.Name).HasMaxLength(255).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Image).HasMaxLength(255);
            entity.Property(e => e.Create_at).HasColumnType("datetime");
        });

        // DollModel
        modelBuilder.Entity<DollModel>(entity =>
        {
            entity.HasKey(e => e.DollModelID);
            entity.Property(e => e.DollModelID).ValueGeneratedOnAdd();
            entity.ToTable("DollModel");
            entity.Property(e => e.Name).HasMaxLength(255).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Image).HasMaxLength(255);
            entity.Property(e => e.Create_at).HasColumnType("datetime");
            entity.HasOne(e => e.DollType)
                .WithMany(t => t.DollModels)
                .HasForeignKey(e => e.DollTypeID)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_DollModel_DollType");
        });

        // DollVariant
        modelBuilder.Entity<DollVariant>(entity =>
        {
            entity.HasKey(e => e.DollVariantID);
            entity.Property(e => e.DollVariantID).ValueGeneratedOnAdd();
            entity.ToTable("DollVariant");
            entity.Property(e => e.Name).HasMaxLength(255).IsRequired();
            entity.Property(e => e.Image).HasMaxLength(255);
            entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Color).HasMaxLength(20).IsRequired();
            entity.Property(e => e.Size).HasMaxLength(5).IsRequired();
            entity.HasOne(e => e.DollModel)
                .WithMany()
                .HasForeignKey(e => e.DollModelID)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_DollVariant_DollModel");
        });

        // User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserID);
            entity.ToTable("User");
            entity.Property(e => e.UserName).HasMaxLength(255).IsRequired();
            entity.HasIndex(e => e.UserName).IsUnique();

            entity.Property(e => e.FullName)
                .HasMaxLength(255)
                .IsRequired(false);
            
            entity.Property(e => e.Phones)
                .HasColumnName("Phones")
                .HasMaxLength(255)
                .IsRequired(false);

            entity.Property(e => e.Email).HasMaxLength(255).IsRequired();
            entity.HasIndex(e => e.Email).IsUnique();
            
            entity.Property(e => e.Password).HasMaxLength(255).IsRequired();

            entity.Property(e => e.Age)
                .IsRequired(false); 
            
            entity.Property(e => e.Status)
               .HasConversion<string>()
               .HasMaxLength(50)
               .IsRequired();
            entity.Property(e => e.Role)
                .IsRequired()
                .HasDefaultValue("Customer");  
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime2")
                .HasDefaultValueSql("SYSUTCDATETIME()");
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
        });

        // Order
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderID);
            entity.ToTable("Order");
            entity.Property(e => e.OrderDate).HasColumnType("datetime");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.ShippingAddress).HasMaxLength(500).IsRequired();
            entity.Property(e => e.Status)
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();

            entity.HasOne<User>()
                .WithMany()
                .HasForeignKey(e => e.UserID)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Order_User");
            
            entity.HasOne<Payment>()
                .WithMany()
                .HasForeignKey(e => e.PaymentID)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_Order_Payment");
        });

        // OrderItem
        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.OrderItemID);
            entity.ToTable("OrderItem");
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18,2)");
            entity.Property(e => e.LineTotal).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Status)
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();


            entity.HasOne(e => e.Order)  
                .WithMany(o => o.OrderItems)  
                .HasForeignKey(e => e.OrderID)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_OrderItem_Order");
            
            entity.HasOne(e => e.DollVariant) 
                .WithMany()  
                .HasForeignKey(e => e.DollVariantID)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_OrderItem_DollVariant");
        });

        // OwnedDoll
        modelBuilder.Entity<OwnedDoll>(entity =>
        {
            entity.HasKey(e => e.OwnedDollID);
            entity.ToTable("OwnedDoll");
            entity.Property(e => e.SerialCode).HasMaxLength(255).IsRequired();
            entity.Property(e => e.Status)
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();
            entity.Property(e => e.Acquired_at).HasColumnType("datetime");
            entity.Property(e => e.Expired_at).HasColumnType("datetime");
            
            entity.HasOne<User>()
                .WithMany()
                .HasForeignKey(e => e.UserID)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_OwnedDoll_User");
            
            
            entity.HasOne(e => e.DollVariant)
                .WithMany(d => d.OwnedDolls)
                .HasForeignKey(e => e.DollVariantID)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_OwnedDoll_DollVariant");
        });

        // Character
        modelBuilder.Entity<Character>(entity =>
        {
            entity.HasKey(e => e.CharacterId);
            entity.ToTable("Character");
            entity.Property(e => e.Name).HasMaxLength(255).IsRequired();

            entity.Property(e => e.AgeRange).IsRequired(); 

            entity.Property(e => e.Image).HasMaxLength(500).IsRequired();

            entity.Property(e => e.Personality).HasMaxLength(255).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(255).IsRequired();

            entity.Property(e => e.AIUrl)
                .HasMaxLength(500)
                .IsRequired(false);

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
        });

        // CharacterPackage
        modelBuilder.Entity<CharacterPackage>(entity =>
        {
            entity.HasKey(e => e.PackageId);
            entity.ToTable("CharacterPackage");
            entity.Property(e => e.Name).HasMaxLength(255).IsRequired();
            entity.Property(e => e.Billing_Cycle).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
            entity.Property(e => e.DurationDays).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Status)
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            
            entity.HasOne<Character>()
                .WithMany()
                .HasForeignKey(e => e.CharacterId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_CharacterPackage_Character");
        });

        // UserCharacter
        modelBuilder.Entity<UserCharacter>(entity =>
        {
            entity.HasKey(e => e.UserCharacterID);
            entity.ToTable("UserCharacter");
            entity.Property(e => e.Status)
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();
            entity.Property(e => e.StartAt).HasColumnType("datetime");
            entity.Property(e => e.EndAt).HasColumnType("datetime");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            
            
            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserID)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_UserCharacter_User");

            entity.HasOne(e => e.Character)
                .WithMany()
                .HasForeignKey(e => e.CharacterID)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_UserCharacter_Character");
            
         
            entity.HasOne(e => e.Package)
                .WithMany()
                .HasForeignKey(e => e.PackageId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_UserCharacter_Package");
        });

        // DollCharacterLink
        modelBuilder.Entity<DollCharacterLink>(entity =>
        {
            entity.HasKey(e => e.LinkID);
            entity.ToTable("DollCharacterLink");
            entity.Property(e => e.BoundAt).HasColumnType("datetime");
            entity.Property(e => e.UnBoundAt).HasColumnType("datetime").IsRequired(false); // ✅ NULLABLE
            entity.Property(e => e.Note).HasMaxLength(255);
            entity.Property(e => e.Status)
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();

            entity.HasOne<OwnedDoll>()
                .WithMany()
                .HasForeignKey(e => e.OwnedDollID)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_DollCharacterLink_OwnedDoll");
            
            entity.HasOne<UserCharacter>()
                .WithMany()
                .HasForeignKey(e => e.UserCharacterID)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_DollCharacterLink_UserCharacter");
        });

        // CharacterOrder
        modelBuilder.Entity<CharacterOrder>(entity =>
        {
            entity.HasKey(e => e.CharacterOrderID);
            entity.ToTable("CharacterOrder");
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Status)
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            
            // ✅ Chỉ định navigation properties rõ ràng
            entity.HasOne(e => e.Character)
                .WithMany()
                .HasForeignKey(e => e.CharacterID)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_CharacterOrder_Character");
            
            entity.HasOne(e => e.Package)
                .WithMany()
                .HasForeignKey(e => e.PackageID)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_CharacterOrder_Package");
        });

        // Payment
        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentID);
            entity.ToTable("Payment");

            entity.Property(e => e.Provider).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Method).HasMaxLength(50);
            entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Status)
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();
            entity.Property(e => e.CreatedAt).HasColumnType("datetime2").HasDefaultValueSql("SYSUTCDATETIME()");
            entity.Property(e => e.Target_Type).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Target_Id).IsRequired();
            entity.Property(e => e.TransactionId).HasMaxLength(100);
            entity.Property(e => e.PayUrl).HasMaxLength(500);
            entity.Property(e => e.OrderId).HasMaxLength(100).HasColumnName("MoMoOrderId");
            entity.Property(e => e.OrderInfo).HasMaxLength(255);
            entity.Property(e => e.CompletedAt).HasColumnType("datetime2");
            entity.Property(e => e.RawResponse).HasColumnType("nvarchar(max)");

            entity.HasOne<CharacterOrder>()
                .WithMany()
                .HasForeignKey(e => e.CharacterOrderID)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Payment_CharacterOrder");

            entity.HasOne<Order>()
                .WithMany()
                .HasForeignKey(e => e.OrderID)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Payment_Order");
        });

        // RefreshToken
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.RefreshTokenID);
            entity.ToTable("RefreshTokens");
            entity.Property(e => e.Token).IsRequired();
            entity.Property(e => e.Expires).HasColumnType("datetime2");
            entity.Property(e => e.Revoked).HasColumnType("datetime2");
            entity.Property(e => e.Created).HasColumnType("datetime2");
            entity.Property(e => e.CreatedByIp).HasMaxLength(50);
            
            entity.HasOne(e => e.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(e => e.UserID)
                .OnDelete(DeleteBehavior.Cascade);
        });

        

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
