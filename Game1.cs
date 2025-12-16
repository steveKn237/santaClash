using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace SantaClash
{
    /// <summary>
    /// Classe principale du jeu Santa Clash
    /// Un jeu simple où le Père Noël tire sur des ennemis qui descendent
    /// </summary>
    public class Game1 : Game
    {
        // ===== CONFIGURATION DU JEU =====
        private const int LARGEUR_FENETRE = 800;
        private const int HAUTEUR_FENETRE = 600;
        
        // Tailles des entités
        private const int TAILLE_SANTA = 40;
        private const int TAILLE_ENNEMI = 30;
        private const int LARGEUR_PROJECTILE = 5;
        private const int HAUTEUR_PROJECTILE = 15;
        
        // Vitesses (en pixels par seconde)
        private const float VITESSE_SANTA = 300f;
        private const float VITESSE_PROJECTILE = 300f;
        private const float VITESSE_ENNEMI = 100f;
        
        // Délai entre les spawns d'ennemis (en secondes)
        private const float DELAI_SPAWN_ENNEMI = 2f;
        
        // Position Y du Père Noël
        private const int POSITION_Y_SANTA = 550;

        // ===== VARIABLES DU JEU =====
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        
        // Texture 1x1 pixel blanc pour dessiner des formes
        private Texture2D _pixelBlanc;
        
        // Position du Père Noël
        private Vector2 _positionSanta;
        
        // Listes des entités du jeu
        private List<Vector2> _projectiles;
        private List<Vector2> _ennemis;
        
        // Générateur de nombres aléatoires pour les positions des ennemis
        private Random _random;
        
        // Timer pour le spawn des ennemis
        private float _timerSpawnEnnemi;
        
        // États des entrées (pour détecter les pressions uniques)
        private KeyboardState _ancienEtatClavier;
        private GamePadState _ancienEtatManette;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            
            // Configuration de la fenêtre
            _graphics.PreferredBackBufferWidth = LARGEUR_FENETRE;
            _graphics.PreferredBackBufferHeight = HAUTEUR_FENETRE;
        }

        /// <summary>
        /// Initialisation du jeu - appelé une fois au démarrage
        /// </summary>
        protected override void Initialize()
        {
            // Initialisation de la position du Père Noël (au centre en bas)
            _positionSanta = new Vector2(
                LARGEUR_FENETRE / 2 - TAILLE_SANTA / 2,
                POSITION_Y_SANTA
            );
            
            // Initialisation des listes
            _projectiles = new List<Vector2>();
            _ennemis = new List<Vector2>();
            
            // Initialisation du générateur aléatoire
            _random = new Random();
            
            // Initialisation du timer de spawn
            _timerSpawnEnnemi = 0f;
            
            base.Initialize();
        }

        /// <summary>
        /// Chargement du contenu - appelé après Initialize
        /// </summary>
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            
            // Création d'une texture 1x1 pixel blanc pour dessiner des rectangles
            _pixelBlanc = new Texture2D(GraphicsDevice, 1, 1);
            _pixelBlanc.SetData(new[] { Color.White });
        }

        /// <summary>
        /// Mise à jour de la logique du jeu - appelé à chaque frame
        /// </summary>
        /// <param name="gameTime">Informations de temps du jeu</param>
        protected override void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            // Récupération des états des entrées
            KeyboardState etatClavier = Keyboard.GetState();
            GamePadState etatManette = GamePad.GetState(PlayerIndex.One);
            
            // Quitter le jeu avec Échap ou bouton Back
            if (etatClavier.IsKeyDown(Keys.Escape) || etatManette.Buttons.Back == ButtonState.Pressed)
                Exit();
            
            // ===== GESTION DU DÉPLACEMENT =====
            GererDeplacement(deltaTime, etatClavier, etatManette);
            
            // ===== GESTION DU TIR =====
            GererTir(etatClavier, etatManette);
            
            // ===== MISE À JOUR DES PROJECTILES =====
            MettreAJourProjectiles(deltaTime);
            
            // ===== MISE À JOUR DES ENNEMIS =====
            MettreAJourEnnemis(deltaTime);
            
            // ===== SPAWN DES ENNEMIS =====
            GererSpawnEnnemis(deltaTime);
            
            // ===== DÉTECTION DES COLLISIONS =====
            DetecterCollisions();
            
            // Sauvegarde des états pour la prochaine frame
            _ancienEtatClavier = etatClavier;
            _ancienEtatManette = etatManette;
            
            base.Update(gameTime);
        }

        /// <summary>
        /// Gère le déplacement horizontal du Père Noël
        /// </summary>
        private void GererDeplacement(float deltaTime, KeyboardState clavier, GamePadState manette)
        {
            float deplacement = 0f;
            
            // Déplacement avec le clavier (flèches gauche/droite)
            if (clavier.IsKeyDown(Keys.Left))
                deplacement -= VITESSE_SANTA * deltaTime;
            if (clavier.IsKeyDown(Keys.Right))
                deplacement += VITESSE_SANTA * deltaTime;
            
            // Déplacement avec la manette (stick gauche ou D-Pad)
            deplacement += manette.ThumbSticks.Left.X * VITESSE_SANTA * deltaTime;
            
            if (manette.DPad.Left == ButtonState.Pressed)
                deplacement -= VITESSE_SANTA * deltaTime;
            if (manette.DPad.Right == ButtonState.Pressed)
                deplacement += VITESSE_SANTA * deltaTime;
            
            // Application du déplacement
            _positionSanta.X += deplacement;
            
            // Limitation pour rester dans l'écran
            if (_positionSanta.X < 0)
                _positionSanta.X = 0;
            if (_positionSanta.X > LARGEUR_FENETRE - TAILLE_SANTA)
                _positionSanta.X = LARGEUR_FENETRE - TAILLE_SANTA;
        }

        /// <summary>
        /// Gère le tir de projectiles
        /// </summary>
        private void GererTir(KeyboardState clavier, GamePadState manette)
        {
            // Détection d'une nouvelle pression sur Espace (clavier)
            bool tirClavier = clavier.IsKeyDown(Keys.Space) && 
                            !_ancienEtatClavier.IsKeyDown(Keys.Space);
            
            // Détection d'une nouvelle pression sur le bouton A (manette)
            bool tirManette = manette.Buttons.A == ButtonState.Pressed && 
                            _ancienEtatManette.Buttons.A == ButtonState.Released;
            
            if (tirClavier || tirManette)
            {
                // Créer un nouveau projectile au centre du Père Noël
                Vector2 positionProjectile = new Vector2(
                    _positionSanta.X + TAILLE_SANTA / 2 - LARGEUR_PROJECTILE / 2,
                    _positionSanta.Y
                );
                _projectiles.Add(positionProjectile);
            }
        }

        /// <summary>
        /// Met à jour les positions des projectiles
        /// </summary>
        private void MettreAJourProjectiles(float deltaTime)
        {
            // Parcourir tous les projectiles en ordre inverse pour pouvoir les supprimer
            for (int i = _projectiles.Count - 1; i >= 0; i--)
            {
                // Déplacer le projectile vers le haut
                Vector2 position = _projectiles[i];
                position.Y -= VITESSE_PROJECTILE * deltaTime;
                _projectiles[i] = position;
                
                // Supprimer le projectile s'il sort de l'écran
                if (position.Y < -HAUTEUR_PROJECTILE)
                {
                    _projectiles.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Met à jour les positions des ennemis
        /// </summary>
        private void MettreAJourEnnemis(float deltaTime)
        {
            // Parcourir tous les ennemis en ordre inverse
            for (int i = _ennemis.Count - 1; i >= 0; i--)
            {
                // Déplacer l'ennemi vers le haut
                Vector2 position = _ennemis[i];
                position.Y -= VITESSE_ENNEMI * deltaTime;
                _ennemis[i] = position;
                
                // Supprimer l'ennemi s'il sort de l'écran (en haut)
                if (position.Y < -TAILLE_ENNEMI)
                {
                    _ennemis.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Gère le spawn périodique des ennemis
        /// </summary>
        private void GererSpawnEnnemis(float deltaTime)
        {
            _timerSpawnEnnemi += deltaTime;
            
            // Si le délai est écoulé, créer un nouvel ennemi
            if (_timerSpawnEnnemi >= DELAI_SPAWN_ENNEMI)
            {
                _timerSpawnEnnemi = 0f;
                
                // Position X aléatoire, position Y en bas de l'écran
                int posX = _random.Next(0, LARGEUR_FENETRE - TAILLE_ENNEMI);
                Vector2 positionEnnemi = new Vector2(posX, HAUTEUR_FENETRE);
                _ennemis.Add(positionEnnemi);
            }
        }

        /// <summary>
        /// Détecte les collisions entre projectiles et ennemis
        /// </summary>
        private void DetecterCollisions()
        {
            // Pour chaque projectile
            for (int i = _projectiles.Count - 1; i >= 0; i--)
            {
                Rectangle rectProjectile = new Rectangle(
                    (int)_projectiles[i].X,
                    (int)_projectiles[i].Y,
                    LARGEUR_PROJECTILE,
                    HAUTEUR_PROJECTILE
                );
                
                // Vérifier la collision avec chaque ennemi
                for (int j = _ennemis.Count - 1; j >= 0; j--)
                {
                    Rectangle rectEnnemi = new Rectangle(
                        (int)_ennemis[j].X,
                        (int)_ennemis[j].Y,
                        TAILLE_ENNEMI,
                        TAILLE_ENNEMI
                    );
                    
                    // Si collision détectée
                    if (rectProjectile.Intersects(rectEnnemi))
                    {
                        // Supprimer le projectile et l'ennemi
                        _projectiles.RemoveAt(i);
                        _ennemis.RemoveAt(j);
                        break; // Un projectile ne peut toucher qu'un ennemi
                    }
                }
            }
        }

        /// <summary>
        /// Dessine le jeu à l'écran - appelé à chaque frame
        /// </summary>
        /// <param name="gameTime">Informations de temps du jeu</param>
        protected override void Draw(GameTime gameTime)
        {
            // Effacer l'écran avec une couleur noire
            GraphicsDevice.Clear(Color.Black);
            
            _spriteBatch.Begin();
            
            // ===== DESSIN DU PÈRE NOËL =====
            DessinerRectangle(_positionSanta, TAILLE_SANTA, TAILLE_SANTA, Color.Red);
            
            // ===== DESSIN DES PROJECTILES =====
            foreach (Vector2 projectile in _projectiles)
            {
                DessinerRectangle(projectile, LARGEUR_PROJECTILE, HAUTEUR_PROJECTILE, Color.Yellow);
            }
            
            // ===== DESSIN DES ENNEMIS =====
            foreach (Vector2 ennemi in _ennemis)
            {
                DessinerRectangle(ennemi, TAILLE_ENNEMI, TAILLE_ENNEMI, Color.LightGray);
            }
            
            _spriteBatch.End();
            
            base.Draw(gameTime);
        }

        /// <summary>
        /// Méthode utilitaire pour dessiner un rectangle coloré
        /// </summary>
        private void DessinerRectangle(Vector2 position, int largeur, int hauteur, Color couleur)
        {
            Rectangle rectangle = new Rectangle((int)position.X, (int)position.Y, largeur, hauteur);
            _spriteBatch.Draw(_pixelBlanc, rectangle, couleur);
        }
    }
}
