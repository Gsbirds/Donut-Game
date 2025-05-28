using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using monogame.Sprites;
using monogame.Screens;
using monogame.Animation;
using monogame.UI;
using monogame.Effects;

namespace monogame
{

    public class Game2 : IGameState
    {
        private Donut donut;
        private DonutHole donutHole; 
        private Sushi sushiSprite;
        private Ginger gingerSprite;
        private CheeseProjectile cheeseProjectile;
        private FruitProjectileManager fruitManager;        
        private Texture2D donutTexture;
        private Texture2D donutHoleTexture;
        private Texture2D sushiTexture;
        private Texture2D sushiHitTexture;
        private Texture2D gingerTexture;
        private Texture2D sushiWallpaper;
        private Texture2D mochiTree;
        private Texture2D puprmushSpritesheet;
        private SpriteFont font;
        private bool isColorEffectActive = false;

        
        private Button pinkDonutButton;
        private Texture2D buttonTexture;
        
        private float projectileCooldownTimer = 0f;
        private const float ProjectileCooldown = 2.0f;
        private float minDistanceBetweenSushiAndGinger = 100f;
        
        private bool showSplashEffect = false;
        private Vector2 splashPosition;
        private float splashTimer = 0f;
        private const float SPLASH_DURATION = 0.5f;
        private MouseState previousMouseState;
        private SpriteBatch _spriteBatch;
        private GraphicsDevice _graphicsDevice;
        private MainGame _mainGame;
        
        private Rectangle[] puprmushFrames;
        private int currentPuprmushFrame;
        private float puprmushFrameTimer;
        private const float PuprmushFrameDuration = 0.2f;
        
        private GameOverScreen gameOverScreen;

        public Game2(MainGame mainGame, SpriteBatch spriteBatch)
        {
            _mainGame = mainGame;
            _spriteBatch = spriteBatch;
            _graphicsDevice = mainGame.GraphicsDevice;
            
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
            sushiTexture = _mainGame.Content.Load<Texture2D>("sushisprites10");
            sushiHitTexture = _mainGame.Content.Load<Texture2D>("Sushihit");
            gingerTexture = _mainGame.Content.Load<Texture2D>("gingersprites4");
            sushiWallpaper = _mainGame.Content.Load<Texture2D>("sushilevelsetting");
            mochiTree = _mainGame.Content.Load<Texture2D>("mochitree");
            puprmushSpritesheet = _mainGame.Content.Load<Texture2D>("pinkmush");
            
            font = _mainGame.Content.Load<SpriteFont>("DefaultFont1");
            
            int frameHeight = gingerTexture.Height / 4;            
            var rnd = new Random();
            
            Texture2D cheeseTexture = _mainGame.Content.Load<Texture2D>("cheeselaunch");
            Texture2D cheeseSplashTexture = _mainGame.Content.Load<Texture2D>("splashcheese");
            
            fruitManager = new FruitProjectileManager(_mainGame.Content);
            
            int frameWidth = puprmushSpritesheet.Width / 5;
            int frameHeight2 = puprmushSpritesheet.Height; 
            puprmushFrames = new Rectangle[5];
            for (int i = 0; i < 5; i++)
            {
                puprmushFrames[i] = new Rectangle(i * frameWidth, 0, frameWidth, frameHeight2);
            }
            currentPuprmushFrame = 0;
            puprmushFrameTimer = 0f;
            
            donut = new Donut(donutTexture, new Vector2(_graphicsDevice.Viewport.Width - 96, _graphicsDevice.Viewport.Height - 128), 160f);
            donutHole = new DonutHole(donutHoleTexture, donut, new Vector2(-55, -55), 160f);
            donut.SetInGame2(true);
            if (_mainGame.HasPickedUpAxe)
            {
                donut.PickupAxe();
            }
            int screenWidth = _graphicsDevice.Viewport.Width;
            int screenHeight = _graphicsDevice.Viewport.Height;
            
            sushiSprite = new Sushi(sushiTexture, sushiHitTexture, 
                new Vector2(50, screenHeight - 150), 120f);
                
            gingerSprite = new Ginger(gingerTexture, 
                new Vector2(150, screenHeight / 2 - 100), 70f);
            cheeseProjectile = new CheeseProjectile(cheeseTexture, cheeseSplashTexture);
            
            sushiSprite.OnDamageDealt += (damage) => {
                donut.TakeDamage(3f);  
            };
            
            gameOverScreen = new GameOverScreen(_mainGame, _graphicsDevice, font);
            
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
            
        }

        public void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            MouseState currentMouseState = Mouse.GetState();
            
            fruitManager.Update(gameTime, donut.Position, donut.GetColor(), currentMouseState, previousMouseState);
            
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
            
            MouseState mouseState = Mouse.GetState();
            HandleMouseAttacks();
            
            fruitManager.Update(gameTime, donut.Position, donut.GetColor(), mouseState, previousMouseState);
            fruitManager.CheckCollisions(new Sprite[] { sushiSprite, gingerSprite }, _graphicsDevice);
            
            if (pinkDonutButton != null)
            {
                pinkDonutButton.SetCooldownPercentage(fruitManager.GetCooldownPercentage());
            }
            
            if (donut.Health <= 0)
            {
                if (gameOverScreen != null)
                    gameOverScreen.Activate();
                return;
            }

            donut.Update(gameTime);
            donutHole.Update(gameTime);
            
            Vector2 donutPos = donut.Position;
            
            sushiSprite.SetTargetPosition(donutPos);
            gingerSprite.SetTargetPosition(donutPos);
            sushiSprite.Update(gameTime);
            gingerSprite.Update(gameTime);
            
            if (donut.CollidesWith(sushiSprite))
                Sprite.ResolveCollision(donut, sushiSprite);
                
            if (donut.CollidesWith(gingerSprite))
                Sprite.ResolveCollision(donut, gingerSprite);
            
            Vector2 sushiToGinger = gingerSprite.Position - sushiSprite.Position;
            if (sushiToGinger.Length() < minDistanceBetweenSushiAndGinger)
            {
                Vector2 separation = Vector2.Normalize(sushiToGinger) * minDistanceBetweenSushiAndGinger;
                gingerSprite.Position = sushiSprite.Position + separation;
            }
            
            puprmushFrameTimer += deltaTime;
            if (puprmushFrameTimer >= PuprmushFrameDuration)
            {
                puprmushFrameTimer = 0f;
                currentPuprmushFrame = (currentPuprmushFrame + 1) % puprmushFrames.Length;
            }
            
            donut.Position = new Vector2(
                Math.Clamp(donut.Position.X, 48, _graphicsDevice.Viewport.Width - 48),
                Math.Clamp(donut.Position.Y, 64, _graphicsDevice.Viewport.Height - 64)
            );
            
            UpdateProjectile(gameTime, deltaTime);
            
            HandleMouseAttacks();
            
            Sprite[] enemies = { sushiSprite, gingerSprite };
            fruitManager.CheckCollisions(enemies, _graphicsDevice);
            
            previousMouseState = currentMouseState;
        }

        public void Draw(GameTime gameTime)
        {
            _graphicsDevice.Clear(Color.Black);

            _spriteBatch.Draw(sushiWallpaper, new Rectangle(0, 0, 850, 850), Color.White);
            
            _spriteBatch.Draw(mochiTree, new Rectangle(250, 50, 878, 878), Color.White);
            
            Vector2 puprmushPosition = new Vector2(120, 50 + 550);
            _spriteBatch.Draw(
                puprmushSpritesheet,
                puprmushPosition,
                puprmushFrames[currentPuprmushFrame],
                Color.White,
                0f,
                new Vector2(puprmushFrames[currentPuprmushFrame].Width / 2, puprmushFrames[currentPuprmushFrame].Height / 2),
                0.18f,
                SpriteEffects.None,
                0f
            );

            // Custom drawing code for Sushi to ensure it shows defeated state properly
            if (sushiSprite.Health <= 0)
            {
                // When defeated, draw with rotation and gray color
                Texture2D texture = sushiSprite.Texture;
                Vector2 position = sushiSprite.Position;
                Rectangle currentFrame;
                
                // Use first frame of the appropriate direction for simplicity
                if (sushiSprite.FacingDirection == Direction.Up)
                    currentFrame = new Rectangle(0, 0, 110, 133);
                else if (sushiSprite.FacingDirection == Direction.Right)
                    currentFrame = new Rectangle(0, 133, 110, 133);
                else if (sushiSprite.FacingDirection == Direction.Down)
                    currentFrame = new Rectangle(0, 266, 110, 133);
                else // Left
                    currentFrame = new Rectangle(0, 399, 110, 133);
                
                Vector2 origin = new Vector2(currentFrame.Width / 2, currentFrame.Height / 2);
                
                // Draw with rotation and gray color
                _spriteBatch.Draw(
                    texture,
                    position,
                    currentFrame,
                    Color.Gray * sushiSprite.FadeAlpha,
                    MathHelper.PiOver2, // 90 degrees rotation
                    origin,
                    1.0f,
                    SpriteEffects.None,
                    0f
                );
            }
            else
            {
                // Normal drawing when not defeated
                sushiSprite.Draw(_spriteBatch);
            }
            
            donut.DrawWithColorReplacement(_spriteBatch);
            donutHole.Draw(_spriteBatch);
            
            // Custom drawing code for Ginger to ensure it shows defeated state properly
            if (gingerSprite.Health <= 0)
            {
                // When defeated, draw with rotation and gray color
                Texture2D texture = gingerSprite.Texture;
                Vector2 position = gingerSprite.Position;
                Rectangle currentFrame;
                
                // Use first frame of the appropriate direction for simplicity
                if (gingerSprite.FacingDirection == Direction.Up)
                    currentFrame = new Rectangle(0, 0, 110, 133);
                else if (gingerSprite.FacingDirection == Direction.Right)
                    currentFrame = new Rectangle(0, 133, 110, 133);
                else if (gingerSprite.FacingDirection == Direction.Down)
                    currentFrame = new Rectangle(0, 266, 110, 133);
                else // Left
                    currentFrame = new Rectangle(0, 399, 110, 133);
                
                Vector2 origin = new Vector2(currentFrame.Width / 2, currentFrame.Height / 2);
                
                // Draw with rotation and gray color
                _spriteBatch.Draw(
                    texture,
                    position,
                    currentFrame,
                    Color.Gray * gingerSprite.FadeAlpha,
                    MathHelper.PiOver2, // 90 degrees rotation
                    origin,
                    1.0f,
                    SpriteEffects.None,
                    0f
                );
            }
            else
            {
                // Normal drawing when not defeated
                gingerSprite.Draw(_spriteBatch);
            }
            
            fruitManager.Draw(_spriteBatch);
            
            if (cheeseProjectile.IsActive)
            {
                _spriteBatch.Draw(
                    cheeseProjectile.Texture,
                    cheeseProjectile.Position,
                    null,
                    Color.White,
                    cheeseProjectile.Rotation,
                    new Vector2(cheeseProjectile.Texture.Width / 2, cheeseProjectile.Texture.Height / 2),
                    0.5f,
                    SpriteEffects.None,
                    0
                );
            }

            if (showSplashEffect)
            {
                _spriteBatch.Draw(
                    cheeseProjectile.SplashTexture,
                    splashPosition,
                    null,
                    Color.White * (1.0f - splashTimer / SPLASH_DURATION),
                    0f,
                    new Vector2(cheeseProjectile.SplashTexture.Width / 2, cheeseProjectile.SplashTexture.Height / 2),
                    1.0f,
                    SpriteEffects.None,
                    0f
                );
            }
            
            pinkDonutButton.Draw(_spriteBatch);
            
            if (gameOverScreen.IsActive)
            {
                gameOverScreen.Draw(_spriteBatch);
            }

        }

        
        private void UpdateProjectile(GameTime gameTime, float deltaTime)
        {
            if (projectileCooldownTimer > 0)
            {
                projectileCooldownTimer -= deltaTime;
            }
            
            float distanceToDonut = Vector2.Distance(gingerSprite.Position, donut.Position);
            if (distanceToDonut < 200f && projectileCooldownTimer <= 0 && !cheeseProjectile.IsActive)
            {
                Vector2 launchPosition = gingerSprite.Position + new Vector2(0, -40);
                Vector2 directionToDonut = donut.Position - launchPosition;
                cheeseProjectile.Launch(launchPosition, directionToDonut);
                
                projectileCooldownTimer = ProjectileCooldown;
            }
            
            if (cheeseProjectile.IsActive)
            {
                cheeseProjectile.Update(gameTime);
                
                Vector2 projectilePos = cheeseProjectile.Position;
                if (projectilePos.X < -100 || projectilePos.Y < -100 || 
                    projectilePos.X > _graphicsDevice.Viewport.Width + 100 || 
                    projectilePos.Y > _graphicsDevice.Viewport.Height + 100)
                {
                    cheeseProjectile.Reset();
                }
                else
                {
                    Rectangle cheeseRect = new Rectangle(
                        (int)cheeseProjectile.Position.X - 32,
                        (int)cheeseProjectile.Position.Y - 32,
                        64,
                        64
                    );

                    Rectangle donutRect = new Rectangle(
                        (int)donut.Position.X - 48,
                        (int)donut.Position.Y - 64,
                        96,
                        128
                    );

                    if (cheeseRect.Intersects(donutRect) && !cheeseProjectile.HasDealtDamage)
                    {
                        showSplashEffect = true;
                        splashPosition = donut.Position;
                        splashTimer = 0f;
                        
                        cheeseProjectile.DealDamageTo(donut);
                        
                        cheeseProjectile.Reset();
                    }
                }
            }
            
            if (showSplashEffect)
            {
                splashTimer += deltaTime;
                if (splashTimer >= SPLASH_DURATION)
                {
                    showSplashEffect = false;
                }
            }
        }
        
        private void HandleMouseAttacks()
        {
            var mouseState = Mouse.GetState();
            
            if (mouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton != ButtonState.Pressed)
            {
                float donutSushiDistance = Vector2.Distance(donut.Position, sushiSprite.Position);
                if (donutSushiDistance < 70) 
                {
                    sushiSprite.TakeDamage(20f);
                    
                    if (sushiSprite.Health <= 0 && gingerSprite.Health <= 0)
                    {
                        _mainGame.SwitchGameState(MainGame.GameStateType.Game1);
                        return;
                    }
                }
                
                float donutGingerDistance = Vector2.Distance(donut.Position, gingerSprite.Position);
                if (donutGingerDistance < 70)
                {
                    gingerSprite.TakeDamage(20f);
                    
                    if (sushiSprite.Health <= 0 && gingerSprite.Health <= 0)
                    {
                        _mainGame.SwitchGameState(MainGame.GameStateType.Game1);
                        return;
                    }
                }
            }
        }

        public static class Texture2DHelper
        {
            public static Texture2D CreateRectangle(GraphicsDevice graphicsDevice, int width, int height, Color color)
            {
                Texture2D texture = new Texture2D(graphicsDevice, width, height);
                Color[] colorData = new Color[width * height];
                for (int i = 0; i < colorData.Length; i++)
                {
                    colorData[i] = color;
                }
                texture.SetData(colorData);
                return texture;
            }
        }

    }
}