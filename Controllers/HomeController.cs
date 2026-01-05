using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParfumerieOnline.Data;

namespace ParfumerieOnline.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var featuredProducts = await _context.Produits
                .OrderByDescending(p => p.CreatedAt)
                .Take(4)
                .ToListAsync();
            return View(featuredProducts);
        }
    }
}
