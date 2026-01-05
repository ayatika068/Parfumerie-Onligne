using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParfumerieOnline.Data;
using ParfumerieOnline.Models;
using System.Security.Claims;

namespace ParfumerieOnline.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password, string returnUrl = null)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.MotDePasse))
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Nom),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role),
                    new Claim("UserId", user.Id.ToString())
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity));

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                if (user.Role == "admin")
                {
                    return RedirectToAction("Dashboard", "Admin");
                }
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Email ou mot de passe incorrect.";
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(User user)
        {
            if (ModelState.IsValid)
            {
                if (await _context.Users.AnyAsync(u => u.Email == user.Email))
                {
                    ViewBag.Error = "Cet email est déjà utilisé.";
                    return View(user);
                }

                user.MotDePasse = BCrypt.Net.BCrypt.HashPassword(user.MotDePasse);
                user.Role = "client";
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return RedirectToAction("Login");
            }
            return View(user);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}
