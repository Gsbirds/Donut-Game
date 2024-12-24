using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace monogame
{

    public class Game2 : IGameState
    {
        Texture2D charaset;
        Texture2D nacho;
        Texture2D sushi;
        Vector2 ballPosition;
        Texture2D cheeseLaunch;
        Texture2D nachoMouth;
        Vector2 nachoPosition;

        Vector2 cheesePosition;

        Texture2D sombreroWallpaper;
        Texture2D sushiWallpaper;
        float ballSpeed;
        float nachoSpeed = 40f;
        float nachoRotation = 0f;
        bool rotatingRight = true;
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        float health;
        float timer;
        int threshold;
        private SpriteFont font;

        Rectangle[] downRectangles;
        Rectangle[] upRectangles;
        Rectangle[] leftRectangles;
        Rectangle[] rightRectangles;

        Vector2 lastDonutPosition;

        bool cheeseVisible = true;

        byte currentAnimationIndex;


        enum Direction { Down, Up, Left, Right }
        Direction currentDirection;

        bool hasCheeseDealtDamage = false;
        float cheeseRotation = 0f;

        int animationCycleCount = 0;
        bool useBlinkingFrame = false;

        bool useSpacebarFrame = false;

        bool useOpenMouthFrame = false;

        private bool isSpacebarAnimationActive = false;
        private float spacebarAnimationTimer = 0f;
        private float spacebarFirstFrameDuration = 0.5f;
        private float spacebarSecondFrameDuration = 0.8f;
        private int doubleWidth = 192;
        float nachoHealth;
        private bool nachoDamagedThisCycle = false;
        private KeyboardState previousKeyboardState;

        float cheeseVisibilityTimer = 0f;

        Texture2D splashCheese;
        private Texture2D mainmenu;
        bool showSplashCheese = false;
        float splashCheeseTimer = 0f;
        const float splashCheeseDuration = 1f;
        Vector2 splashPosition;
        Vector2 sushiPosition;

        private Direction currentDirectionNacho2 = Direction.Down;
        private float nachoDirectionDelayTimer = 0f;
        private const float NachoDirectionDelayDuration = 2f;

        private bool usePostHitFrame = false;
        private float postHitAnimationTimer = 0f;
        private const float postHitAnimationDuration = 0.5f;

        private bool nachoDefeated = false;
        private float nachoDefeatedTimer = 0f;
        private const float nachoDefeatedDuration = 3f;

        private GraphicsDevice _graphicsDevice;
        private MainGame _mainGame;



        public Game2(MainGame mainGame, SpriteBatch spriteBatch)
        {
            _mainGame = mainGame;
            _spriteBatch = spriteBatch;
            _graphicsDevice = mainGame.GraphicsDevice;
        }

        public void LoadContent()
        {
            charaset = _mainGame.Content.Load<Texture2D>("donutsprites17");
            nacho = _mainGame.Content.Load<Texture2D>("nachosprites4");
            sushi = _mainGame.Content.Load<Texture2D>("sushisprites10");
            font = _mainGame.Content.Load<SpriteFont>("DefaultFont1");
            cheeseLaunch = _mainGame.Content.Load<Texture2D>("cheeselaunch");
            nachoMouth = _mainGame.Content.Load<Texture2D>("openmountnacho2");
            sombreroWallpaper = _mainGame.Content.Load<Texture2D>("sombrerosetting");
            sushiWallpaper = _mainGame.Content.Load<Texture2D>("japaneselevel2");
            splashCheese = _mainGame.Content.Load<Texture2D>("splashcheese");

            ballPosition = new Vector2(300, 300);
            nachoPosition = new Vector2(100, 100);
            sushiPosition = new Vector2(100, 100);

            ballSpeed = 100f;

            downRectangles = new Rectangle[3]
            {
                new Rectangle(0, 256, 96, 128),
                new Rectangle(96, 256, 96, 128),
                new Rectangle(192, 256, 96, 128)
            };

            upRectangles = new Rectangle[3]
            {
                new Rectangle(0, 0, 96, 128),
                new Rectangle(96, 0, 96, 128),
                new Rectangle(192, 0, 96, 128)
            };

            rightRectangles = new Rectangle[3]
            {
                new Rectangle(0, 128, 96, 128),
                new Rectangle(96, 128, 96, 128),
                new Rectangle(192, 128, 96, 128)
            };

            leftRectangles = new Rectangle[3]
            {
                new Rectangle(0, 384, 96, 128),
                new Rectangle(96, 384, 96, 128),
                new Rectangle(192, 384, 96, 128)
            };

            currentAnimationIndex = 1;
            currentDirection = Direction.Down; // Initialize currentDirection
            nachoHealth = 4;
        }

        private void CheeseLauncher(GameTime gameTime)
        {
            float distanceToBall = Vector2.Distance(nachoPosition, ballPosition);
            if (distanceToBall <= 150)
            {
                if (!cheeseVisible)
                {
                    cheesePosition = GetNachoMouthPosition();
                    cheeseVisible = true;
                }

                Vector2 directionToBall = ballPosition - cheesePosition;
                directionToBall.Normalize();
                cheeseRotation = (float)Math.Atan2(directionToBall.Y, directionToBall.X);
                cheesePosition += directionToBall * nachoSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

                Rectangle cheeseRect = new Rectangle((int)cheesePosition.X - 10, (int)cheesePosition.Y - 10, 20, 20);
                Rectangle ballRect = new Rectangle((int)ballPosition.X - 48, (int)ballPosition.Y - 64, 96, 128);

                if (cheeseRect.Intersects(ballRect))
                {
                    showSplashCheese = true;
                    splashPosition = ballPosition;
                    nachoHealth -= 0.5f;
                    cheeseVisible = false;
                }
            }
        }

        private Vector2 GetNachoMouthPosition()
        {
            Rectangle currentRect = downRectangles[currentAnimationIndex];
            return nachoPosition + new Vector2(currentRect.Width / 2, -70);
        }

        private void KeyboardTracker(GameTime gameTime)
        {
            KeyboardState kstate = Keyboard.GetState();
            Vector2 movement = Vector2.Zero;

            if (kstate.IsKeyDown(Keys.Up))
            {
                movement.Y -= 1;
                currentDirection = Direction.Up; // Update direction
            }
            if (kstate.IsKeyDown(Keys.Down))
            {
                movement.Y += 1;
                currentDirection = Direction.Down; // Update direction
            }
            if (kstate.IsKeyDown(Keys.Left))
            {
                movement.X -= 1;
                currentDirection = Direction.Left; // Update direction
            }
            if (kstate.IsKeyDown(Keys.Right))
            {
                movement.X += 1;
                currentDirection = Direction.Right; // Update direction
            }

            if (movement != Vector2.Zero)
            {
                movement.Normalize();
                ballPosition += movement * ballSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
        }

        public void Update(GameTime gameTime)
        {
            if (nachoDefeated)
            {
                nachoDefeatedTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (nachoDefeatedTimer >= nachoDefeatedDuration)
                {
                    // Handle game transition here
                }
                return;
            }

            KeyboardTracker(gameTime);
            CheeseLauncher(gameTime);

            if (nachoHealth <= 0)
            {
                nachoDefeated = true;
                nachoDefeatedTimer = 0f;
            }
        }

        public void Draw(GameTime gameTime)
        {
            _graphicsDevice.Clear(Color.Black);

            _spriteBatch.Draw(sushiWallpaper, new Rectangle(0, 0, 800, 600), Color.White);

            if (showSplashCheese)
            {
                _spriteBatch.Draw(splashCheese, splashPosition, null, Color.White);
            }

            _spriteBatch.Draw(charaset, ballPosition, GetCurrentRectangles()[currentAnimationIndex], Color.White);

            _spriteBatch.Draw(
           sushi,
           sushiPosition,
           GetCurrentRectangleSushi()[currentAnimationIndex],
           Color.White,
           0f,
           new Vector2(70, 66), // Center of the frame
           1.0f,
           SpriteEffects.None,
           0f
       );

        }



        private Rectangle[] GetCurrentRectangles()
        {
            Rectangle[] baseRectangles = currentDirection switch
            {
                Direction.Up => new Rectangle[]
                {
            isSpacebarAnimationActive
                ? (useSpacebarFrame
                    ? new Rectangle(384, 0, 96, 128)  // Fifth column
                    : new Rectangle(480, 0, doubleWidth, 128))  // Sixth column
                : new Rectangle(0, 0, 96, 128),
            new Rectangle(96, 0, 96, 128),
            new Rectangle(192, 0, 96, 128)
                },
                Direction.Down => new Rectangle[]
                {
            isSpacebarAnimationActive
                ? (useSpacebarFrame
                    ? new Rectangle(384, 256, 96, 128) // Fifth column
                    : new Rectangle(480, 256, doubleWidth, 128)) // Sixth column
                : new Rectangle(0, 256, 96, 128),
            new Rectangle(96, 256, 96, 128),
            new Rectangle(192, 256, 96, 128)
                },
                Direction.Left => new Rectangle[]
                {
            isSpacebarAnimationActive
                ? (useSpacebarFrame
                    ? new Rectangle(384, 384, 96, 128) // Fifth column
                    : new Rectangle(480, 384, doubleWidth, 128)) // Sixth column
                : new Rectangle(0, 384, 96, 128),
            new Rectangle(96, 384, 96, 128),
            new Rectangle(192, 384, 96, 128)
                },
                Direction.Right => new Rectangle[]
                {
            isSpacebarAnimationActive
                ? (useSpacebarFrame
                    ? new Rectangle(384, 128, 96, 128) // Fifth column
                    : new Rectangle(480, 128, doubleWidth, 128)) // Sixth column
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
                baseRectangles[2] = new Rectangle(288, baseRectangles[2].Y, 96, 128); // Blinking frame
            }

            return baseRectangles;
        }
        private Rectangle[] GetCurrentRectangleSushi()
        {
            int frameWidth = 110;
            int frameHeight = 133;
            int doubleWidth = frameWidth * 2;

            Rectangle[] baseRectangles = currentDirection switch
            {
                Direction.Up => new Rectangle[]
                {
            isSpacebarAnimationActive
                ? (useSpacebarFrame
                    ? new Rectangle(frameWidth * 4, 0, frameWidth, frameHeight)  // Fifth column
                    : new Rectangle(frameWidth * 5, 0, doubleWidth, frameHeight))  // Sixth column
                : new Rectangle(0, 0, frameWidth, frameHeight),
            new Rectangle(frameWidth, 0, frameWidth, frameHeight),
            new Rectangle(frameWidth * 2, 0, frameWidth, frameHeight)
                },
                Direction.Down => new Rectangle[]
                {
            isSpacebarAnimationActive
                ? (useSpacebarFrame
                    ? new Rectangle(frameWidth * 4, frameHeight * 2, frameWidth, frameHeight) // Fifth column
                    : new Rectangle(frameWidth * 5, frameHeight * 2, doubleWidth, frameHeight)) // Sixth column
                : new Rectangle(0, frameHeight * 2, frameWidth, frameHeight),
            new Rectangle(frameWidth, frameHeight * 2, frameWidth, frameHeight),
            new Rectangle(frameWidth * 2, frameHeight * 2, frameWidth, frameHeight)
                },
                Direction.Left => new Rectangle[]
                {
            isSpacebarAnimationActive
                ? (useSpacebarFrame
                    ? new Rectangle(frameWidth * 4, frameHeight * 3, frameWidth, frameHeight) // Fifth column
                    : new Rectangle(frameWidth * 5, frameHeight * 3, doubleWidth, frameHeight)) // Sixth column
                : new Rectangle(0, frameHeight * 3, frameWidth, frameHeight),
            new Rectangle(frameWidth, frameHeight * 3, frameWidth, frameHeight),
            new Rectangle(frameWidth * 2, frameHeight * 3, frameWidth, frameHeight)
                },
                Direction.Right => new Rectangle[]
                {
            isSpacebarAnimationActive
                ? (useSpacebarFrame
                    ? new Rectangle(frameWidth * 4, frameHeight, frameWidth, frameHeight) // Fifth column
                    : new Rectangle(frameWidth * 5, frameHeight, doubleWidth, frameHeight)) // Sixth column
                : new Rectangle(0, frameHeight, frameWidth, frameHeight),
            new Rectangle(frameWidth, frameHeight, frameWidth, frameHeight),
            new Rectangle(frameWidth * 2, frameHeight, frameWidth, frameHeight)
                },
                _ => new Rectangle[]
                {
            new Rectangle(0, frameHeight, frameWidth, frameHeight),
            new Rectangle(frameWidth, frameHeight, frameWidth, frameHeight),
            new Rectangle(frameWidth * 2, frameHeight, frameWidth, frameHeight)
                },
            };

            if (useBlinkingFrame && !isSpacebarAnimationActive)
            {
                baseRectangles[2] = new Rectangle(frameWidth * 3, baseRectangles[2].Y, frameWidth, frameHeight); // Blinking frame
            }

            return baseRectangles;
        }

    }
}
