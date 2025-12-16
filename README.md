# Santa Clash - Jeu MonoGame Simple

Un jeu MonoGame simple pour débutants où le Père Noël tire sur des ennemis qui montent du bas de l'écran.

## Description du jeu

- **Père Noël** : Rectangle rouge de 40x40 pixels positionné en bas au centre de l'écran
- **Ennemis** : Rectangles gris de 30x30 pixels qui apparaissent en bas et montent vers le haut
- **Projectiles** : Petits rectangles jaunes de 5x15 pixels tirés vers le haut
- **Résolution** : 800x600 pixels

## Contrôles

### Manette 8BitDo Ultimate 2C (ou autre manette compatible)
- **Stick gauche** ou **D-Pad** : Déplacement gauche/droite
- **Bouton A** : Tirer
- **Bouton Back** : Quitter le jeu

### Clavier (backup)
- **Flèche gauche/droite** : Déplacement
- **Espace** : Tirer
- **Échap** : Quitter le jeu

## Spécifications techniques

- **Vitesse du Père Noël** : 300 pixels/seconde
- **Vitesse des projectiles** : 300 pixels/seconde
- **Vitesse des ennemis** : 100 pixels/seconde
- **Spawn des ennemis** : Toutes les 2 secondes à une position X aléatoire

## Comment compiler et exécuter

### Prérequis
- .NET 6.0 ou supérieur
- MonoGame Framework

### Compilation
```bash
dotnet restore
dotnet build
```

### Exécution
```bash
dotnet run
```

## Structure du projet

- **Program.cs** : Point d'entrée de l'application
- **Game1.cs** : Classe principale contenant toute la logique du jeu
- **SantaClash.csproj** : Configuration du projet MonoGame
- **Content/Content.mgcb** : Pipeline de contenu MonoGame (vide, car on utilise des formes simples)
- **app.manifest** : Manifeste de l'application Windows
- **Icon.ico / Icon.bmp** : Icône de l'application

## Code pour débutants

Le code est entièrement commenté en français avec :
- Variables aux noms explicites
- Structure simple et linéaire
- Toute la logique du jeu dans Game1.cs
- Pas de patterns complexes
- Utilisation de formes simples (rectangles) sans textures

## Fonctionnalités implémentées

✅ Déplacement horizontal du Père Noël  
✅ Tir de projectiles vers le haut  
✅ Spawn périodique d'ennemis  
✅ Déplacement automatique des ennemis vers le haut  
✅ Détection des collisions projectile-ennemi  
✅ Support complet des manettes (GamePad)  
✅ Support clavier en backup  
✅ Limitation du mouvement dans les bordures de l'écran  
✅ Suppression automatique des entités hors écran  

## Architecture du code

Le code suit une architecture simple MonoGame :

1. **Initialize()** : Initialisation des variables et positions
2. **LoadContent()** : Chargement du contenu (texture 1x1 pixel pour dessiner)
3. **Update()** : Mise à jour de la logique du jeu à chaque frame
   - Gestion des entrées (clavier + manette)
   - Mise à jour des positions
   - Détection des collisions
4. **Draw()** : Rendu graphique à l'écran

Chaque aspect du jeu est séparé dans des méthodes dédiées pour faciliter la compréhension.
