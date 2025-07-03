using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
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
        private DonutHole donutHole;
        private Nacho nachoSprite;
        private Nacho nachoSprite2; 
        private Empanada empanadaSprite;
        private const float MinDistanceBetweenNachoAndEmpanada = 170f;
        private const float MinDistanceBetweenNachos = 100f; 
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

        private float nacho2HitFrameTimer = 0f; 
        private bool isNacho2HitActive = false; 

        private Projectile cheeseProjectile;
        private FruitProjectileManager fruitManager;
        private float projectileCooldownTimer = 0f;
        private Projectile cheeseProjectile2; 
        private float projectileCooldownTimer2 = 0f; 
        private const float ProjectileCooldown = 5f;

        private bool showSplashCheese = false;
        private float splashCheeseTimer = 0f;
        private const float splashCheeseDuration = 1f;
        private Vector2 splashPosition;
        
        private SoundEffect empanadaHitSound;

        private Texture2D charaset;
        private Texture2D donutHoleTexture;
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
        private Texture2D empanadaHit;
        private Texture2D strawberryTexture;
        private Texture2D blueberryTexture;
        private Texture2D bananaTexture;
        private const float SPLASH_DURATION = 1f;

        private GameOverScreen gameOverScreen;

        private void UpdateUIControls(MouseState currentMouseState, KeyboardState keyboardState)
        {
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
            
            if (keyboardState.IsKeyDown(Keys.P) && !previousKeyboardState.IsKeyDown(Keys.P))
            {
                isColorEffectActive = !isColorEffectActive;
            }
        }
        
        private void UpdatePlayer(GameTime gameTime)
        {
            donut.Update(gameTime);
            donutHole.Update(gameTime);
            
            donut.Position = new Vector2(
                Math.Clamp(donut.Position.X, 48, _graphicsDevice.Viewport.Width - 48),
                Math.Clamp(donut.Position.Y, 64, _graphicsDevice.Viewport.Height - 64)
            );
        }
        
        private void UpdateEnemies(float deltaTime, GameTime gameTime)
        {
            nachoSprite.Update(gameTime);
            nachoSprite2.Update(gameTime); 
            empanadaSprite.Update(gameTime);
            
            empanadaSprite.Update(deltaTime, donut.Position, nachoSprite.Position);
            
            nachoSprite.SetTargetPosition(donut.Position);
            nachoSprite2.SetTargetPosition(donut.Position); 
            empanadaSprite.SetTargetPosition(donut.Position);
            
            Vector2 donutPos = donut.Position;
            float distanceToDonut = Vector2.Distance(empanadaSprite.Position, donutPos);
            
            if (distanceToDonut <= AttackRange * 3.0f)
            {
                empanadaSprite.StartAttack();
            }
            
            Vector2 nachoToEmpanada = empanadaSprite.Position - nachoSprite.Position;
            if (nachoToEmpanada.Length() < MinDistanceBetweenNachoAndEmpanada && nachoToEmpanada.Length() > 0)
            {
                Vector2 separation = Vector2.Normalize(nachoToEmpanada) * MinDistanceBetweenNachoAndEmpanada;
                nachoSprite.Position = empanadaSprite.Position - separation;
            }

            Vector2 nacho2ToEmpanada = empanadaSprite.Position - nachoSprite2.Position;
            if (nacho2ToEmpanada.Length() < MinDistanceBetweenNachoAndEmpanada && nacho2ToEmpanada.Length() > 0)
            {
                Vector2 separation2 = Vector2.Normalize(nacho2ToEmpanada) * MinDistanceBetweenNachoAndEmpanada;
                nachoSprite2.Position = empanadaSprite.Position - separation2;
            }

            Vector2 nacho1ToNacho2 = nachoSprite2.Position - nachoSprite.Position;
            if (nacho1ToNacho2.Length() < MinDistanceBetweenNachos && nacho1ToNacho2.Length() > 0)
            {
                Vector2 separation3 = Vector2.Normalize(nacho1ToNacho2) * MinDistanceBetweenNachos;
                nachoSprite.Position = nachoSprite2.Position - separation3;
            }
        }
        
        private void UpdateNachoProjectile(float deltaTime, GameTime gameTime)
        {
            float distanceToDonutNacho1 = Vector2.Distance(nachoSprite.Position, donut.Position);
            if (projectileCooldownTimer > 0)
            {
                projectileCooldownTimer -= deltaTime;
            }

            if (distanceToDonutNacho1 < 200f && !nachoSprite.IsDefeated)
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

            float distanceToDonutNacho2 = Vector2.Distance(nachoSprite2.Position, donut.Position);
            if (projectileCooldownTimer2 > 0)
            {
                projectileCooldownTimer2 -= deltaTime;
            }

            if (distanceToDonutNacho2 < 200f && !nachoSprite2.IsDefeated)
            {
                nachoSprite2.SetOpenMouthFrame(true);
                if (projectileCooldownTimer2 <= 0 && !cheeseProjectile2.IsActive) 
                {
                    Vector2 launchPosition2 = nachoSprite2.Position + new Vector2(0, -40);
                    Vector2 directionToDonut2 = donut.Position - launchPosition2;
                    cheeseProjectile2.Launch(launchPosition2, directionToDonut2); 
                    projectileCooldownTimer2 = ProjectileCooldown; 
                }
            }
            else
            {
                nachoSprite2.SetOpenMouthFrame(false);
                if (cheeseProjectile2.IsActive && Vector2.Distance(cheeseProjectile2.Position, donut.Position) > 500f) 
                {
                    cheeseProjectile2.Reset(); 
                }
            }
        }
        
        private void UpdateAnimations(float deltaTime)
        {
            for (int i = 0; i < pipePositions.Length; i++)
            {
                pipeAnimationTimers[i] += deltaTime;
                if (pipeAnimationTimers[i] >= pipeFrameDuration)
                {
                    pipeAnimationTimers[i] = 0f;
                    currentPipeFrameIndices[i] = (currentPipeFrameIndices[i] + 1) % pipeSourceRectangles.Length;
                }
            }

            puprmushFrameTimer += deltaTime;
            if (puprmushFrameTimer >= PuprmushFrameDuration)
            {
                puprmushFrameTimer = 0f;
                currentPuprmushFrame = (currentPuprmushFrame + 1) % puprmushFrames.Length;
            }
            
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

            if (isNacho2HitActive)
            {
                nacho2HitFrameTimer -= deltaTime;
                if (nacho2HitFrameTimer <= 0)
                {
                    isNacho2HitActive = false;
                    nachoSprite2.SetPostHitFrame(false);
                }
            }
        }
        
        private void HandlePlayerAttack(MouseState currentMouseState)
        {
            bool mouseJustClicked = currentMouseState.LeftButton == ButtonState.Pressed && 
                                   previousMouseState.LeftButton != ButtonState.Pressed;
                                   
            if (!mouseJustClicked) return;
            
            float donutNachoDistance = Vector2.Distance(donut.Position, nachoSprite.Position);
            float donutEmpanadaDistance = Vector2.Distance(donut.Position, empanadaSprite.Position);
            
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
                if (empanadaHitSound != null) {
                    empanadaHitSound.Play();
                }
            }
            float donutNacho2Distance = Vector2.Distance(donut.Position, nachoSprite2.Position);
            if (donutNacho2Distance < 70 && nachoSprite2.Health > 0)
            {
                nachoSprite2.SetPostHitFrame(true);
                nachoSprite2.TakeDamage(20f);
                isNacho2HitActive = true;
                nacho2HitFrameTimer = NachoHitFrameDuration;
            }
        }
        
        private void CheckCollisions(MouseState currentMouseState, GameTime gameTime)
        {
            Sprite.ResolveCollision(donut, nachoSprite);
            Sprite.ResolveCollision(donut, nachoSprite2);
            Sprite.ResolveCollision(donut, empanadaSprite);
            Sprite.ResolveCollision(nachoSprite, empanadaSprite);
            Sprite.ResolveCollision(nachoSprite2, empanadaSprite);
            Sprite.ResolveCollision(nachoSprite, nachoSprite2);
            
            donutHole.CheckCollision(nachoSprite);
            donutHole.CheckCollision(nachoSprite2);
            donutHole.CheckCollision(empanadaSprite);
            
            Sprite[] enemies = { nachoSprite, nachoSprite2, empanadaSprite };
            fruitManager.Update(gameTime, donut.Position, donut.GetColor(), currentMouseState, previousMouseState);
            fruitManager.CheckCollisions(enemies, _graphicsDevice);
        }
        
        private void CheckCheeseProjectile(float deltaTime)
        {
            if (cheeseProjectile.IsActive)
            {
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
                        64, 64
                    );

                    Rectangle donutRect = new Rectangle(
                        (int)donut.Position.X - 48,
                        (int)donut.Position.Y - 64,
                        96, 128
                    );

                    if (cheeseRect.Intersects(donutRect) && !cheeseProjectile.HasDealtDamage)
                    {
                        showSplashEffect = true;
                        splashPosition = new Vector2(donut.Position.X - 60, donut.Position.Y - 55);
                        splashTimer = 0f;
                        
                        cheeseProjectile.DealDamageTo(donut);
                        cheeseProjectile.Reset();
                    }
                }
            }
            
            if (cheeseProjectile2.IsActive)
            {
                Vector2 projectilePos = cheeseProjectile2.Position;
                
                if (projectilePos.X < -100 || projectilePos.Y < -100 || 
                    projectilePos.X > _graphicsDevice.Viewport.Width + 100 || 
                    projectilePos.Y > _graphicsDevice.Viewport.Height + 100)
                {
                    cheeseProjectile2.Reset();
                }
                else
                {
                    Rectangle cheeseRect = new Rectangle(
                        (int)cheeseProjectile2.Position.X - 32,
                        (int)cheeseProjectile2.Position.Y - 32,
                        64, 64
                    );

                    Rectangle donutRect = new Rectangle(
                        (int)donut.Position.X - 48,
                        (int)donut.Position.Y - 64,
                        96, 128
                    );

                    if (cheeseRect.Intersects(donutRect) && !cheeseProjectile2.HasDealtDamage)
                    {
                        showSplashEffect = true;
                        splashPosition = new Vector2(donut.Position.X - 60, donut.Position.Y - 55);
                        splashTimer = 0f;
                        
                        cheeseProjectile2.DealDamageTo(donut);
                        cheeseProjectile2.Reset();
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
        
        private bool CheckLevelCompletion()
        {
            if (nachoSprite.Health <= 0 && nachoSprite2.Health <= 0 && empanadaSprite.Health <= 0)
            {
                if (!enemiesDefeated)
                {
                    enemiesDefeated = true;
                    axeSprite.Show();
                }
                
                if (!axePickedUp && !axeSprite.IsVisible)
                {
                    axeSprite.Show();
                }
            }
            
            if (enemiesDefeated && axeSprite.IsPlayingPickupAnimation)
            {
                if (axeSprite.IsPickedUp)
                {
                    axePickedUp = true;
                    _mainGame.SwitchGameState(MainGame.GameStateType.Game2);
                    return true;
                }
                return true;
            }
            
            if (enemiesDefeated && !axePickedUp && axeSprite.IsPickedUp)
            {
                axePickedUp = true;
                _mainGame.SwitchGameState(MainGame.GameStateType.Game2);
                return true;
            }
            
            if (enemiesDefeated && axePickedUp)
            {
                _mainGame.SwitchGameState(MainGame.GameStateType.Game2);
                return true;
            }
            
            return false;
        }

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
            donutHoleTexture = _mainGame.Content.Load<Texture2D>("Donuthole");
            axeTexture = _mainGame.Content.Load<Texture2D>("Axe");
            nacho = _mainGame.Content.Load<Texture2D>("nachosprites4");
            font = _mainGame.Content.Load<SpriteFont>("DefaultFont1");
            cheeseLaunch = _mainGame.Content.Load<Texture2D>("cheeselaunch");
            var nachoOpenMouthTexture = _mainGame.Content.Load<Texture2D>("openmountnacho2");
            sombreroWallpaper = _mainGame.Content.Load<Texture2D>("sombrerosetting");
            splashCheese = _mainGame.Content.Load<Texture2D>("splashcheese");
            empanada = _mainGame.Content.Load<Texture2D>("empanadasprites11");
            empanadaHit = _mainGame.Content.Load<Texture2D>("Empanadahit");
            
            try {
                empanadaHitSound = _mainGame.Content.Load<SoundEffect>("fu_oof");
                SoundEffect donutHoleDokenSound = _mainGame.Content.Load<SoundEffect>("donutholedoken");
                DonutHole.SetAttackSound(donutHoleDokenSound);
            } catch (System.Exception e) {
                System.Console.WriteLine("Failed to load sound effects: " + e.Message);
            }
            
            sombrero = _mainGame.Content.Load<Texture2D>("Sombrero");
            background = _mainGame.Content.Load<Texture2D>("Levelone");
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
            
            donut = new Donut(charaset, new Vector2(_graphicsDevice.Viewport.Width - 96, _graphicsDevice.Viewport.Height - 128), 240f);

            donutHole = new DonutHole(donutHoleTexture, donut, new Vector2(-55, -55), 240f);
            
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
            int screenWidth = _graphicsDevice.Viewport.Width;
            int screenHeight = _graphicsDevice.Viewport.Height;
            
            nachoSprite = new Nacho(nacho, nachoOpenMouthTexture, 
                new Vector2(50, screenHeight / 2 - 100), 120f);
            nachoSprite2 = new Nacho(nacho, nachoOpenMouthTexture, 
                new Vector2(screenWidth - 100, screenHeight / 2 + 50), 120f);
                
            empanadaSprite = new Empanada(empanada, empanadaHit, 
                new Vector2(150, screenHeight - 100), 90f);
            cheeseProjectile = new Projectile(cheeseLaunch, new Vector2(-100, -100), 900f);
            cheeseProjectile.Reset();

            cheeseProjectile2 = new Projectile(cheeseLaunch, new Vector2(-100, -100), 900f);
            cheeseProjectile2.Reset();
            
            fruitManager = new FruitProjectileManager(_mainGame.Content);
            
            donutHole.SetTargets(nachoSprite, empanadaSprite);
            
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
            KeyboardState keyboardState = Keyboard.GetState();
            
            UpdateUIControls(currentMouseState, keyboardState);
            
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
            UpdatePlayer(gameTime);
            UpdateEnemies(deltaTime, gameTime);
            
            HandlePlayerAttack(currentMouseState);
            
            CheckCollisions(currentMouseState, gameTime);
            
            UpdateAnimations(deltaTime);
            
            UpdateNachoProjectile(deltaTime, gameTime);
            
            cheeseProjectile.Update(gameTime);
            cheeseProjectile2.Update(gameTime);
            
            CheckCheeseProjectile(deltaTime);
            
            if (CheckLevelCompletion())
            {
                return;
            }
            
            previousMouseState = currentMouseState;
            previousKeyboardState = keyboardState;
        }

        public void Draw(GameTime gameTime)
        {
            if (gameOverScreen.IsActive)
            {
                gameOverScreen.Draw(_spriteBatch);
                return;
            }
            
            int backgroundWidth = background.Width;
            int backgroundHeight = background.Height;
            int screenWidth = _graphicsDevice.Viewport.Width;
            int screenHeight = _graphicsDevice.Viewport.Height;
            
            int x = (screenWidth - backgroundWidth) / 2;
            int y = (screenHeight - backgroundHeight) / 2;
            
            _spriteBatch.Draw(
                background,
                new Rectangle(x, y, backgroundWidth, backgroundHeight), 
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

            if (nachoSprite.Health <= 0)
            {
                Texture2D texture = nachoSprite.Texture;
                Vector2 position = nachoSprite.Position;
                Rectangle currentFrame;
                
                if (nachoSprite.FacingDirection == monogame.Animation.Direction.Up)
                    currentFrame = new Rectangle(0, 0, 110, 133);
                else if (nachoSprite.FacingDirection == monogame.Animation.Direction.Right)
                    currentFrame = new Rectangle(0, 133, 110, 133);
                else if (nachoSprite.FacingDirection == monogame.Animation.Direction.Down)
                    currentFrame = new Rectangle(0, 266, 110, 133);
                else
                    currentFrame = new Rectangle(0, 399, 110, 133);
                
                Vector2 origin = new Vector2(currentFrame.Width / 2, currentFrame.Height / 2);
                
                _spriteBatch.Draw(
                    texture,
                    position,
                    currentFrame,
                    Color.Gray * nachoSprite.FadeAlpha,
                    MathHelper.PiOver2,
                    origin,
                    1.0f,
                    SpriteEffects.None,
                    0f
                );
            }
            else
            {
                nachoSprite.Draw(_spriteBatch);
            }
            if (nachoSprite2.Health <= 0)
            {
                Texture2D texture2 = nachoSprite2.Texture;
                Vector2 position2 = nachoSprite2.Position;
                Rectangle currentFrame2;
                
                if (nachoSprite2.FacingDirection == monogame.Animation.Direction.Up)
                    currentFrame2 = new Rectangle(0, 0, 110, 133);
                else if (nachoSprite2.FacingDirection == monogame.Animation.Direction.Right)
                    currentFrame2 = new Rectangle(0, 133, 110, 133);
                else if (nachoSprite2.FacingDirection == monogame.Animation.Direction.Down)
                    currentFrame2 = new Rectangle(0, 266, 110, 133);
                else
                    currentFrame2 = new Rectangle(0, 399, 110, 133);
                
                Vector2 origin2 = new Vector2(currentFrame2.Width / 2, currentFrame2.Height / 2);
                
                _spriteBatch.Draw(
                    texture2,
                    position2,
                    currentFrame2,
                    Color.Gray * nachoSprite2.FadeAlpha,
                    MathHelper.PiOver2,
                    origin2,
                    1.0f,
                    SpriteEffects.None,
                    0f
                );
            }
            else
            {
                nachoSprite2.Draw(_spriteBatch);
            }

            if (empanadaSprite.Health <= 0)
            {
                Texture2D texture = empanadaSprite.Texture;
                Vector2 position = empanadaSprite.Position;
                Rectangle currentFrame;
                
                if (empanadaSprite.FacingDirection == monogame.Animation.Direction.Up)
                    currentFrame = new Rectangle(0, 0, 110, 133);
                else if (empanadaSprite.FacingDirection == monogame.Animation.Direction.Right)
                    currentFrame = new Rectangle(0, 133, 110, 133);
                else if (empanadaSprite.FacingDirection == monogame.Animation.Direction.Down)
                    currentFrame = new Rectangle(0, 266, 110, 133);
                else // Left
                    currentFrame = new Rectangle(0, 399, 110, 133);
                
                Vector2 origin = new Vector2(currentFrame.Width / 2, currentFrame.Height / 2);
                
                _spriteBatch.Draw(
                    texture,
                    position,
                    currentFrame,
                    Color.Gray * empanadaSprite.FadeAlpha,
                    MathHelper.PiOver2,
                    origin,
                    1.0f,
                    SpriteEffects.None,
                    0f
                );
            }
            else
            {
                empanadaSprite.Draw(_spriteBatch, empanadaSprite.IsAttacking);
            }
            
            axeSprite.Draw(_spriteBatch);
            
            donut.DrawWithColorReplacement(_spriteBatch);
            donutHole.Draw(_spriteBatch);

            if (cheeseProjectile.IsActive)
            {
                cheeseProjectile.Draw(_spriteBatch);
            }

            if (cheeseProjectile2.IsActive)
            {
                cheeseProjectile2.Draw(_spriteBatch);
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