using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using monogame.Sprites;
using monogame.Screens;
using monogame.Animation;

namespace monogame
{

    public class Game2 : IGameState
    {
        private Donut donut;
        private Sushi sushiSprite;
        private Ginger gingerSprite;
        private CheeseProjectile cheeseProjectile;
        
        private Texture2D donutTexture;
        private Texture2D sushiTexture;
        private Texture2D gingerTexture;
        private Texture2D sushiWallpaper;
        private Texture2D mochiTree;
        private Texture2D puprmushSpritesheet;
        private SpriteFont font;
        
        private float projectileCooldownTimer = 0f;
        private const float ProjectileCooldown = 2.0f;
        private float minDistanceBetweenSushiAndGinger = 100f;
        
        private bool showSplashEffect = false;
        private Vector2 splashPosition;
        private float splashTimer = 0f;
        private const float SPLASH_DURATION = 0.5f;
        private MouseState previousMouseState;

        private Direction currentDirection = Direction.Down;
        private KeyboardState previousKeyboardState;
        private SpriteBatch _spriteBatch;
        private GraphicsDevice _graphicsDevice;
        private MainGame _mainGame;
        
        private Rectangle[] puprmushFrames;
        private int currentPuprmushFrame;
        private float puprmushFrameTimer;
        private const float PuprmushFrameDuration = 0.2f;
        
        private GameOverScreen gameOverScreen;
        
        private bool isSpacebarAnimationActive = false;
        private bool useSpacebarFrame = false;
        private bool useBlinkingFrame = false;
        private int doubleWidth = 96 * 2;
        
        private Direction currentDirectionSushi = Direction.Down;
        private bool isSushiAttacking = false;
        private bool useSushiAttackFrame = false;

        public Game2(MainGame mainGame, SpriteBatch spriteBatch)
        {
            _mainGame = mainGame;
            _spriteBatch = spriteBatch;
            _graphicsDevice = mainGame.GraphicsDevice;
            
            previousMouseState = Mouse.GetState();
            currentDirection = Direction.Down;
        }

        public void LoadContent()
        {
            donutTexture = _mainGame.Content.Load<Texture2D>("donutsprites20");
            sushiTexture = _mainGame.Content.Load<Texture2D>("sushisprites10");
            gingerTexture = _mainGame.Content.Load<Texture2D>("gingersprites4");
            sushiWallpaper = _mainGame.Content.Load<Texture2D>("sushilevelsetting");
            mochiTree = _mainGame.Content.Load<Texture2D>("mochitree");
            puprmushSpritesheet = _mainGame.Content.Load<Texture2D>("pinkmush");
            
            font = _mainGame.Content.Load<SpriteFont>("DefaultFont1");
            
            donut = new Donut(donutTexture, new Vector2(_graphicsDevice.Viewport.Width - 96, _graphicsDevice.Viewport.Height - 128), 100f);
            sushiSprite = new Sushi(sushiTexture, new Vector2(200, 200), 60f);
            gingerSprite = new Ginger(gingerTexture, new Vector2(100, 100), 40f);
            
            Texture2D cheeseTexture = _mainGame.Content.Load<Texture2D>("cheeselaunch");
            Texture2D cheeseSplashTexture = _mainGame.Content.Load<Texture2D>("splashcheese");
            cheeseProjectile = new CheeseProjectile(cheeseTexture, cheeseSplashTexture);
            
            sushiSprite.OnDamageDealt += (damage) => {
                donut.TakeDamage(3f);  
            };
            
            int frameWidth = puprmushSpritesheet.Width / 5;
            int frameHeight = puprmushSpritesheet.Height;
            puprmushFrames = new Rectangle[5];
            for (int i = 0; i < 5; i++)
            {
                puprmushFrames[i] = new Rectangle(i * frameWidth, 0, frameWidth, frameHeight);
            }
            currentPuprmushFrame = 0;
            puprmushFrameTimer = 0f;
            
            gameOverScreen = new GameOverScreen(_mainGame, _graphicsDevice, font);
        }

        public void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            if (gameOverScreen.IsActive)
            {
                gameOverScreen.Update(deltaTime);
                return;
            }
            
            donut.Update(gameTime);
            if (donut.Health <= 0)
            {
                gameOverScreen.Activate();
                return;
            }
            
            sushiSprite.SetTargetPosition(donut.Position);
            sushiSprite.Update(gameTime);
            
            gingerSprite.SetTargetPosition(donut.Position);
            gingerSprite.Update(gameTime);

            if (sushiSprite.Health <= 0 && gingerSprite.Health <= 0)
            {
                _mainGame.SwitchGameState(MainGame.GameStateType.Game1);
                return;
            }

            donut.Update(gameTime);
            
            Vector2 donutPos = donut.Position;
            
            sushiSprite.SetTargetPosition(donutPos);
            gingerSprite.SetTargetPosition(donutPos);
            sushiSprite.Update(gameTime);
            gingerSprite.Update(gameTime);
            
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
            
            previousMouseState = Mouse.GetState();
        }

        public void Draw(GameTime gameTime)
        {
            if (gameOverScreen.IsActive)
            {
                gameOverScreen.Draw(_spriteBatch);
                return;
            }
            
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

            donut.Draw(_spriteBatch);

            sushiSprite.Draw(_spriteBatch);
            gingerSprite.Draw(_spriteBatch);

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
                float scale = 1.0f - (splashTimer / SPLASH_DURATION);
                _spriteBatch.Draw(
                    cheeseProjectile.SplashTexture,
                    splashPosition,
                    null,
                    Color.White * (1.0f - (splashTimer / SPLASH_DURATION)),
                    0f,
                    new Vector2(cheeseProjectile.SplashTexture.Width / 2, cheeseProjectile.SplashTexture.Height / 2),
                    scale,
                    SpriteEffects.None,
                    0
                );
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


        private Rectangle[] GetCurrentRectangles()
        {
            Rectangle[] baseRectangles = currentDirection switch
            {
                Direction.Up => new Rectangle[]
                {
            isSpacebarAnimationActive
                ? (useSpacebarFrame
                    ? new Rectangle( 80, 0, doubleWidth, 128)
                    : new Rectangle(384, 0, 96, 128))
                : new Rectangle(0, 0, 96, 128),
            new Rectangle(96, 0, 96, 128),
            new Rectangle(192, 0, 96, 128)
                },
                Direction.Down => new Rectangle[]
                {
            isSpacebarAnimationActive
                ? (useSpacebarFrame
                    ? new Rectangle(480, 256, doubleWidth, 128)
                    : new Rectangle(384, 256, 96, 128))
                : new Rectangle(0, 256, 96, 128),
            new Rectangle(96, 256, 96, 128),
            new Rectangle(192, 256, 96, 128)
                },
                Direction.Left => new Rectangle[]
                {
            isSpacebarAnimationActive
                ? (useSpacebarFrame
                    ? new Rectangle(480, 384, doubleWidth, 128)
                    : new Rectangle(384, 384, 96, 128))
                : new Rectangle(0, 384, 96, 128),
            new Rectangle(96, 384, 96, 128),
            new Rectangle(192, 384, 96, 128)
                },
                Direction.Right => new Rectangle[]
                {
            isSpacebarAnimationActive
                ? (useSpacebarFrame
                    ? new Rectangle(480, 128, doubleWidth, 128)
                    : new Rectangle(384, 128, 96, 128))
                : new Rectangle(0, 128, 96, 128),
                new Rectangle(96, 128, 96, 128),
                new Rectangle(192, 128, 96, 128)
                },
                _ => new Rectangle[]
                {
            new Rectangle(0, 128, 96, 128),
            new Rectangle(96, 128, 96, 128),
            new Rectangle(192, 128, 96, 128)
                },
            };

            if (useBlinkingFrame && !isSpacebarAnimationActive)
            {
                baseRectangles[2] = new Rectangle(288, baseRectangles[2].Y, 96, 128);
            }

            return baseRectangles;
        }

    }
}