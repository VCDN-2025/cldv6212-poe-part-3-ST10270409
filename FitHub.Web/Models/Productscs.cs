namespace FitHub.Web.Models
{
    public class Product
    {
        public Guid ProductId { get; set; }
        public string Name { get; set; } = default!;
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
