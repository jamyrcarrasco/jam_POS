using System.ComponentModel.DataAnnotations;

namespace jam_POS.Core.Entities
{
    public class User
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;
        
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? LastLoginAt { get; set; }
        
        // Foreign keys
        public int RoleId { get; set; }
        public int? EmpresaId { get; set; }
        
        // Navigation properties
        public virtual Role Role { get; set; } = null!;
        public virtual Empresa? Empresa { get; set; }
    }
}
