namespace jam_POS.Core.Entities
{
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        
        // Navigation property
        public virtual ICollection<User> Users { get; set; } = new List<User>();
    }
}
