using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using BookStore_WebApplication.Models;

namespace BookStore_WebApplication
{
    public partial class StoreDBContext : DbContext
    {
        public StoreDBContext()
        {
            Database.EnsureDeleted();
            Database.EnsureCreated();
        }

        public StoreDBContext(DbContextOptions<StoreDBContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        public virtual DbSet<Author> Authors { get; set; } = null!;
        public virtual DbSet<Book> Books { get; set; } = null!;
        public virtual DbSet<BookOrder> BookOrders { get; set; } = null!;
        public virtual DbSet<BookStore> BookStores { get; set; } = null!;
        public virtual DbSet<Client> Clients { get; set; } = null!;
        public virtual DbSet<Delivery> Deliveries { get; set; } = null!;
        public virtual DbSet<Employee> Employees { get; set; } = null!;
        public virtual DbSet<Order> Orders { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server= VLADSFA; Database=StoreDB; Trusted_Connection = True; TrustServerCertificate=True; ");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Author>(entity =>
            {
                entity.ToTable("Author");

                entity.Property(e => e.Name).HasMaxLength(50);
            });

            modelBuilder.Entity<Book>(entity =>
            {
                entity.ToTable("Book");

                entity.Property(e => e.Cost).HasColumnType("money");

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.Type).HasMaxLength(20);

                entity.HasOne(d => d.IdStoreNavigation)
                    .WithMany(p => p.Books)
                    .HasForeignKey(d => d.IdStore)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Book_BookStore");

                entity.HasMany(d => d.IdAuthors)
                    .WithMany(p => p.IdBooks)
                    .UsingEntity<Dictionary<string, object>>(
                        "BookAuthor",
                        l => l.HasOne<Author>().WithMany().HasForeignKey("IdAuthor").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_AuthorList_Author"),
                        r => r.HasOne<Book>().WithMany().HasForeignKey("IdBook").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_AuthorList_Book"),
                        j =>
                        {
                            j.HasKey("IdBook", "IdAuthor").HasName("PK_AuthorList_1");

                            j.ToTable("BookAuthor");

                            j.IndexerProperty<int>("IdBook").ValueGeneratedOnAdd();
                        });
            });

            modelBuilder.Entity<BookOrder>(entity =>
            {
                entity.HasKey(e => new { e.IdOrder, e.IdBook })
                    .HasName("PK_OrderList_1");

                entity.ToTable("BookOrder");

                entity.Property(e => e.IdOrder).ValueGeneratedOnAdd();

                entity.HasOne(d => d.IdBookNavigation)
                    .WithMany(p => p.BookOrders)
                    .HasForeignKey(d => d.IdBook)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrderList_Book");

                entity.HasOne(d => d.IdOrderNavigation)
                    .WithMany(p => p.BookOrders)
                    .HasForeignKey(d => d.IdOrder)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrderList_Order");
            });

            modelBuilder.Entity<BookStore>(entity =>
            {
                entity.ToTable("BookStore");

                entity.Property(e => e.Email).HasMaxLength(40);

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(12)
                    .IsFixedLength();
            });

            modelBuilder.Entity<Client>(entity =>
            {
                entity.ToTable("Client");

                entity.Property(e => e.FullName).HasMaxLength(100);

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(12)
                    .IsFixedLength();
            });

            modelBuilder.Entity<Delivery>(entity =>
            {
                entity.ToTable("Delivery");

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.DeliveryName).HasMaxLength(50);

                entity.Property(e => e.FullCost).HasColumnType("money");

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(12)
                    .IsFixedLength();
            });

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.ToTable("Employee");

                entity.Property(e => e.FullName).HasMaxLength(100);

                entity.Property(e => e.Salary).HasColumnType("money");

                entity.HasOne(d => d.IdStoreNavigation)
                    .WithMany(p => p.Employees)
                    .HasForeignKey(d => d.IdStore)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Employee_BookStore");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("Order");

                entity.Property(e => e.OrderDate).HasColumnType("date");

                entity.HasOne(d => d.IdClientNavigation)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.IdClient)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Order_Client");

                entity.HasOne(d => d.IdDeliveryNavigation)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.IdDelivery)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Order_Delivery");

                entity.HasOne(d => d.IdEmployeeNavigation)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.IdEmployee)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Order_Employee");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
