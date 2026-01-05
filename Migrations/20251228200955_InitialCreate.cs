using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ParfumerieOnline.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nom = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Panier",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Panier", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "PanierItems",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    panier_id = table.Column<int>(type: "int", nullable: false),
                    produit_id = table.Column<int>(type: "int", nullable: false),
                    quantite = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PanierItems", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nom = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    mot_de_passe = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Produits",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nom = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    prix = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    image = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    stock = table.Column<int>(type: "int", nullable: false),
                    categorie_id = table.Column<int>(type: "int", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Produits", x => x.id);
                    table.ForeignKey(
                        name: "FK_Produits_Categories_categorie_id",
                        column: x => x.categorie_id,
                        principalTable: "Categories",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "Commandes",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    total = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    date_commande = table.Column<DateTime>(type: "datetime2", nullable: false),
                    statut = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Commandes", x => x.id);
                    table.ForeignKey(
                        name: "FK_Commandes_Users_user_id",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CommandeItems",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    commande_id = table.Column<int>(type: "int", nullable: false),
                    produit_id = table.Column<int>(type: "int", nullable: true),
                    quantite = table.Column<int>(type: "int", nullable: false),
                    prix = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommandeItems", x => x.id);
                    table.ForeignKey(
                        name: "FK_CommandeItems_Commandes_commande_id",
                        column: x => x.commande_id,
                        principalTable: "Commandes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommandeItems_Produits_produit_id",
                        column: x => x.produit_id,
                        principalTable: "Produits",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CommandeItems_commande_id",
                table: "CommandeItems",
                column: "commande_id");

            migrationBuilder.CreateIndex(
                name: "IX_CommandeItems_produit_id",
                table: "CommandeItems",
                column: "produit_id");

            migrationBuilder.CreateIndex(
                name: "IX_Commandes_user_id",
                table: "Commandes",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Produits_categorie_id",
                table: "Produits",
                column: "categorie_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommandeItems");

            migrationBuilder.DropTable(
                name: "Panier");

            migrationBuilder.DropTable(
                name: "PanierItems");

            migrationBuilder.DropTable(
                name: "Commandes");

            migrationBuilder.DropTable(
                name: "Produits");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
