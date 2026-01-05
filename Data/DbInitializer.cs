using Microsoft.EntityFrameworkCore;
using System.IO;

namespace ParfumerieOnline.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            // Ensure database is created
            // context.Database.EnsureCreated();

            // Check if tables exist (simple check if users table has data or if table exists)
            // But EnsureCreated() already creates tables based on Model!
            // Wait, if we use EnsureCreated(), it creates tables from C# classes.
            // If we want to use the SQL script specifically (for the data seeding), we should run it.
            
            // Let's check if we have products. If not, we seed.
            // if (context.Produits.Any())
            // {
            //    return; // DB has been seeded
            // }

            // Read SQL file
            var sqlFile = Path.Combine(Directory.GetCurrentDirectory(), "database.sql");
            if (File.Exists(sqlFile))
            {
                var script = File.ReadAllText(sqlFile);
                
                // We need to split script because ExecuteSqlRaw might not handle multiple statements depending on provider
                // But usually MySQL connector supports it if AllowUserVariables=True or similar, but let's try raw execution.
                // Actually, EnsureCreated already created the schema. We just need the DATA.
                // The SQL script has CREATE TABLE IF NOT EXISTS.
                
                // Let's try to execute the script. 
                // Note: The script has "CREATE DATABASE" and "USE" which might fail if we are already connected.
                // We should filter those out or just rely on EnsureCreated + Manual Seeding.
                
                // Better approach for stability: Manual Seeding in C# since we have the data in the SQL script.
                // It's safer than parsing SQL.
                
                SeedData(context);
            }
        }

        private static void SeedData(ApplicationDbContext context)
        {
            // Categories
            if (!context.Categories.Any())
            {
                var categories = new[]
                {
                    new Models.Category { Nom = "Parfums Homme" },
                    new Models.Category { Nom = "Parfums Femme" },
                    new Models.Category { Nom = "Parfums Unisexe" },
                    new Models.Category { Nom = "Coffrets Cadeaux" }
                };
                context.Categories.AddRange(categories);
                context.SaveChanges();
            }

            // Users
            if (!context.Users.Any())
            {
                // Password hash for "password"
                string hash = BCrypt.Net.BCrypt.HashPassword("password");
                
                var users = new[]
                {
                    new Models.User { Nom = "Admin User", Email = "admin@parfumerie.com", MotDePasse = hash, Role = "admin" },
                    new Models.User { Nom = "Client Test", Email = "client@test.com", MotDePasse = hash, Role = "client" }
                };
                context.Users.AddRange(users);
                context.SaveChanges();
            }

            // Products
            var cats = context.Categories.ToList();
            var products = new[]
            {
                new Models.Product { Nom = "Eau Sauvage", Description = "Un classique intemporel pour homme.", Prix = 85.00m, Image = "eau_sauvage.jpg", Stock = 50, CategorieId = cats.FirstOrDefault(c => c.Nom == "Parfums Homme")?.Id },
                new Models.Product { Nom = "Chanel N°5", Description = "L'essence de la féminité.", Prix = 120.00m, Image = "chanel_n5.jpg", Stock = 30, CategorieId = cats.FirstOrDefault(c => c.Nom == "Parfums Femme")?.Id },
                new Models.Product { Nom = "CK One", Description = "Un parfum frais et unisexe.", Prix = 45.00m, Image = "ck_one.jpg", Stock = 100, CategorieId = cats.FirstOrDefault(c => c.Nom == "Parfums Unisexe")?.Id },
                new Models.Product { Nom = "Coffret Découverte", Description = "Assortiment de 5 miniatures.", Prix = 60.00m, Image = "coffret.jpg", Stock = 20, CategorieId = cats.FirstOrDefault(c => c.Nom == "Coffrets Cadeaux")?.Id },
                
                // New Products
                new Models.Product { Nom = "Bleu de Chanel", Description = "Un aromatique-boisé aux notes ambrées et musquées.", Prix = 95.00m, Image = "bleu_chanel.jpg", Stock = 40, CategorieId = cats.FirstOrDefault(c => c.Nom == "Parfums Homme")?.Id },
                new Models.Product { Nom = "J'adore Dior", Description = "Le grand floral féminin de Dior.", Prix = 110.00m, Image = "jadore.jpg", Stock = 35, CategorieId = cats.FirstOrDefault(c => c.Nom == "Parfums Femme")?.Id },
                new Models.Product { Nom = "Acqua di Gio", Description = "Une fragrance mythique, fraîche et aquatique.", Prix = 75.00m, Image = "acqua_di_gio.jpg", Stock = 60, CategorieId = cats.FirstOrDefault(c => c.Nom == "Parfums Homme")?.Id },
                new Models.Product { Nom = "La Vie Est Belle", Description = "Un parfum de bonheur et de liberté.", Prix = 90.00m, Image = "lavieestbelle.jpg", Stock = 45, CategorieId = cats.FirstOrDefault(c => c.Nom == "Parfums Femme")?.Id },
                new Models.Product { Nom = "Terre d'Hermès", Description = "Une eau entre terre et ciel.", Prix = 88.00m, Image = "terre_hermes.jpg", Stock = 25, CategorieId = cats.FirstOrDefault(c => c.Nom == "Parfums Homme")?.Id },
                new Models.Product { Nom = "Black Opium", Description = "Le premier café floral par Yves Saint Laurent.", Prix = 105.00m, Image = "black_opium.jpg", Stock = 30, CategorieId = cats.FirstOrDefault(c => c.Nom == "Parfums Femme")?.Id },
                new Models.Product { Nom = "Santal 33", Description = "Un parfum unisexe culte aux notes de santal et de cardamome.", Prix = 180.00m, Image = "santal33.jpg", Stock = 15, CategorieId = cats.FirstOrDefault(c => c.Nom == "Parfums Unisexe")?.Id },
                new Models.Product { Nom = "Tobacco Vanille", Description = "Opulent, chaud et iconique.", Prix = 220.00m, Image = "tobacco_vanille.jpg", Stock = 10, CategorieId = cats.FirstOrDefault(c => c.Nom == "Parfums Unisexe")?.Id },
                new Models.Product { Nom = "Coffret Luxe", Description = "Sélection prestige pour les connaisseurs.", Prix = 150.00m, Image = "coffret_luxe.jpg", Stock = 8, CategorieId = cats.FirstOrDefault(c => c.Nom == "Coffrets Cadeaux")?.Id },
                new Models.Product { Nom = "Coffret Voyage", Description = "Vos favoris en format voyage.", Prix = 55.00m, Image = "coffret_voyage.jpg", Stock = 50, CategorieId = cats.FirstOrDefault(c => c.Nom == "Coffrets Cadeaux")?.Id }
            };

            foreach (var p in products)
            {
                if (!context.Produits.Any(dbP => dbP.Nom == p.Nom))
                {
                    context.Produits.Add(p);
                }
            }
            context.SaveChanges();
        }
    }
}
