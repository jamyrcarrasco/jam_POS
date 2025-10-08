using Microsoft.EntityFrameworkCore;
using jam_POS.Core.Entities;

namespace jam_POS.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                return;
            }
            
            optionsBuilder.ConfigureWarnings(warnings => 
                warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
        }

        public DbSet<Producto> Productos { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Producto entity
            modelBuilder.Entity<Producto>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasMaxLength(200);
                entity.Property(e => e.Precio)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();
                entity.Property(e => e.Stock)
                    .IsRequired();
            });

            // Configure Role entity
            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
                entity.Property(e => e.Description)
                    .HasMaxLength(200);
            });

            // Configure User entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(50);
                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(100);
                entity.Property(e => e.PasswordHash)
                    .IsRequired();
                entity.Property(e => e.FirstName)
                    .HasMaxLength(100);
                entity.Property(e => e.LastName)
                    .HasMaxLength(100);
                entity.Property(e => e.CreatedAt)
                    .IsRequired();
                
                // Configure relationship
                entity.HasOne(u => u.Role)
                    .WithMany(r => r.Users)
                    .HasForeignKey(u => u.RoleId)
                    .OnDelete(DeleteBehavior.Restrict);
                
                // Unique constraints
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
            });

            // Seed data
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "SuperAdmin", Description = "Super Administrator with full access" },
                new Role { Id = 2, Name = "Seller", Description = "Sales person with limited access" }
            );

            modelBuilder.Entity<User>().HasData(
                new User 
                { 
                    Id = 1, 
                    Username = "admin", 
                    Email = "admin@jampos.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                    FirstName = "Super",
                    LastName = "Admin",
                    RoleId = 1,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new User 
                { 
                    Id = 2, 
                    Username = "seller", 
                    Email = "seller@jampos.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("seller123"),
                    FirstName = "John",
                    LastName = "Seller",
                    RoleId = 2,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );
        }
    }
}
