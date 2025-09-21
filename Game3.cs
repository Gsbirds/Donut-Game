using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System.Linq;
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
        private Eggroll eggrollSprite;
        private Eggroll eggrollSprite2;
        private Eggroll eggrollSprite3;
        private CheeseProjectile cheeseProjectile;
        private FruitProjectileManager fruitManager;        
        private Texture2D donutTexture;
        private Texture2D donutHoleTexture;
        private Texture2D lomeinTexture;
        private Texture2D eggrollTexture;
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
            eggrollTexture = _mainGame.Content.Load<Texture2D>("Eggroll");
            
            chineseWallpaper = _mainGame.Content.Load<Texture2D>("chinesewallpaperNEW");
            
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
            donutHole = new DonutHole(donutHoleTexture, donut, new Vector2(50, -15), 400f);
            
            Vector2 pos1 = new Vector2(100, 100);
            Vector2 pos2 = new Vector2(700, 500);
            
            lomeinSprite = new Lomein(lomeinTexture, pos1, 100f);
            lomeinSprite2 = new Lomein(lomeinTexture, pos2, 100f);
            
            lomeinSprite.OnDamageDealt += (damage) => donut.TakeDamage(damage);
            lomeinSprite2.OnDamageDealt += (damage) => donut.TakeDamage(damage);
            
            Vector2 eggPos1 = new Vector2(200, 150);
            Vector2 eggPos2 = new Vector2(600, 200);
            Vector2 eggPos3 = new Vector2(400, 450);
            
            eggrollSprite = new Eggroll(eggrollTexture, eggPos1, 120f);
            eggrollSprite2 = new Eggroll(eggrollTexture, eggPos2, 120f);
            eggrollSprite3 = new Eggroll(eggrollTexture, eggPos3, 120f);
            
            eggrollSprite.SetOrbitCenter(eggPos1, 100f, 0f);
            eggrollSprite2.SetOrbitCenter(eggPos2, 120f, MathHelper.PiOver2);
            eggrollSprite3.SetOrbitCenter(eggPos3, 110f, MathHelper.Pi);
            
            eggrollSprite.OnDamageDealt += (damage) => donut.TakeDamage(damage);
            eggrollSprite2.OnDamageDealt += (damage) => donut.TakeDamage(damage);
            eggrollSprite3.OnDamageDealt += (damage) => donut.TakeDamage(damage);
            
            fruitManager = new FruitProjectileManager(_mainGame.Content);
            
            buttonTexture = new Texture2D(_graphicsDevice, 1, 1);
            Color[] colorData = new Color[1];
            colorData[0] = Color.White;
            buttonTexture.SetData(colorData);
            
            pinkDonutButton = new Button(
                new Rectangle(20, 20, 200, 50),
                buttonTexture, 
                font, 
                "Pink Donut");
                
            pinkDonutButton.SetColorIndex(_mainGame.ColorButtonIndex);
            isColorEffectActive = _mainGame.IsColorEffectActive;
            
            if (_mainGame.IsColorEffectActive)
            {
                donut.SetColor(pinkDonutButton.GetCurrentColor());
            }
            
            donutHole.SetTargets(lomeinSprite, lomeinSprite2, eggrollSprite, eggrollSprite2, eggrollSprite3);
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
            
            if (eggrollSprite.Health > 0)
            {
                eggrollSprite.Update(deltaTime, donut.Position);
            }
            if (eggrollSprite2.Health > 0)
            {
                eggrollSprite2.Update(deltaTime, donut.Position);
            }
            if (eggrollSprite3.Health > 0)
            {
                eggrollSprite3.Update(deltaTime, donut.Position);
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
            donutHole.CheckCollision(eggrollSprite);
            donutHole.CheckCollision(eggrollSprite2);
            donutHole.CheckCollision(eggrollSprite3);

            UpdateUIControls(currentMouseState);

            var enemies = new List<Sprite>();
            if (lomeinSprite.Health > 0) enemies.Add(lomeinSprite);
            if (lomeinSprite2.Health > 0) enemies.Add(lomeinSprite2);
            if (eggrollSprite.Health > 0) enemies.Add(eggrollSprite);
            if (eggrollSprite2.Health > 0) enemies.Add(eggrollSprite2);
            if (eggrollSprite3.Health > 0) enemies.Add(eggrollSprite3);
            
            fruitManager.Update(gameTime, donut.Position, donut.GetColor(), currentMouseState, previousMouseState);
            fruitManager.CheckCollisions(enemies.ToArray(), _graphicsDevice);

            if (currentMouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton != ButtonState.Pressed)
            {
                float donutLomeinDistance = Vector2.Distance(donut.Position, lomeinSprite.Position);
                if (donutLomeinDistance < 70 && lomeinSprite.Health > 0) 
                {
                    lomeinSprite.TakeDamage(20f);
                    CheckAllEnemiesDefeated();
                }
                
                float donutLomein2Distance = Vector2.Distance(donut.Position, lomeinSprite2.Position);
                if (donutLomein2Distance < 70 && lomeinSprite2.Health > 0)
                {
                    lomeinSprite2.TakeDamage(20f);
                    CheckAllEnemiesDefeated();
                }
                
                float donutEggroll1Distance = Vector2.Distance(donut.Position, eggrollSprite.Position);
                if (donutEggroll1Distance < 70 && eggrollSprite.Health > 0)
                {
                    eggrollSprite.TakeDamage(20f);
                    CheckAllEnemiesDefeated();
                }
                
                float donutEggroll2Distance = Vector2.Distance(donut.Position, eggrollSprite2.Position);
                if (donutEggroll2Distance < 70 && eggrollSprite2.Health > 0)
                {
                    eggrollSprite2.TakeDamage(20f);
                    CheckAllEnemiesDefeated();
                }
                
                float donutEggroll3Distance = Vector2.Distance(donut.Position, eggrollSprite3.Position);
                if (donutEggroll3Distance < 70 && eggrollSprite3.Health > 0)
                {
                    eggrollSprite3.TakeDamage(20f);
                    CheckAllEnemiesDefeated();
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


            CheckAllEnemiesDefeated();


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
            
            donut.DrawWithColorReplacement(_spriteBatch);
            donutHole.Draw(_spriteBatch);

            if (lomeinSprite.Health > 0)
            {
                lomeinSprite.Draw(_spriteBatch);
            }
            if (lomeinSprite2.Health > 0)
            {
                lomeinSprite2.Draw(_spriteBatch);
            }
            
            if (eggrollSprite.Health > 0)
            {
                eggrollSprite.Draw(_spriteBatch);
            }
            if (eggrollSprite2.Health > 0)
            {
                eggrollSprite2.Draw(_spriteBatch);
            }
            if (eggrollSprite3.Health > 0)
            {
                eggrollSprite3.Draw(_spriteBatch);
            }

            fruitManager.Draw(_spriteBatch);

            pinkDonutButton.Draw(_spriteBatch);
        }

        private void UpdateUIControls(MouseState currentMouseState)
        {
            pinkDonutButton.Update(currentMouseState);
            pinkDonutButton.SetCooldownPercentage(fruitManager.GetCooldownPercentage());
            
            if (pinkDonutButton.IsClicked)
            {
                pinkDonutButton.CycleToNextColor();
                DonutColor newColor = pinkDonutButton.GetCurrentColor();
                donut.SetColor(newColor);
                _mainGame.CurrentDonutColor = newColor;
                _mainGame.ColorButtonIndex = pinkDonutButton.GetCurrentColorIndex();
                
                if (_mainGame.Game1Instance != null)
                {
                    _mainGame.Game1Instance.UpdateDonutColor(newColor, pinkDonutButton.GetCurrentColorIndex());
                }
            }
        }

        private void CheckAllEnemiesDefeated()
        {
            if (lomeinSprite.Health <= 0 && lomeinSprite2.Health <= 0 && 
                eggrollSprite.Health <= 0 && eggrollSprite2.Health <= 0 && eggrollSprite3.Health <= 0)
            {
                // Level 3 completed - could transition to next level or back to main menu
                // For now, just stay in level 3
            }
        }

        public void HandleInput(KeyboardState keyboardState, MouseState mouseState)
        {
        }
    }
}
