using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ParfumerieOnline.Models
{
    public class User
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("nom")]
        public string Nom { get; set; }

        [Required]
        [EmailAddress]
        [Column("email")]
        public string Email { get; set; }

        [Required]
        [Column("mot_de_passe")]
        public string MotDePasse { get; set; }

        [Column("role")]
        public string Role { get; set; } = "client";

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }

    public class Category
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("nom")]
        public string Nom { get; set; }

        public ICollection<Product> Products { get; set; }
    }

    public class Product
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("nom")]
        public string Nom { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Column("prix")]
        public decimal Prix { get; set; }

        [Column("image")]
        public string? Image { get; set; }

        [Column("stock")]
        public int Stock { get; set; }

        [Column("categorie_id")]
        public int? CategorieId { get; set; }
        public Category? Category { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }

    public class Order
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }
        public User User { get; set; }

        [Column("total")]
        public decimal Total { get; set; }

        [Column("date_commande")]
        public DateTime DateCommande { get; set; } = DateTime.Now;

        [Column("statut")]
        public string Statut { get; set; } = "en_attente";

        [Column("mode_paiement")]
        public string ModePaiement { get; set; } = "carte_bancaire";

        public ICollection<OrderItem> Items { get; set; }
    }

    public class OrderItem
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("commande_id")]
        public int CommandeId { get; set; }
        public Order Order { get; set; }

        [Column("produit_id")]
        public int? ProduitId { get; set; }
        public Product Product { get; set; }

        [Column("quantite")]
        public int Quantite { get; set; }

        [Column("prix")]
        public decimal Prix { get; set; }
    }

    public class Cart
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
    }

    public class CartItem
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("panier_id")]
        public int PanierId { get; set; }

        [Column("produit_id")]
        public int ProduitId { get; set; }

        [Column("quantite")]
        public int Quantite { get; set; }
    }
}
