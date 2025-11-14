using System.Security.Claims;
using FitHub.Web.Data;
using FitHub.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitHub.Web.Controllers
{
    [Authorize] // only logged-in users can use cart
    public class CartController : Controller
    {
        private const string CartKey = "CART";
        private readonly AppDbContext _db;

        public CartController(AppDbContext db) => _db = db;

        // GET: /Cart
        public IActionResult Index()
        {
            var cart = GetCart();
            return View(cart);
        }

        // POST: /Cart/Add
        [HttpPost]
        public async Task<IActionResult> Add(Guid productId)
        {
            var product = await _db.Products.FirstOrDefaultAsync(p => p.ProductId == productId && p.IsActive);
            if (product == null) return RedirectToAction("Index", "Products");

            var cart = GetCart();
            var existing = cart.FirstOrDefault(c => c.ProductId == productId);

            if (existing == null)
            {
                cart.Add(new CartItemViewModel
                {
                    ProductId = product.ProductId,
                    Name = product.Name,
                    Price = product.Price,
                    Quantity = 1
                });
            }
            else
            {
                existing.Quantity += 1;
            }

            SaveCart(cart);
            TempData["ok"] = "Item added to cart.";
            return RedirectToAction("Index");
        }

        // GET: /Cart/Checkout
        public IActionResult Checkout()
        {
            var cart = GetCart();
            if (!cart.Any())
            {
                TempData["ok"] = "Your cart is empty.";
                return RedirectToAction("Index");
            }

            return View(cart);
        }

        // POST: /Cart/PlaceOrder
        [HttpPost]
        public async Task<IActionResult> PlaceOrder()
        {
            var cart = GetCart();
            if (!cart.Any())
            {
                TempData["ok"] = "Your cart is empty.";
                return RedirectToAction("Index");
            }

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return RedirectToAction("Login", "Account");

            var userId = Guid.Parse(userIdClaim);

            var order = new Order
            {
                OrderId = Guid.NewGuid(),
                UserId = userId,
                CreatedUtc = DateTime.UtcNow,
                Status = "Pending",
                TotalAmount = cart.Sum(c => c.Price * c.Quantity)
            };

            var items = cart.Select(c => new OrderItem
            {
                OrderItemId = Guid.NewGuid(),
                OrderId = order.OrderId,
                ProductId = c.ProductId,
                Quantity = c.Quantity,
                UnitPrice = c.Price
            }).ToList();

            await _db.Orders.AddAsync(order);
            await _db.OrderItems.AddRangeAsync(items);
            await _db.SaveChangesAsync();

            SaveCart(new List<CartItemViewModel>());
            TempData["ok"] = "Order placed successfully.";
            return RedirectToAction("MyOrders");
        }

        // GET: /Cart/MyOrders
        public async Task<IActionResult> MyOrders()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return RedirectToAction("Login", "Account");

            var userId = Guid.Parse(userIdClaim);

            var orders = await _db.Orders
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedUtc)
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .ToListAsync();

            return View(orders);
        }

        // Helpers
        private List<CartItemViewModel> GetCart()
        {
            var cart = HttpContext.Session.GetObject<List<CartItemViewModel>>(CartKey);
            return cart ?? new List<CartItemViewModel>();
        }

        private void SaveCart(List<CartItemViewModel> cart)
        {
            HttpContext.Session.SetObject(CartKey, cart);
        }
    }
}
