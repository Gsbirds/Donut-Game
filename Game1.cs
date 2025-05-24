using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using monogame.Sprites;
using monogame.Screens;
using monogame.UI;
using monogame.Effects;

namespace monogame
{
    public class Game1 : IGameState
    {
        public float GetFruitCooldownPercentage()
        {
            return fruitManager?.GetCooldownPercentage() ?? 1.0f;
        }
        
        public void UpdateDonutColor(DonutColor newColor, int colorIndex)
        {
            if (donut != null)
            {
                donut.SetColor(newColor);
            }
            
            if (pinkDonutButton != null)
            {
                pinkDonutButton.SetColorIndex(colorIndex);
            }
        }
        
        public void ResetFruitCooldown()
        {
            if (fruitManager != null)
            {
                fruitManager.ResetCooldown();
            }
        }
        #region Fields

        private SpriteBatch _spriteBatch;
        private GraphicsDevice _graphicsDevice;
        private MainGame _mainGame;
        private SpriteFont font;

        private Donut donut;
        private Nacho nachoSprite;
        private Empanada empanadaSprite;
        private const float MinDistanceBetweenNachoAndEmpanada = 170f;
        private const float AttackRange = 50f;
        private Rectangle[] empanadaFrames;
        private Rectangle[] downRectangles;
        private Rectangle[] upRectangles;
        private Rectangle[] leftRectangles;
        private Rectangle[] rightRectangles;
        private int doubleWidth = 192;

        private float nachoHitFrameTimer = 0f;
        private const float NachoHitFrameDuration = .65f;
        private bool isNachoHitActive = false;

        private Projectile cheeseProjectile;
        private FruitProjectileManager fruitManager;
        private float projectileCooldownTimer = 0f;
        private const float ProjectileCooldown = 5f;

        private bool showSplashCheese = false;
        private float splashCheeseTimer = 0f;
        private const float splashCheeseDuration = 1f;
        private Vector2 splashPosition;

        private Texture2D charaset;
        private Texture2D cheeseLaunch;
        private Texture2D sombreroWallpaper;
        private Texture2D splashCheese;
        private Texture2D sombrero;
        private Texture2D background;
        private Texture2D weed;
        private Texture2D pipe;
        private Texture2D mushroom;
        private Texture2D churroTree;
        public static Texture2D WhitePixel;

        private Texture2D pipeTexture;
        private Vector2[] pipePositions;
        private int[] currentPipeFrameIndices;
        private float[] pipeAnimationTimers;
        private float pipeFrameDuration = 0.5f;
        private Rectangle[] pipeSourceRectangles;
        private Texture2D puprmushSpritesheet;
        private Rectangle[] puprmushFrames;
        private int currentPuprmushFrame;
        
        private bool isColorEffectActive = false;
        private KeyboardState previousKeyboardState;
        private Button pinkDonutButton;
        private Texture2D buttonTexture;
        
        private bool enemiesDefeated = false;
        private bool axePickedUp = false;
        private Texture2D axeTexture;
        private Axe axeSprite;

        private float puprmushFrameTimer;
        private const float PuprmushFrameDuration = 0.4f;
        private Vector2 sombreroPosition = new Vector2(300, 300);

        #endregion

        private bool showSplashEffect;
        private float splashTimer;
        private MouseState previousMouseState;
    
        private Texture2D nacho;
        private Texture2D empanada;
        private Texture2D strawberryTexture;
        private Texture2D blueberryTexture;
        private Texture2D bananaTexture;
        private const float SPLASH_DURATION = 1f;

        private GameOverScreen gameOverScreen;

        public Game1(MainGame mainGame, SpriteBatch spriteBatch)
        {
            _mainGame = mainGame;
            _spriteBatch = spriteBatch;
            _graphicsDevice = mainGame.GraphicsDevice;
        }

        public void LoadContent()
        {
            previousMouseState = Mouse.GetState();
            
            charaset = _mainGame.Content.Load<Texture2D>("donutspritesnew");
            axeTexture = _mainGame.Content.Load<Texture2D>("Axe");
            nacho = _mainGame.Content.Load<Texture2D>("nachosprites4");
            font = _mainGame.Content.Load<SpriteFont>("DefaultFont1");
            cheeseLaunch = _mainGame.Content.Load<Texture2D>("cheeselaunch");
            var nachoOpenMouthTexture = _mainGame.Content.Load<Texture2D>("openmountnacho2");
            sombreroWallpaper = _mainGame.Content.Load<Texture2D>("sombrerosetting");
            splashCheese = _mainGame.Content.Load<Texture2D>("splashcheese");
            empanada = _mainGame.Content.Load<Texture2D>("empanadasprites11");
            sombrero = _mainGame.Content.Load<Texture2D>("Sombrero");
            background = _mainGame.Content.Load<Texture2D>("pink_purp_background");
            weed = _mainGame.Content.Load<Texture2D>("weed");
            churroTree = _mainGame.Content.Load<Texture2D>("churroTree");
            pipeTexture = _mainGame.Content.Load<Texture2D>("pipe2");
            puprmushSpritesheet = _mainGame.Content.Load<Texture2D>("Puprmush");

            WhitePixel = new Texture2D(_graphicsDevice, 1, 1);
            WhitePixel.SetData(new[] { Color.White });

            buttonTexture = new Texture2D(_graphicsDevice, 1, 1);
            Color[] colorData = new Color[1];
            colorData[0] = Color.White;
            buttonTexture.SetData(colorData);
            
            donut = new Donut(charaset, new Vector2(_graphicsDevice.Viewport.Width - 96, _graphicsDevice.Viewport.Height - 128), 160f);
            
            axeSprite = new Axe(axeTexture, new Vector2(_graphicsDevice.Viewport.Width / 2, _graphicsDevice.Viewport.Height / 2));
            
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
            nachoSprite = new Nacho(nacho, nachoOpenMouthTexture, new Vector2(100, 100), 80f);
            empanadaSprite = new Empanada(empanada, new Vector2(200, 200), 60f);
            cheeseProjectile = new Projectile(cheeseLaunch, new Vector2(-100, -100), 600f);
            cheeseProjectile.Reset();
            
            fruitManager = new FruitProjectileManager(_mainGame.Content);
            
            empanadaSprite.OnDamageDealt += (damage) => {
                donut.TakeDamage(10f);
            };

            pipeSourceRectangles = new Rectangle[3];
            int pipeWidth = pipeTexture.Width / 3;
            int pipeHeight = pipeTexture.Height;
            for (int i = 0; i < 3; i++)
            {
                pipeSourceRectangles[i] = new Rectangle(i * pipeWidth, 0, pipeWidth, pipeHeight);
            }
            
            pipePositions = new Vector2[2];
            
            Vector2 centerPosition = new Vector2(
                _graphicsDevice.Viewport.Width / 2 + 20,
                _graphicsDevice.Viewport.Height / 2 - 70
            );
            
            int distance = 105;
            int y = (int)centerPosition.Y + 50;
            
            pipePositions[0] = new Vector2(centerPosition.X - distance, y);
            pipePositions[1] = new Vector2(centerPosition.X + distance, y);
            
            currentPipeFrameIndices = new int[2];
            pipeAnimationTimers = new float[2];

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
            
            MouseState currentMouseState = Mouse.GetState();
            pinkDonutButton.Update(currentMouseState);
            pinkDonutButton.SetCooldownPercentage(fruitManager.GetCooldownPercentage());
            
            if (pinkDonutButton.IsClicked)
            {
                if (_mainGame.IsColorEffectActive)
                {
                    pinkDonutButton.CycleToNextColor();
                    donut.SetColor(pinkDonutButton.GetCurrentColor());
                    
                    _mainGame.CurrentDonutColor = pinkDonutButton.GetCurrentColor();
                    _mainGame.ColorButtonIndex = (int)pinkDonutButton.GetCurrentColorIndex();
                }
                else
                {
                    _mainGame.IsColorEffectActive = true;
                    donut.SetColor(pinkDonutButton.GetCurrentColor());
                    
                    _mainGame.CurrentDonutColor = pinkDonutButton.GetCurrentColor();
                }
            }
            
            KeyboardState keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.P) && !previousKeyboardState.IsKeyDown(Keys.P))
            {
                isColorEffectActive = !isColorEffectActive;
            }
            previousKeyboardState = keyboardState;
            
            if (gameOverScreen.IsActive)
            {
                gameOverScreen.Update(deltaTime);
                return;
            }
            else if (donut.Health <= 0)
            {
                gameOverScreen.Activate();
                return;
            }

            axeSprite.Update(gameTime);
            
            donut.Update(gameTime);
            nachoSprite.Update(gameTime);
            empanadaSprite.Update(deltaTime, donut.Position, nachoSprite.Position);
            
            Sprite.ResolveCollision(donut, nachoSprite);
            Sprite.ResolveCollision(donut, empanadaSprite);
            Sprite.ResolveCollision(nachoSprite, empanadaSprite);

            Vector2 donutPos = donut.Position;
            float distanceToDonut = Vector2.Distance(empanadaSprite.Position, donutPos);

            if (distanceToDonut <= AttackRange * 3.0f)
            {
                empanadaSprite.StartAttack();
            }

            Vector2 nachoToEmpanada = empanadaSprite.Position - nachoSprite.Position;
            if (nachoToEmpanada.Length() < MinDistanceBetweenNachoAndEmpanada)
            {
                Vector2 separation = Vector2.Normalize(nachoToEmpanada) * MinDistanceBetweenNachoAndEmpanada;
                nachoSprite.Position = empanadaSprite.Position - separation;
            }

            for (int i = 0; i < pipePositions.Length; i++)
            {
                pipeAnimationTimers[i] += deltaTime;
                if (pipeAnimationTimers[i] >= pipeFrameDuration)
                {
                    pipeAnimationTimers[i] = 0f;
                    currentPipeFrameIndices[i] = (currentPipeFrameIndices[i] + 1) % 3;
                }
            }

            puprmushFrameTimer += deltaTime;
            if (puprmushFrameTimer >= PuprmushFrameDuration)
            {
                puprmushFrameTimer = 0f;
                currentPuprmushFrame = (currentPuprmushFrame + 1) % 5;
            }

            donut.Position = new Vector2(
                Math.Clamp(donut.Position.X, 48, _graphicsDevice.Viewport.Width - 48),
                Math.Clamp(donut.Position.Y, 64, _graphicsDevice.Viewport.Height - 64)
            );

            nachoSprite.SetTargetPosition(donut.Position);
            empanadaSprite.SetTargetPosition(donut.Position);
            nachoSprite.Update(gameTime);
            empanadaSprite.Update(gameTime);

            float distanceToDonutNacho = Vector2.Distance(nachoSprite.Position, donut.Position);

            if (projectileCooldownTimer > 0)
            {
                projectileCooldownTimer -= deltaTime;
            }

            var mouse = Mouse.GetState();
            if (distanceToDonutNacho < 200f && !nachoSprite.IsDefeated)
            {
                nachoSprite.SetOpenMouthFrame(true);
                
                if (projectileCooldownTimer <= 0 && !cheeseProjectile.IsActive)
                {
                    Vector2 launchPosition = nachoSprite.Position + new Vector2(0, -40);
                    Vector2 directionToDonut = donut.Position - launchPosition;
                    cheeseProjectile.Launch(launchPosition, directionToDonut);
                    
                    projectileCooldownTimer = ProjectileCooldown;
                }
            }
            else
            {
                nachoSprite.SetOpenMouthFrame(false);
                
                if (cheeseProjectile.IsActive && Vector2.Distance(cheeseProjectile.Position, donut.Position) > 500f)
                {
                    cheeseProjectile.Reset();
                }
            }

            var mouseState = Mouse.GetState();
            bool mouseJustClicked = mouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton != ButtonState.Pressed;
            bool mouseJustRightClicked = mouseState.RightButton == ButtonState.Pressed && previousMouseState.RightButton != ButtonState.Pressed;
            
            fruitManager.Update(gameTime, donut.Position, donut.GetColor(), mouseState, previousMouseState);
            
            Sprite[] enemies = { nachoSprite, empanadaSprite };
            fruitManager.CheckCollisions(enemies, _graphicsDevice);
            
            float donutNachoDistance = Vector2.Distance(donut.Position, nachoSprite.Position);
            float donutEmpanadaDistance = Vector2.Distance(donut.Position, empanadaSprite.Position);

            if (isNachoHitActive)
            {
                nachoHitFrameTimer -= deltaTime;
                if (nachoHitFrameTimer <= 0)
                {
                    nachoHitFrameTimer = 0;
                    isNachoHitActive = false;
                    nachoSprite.SetPostHitFrame(false);
                }
            }
            
            if (mouseJustClicked)
            {   
                if (enemiesDefeated && !axePickedUp && axeSprite.CheckPickup(donut))
                {
                    _mainGame.HasPickedUpAxe = true;
                    donut.PickupAxe();
                }
                else if (donutNachoDistance < 70 && nachoSprite.Health > 0)
                {
                    nachoSprite.SetPostHitFrame(true);
                    nachoSprite.TakeDamage(20f);
                    isNachoHitActive = true;
                    nachoHitFrameTimer = NachoHitFrameDuration;
                }
                else if (donutEmpanadaDistance < 70 && empanadaSprite.Health > 0)
                {
                    empanadaSprite.TakeDamage(20f);
                }
            }
            
            if (!enemiesDefeated && nachoSprite.Health <= 0 && empanadaSprite.Health <= 0)
            {
                enemiesDefeated = true;
                axeSprite.Show();
            }
            
            if (enemiesDefeated && axeSprite.IsPlayingPickupAnimation)
            {
                return;
            }
            
            if (enemiesDefeated && !axePickedUp && axeSprite.IsPickedUp)
            {
                axePickedUp = true;
            }
            
            if (enemiesDefeated && axePickedUp)
            {
                _mainGame.SwitchGameState(MainGame.GameStateType.Game2);
                return;
            }
            
            previousMouseState = mouseState;

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

            puprmushFrameTimer += deltaTime;
            if (puprmushFrameTimer >= PuprmushFrameDuration)
            {
                puprmushFrameTimer = 0f;
                currentPuprmushFrame = (currentPuprmushFrame + 1) % puprmushFrames.Length;
            }

            for (int i = 0; i < pipePositions.Length; i++)
            {
                pipeAnimationTimers[i] += deltaTime;
                if (pipeAnimationTimers[i] >= pipeFrameDuration)
                {
                    pipeAnimationTimers[i] = 0f;
                    currentPipeFrameIndices[i] = (currentPipeFrameIndices[i] + 1) % pipeSourceRectangles.Length;
                }
            }
        }

        public void Draw(GameTime gameTime)
        {
            if (gameOverScreen.IsActive)
            {
                gameOverScreen.Draw(_spriteBatch);
                return;
            }
            
            _spriteBatch.Draw(
                background,
                new Rectangle(0, 0, 850, 850), 
                Color.White
            );

            _spriteBatch.Draw(
                churroTree,
                new Rectangle(500, 350, 400, 400), 
                Color.White
            );

            _spriteBatch.Draw(
                sombrero,
                sombreroPosition,
                null,
                Color.White,
                0f,
                new Vector2(350, -100),
                1.0f,
                SpriteEffects.None,
                0f
            );

            Vector2 puprmushPosition = new Vector2(
                _graphicsDevice.Viewport.Width / 2 + 20,
                _graphicsDevice.Viewport.Height / 2 - 70
            );

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

            for (int i = 0; i < pipePositions.Length; i++)
            {
                _spriteBatch.Draw(
                    pipeTexture,
                    pipePositions[i],
                    pipeSourceRectangles[currentPipeFrameIndices[i]],
                    Color.White,
                    0f,
                    new Vector2(pipeSourceRectangles[0].Width / 2, pipeSourceRectangles[0].Height / 2),
                    1.0f,
                    SpriteEffects.None,
                    0f
                );
            }

            nachoSprite.Draw(_spriteBatch);
            empanadaSprite.Draw(_spriteBatch);
            
            axeSprite.Draw(_spriteBatch);
            
            donut.DrawWithColorReplacement(_spriteBatch);

            if (cheeseProjectile.IsActive)
            {
                cheeseProjectile.Draw(_spriteBatch);
            }
            
            fruitManager.Draw(_spriteBatch);
            
            pinkDonutButton.Draw(_spriteBatch);

            if (showSplashEffect)
            {
                _spriteBatch.Draw(
                    splashCheese,
                    new Vector2(splashPosition.X - 32, splashPosition.Y - 32),
                    null,
                    Color.White
                );
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