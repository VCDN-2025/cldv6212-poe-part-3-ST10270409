using System;

namespace FitHub.Web.Models
{
    public class CartItemViewModel
    {
        public Guid ProductId { get; set; }
        public string Name { get; set; } = default!;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
