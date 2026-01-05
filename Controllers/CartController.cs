using Microsoft.AspNetCore.Mvc;
using ParfumerieOnline.Data;
using ParfumerieOnline.Models;
using System.Text.Json;

namespace ParfumerieOnline.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var cart = GetCart();
            return View(cart);
        }

        [HttpPost]
        public IActionResult Add(int productId, int quantity)
        {
            var product = _context.Produits.Find(productId);
            if (product != null)
            {
                var cart = GetCart();
                var existingItem = cart.FirstOrDefault(i => i.ProduitId == productId);

                if (existingItem != null)
                {
                    existingItem.Quantite += quantity;
                }
                else
                {
                    cart.Add(new CartItemViewModel
                    {
                        ProduitId = product.Id,
                        Nom = product.Nom,
                        Prix = product.Prix,
                        Image = product.Image,
                        Quantite = quantity
                    });
                }

                SaveCart(cart);
            }
            return RedirectToAction("Index");
        }

        public IActionResult Remove(int productId)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(i => i.ProduitId == productId);
            if (item != null)
            {
                cart.Remove(item);
                SaveCart(cart);
            }
            return RedirectToAction("Index");
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

        [HttpPost]
        public IActionResult AddToCartApi(int productId, int quantity = 1)
        {
            var product = _context.Produits.Find(productId);
            if (product != null)
            {
                var cart = GetCart();
                var existingItem = cart.FirstOrDefault(i => i.ProduitId == productId);

                if (existingItem != null)
                {
                    existingItem.Quantite += quantity;
                }
                else
                {
                    cart.Add(new CartItemViewModel
                    {
                        ProduitId = product.Id,
                        Nom = product.Nom,
                        Prix = product.Prix,
                        Image = product.Image,
                        Quantite = quantity
                    });
                }

                SaveCart(cart);
                return Json(new { success = true, message = "Produit ajouté au panier !", cartCount = cart.Sum(i => i.Quantite) });
            }
            return Json(new { success = false, message = "Produit introuvable." });
        }

        [HttpGet]
        public IActionResult GetCartCount()
        {
            var cart = GetCart();
            return Json(new { count = cart.Sum(i => i.Quantite) });
        }

        private void SaveCart(List<CartItemViewModel> cart)
        {
            HttpContext.Session.SetString("Cart", JsonSerializer.Serialize(cart));
        }
    }

    public class CartItemViewModel
    {
        public int ProduitId { get; set; }
        public string Nom { get; set; }
        public decimal Prix { get; set; }
        public string Image { get; set; }
        public int Quantite { get; set; }
    }
}
