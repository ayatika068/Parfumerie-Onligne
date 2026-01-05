using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ParfumerieOnline.Data;
using ParfumerieOnline.Models;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace ParfumerieOnline.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CheckoutController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var cart = GetCart();
            if (cart.Count == 0)
            {
                return RedirectToAction("Index", "Cart");
            }

            ViewBag.Total = cart.Sum(i => i.Prix * i.Quantite);
            return View(new PaymentViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> ProcessPayment(PaymentViewModel model)
        {
            var cart = GetCart();
            if (cart.Count == 0)
            {
                return RedirectToAction("Index", "Cart");
            }

            if (model.PaymentMethod == "COD")
            {
                ModelState.Remove(nameof(model.CardNumber));
                ModelState.Remove(nameof(model.ExpiryDate));
                ModelState.Remove(nameof(model.CVV));
                ModelState.Remove(nameof(model.CardHolderName));
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Total = cart.Sum(i => i.Prix * i.Quantite);
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                ViewBag.Errors = errors;
                return View("Index", model);
            }

            // Simulate Payment Processing (Success)
            
            // Create Order
            var userId = int.Parse(User.FindFirst("UserId").Value);
            var order = new Order
            {
                UserId = userId,
                Total = cart.Sum(i => i.Prix * i.Quantite),
                DateCommande = DateTime.Now,
                Statut = model.PaymentMethod == "COD" ? "en_attente" : "validee", // COD orders pending initially
                ModePaiement = model.PaymentMethod == "COD" ? "paiement_livraison" : "carte_bancaire"
            };

            _context.Commandes.Add(order);
            await _context.SaveChangesAsync();

            // Create Order Items
            foreach (var item in cart)
            {
                var orderItem = new OrderItem
                {
                    CommandeId = order.Id,
                    ProduitId = item.ProduitId,
                    Quantite = item.Quantite,
                    Prix = item.Prix
                };
                _context.CommandeItems.Add(orderItem);

                // Update Stock (Optional)
                var product = await _context.Produits.FindAsync(item.ProduitId);
                if (product != null)
                {
                    product.Stock -= item.Quantite;
                }
            }

            await _context.SaveChangesAsync();

            // Clear Cart
            HttpContext.Session.Remove("Cart");

            return RedirectToAction("Confirmation", new { id = order.Id });
        }

        public IActionResult Confirmation(int id)
        {
            return View(id);
        }

        public async Task<IActionResult> Invoice(int id)
        {
            var userId = int.Parse(User.FindFirst("UserId").Value);
            var order = await _context.Commandes
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        private List<CartItemViewModel> GetCart()
        {
            var sessionCart = HttpContext.Session.GetString("Cart");
            if (string.IsNullOrEmpty(sessionCart))
            {
                return new List<CartItemViewModel>();
            }
            return JsonSerializer.Deserialize<List<CartItemViewModel>>(sessionCart);
        }
    }
}
