using FitHub.Web.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitHub.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _db;
        public AdminController(AppDbContext db) => _db = db;

        // Show all orders for admin
        public async Task<IActionResult> Index()
        {
            var orders = await _db.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .OrderByDescending(o => o.CreatedUtc)
                .ToListAsync();

            return View(orders);
        }

        // Mark order as Processed
        [HttpPost]
        public async Task<IActionResult> Process(Guid id)
        {
            var order = await _db.Orders.FindAsync(id);
            if (order != null)
            {
                order.Status = "Processed";
                await _db.SaveChangesAsync();
                TempData["ok"] = "Order processed.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
