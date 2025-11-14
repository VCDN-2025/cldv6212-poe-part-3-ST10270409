namespace FitHub.Web.Models
{
    public class Customer
    {
        public Guid CustomerId { get; set; }
        public Guid UserId { get; set; }
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string? Phone { get; set; }
        public User User { get; set; } = default!;
    }
}
