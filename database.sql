-- Base de données : parfumerieonline
CREATE DATABASE IF NOT EXISTS parfumerieonline CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE parfumerieonline;

-- Table : users
CREATE TABLE IF NOT EXISTS users (
    id INT AUTO_INCREMENT PRIMARY KEY,
    nom VARCHAR(100) NOT NULL,
    email VARCHAR(150) NOT NULL UNIQUE,
    mot_de_passe VARCHAR(255) NOT NULL,
    role ENUM('client', 'admin') DEFAULT 'client',
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB;

-- Table : categories
CREATE TABLE IF NOT EXISTS categories (
    id INT AUTO_INCREMENT PRIMARY KEY,
    nom VARCHAR(100) NOT NULL UNIQUE
) ENGINE=InnoDB;

-- Table : produits
CREATE TABLE IF NOT EXISTS produits (
    id INT AUTO_INCREMENT PRIMARY KEY,
    nom VARCHAR(150) NOT NULL,
    description TEXT,
    prix DECIMAL(10, 2) NOT NULL,
    image VARCHAR(255),
    stock INT DEFAULT 0,
    categorie_id INT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (categorie_id) REFERENCES categories(id) ON DELETE SET NULL
) ENGINE=InnoDB;

-- Table : panier
CREATE TABLE IF NOT EXISTS panier (
    id INT AUTO_INCREMENT PRIMARY KEY,
    user_id INT NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE
) ENGINE=InnoDB;

-- Table : panier_items
CREATE TABLE IF NOT EXISTS panier_items (
    id INT AUTO_INCREMENT PRIMARY KEY,
    panier_id INT NOT NULL,
    produit_id INT NOT NULL,
    quantite INT DEFAULT 1,
    FOREIGN KEY (panier_id) REFERENCES panier(id) ON DELETE CASCADE,
    FOREIGN KEY (produit_id) REFERENCES produits(id) ON DELETE CASCADE
) ENGINE=InnoDB;

-- Table : commandes
CREATE TABLE IF NOT EXISTS commandes (
    id INT AUTO_INCREMENT PRIMARY KEY,
    user_id INT NOT NULL,
    total DECIMAL(10, 2) NOT NULL,
    date_commande TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    statut ENUM('en_attente', 'validee', 'expediee', 'livree', 'annulee') DEFAULT 'en_attente',
    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE
) ENGINE=InnoDB;

-- Table : commande_items
CREATE TABLE IF NOT EXISTS commande_items (
    id INT AUTO_INCREMENT PRIMARY KEY,
    commande_id INT NOT NULL,
    produit_id INT,
    quantite INT NOT NULL,
    prix DECIMAL(10, 2) NOT NULL,
    FOREIGN KEY (commande_id) REFERENCES commandes(id) ON DELETE CASCADE,
    FOREIGN KEY (produit_id) REFERENCES produits(id) ON DELETE SET NULL
) ENGINE=InnoDB;

-- Insertion de données de test (Fixtures)

-- Catégories
INSERT INTO categories (nom) VALUES ('Parfums Homme'), ('Parfums Femme'), ('Parfums Unisexe'), ('Coffrets Cadeaux');

-- Admin (Mot de passe: admin123)
INSERT INTO users (nom, email, mot_de_passe, role) VALUES 
('Admin User', 'admin@parfumerie.com', '$2y$10$92IXUNpkjO0rOQ5byMi.Ye4oKoEa3Ro9llC/.og/at2.uheWG/igi', 'admin'),
('Client Test', 'client@test.com', '$2y$10$92IXUNpkjO0rOQ5byMi.Ye4oKoEa3Ro9llC/.og/at2.uheWG/igi', 'client');

-- Produits
INSERT INTO produits (nom, description, prix, image, stock, categorie_id) VALUES 
('Eau Sauvage', 'Un classique intemporel pour homme.', 85.00, 'eau_sauvage.jpg', 50, 1),
('Chanel N°5', 'L\'essence de la féminité.', 120.00, 'chanel_n5.jpg', 30, 2),
('CK One', 'Un parfum frais et unisexe.', 45.00, 'ck_one.jpg', 100, 3),
('Coffret Découverte', 'Assortiment de 5 miniatures.', 60.00, 'coffret.jpg', 20, 4);
