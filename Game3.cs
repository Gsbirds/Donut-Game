using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using monogame.Sprites;
using monogame.Screens;
using monogame.Animation;
using monogame.UI;
using monogame.Effects;

namespace monogame
{

    public class Game3 : IGameState
    {
        private Donut donut;
        private DonutHole donutHole; 
        private Lomein lomeinSprite;
        private Lomein lomeinSprite2;
        private CheeseProjectile cheeseProjectile;
        private FruitProjectileManager fruitManager;        
        private Texture2D donutTexture;
        private Texture2D donutHoleTexture;
        private Texture2D lomeinTexture;
        private Texture2D chineseWallpaper;
        private SpriteFont font;
        private bool isColorEffectActive = false;

        
        private Button pinkDonutButton;
        private Texture2D buttonTexture;
        
        private float projectileCooldownTimer = 0f;
        private const float ProjectileCooldown = 2.0f;
        private float minDistanceBetweenLomeins = 100f;
        
        private bool showSplashEffect = false;
        private Vector2 splashPosition;
        private float splashTimer = 0f;
        private const float SPLASH_DURATION = 0.5f;
        private MouseState previousMouseState;

        private GraphicsDevice _graphicsDevice;
        private SpriteBatch _spriteBatch;
        private MainGame _mainGame;

        public Game3(MainGame mainGame, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {
            _mainGame = mainGame;
            _graphicsDevice = graphicsDevice;
            _spriteBatch = spriteBatch;
            
            previousMouseState = Mouse.GetState();
        }

        public void LoadContent()
        {
            if (_mainGame.Game1Instance != null)
            {
                _mainGame.Game1Instance.ResetFruitCooldown();
            }
            
            donutTexture = _mainGame.Content.Load<Texture2D>("donutspritesnew");
            donutHoleTexture = _mainGame.Content.Load<Texture2D>("Donuthole"); 
            
            lomeinTexture = _mainGame.Content.Load<Texture2D>("Lomein");
            
            chineseWallpaper = _mainGame.Content.Load<Texture2D>("chinese_wallpaper_fullsize");
            
            if (Game1.WhitePixel == null) {
                Game1.WhitePixel = new Texture2D(_graphicsDevice, 1, 1);
                Game1.WhitePixel.SetData(new[] { Color.White });
            }
            
            try {
                SoundEffect donutHoleDokenSound = _mainGame.Content.Load<SoundEffect>("donutholedoken");
                DonutHole.SetAttackSound(donutHoleDokenSound);
            } catch (System.Exception e) {
                System.Console.WriteLine("Failed to load donutholedoken sound: " + e.Message);
            }
            
            font = _mainGame.Content.Load<SpriteFont>("DefaultFont1");
            
            var rnd = new Random();
            
            Texture2D cheeseTexture = _mainGame.Content.Load<Texture2D>("cheeselaunch");
            Texture2D cheeseSplashTexture = _mainGame.Content.Load<Texture2D>("splashcheese");
            
            cheeseProjectile = new CheeseProjectile(cheeseTexture, cheeseSplashTexture);
            
            donut = new Donut(donutTexture, new Vector2(400, 300), 200f);
            donutHole = new DonutHole(donutHoleTexture, donut, Vector2.Zero, 400f);
            
            Vector2 pos1 = new Vector2(100, 100);
            Vector2 pos2 = new Vector2(700, 500);
            
            lomeinSprite = new Lomein(lomeinTexture, pos1, 150f);
            lomeinSprite2 = new Lomein(lomeinTexture, pos2, 150f);
            
            lomeinSprite.OnDamageDealt += (damage) => donut.TakeDamage(damage);
            lomeinSprite2.OnDamageDealt += (damage) => donut.TakeDamage(damage);
            
            fruitManager = new FruitProjectileManager(_mainGame.Content);
        }

        public void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            if (donut.Health <= 0)
            {
                _mainGame.SwitchGameState(MainGame.GameStateType.MainMenu);
                return;
            }

            MouseState currentMouseState = Mouse.GetState();
            KeyboardState keyboardState = Keyboard.GetState();

            donut.Update(gameTime);
            donutHole.Update(gameTime);
            
            donut.Position = new Vector2(
                Math.Clamp(donut.Position.X, 48, _graphicsDevice.Viewport.Width - 48),
                Math.Clamp(donut.Position.Y, 64, _graphicsDevice.Viewport.Height - 64)
            );

            if (lomeinSprite.Health > 0)
            {
                lomeinSprite.Update(deltaTime, donut.Position);
            }
            if (lomeinSprite2.Health > 0)
            {
                lomeinSprite2.Update(deltaTime, donut.Position);
            }
            
            if (lomeinSprite.Health > 0 && lomeinSprite2.Health > 0)
            {
                float minDistance = 120f;
                Vector2 distance = lomeinSprite.Position - lomeinSprite2.Position;
                float currentDistance = distance.Length();
                
                if (currentDistance < minDistance && currentDistance > 0)
                {
                    distance.Normalize();
                    
                    float pushDistance = (minDistance - currentDistance) * 0.5f;
                    lomeinSprite.Position += distance * pushDistance;
                    lomeinSprite2.Position -= distance * pushDistance;
                }
            }

            donutHole.CheckCollision(lomeinSprite);
            donutHole.CheckCollision(lomeinSprite2);


            fruitManager.Update(gameTime, donut.Position, donut.GetColor(), currentMouseState, previousMouseState);

            var activeFruits = fruitManager.Projectiles;
            foreach (var fruit in activeFruits)
            {
                Rectangle fruitBounds = fruit.GetBounds();
                
                if (lomeinSprite.Health > 0 && lomeinSprite.GetBounds().Intersects(fruitBounds))
                {
                    lomeinSprite.TakeDamage(15f);
                    fruit.Reset();
                }
                if (lomeinSprite2.Health > 0 && lomeinSprite2.GetBounds().Intersects(fruitBounds))
                {
                    lomeinSprite2.TakeDamage(15f);
                    fruit.Reset();
                }
            }

            if (showSplashEffect)
            {
                splashTimer += deltaTime;
                if (splashTimer >= SPLASH_DURATION)
                {
                    showSplashEffect = false;
                    splashTimer = 0f;
                }
            }


            if (lomeinSprite.Health <= 0 && lomeinSprite2.Health <= 0)
            {
            }


            if (keyboardState.IsKeyDown(Keys.Escape))
            {
                _mainGame.SwitchGameState(MainGame.GameStateType.MainMenu);
            }
            
            previousMouseState = currentMouseState;
        }

        public void Draw(GameTime gameTime)
        {
            _graphicsDevice.Clear(Color.Black);

            int backgroundWidth = chineseWallpaper.Width;
            int backgroundHeight = chineseWallpaper.Height;
            int screenWidth = _graphicsDevice.Viewport.Width;
            int screenHeight = _graphicsDevice.Viewport.Height;
            
            int x = (screenWidth - backgroundWidth) / 2;
            int y = (screenHeight - backgroundHeight) / 2;
            
            _spriteBatch.Draw(chineseWallpaper, new Rectangle(x, y, backgroundWidth, backgroundHeight), Color.White);
            
            donut.Draw(_spriteBatch);
            donutHole.Draw(_spriteBatch);

            if (lomeinSprite.Health > 0)
            {
                lomeinSprite.Draw(_spriteBatch);
            }
            if (lomeinSprite2.Health > 0)
            {
                lomeinSprite2.Draw(_spriteBatch);
            }

            fruitManager.Draw(_spriteBatch);


            string healthText = $"Health: {donut.Health:F0}";
            _spriteBatch.DrawString(font, healthText, new Vector2(10, 10), Color.White);

            int aliveEnemies = 0;
            if (lomeinSprite.Health > 0) aliveEnemies++;
            if (lomeinSprite2.Health > 0) aliveEnemies++;
            
            string enemyText = $"Lomein Enemies: {aliveEnemies}";
            _spriteBatch.DrawString(font, enemyText, new Vector2(10, 40), Color.White);

            _spriteBatch.DrawString(font, "Level 3 - Chinese Garden", new Vector2(10, 70), Color.Yellow);
        }

        public void HandleInput(KeyboardState keyboardState, MouseState mouseState)
        {
        }
    }
}
