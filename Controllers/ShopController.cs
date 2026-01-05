using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParfumerieOnline.Data;

namespace ParfumerieOnline.Controllers
{
    public class ShopController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ShopController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Categories = await _context.Categories.ToListAsync();
            var products = await _context.Produits.Include(p => p.Category).ToListAsync();
            return View(products);
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _context.Produits
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpGet]
        public async Task<IActionResult> Filter(int? categoryId, decimal? minPrice, decimal? maxPrice, string? searchString)
        {
            var query = _context.Produits.Include(p => p.Category).AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(p => p.Nom.Contains(searchString) || (p.Description != null && p.Description.Contains(searchString)));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategorieId == categoryId);
            }

            if (minPrice.HasValue)
            {
                query = query.Where(p => p.Prix >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.Prix <= maxPrice.Value);
            }

            var products = await query.ToListAsync();
            return PartialView("_ProductList", products);
        }
        [HttpGet]
        public async Task<IActionResult> SearchPreview(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return Json(new List<object>());
            }

            var products = await _context.Produits
                .Where(p => p.Nom.Contains(query) || (p.Description != null && p.Description.Contains(query)))
                .Take(5)
                .Select(p => new
                {
                    id = p.Id,
                    nom = p.Nom,
                    prix = p.Prix,
                    image = p.Image != null ? "/uploads/" + p.Image : "/images/default.png"
                })
                .ToListAsync();

            return Json(products);
        }
    }
}
