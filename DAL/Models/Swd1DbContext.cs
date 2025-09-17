using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using DAL.Models;

namespace DAL.Models;

public partial class Swd1DbContext : DbContext
{
    public Swd1DbContext()
    {
    }

    public Swd1DbContext(DbContextOptions<Swd1DbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<BookingDetail> BookingDetails { get; set; }

    public virtual DbSet<BookingEmotion> BookingEmotions { get; set; }

    public virtual DbSet<BookingTracking> BookingTrackings { get; set; }

    public virtual DbSet<Color> Colors { get; set; }

    public virtual DbSet<Doll> Dolls { get; set; }

    public virtual DbSet<DollVariant> DollVariants { get; set; }

    public virtual DbSet<Emotion> Emotions { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<PaymentMethod> PaymentMethods { get; set; }

    public virtual DbSet<Size> Sizes { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=(local);uid=sa;pwd=123456;database=swd1;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.BookingId).HasName("PK__booking__C6D03BCD3C855DFD");

            entity.ToTable("booking");

            entity.Property(e => e.BookingId).HasColumnName("bookingId");
            entity.Property(e => e.BookingDate)
                .HasColumnType("datetime")
                .HasColumnName("booking_date");
            entity.Property(e => e.ShippingAddress)
                .HasMaxLength(500)
                .HasColumnName("shipping_address");
            entity.Property(e => e.ShippingFree)
                .HasMaxLength(100)
                .HasColumnName("shipping_free");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");
            entity.Property(e => e.TotalAmount)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("total_amount");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.User).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_booking_user");
        });

        modelBuilder.Entity<BookingDetail>(entity =>
        {
            entity.HasKey(e => e.BookingDetailId).HasName("PK__booking___CE75B7360F55AE58");

            entity.ToTable("booking_detail");

            entity.Property(e => e.BookingDetailId).HasColumnName("booking_detailId");
            entity.Property(e => e.BookingId).HasColumnName("bookingId");
            entity.Property(e => e.DollVariantId).HasColumnName("doll_variantId");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("price");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.Subtotal)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("subtotal");

            entity.HasOne(d => d.Booking).WithMany(p => p.BookingDetails)
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_booking_detail_booking");

            entity.HasOne(d => d.DollVariant).WithMany(p => p.BookingDetails)
                .HasForeignKey(d => d.DollVariantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_booking_detail_variant");
        });

        modelBuilder.Entity<BookingEmotion>(entity =>
        {
            entity.HasKey(e => e.BookingEmotionId).HasName("PK__booking___934EFEB07127717E");

            entity.ToTable("booking_emotion");

            entity.Property(e => e.BookingEmotionId).HasColumnName("booking_emotionId");
            entity.Property(e => e.BookingDetailId).HasColumnName("booking_detailId");
            entity.Property(e => e.BookingId).HasColumnName("bookingId");
            entity.Property(e => e.EmotionId).HasColumnName("emotionId");
            entity.Property(e => e.EmotionPrice)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("emotion_price");

            entity.HasOne(d => d.BookingDetail).WithMany(p => p.BookingEmotions)
                .HasForeignKey(d => d.BookingDetailId)
                .HasConstraintName("FK_booking_emotion_booking_detail");

            entity.HasOne(d => d.Booking).WithMany(p => p.BookingEmotions)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("FK_booking_emotion_booking");

            entity.HasOne(d => d.Emotion).WithMany(p => p.BookingEmotions)
                .HasForeignKey(d => d.EmotionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_booking_emotion_emotion");
        });

        modelBuilder.Entity<BookingTracking>(entity =>
        {
            entity.HasKey(e => e.TrackingId).HasName("PK__Booking___A81574EE5F1AFE92");

            entity.ToTable("Booking_tracking");

            entity.Property(e => e.TrackingId).HasColumnName("trackingId");
            entity.Property(e => e.BookingId).HasColumnName("bookingId");
            entity.Property(e => e.Note)
                .HasMaxLength(500)
                .HasColumnName("note");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");
            entity.Property(e => e.UpdateAt)
                .HasColumnType("datetime")
                .HasColumnName("update_at");

            entity.HasOne(d => d.Booking).WithMany(p => p.BookingTrackings)
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_booking_tracking_booking");
        });

        modelBuilder.Entity<Color>(entity =>
        {
            entity.HasKey(e => e.ColorId).HasName("PK__color__70A64FDDB27A1EA2");

            entity.ToTable("color");

            entity.Property(e => e.ColorId).HasColumnName("colorId");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Doll>(entity =>
        {
            entity.HasKey(e => e.DollId).HasName("PK__Doll__B98E38B2D8D41151");

            entity.ToTable("Doll");

            entity.Property(e => e.DollId).HasColumnName("dollId");
            entity.Property(e => e.BasePrice)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("base_price");
            entity.Property(e => e.CreateAt).HasColumnName("create_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Avatar)
                .HasMaxLength(500)
                .HasColumnName("avatar");
            entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        modelBuilder.Entity<DollVariant>(entity =>
        {
            entity.HasKey(e => e.DollVariantId).HasName("PK__doll_var__7732150F172BF79D");

            entity.ToTable("doll_variant");

            entity.Property(e => e.DollVariantId).HasColumnName("doll_variantId");
            entity.Property(e => e.ColorId).HasColumnName("colorId");
            entity.Property(e => e.DollId).HasColumnName("dollId");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("price");
            entity.Property(e => e.SizeId).HasColumnName("sizeId");
            entity.Property(e => e.Stock).HasColumnName("stock");

            entity.HasOne(d => d.Color).WithMany(p => p.DollVariants)
                .HasForeignKey(d => d.ColorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_doll_variant_color");

            entity.HasOne(d => d.Doll).WithMany(p => p.DollVariants)
                .HasForeignKey(d => d.DollId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_doll_variant_doll");

            entity.HasOne(d => d.Size).WithMany(p => p.DollVariants)
                .HasForeignKey(d => d.SizeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_doll_variant_size");
        });

        modelBuilder.Entity<Emotion>(entity =>
        {
            entity.HasKey(e => e.EmotionId).HasName("PK__emotion__F10B8722ADE7BC2E");

            entity.ToTable("emotion");

            entity.Property(e => e.EmotionId).HasColumnName("emotionId");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Image)
                .HasMaxLength(500)
                .HasColumnName("image");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("price");
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.FeedbackId).HasName("PK__feedback__2613FD24316A81DA");

            entity.ToTable("feedback");

            entity.Property(e => e.FeedbackId).HasColumnName("feedbackId");
            entity.Property(e => e.BookingDetailId).HasColumnName("booking_detailId");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");
            entity.Property(e => e.RatingScore).HasColumnName("rating_score");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.BookingDetail).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.BookingDetailId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_feedback_booking_detail");

            entity.HasOne(d => d.User).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_feedback_user");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__payment__A0D9EFC6501E481F");

            entity.ToTable("payment");

            entity.Property(e => e.PaymentId).HasColumnName("paymentId");
            entity.Property(e => e.BookingId).HasColumnName("bookingId");
            entity.Property(e => e.Currency)
                .HasMaxLength(20)
                .HasColumnName("currency");
            entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");
            entity.Property(e => e.PaymentDate)
                .HasColumnType("datetime")
                .HasColumnName("payment_date");
            entity.Property(e => e.PaymentMethodId).HasColumnName("payment_methodId");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");
            entity.Property(e => e.TotalPrice)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("total_price");

            entity.HasOne(d => d.Booking).WithMany(p => p.Payments)
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_payment_booking");

            entity.HasOne(d => d.PaymentMethod).WithMany(p => p.Payments)
                .HasForeignKey(d => d.PaymentMethodId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_payment_method");
        });

        modelBuilder.Entity<PaymentMethod>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__payment___3214EC07FED5E286");

            entity.ToTable("payment_method");

            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Size>(entity =>
        {
            entity.HasKey(e => e.SizeId).HasName("PK__size__55B1E557F5D454F9");

            entity.ToTable("size");

            entity.Property(e => e.SizeId).HasColumnName("sizeId");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__user__CB9A1CFF62062B4C");

            entity.ToTable("user");

            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.Property(e => e.UserName)
                .HasMaxLength(255)
                .HasColumnName("userName");

            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");

            entity.Property(e => e.Gmail)
                .HasMaxLength(255)
                .HasColumnName("gmail");

            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(255)   // trong DB phone_number là NVARCHAR(255)
                .HasColumnName("phone_number");

            entity.Property(e => e.DateOfBirth)
                .HasColumnName("date_of_birth");

            entity.Property(e => e.Address)
                .HasMaxLength(500)
                .HasColumnName("address");

            entity.Property(e => e.Gender)
                .HasMaxLength(50)
                .HasColumnName("gender");

            entity.Property(e => e.CreateAt)
                .HasColumnName("create_at");

            entity.Property(e => e.IsDelete)
                .HasColumnName("is_delete");

            entity.Property(e => e.Role)
                .HasMaxLength(50)
                .HasColumnName("role");

            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");

            entity.Property(e => e.Avatar)
                .HasMaxLength(500)    
                .HasColumnName("avatar");
        });


        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
