using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ParfumerieOnline.Data;
using ParfumerieOnline.Models;

namespace ParfumerieOnline.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public AdminController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        public async Task<IActionResult> Dashboard()
        {
            var orders = await _context.Commandes.Include(o => o.User).ToListAsync();
            ViewBag.TotalOrders = orders.Count;
            ViewBag.TotalRevenue = orders.Sum(o => o.Total);

            // Sales Graph Data (Last 7 Days)
            var last7Days = Enumerable.Range(0, 7).Select(i => DateTime.Today.AddDays(-6 + i)).ToList();
            var salesData = new List<decimal>();
            var dates = new List<string>();

            foreach (var date in last7Days)
            {
                var dailyTotal = orders
                    .Where(o => o.DateCommande.Date == date)
                    .Sum(o => o.Total);
                
                salesData.Add(dailyTotal);
                dates.Add(date.ToString("dd/MM"));
            }

            ViewBag.ChartLabels = dates;
            ViewBag.ChartData = salesData;

            return View(orders);
        }

        public async Task<IActionResult> Products()
        {
            var products = await _context.Produits.Include(p => p.Category).ToListAsync();
            return View(products);
        }

        [HttpGet]
        public IActionResult AddProduct()
        {
            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Nom");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct(Product product, IFormFile imageFile)
        {
            // Handle optional description for DB NOT NULL constraint
            if (string.IsNullOrEmpty(product.Description))
            {
                product.Description = "";
                // Clear validation error for Description if it was caused by being null
                ModelState.Remove("Description");
            }

            if (ModelState.IsValid)
            {
                if (imageFile != null)
                {
                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    string fileName = Path.GetFileNameWithoutExtension(imageFile.FileName);
                    string extension = Path.GetExtension(imageFile.FileName);
                    product.Image = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    string path = Path.Combine(wwwRootPath + "/uploads/", fileName);
                    
                    // Ensure directory exists
                    Directory.CreateDirectory(Path.Combine(wwwRootPath, "uploads"));

                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(fileStream);
                    }
                }
                else
                {
                    // Assign default image if none provided to satisfy DB constraint
                    product.Image = "default.jpg";
                    // Remove validation error for Image if it exists
                    ModelState.Remove("Image");
                }

                product.CreatedAt = DateTime.Now;
                _context.Produits.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Products));
            }
            
            // Debugging: Log errors to ViewBag
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            ViewBag.Error = "Erreur lors de l'ajout : " + string.Join(", ", errors);
            
            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Nom", product.CategorieId);
            return View(product);
        }

        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Produits.FindAsync(id);
            if (product != null)
            {
                _context.Produits.Remove(product);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Products));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateOrderStatus(int id, string status)
        {
            var order = await _context.Commandes.FindAsync(id);
            if (order != null)
            {
                order.Statut = status;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Dashboard));
        }
    }
}
