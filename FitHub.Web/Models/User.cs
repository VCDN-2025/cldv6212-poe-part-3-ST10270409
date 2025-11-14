namespace FitHub.Web.Models
{
    public class User
    {
        public Guid UserId { get; set; }
        public string Username { get; set; } = default!;
        public byte[] PasswordHash { get; set; } = default!;
        public byte[] PasswordSalt { get; set; } = default!;
        public string Role { get; set; } = "Customer"; // Admin | Customer
        public Customer? Customer { get; set; }
    }
}
