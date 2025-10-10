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
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<Empresa> Empresas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Empresa entity
            modelBuilder.Entity<Empresa>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasMaxLength(200);
                
                entity.Property(e => e.NombreComercial)
                    .IsRequired()
                    .HasMaxLength(100);
                
                entity.Property(e => e.RUC)
                    .IsRequired()
                    .HasMaxLength(50);
                
                entity.Property(e => e.Direccion)
                    .HasMaxLength(200);
                
                entity.Property(e => e.Telefono)
                    .HasMaxLength(50);
                
                entity.Property(e => e.Email)
                    .HasMaxLength(100);
                
                entity.Property(e => e.LogoUrl)
                    .HasMaxLength(500);
                
                entity.Property(e => e.Pais)
                    .HasMaxLength(100);
                
                entity.Property(e => e.Ciudad)
                    .HasMaxLength(100);
                
                entity.Property(e => e.CodigoPostal)
                    .HasMaxLength(20);
                
                entity.Property(e => e.Plan)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasDefaultValue("BASICO");
                
                entity.Property(e => e.Activo)
                    .IsRequired()
                    .HasDefaultValue(true);
                
                entity.Property(e => e.CreatedAt)
                    .IsRequired();
                
                entity.Property(e => e.UpdatedAt)
                    .IsRequired();
                
                // Indexes
                entity.HasIndex(e => e.RUC).IsUnique();
                entity.HasIndex(e => e.Activo);
            });

            // Configure Categoria entity
            modelBuilder.Entity<Categoria>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasMaxLength(100);
                
                entity.Property(e => e.Descripcion)
                    .HasMaxLength(500);
                
                entity.Property(e => e.Color)
                    .HasMaxLength(50);
                
                entity.Property(e => e.Icono)
                    .HasMaxLength(50);
                
                entity.Property(e => e.Activo)
                    .IsRequired()
                    .HasDefaultValue(true);
                
                entity.Property(e => e.CreatedAt)
                    .IsRequired();
                
                entity.Property(e => e.UpdatedAt)
                    .IsRequired();
                
                // Configure relationship with Empresa
                entity.HasOne(c => c.Empresa)
                    .WithMany(e => e.Categorias)
                    .HasForeignKey(c => c.EmpresaId)
                    .OnDelete(DeleteBehavior.Cascade);
                
                // Index for better query performance
                entity.HasIndex(e => e.Nombre);
                entity.HasIndex(e => e.Activo);
                entity.HasIndex(e => e.EmpresaId);
            });

            // Configure Producto entity
            modelBuilder.Entity<Producto>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasMaxLength(200);
                
                entity.Property(e => e.Descripcion)
                    .HasMaxLength(500);
                
                entity.Property(e => e.Precio)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();
                
                entity.Property(e => e.Stock)
                    .IsRequired();
                
                entity.Property(e => e.CodigoBarras)
                    .HasMaxLength(50);
                
                entity.Property(e => e.ImagenUrl)
                    .HasMaxLength(500);
                
                entity.Property(e => e.PrecioCompra)
                    .HasColumnType("decimal(18,2)");
                
                entity.Property(e => e.MargenGanancia)
                    .HasColumnType("decimal(5,2)");
                
                entity.Property(e => e.StockMinimo);
                
                entity.Property(e => e.Activo)
                    .IsRequired()
                    .HasDefaultValue(true);
                
                entity.Property(e => e.CreatedAt)
                    .IsRequired();
                
                entity.Property(e => e.UpdatedAt)
                    .IsRequired();
                
                // Configure relationships
                entity.HasOne(p => p.Categoria)
                    .WithMany(c => c.Productos)
                    .HasForeignKey(p => p.CategoriaId)
                    .OnDelete(DeleteBehavior.SetNull);
                
                entity.HasOne(p => p.Empresa)
                    .WithMany(e => e.Productos)
                    .HasForeignKey(p => p.EmpresaId)
                    .OnDelete(DeleteBehavior.Cascade);
                
                // Indexes for better query performance
                entity.HasIndex(e => e.CategoriaId);
                entity.HasIndex(e => e.CodigoBarras);
                entity.HasIndex(e => e.Activo);
                entity.HasIndex(e => e.EmpresaId);
            });

            // Configure Permission entity
            modelBuilder.Entity<Permission>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);
                
                entity.Property(e => e.Module)
                    .IsRequired()
                    .HasMaxLength(50);
                
                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(200);
                
                entity.Property(e => e.IsSystem)
                    .IsRequired()
                    .HasDefaultValue(false);
                
                entity.Property(e => e.CreatedAt)
                    .IsRequired();
                
                // Index for better query performance
                entity.HasIndex(e => e.Module);
                entity.HasIndex(e => e.Name);
            });

            // Configure Role entity
            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
                
                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(200);
                
                entity.Property(e => e.IsSystem)
                    .IsRequired()
                    .HasDefaultValue(false);
                
                entity.Property(e => e.Activo)
                    .IsRequired()
                    .HasDefaultValue(true);
                
                entity.Property(e => e.CreatedAt)
                    .IsRequired();
                
                entity.Property(e => e.UpdatedAt)
                    .IsRequired();
                
                // Index for better query performance
                entity.HasIndex(e => e.Name).IsUnique();
                entity.HasIndex(e => e.Activo);
            });

            // Configure RolePermission entity
            modelBuilder.Entity<RolePermission>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.GrantedAt)
                    .IsRequired();
                
                // Configure relationship
                entity.HasOne(rp => rp.Role)
                    .WithMany(r => r.RolePermissions)
                    .HasForeignKey(rp => rp.RoleId)
                    .OnDelete(DeleteBehavior.Cascade);
                
                entity.HasOne(rp => rp.Permission)
                    .WithMany(p => p.RolePermissions)
                    .HasForeignKey(rp => rp.PermissionId)
                    .OnDelete(DeleteBehavior.Cascade);
                
                // Index for performance
                entity.HasIndex(e => new { e.RoleId, e.PermissionId }).IsUnique();
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
                
                // Configure relationships
                entity.HasOne(u => u.Role)
                    .WithMany(r => r.Users)
                    .HasForeignKey(u => u.RoleId)
                    .OnDelete(DeleteBehavior.Restrict);
                
                entity.HasOne(u => u.Empresa)
                    .WithMany(e => e.Users)
                    .HasForeignKey(u => u.EmpresaId)
                    .OnDelete(DeleteBehavior.Cascade);
                
                // Unique constraints
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.EmpresaId);
            });

            // Seed data for Categorias
            modelBuilder.Entity<Categoria>().HasData(
                new Categoria 
                { 
                    Id = 1, 
                    Nombre = "Bebidas", 
                    Descripcion = "Bebidas y refrescos",
                    Color = "#3B82F6",
                    Icono = "local_drink",
                    Activo = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Categoria 
                { 
                    Id = 2, 
                    Nombre = "Alimentos", 
                    Descripcion = "Alimentos y comestibles",
                    Color = "#10B981",
                    Icono = "restaurant",
                    Activo = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Categoria 
                { 
                    Id = 3, 
                    Nombre = "Electrónica", 
                    Descripcion = "Productos electrónicos y tecnología",
                    Color = "#8B5CF6",
                    Icono = "devices",
                    Activo = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Categoria 
                { 
                    Id = 4, 
                    Nombre = "Limpieza", 
                    Descripcion = "Productos de limpieza e higiene",
                    Color = "#F59E0B",
                    Icono = "cleaning_services",
                    Activo = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Categoria 
                { 
                    Id = 5, 
                    Nombre = "Ropa", 
                    Descripcion = "Ropa y accesorios",
                    Color = "#EC4899",
                    Icono = "checkroom",
                    Activo = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );

            // Seed data for Permissions
            var permissions = new List<Permission>();
            int permissionId = 1;

            // PRODUCTOS
            permissions.Add(new Permission { Id = permissionId++, Name = "productos.view", Module = "PRODUCTOS", Description = "Ver productos", IsSystem = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) });
            permissions.Add(new Permission { Id = permissionId++, Name = "productos.create", Module = "PRODUCTOS", Description = "Crear productos", IsSystem = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) });
            permissions.Add(new Permission { Id = permissionId++, Name = "productos.edit", Module = "PRODUCTOS", Description = "Editar productos", IsSystem = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) });
            permissions.Add(new Permission { Id = permissionId++, Name = "productos.delete", Module = "PRODUCTOS", Description = "Eliminar productos", IsSystem = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) });

            // CATEGORIAS
            permissions.Add(new Permission { Id = permissionId++, Name = "categorias.view", Module = "CATEGORIAS", Description = "Ver categorías", IsSystem = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) });
            permissions.Add(new Permission { Id = permissionId++, Name = "categorias.create", Module = "CATEGORIAS", Description = "Crear categorías", IsSystem = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) });
            permissions.Add(new Permission { Id = permissionId++, Name = "categorias.edit", Module = "CATEGORIAS", Description = "Editar categorías", IsSystem = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) });
            permissions.Add(new Permission { Id = permissionId++, Name = "categorias.delete", Module = "CATEGORIAS", Description = "Eliminar categorías", IsSystem = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) });

            // VENTAS
            permissions.Add(new Permission { Id = permissionId++, Name = "ventas.create", Module = "VENTAS", Description = "Realizar ventas", IsSystem = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) });
            permissions.Add(new Permission { Id = permissionId++, Name = "ventas.view", Module = "VENTAS", Description = "Ver ventas", IsSystem = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) });
            permissions.Add(new Permission { Id = permissionId++, Name = "ventas.cancel", Module = "VENTAS", Description = "Cancelar ventas", IsSystem = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) });
            permissions.Add(new Permission { Id = permissionId++, Name = "ventas.refund", Module = "VENTAS", Description = "Hacer devoluciones", IsSystem = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) });

            // CLIENTES
            permissions.Add(new Permission { Id = permissionId++, Name = "clientes.view", Module = "CLIENTES", Description = "Ver clientes", IsSystem = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) });
            permissions.Add(new Permission { Id = permissionId++, Name = "clientes.create", Module = "CLIENTES", Description = "Crear clientes", IsSystem = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) });
            permissions.Add(new Permission { Id = permissionId++, Name = "clientes.edit", Module = "CLIENTES", Description = "Editar clientes", IsSystem = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) });
            permissions.Add(new Permission { Id = permissionId++, Name = "clientes.delete", Module = "CLIENTES", Description = "Eliminar clientes", IsSystem = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) });

            // REPORTES
            permissions.Add(new Permission { Id = permissionId++, Name = "reportes.ventas", Module = "REPORTES", Description = "Reportes de ventas", IsSystem = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) });
            permissions.Add(new Permission { Id = permissionId++, Name = "reportes.inventario", Module = "REPORTES", Description = "Reportes de inventario", IsSystem = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) });
            permissions.Add(new Permission { Id = permissionId++, Name = "reportes.clientes", Module = "REPORTES", Description = "Reportes de clientes", IsSystem = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) });
            permissions.Add(new Permission { Id = permissionId++, Name = "reportes.financiero", Module = "REPORTES", Description = "Reportes financieros", IsSystem = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) });

            // USUARIOS
            permissions.Add(new Permission { Id = permissionId++, Name = "usuarios.view", Module = "USUARIOS", Description = "Ver usuarios", IsSystem = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) });
            permissions.Add(new Permission { Id = permissionId++, Name = "usuarios.create", Module = "USUARIOS", Description = "Crear usuarios", IsSystem = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) });
            permissions.Add(new Permission { Id = permissionId++, Name = "usuarios.edit", Module = "USUARIOS", Description = "Editar usuarios", IsSystem = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) });
            permissions.Add(new Permission { Id = permissionId++, Name = "usuarios.delete", Module = "USUARIOS", Description = "Eliminar usuarios", IsSystem = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) });
            permissions.Add(new Permission { Id = permissionId++, Name = "usuarios.change_role", Module = "USUARIOS", Description = "Cambiar rol de usuarios", IsSystem = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) });

            // CONFIGURACIONES
            permissions.Add(new Permission { Id = permissionId++, Name = "config.general", Module = "CONFIGURACIONES", Description = "Configuración general", IsSystem = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) });
            permissions.Add(new Permission { Id = permissionId++, Name = "config.empresa", Module = "CONFIGURACIONES", Description = "Configuración de empresa", IsSystem = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) });
            permissions.Add(new Permission { Id = permissionId++, Name = "config.impuestos", Module = "CONFIGURACIONES", Description = "Configuración de impuestos", IsSystem = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) });
            permissions.Add(new Permission { Id = permissionId++, Name = "config.pos", Module = "CONFIGURACIONES", Description = "Configuración de punto de venta", IsSystem = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) });
            permissions.Add(new Permission { Id = permissionId++, Name = "config.roles", Module = "CONFIGURACIONES", Description = "Gestión de roles y permisos", IsSystem = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) });

            modelBuilder.Entity<Permission>().HasData(permissions);

            // Seed data for Roles
            modelBuilder.Entity<Role>().HasData(
                new Role 
                { 
                    Id = 1, 
                    Name = "SuperAdmin", 
                    Description = "Super Administrador con acceso total", 
                    IsSystem = true,
                    Activo = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Role 
                { 
                    Id = 2, 
                    Name = "Seller", 
                    Description = "Vendedor con acceso limitado", 
                    IsSystem = true,
                    Activo = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );

            // Seed data for RolePermissions (SuperAdmin tiene todos los permisos)
            var superAdminPermissions = new List<RolePermission>();
            for (int i = 1; i <= permissions.Count; i++)
            {
                superAdminPermissions.Add(new RolePermission
                {
                    Id = i,
                    RoleId = 1, // SuperAdmin
                    PermissionId = i,
                    GrantedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                });
            }

            // Seller solo tiene permisos básicos de venta
            var sellerPermissions = new List<RolePermission>
            {
                new RolePermission { Id = permissions.Count + 1, RoleId = 2, PermissionId = 1, GrantedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) }, // productos.view
                new RolePermission { Id = permissions.Count + 2, RoleId = 2, PermissionId = 9, GrantedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) }, // ventas.create
                new RolePermission { Id = permissions.Count + 3, RoleId = 2, PermissionId = 10, GrantedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) }, // ventas.view
                new RolePermission { Id = permissions.Count + 4, RoleId = 2, PermissionId = 13, GrantedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) }, // clientes.view
                new RolePermission { Id = permissions.Count + 5, RoleId = 2, PermissionId = 14, GrantedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) }, // clientes.create
            };

            modelBuilder.Entity<RolePermission>().HasData(superAdminPermissions);
            modelBuilder.Entity<RolePermission>().HasData(sellerPermissions);

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
