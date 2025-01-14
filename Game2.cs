using System;
using System.Transactions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace monogame
{

    public class Game2 : IGameState
    {
        Texture2D charaset;
        Texture2D sushi;
        Vector2 ballPosition = new Vector2(300, 300);
        Texture2D cheeseLaunch;
        Vector2 nachoPosition;
        Vector2 cheesePosition;
        Texture2D sushiWallpaper;
        float ballSpeed;
        float nachoSpeed = 30f;
        private SpriteBatch _spriteBatch;
        float health;
        float timer;
        int threshold;
        private SpriteFont font;

        Rectangle[] downRectangles;
        Rectangle[] upRectangles;
        Rectangle[] leftRectangles;
        Rectangle[] rightRectangles;

        bool cheeseVisible = true;

        byte currentAnimationIndex;


        enum Direction { Down, Up, Left, Right }
        Direction currentDirection;

        bool hasCheeseDealtDamage = false;
        int animationCycleCount = 0;
        bool useBlinkingFrame = false;

        bool useSpacebarFrame = false;

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
        private Texture2D ginger;
        bool showSplashCheese = false;
        float splashCheeseTimer = 0f;
        const float splashCheeseDuration = 1f;
        Vector2 splashPosition;
        Vector2 sushiPosition;
        private float nachoDirectionDelayTimer = 0f;
        private const float NachoDirectionDelayDuration = 2f;

        private bool usePostHitFrame = false;
        private float postHitAnimationTimer = 0f;
        private const float postHitAnimationDuration = 0.5f;
        private bool nachoDefeated = false;
        private Direction currentDirectionSushi = Direction.Down;
        private float sushiDirectionDelayTimer = 0f;
        private const float SushiDirectionDelayDuration = 1f;
        private GraphicsDevice _graphicsDevice;
        private MainGame _mainGame;
        private bool sushiMoving;
        private Vector2 gingerPosition;
        private int gingerAnimationIndex;
        private Direction currentDirectionGinger;
        private float gingerAnimationTimer;
        private float sushiAttackCooldown = 1.5f;
        private float sushiAttackTimer = 0f;
        private bool isSushiAttacking = false;
        private bool useSushiAttackFrame = false;
        private byte currentAnimationIndexSushi;
        private bool sushiStopped;
        private float donutTimer = 0f;
        private float sushiTimer = 0f;

        public Game2(MainGame mainGame, SpriteBatch spriteBatch)
        {
            _mainGame = mainGame;
            _spriteBatch = spriteBatch;
            _graphicsDevice = mainGame.GraphicsDevice;

            ballPosition = new Vector2(300, 300);
            nachoPosition = new Vector2(150, 150);
            sushiPosition = new Vector2(200, 200);
            ballSpeed = 100f;
            nachoSpeed = 80f;
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
            currentDirection = Direction.Down;
            nachoHealth = 4;
        }


        public void LoadContent()
        {

            charaset = _mainGame.Content.Load<Texture2D>("donutsprites17");
            sushi = _mainGame.Content.Load<Texture2D>("sushisprites10");
            font = _mainGame.Content.Load<SpriteFont>("DefaultFont1");
            cheeseLaunch = _mainGame.Content.Load<Texture2D>("cheeselaunch");
            sushiWallpaper = _mainGame.Content.Load<Texture2D>("japaneselevel4");
            splashCheese = _mainGame.Content.Load<Texture2D>("splashcheese");
            ginger = _mainGame.Content.Load<Texture2D>("gingersprites5");
            threshold = 150;

        }

        private Vector2 GetNachoMouthPosition()
        {
            Rectangle currentRect = downRectangles[currentAnimationIndex];
            return nachoPosition + new Vector2(currentRect.Width / 2, -70);
        }


        private void spacebarAttack(GameTime gameTime, KeyboardState currentKeyboardState)
        {
            Rectangle donutRect = new Rectangle(
                (int)ballPosition.X - 48,
                (int)ballPosition.Y - 64,
                96,
                128
            );

            Rectangle nachoRect = new Rectangle(
                (int)nachoPosition.X - 48,
                (int)nachoPosition.Y - 64,
                96,
                128
            );

            if (currentKeyboardState.IsKeyDown(Keys.Space) && !previousKeyboardState.IsKeyDown(Keys.Space))
            {
                if (!isSpacebarAnimationActive)
                {
                    isSpacebarAnimationActive = true;
                    spacebarAnimationTimer = 0f;
                    nachoDamagedThisCycle = false;
                }
            }

            if (isSpacebarAnimationActive)
            {
                spacebarAnimationTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (spacebarAnimationTimer <= spacebarFirstFrameDuration)
                {
                    useSpacebarFrame = true;
                }
                else if (spacebarAnimationTimer <= spacebarFirstFrameDuration + spacebarSecondFrameDuration)
                {
                    useSpacebarFrame = false;
                }
                else
                {
                    spacebarAnimationTimer = 0f;
                    isSpacebarAnimationActive = false;
                }
            }

            if (useSpacebarFrame && donutRect.Intersects(nachoRect) && !nachoDamagedThisCycle)
            {
                nachoHealth = Math.Max(0, nachoHealth - 1);
                nachoDamagedThisCycle = true;

                usePostHitFrame = true;
                postHitAnimationTimer = 0f;
            }
        }

        private bool keyboardTracker(float elapsedTime, GameTime gameTime)
        {

            bool isMoving = false;
            float updatedBallSpeed = ballSpeed * elapsedTime;

            var kstate = Keyboard.GetState();

            Vector2 movement = Vector2.Zero;

            if (kstate.IsKeyDown(Keys.Up))
            {
                movement.Y -= 1;
                currentDirection = Direction.Up;
                isMoving = true;
            }
            if (kstate.IsKeyDown(Keys.Down))
            {
                movement.Y += 1;
                currentDirection = Direction.Down;
                isMoving = true;
            }
            if (kstate.IsKeyDown(Keys.Left))
            {
                movement.X -= 1;
                currentDirection = Direction.Left;
                isMoving = true;
            }
            if (kstate.IsKeyDown(Keys.Right))
            {
                movement.X += 1;
                currentDirection = Direction.Right;
                isMoving = true;
            }

            if (movement != Vector2.Zero)
            {
                movement.Normalize();
                ballPosition += movement * updatedBallSpeed;
            }

            if ((currentDirection == Direction.Right && ballPosition.X > sushiPosition.X) ||
            (currentDirection == Direction.Left && ballPosition.X < sushiPosition.X) ||
            (currentDirection == Direction.Down && ballPosition.Y > sushiPosition.Y) ||
            (currentDirection == Direction.Up && ballPosition.Y < sushiPosition.Y))
            {
                nachoDirectionDelayTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (nachoDirectionDelayTimer >= NachoDirectionDelayDuration)
                {
                    nachoDirectionDelayTimer = 0f;
                }
            }

            return isMoving;
        }

        private void animationBlinker(bool isDonutMoving, GameTime gameTime)
        {
            if (isDonutMoving || isSpacebarAnimationActive)
            {
                if (donutTimer > threshold)
                {
                    currentAnimationIndex = (byte)((currentAnimationIndex + 1) % 3);

                    if (currentAnimationIndex == 0)
                    {
                        animationCycleCount++;

                        useBlinkingFrame = (animationCycleCount % 3 == 0);
                    }

                    donutTimer = 0f;
                }
                else
                {
                    donutTimer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                }
            }
            else
            {
                currentAnimationIndex = 1;
            }

            if (sushiMoving)
            {
                if (sushiTimer > threshold)
                {
                    currentAnimationIndexSushi = (byte)((currentAnimationIndexSushi + 1) % 3);

                    if (currentAnimationIndexSushi == 0)
                    {
                        animationCycleCount++;
                        useBlinkingFrame = animationCycleCount % 3 == 0;
                    }

                    sushiTimer = 0f;
                }
                else
                {
                    sushiTimer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                }
            }
            else
            {
                currentAnimationIndexSushi = 1;
            }
        }


        private void cheeseLauncher(float updatedNachoSpeed, GameTime gameTime)
        {
            int cheeseWidth = 20;
            int cheeseHeight = 20;

            float distanceToDonut = Vector2.Distance(nachoPosition, ballPosition);

            cheeseVisibilityTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (showSplashCheese)
            {
                splashCheeseTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (splashCheeseTimer >= splashCheeseDuration)
                {
                    showSplashCheese = false;
                    splashCheeseTimer = 0f;
                }
            }

            if (cheeseVisible && !showSplashCheese)
            {
                cheeseVisibilityTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (cheeseVisibilityTimer >= 4f)
                {
                    cheeseVisible = false;
                    cheeseVisibilityTimer = 0f;
                    cheesePosition = GetNachoMouthPosition();
                    hasCheeseDealtDamage = false;

                    if (cheeseVisible)
                    {
                        cheesePosition = GetNachoMouthPosition();
                        hasCheeseDealtDamage = false;
                    }
                    else
                    {
                        splashPosition = cheesePosition;
                    }
                    return;
                }
            }

            if (distanceToDonut <= 150)
            {
                if (!cheeseVisible && !showSplashCheese)
                {
                    cheesePosition = GetNachoMouthPosition();
                    cheeseVisible = true;
                }

                Vector2 directionToDonut = ballPosition - cheesePosition;
                if (directionToDonut != Vector2.Zero && directionToDonut.LengthSquared() > 1f)
                {
                    directionToDonut.Normalize();
                    cheesePosition += directionToDonut * updatedNachoSpeed * 2.5f;
                }


                Rectangle cheeseRect = new Rectangle(
                    (int)cheesePosition.X - cheeseWidth / 2,
                    (int)cheesePosition.Y - cheeseHeight / 2,
                    cheeseWidth,
                    cheeseHeight
                );

                Rectangle donutRect = new Rectangle(
                    (int)ballPosition.X - 48,
                    (int)ballPosition.Y - 64,
                    96,
                    128
                );

                if (cheeseRect.Intersects(donutRect) && !hasCheeseDealtDamage)
                {
                    showSplashCheese = true;
                    cheeseVisible = false;
                    splashPosition = ballPosition;
                    splashCheeseTimer = 0f;
                    health -= 0.5f;
                    hasCheeseDealtDamage = true;
                }
            }
            else
            {
                cheeseVisible = false;
                hasCheeseDealtDamage = false;
            }
        }

        const float MinDistanceBetweenGingerAndSushi = 210f;

        private void MaintainMinimumDistance(ref Vector2 movingPosition, Vector2 otherPosition, float adjustmentSpeed, float elapsedTime)
        {
            float distance = Vector2.Distance(movingPosition, otherPosition);
            if (distance < MinDistanceBetweenGingerAndSushi)
            {
                Vector2 directionAway = movingPosition - otherPosition;
                if (directionAway != Vector2.Zero)
                {
                    directionAway.Normalize();
                    movingPosition += directionAway * adjustmentSpeed * elapsedTime;
                }
            }
        }


        public void Update(GameTime gameTime)
        {

            if (nachoHealth <= 0)
            {
                nachoDefeated = true;
                return;
            }

            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            float updatedNachoSpeed = nachoSpeed * elapsedTime;
            KeyboardState currentKeyboardState = Keyboard.GetState();

            spacebarAttack(gameTime, currentKeyboardState);

            if (usePostHitFrame)
            {
                postHitAnimationTimer += elapsedTime;
                if (postHitAnimationTimer >= postHitAnimationDuration)
                {
                    usePostHitFrame = false;
                    postHitAnimationTimer = 0f;
                }
            }

            bool isMoving = keyboardTracker(elapsedTime, gameTime);

            cheeseLauncher(updatedNachoSpeed, gameTime);
            previousKeyboardState = currentKeyboardState;

            float sushiOffsetY = 60f;
            Vector2 adjustedSushiTarget = new Vector2(ballPosition.X, ballPosition.Y + sushiOffsetY);

            float updatedSushiSpeed = nachoSpeed * elapsedTime;
            Vector2 directionToDonutFromSushi = adjustedSushiTarget - sushiPosition;

            if (directionToDonutFromSushi != Vector2.Zero)
            {
                sushiMoving = true;
                directionToDonutFromSushi.Normalize();
                sushiPosition += directionToDonutFromSushi * updatedSushiSpeed;
            }
            else
            {
                sushiMoving = false;
            }

            animationBlinker(isMoving, gameTime);

            CheckSushiAttack(elapsedTime);

            sushiDirectionDelayTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if ((currentDirection == Direction.Right && ballPosition.X > sushiPosition.X) ||
                (currentDirection == Direction.Left && ballPosition.X < sushiPosition.X) ||
                (currentDirection == Direction.Down && ballPosition.Y > sushiPosition.Y) ||
                (currentDirection == Direction.Up && ballPosition.Y < sushiPosition.Y))
            {
                nachoDirectionDelayTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (sushiDirectionDelayTimer >= SushiDirectionDelayDuration)
                {
                    currentDirectionSushi = currentDirection;
                    sushiDirectionDelayTimer = 0f;
                }
            }

            gingerUpdate(elapsedTime);

            MaintainMinimumDistance(ref sushiPosition, gingerPosition, updatedSushiSpeed, elapsedTime);
            MaintainMinimumDistance(ref gingerPosition, sushiPosition, nachoSpeed, elapsedTime);
        }


        private void CheckSushiAttack(float elapsedTime)
        {
            float attackRange = 50f;
            float distanceToDonut = Vector2.Distance(sushiPosition, ballPosition);

            if (isSushiAttacking)
            {
                sushiAttackTimer += elapsedTime;
                if (sushiAttackTimer >= sushiAttackCooldown)
                {
                    isSushiAttacking = false;
                    sushiAttackTimer = 0f;
                }
            }

            if (distanceToDonut <= attackRange && !isSushiAttacking)
            {
                isSushiAttacking = true;
                useSushiAttackFrame = true;
                sushiAttackTimer = 0f;
                health -= 0.5f;
            }
        }


        public void gingerUpdate(float elapsedTime)
        {
            const float GingerFrameDuration = 0.2f;
            gingerAnimationTimer += elapsedTime;
            if (gingerAnimationTimer >= GingerFrameDuration)
            {
                gingerAnimationIndex = (gingerAnimationIndex + 1) % 2;
                gingerAnimationTimer = 0f;
            }

            Vector2 directionToDonutFromGinger = ballPosition - gingerPosition;
            if (directionToDonutFromGinger.LengthSquared() > 1f)
            {
                directionToDonutFromGinger.Normalize();
                float updatedGingerSpeed = nachoSpeed * elapsedTime;
                gingerPosition += directionToDonutFromGinger * updatedGingerSpeed;

                if (Math.Abs(directionToDonutFromGinger.X) > Math.Abs(directionToDonutFromGinger.Y))
                {
                    currentDirectionGinger = directionToDonutFromGinger.X > 0 ? Direction.Right : Direction.Left;
                }
                else
                {
                    currentDirectionGinger = directionToDonutFromGinger.Y > 0 ? Direction.Down : Direction.Up;
                }
            }
        }



        public void Draw(GameTime gameTime)
        {

            if (nachoDefeated)
            {
                _graphicsDevice.Clear(Color.Black);
                string defeatMessage = "Sushi Defeated";
                Vector2 textSize = font.MeasureString(defeatMessage);
                Vector2 textPosition = new Vector2(
                    300,
                    300
                );
                _spriteBatch.DrawString(font, defeatMessage, textPosition, Color.White);
                return;
            }
            _graphicsDevice.Clear(Color.Black);

            _spriteBatch.Draw(sushiWallpaper, new Rectangle(0, 0, 800, 600), Color.White);

            if (showSplashCheese)
            {
                _spriteBatch.Draw(splashCheese, splashPosition, null, Color.White);
            }

            _spriteBatch.Draw(
            ginger,
            gingerPosition,
            GetGingerRectangle(currentDirectionGinger, gingerAnimationIndex),
            Color.White
            );


            _spriteBatch.Draw(charaset, ballPosition, GetCurrentRectangles()[currentAnimationIndex], Color.White);

            _spriteBatch.Draw(
           sushi,
           sushiPosition,
           GetCurrentRectangleSushi()[currentAnimationIndexSushi],
           Color.White,
           0f,
           new Vector2(70, 66),
           1.0f,
           SpriteEffects.None,
           0f
       );
            float maxNachoHealth = 4f;
            int nachoHealthBarWidth = 200;
            int nachoHealthBarHeight = 20;
            Vector2 nachoHealthBarPosition = new Vector2(10, 10);

            if (nachoHealthBarWidth != 0)
            {
                _spriteBatch.Draw(
                    Texture2DHelper.CreateRectangle(_graphicsDevice, nachoHealthBarWidth, nachoHealthBarHeight, Color.Gray),
                    new Rectangle((int)nachoHealthBarPosition.X, (int)nachoHealthBarPosition.Y, nachoHealthBarWidth, nachoHealthBarHeight),
                    Color.Gray
                );
            }

            int nachoHealthCurrentWidth = (int)((nachoHealth / maxNachoHealth) * nachoHealthBarWidth);
            if (nachoHealthCurrentWidth != 0)
            {
                _spriteBatch.Draw(
                    Texture2DHelper.CreateRectangle(_graphicsDevice, nachoHealthCurrentWidth, nachoHealthBarHeight, Color.WhiteSmoke),
                    new Rectangle((int)nachoHealthBarPosition.X, (int)nachoHealthBarPosition.Y, nachoHealthCurrentWidth, nachoHealthBarHeight),
                    Color.WhiteSmoke
                );
            }

            string donutHealthText = $"Health: {health}";
            Vector2 donutHealthPosition = new Vector2(650, 10);
            _spriteBatch.DrawString(font, donutHealthText, donutHealthPosition, Color.White);
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


        private Rectangle GetGingerRectangle(Direction direction, int animationIndex)
        {
            int frameWidth = 98;
            int frameHeight = 85;

            int row = direction switch
            {
                Direction.Up => 0,
                Direction.Right => 1,
                Direction.Down => 2,
                Direction.Left => 3,
                _ => 2
            };

            int column = animationIndex % 2;

            return new Rectangle(column * frameWidth, row * frameHeight, frameWidth, frameHeight);
        }


        private Rectangle[] GetCurrentRectangleSushi()
        {
            int frameWidth = 110;
            int frameHeight = 133;
            int doubleWidth = frameWidth * 2;

            Rectangle[] baseRectangles = currentDirectionSushi switch
            {
                Direction.Up => new Rectangle[]
                {
            isSushiAttacking
                ? (useSushiAttackFrame
                    ? new Rectangle(frameWidth * 4, 0, frameWidth, frameHeight)
                    : new Rectangle(frameWidth * 5, 0, doubleWidth, frameHeight))
                : new Rectangle(0, 0, frameWidth, frameHeight),
            new Rectangle(frameWidth, 0, frameWidth, frameHeight),
            new Rectangle(frameWidth * 2, 0, frameWidth, frameHeight)
                },
                Direction.Down => new Rectangle[]
                {
            isSushiAttacking
                ? (useSushiAttackFrame
                    ? new Rectangle(frameWidth * 4, frameHeight * 2, frameWidth, frameHeight)
                    : new Rectangle(frameWidth * 5, frameHeight * 2, doubleWidth, frameHeight))
                : new Rectangle(0, frameHeight * 2, frameWidth, frameHeight),
            new Rectangle(frameWidth, frameHeight * 2, frameWidth, frameHeight),
            new Rectangle(frameWidth * 2, frameHeight * 2, frameWidth, frameHeight)
                },
                Direction.Left => new Rectangle[]
                {
            isSushiAttacking
                ? (useSushiAttackFrame
                    ? new Rectangle(frameWidth * 4, frameHeight * 3, frameWidth, frameHeight)
                    : new Rectangle(frameWidth * 5, frameHeight * 3, doubleWidth, frameHeight))
                : new Rectangle(0, frameHeight * 3, frameWidth, frameHeight),
            new Rectangle(frameWidth, frameHeight * 3, frameWidth, frameHeight),
            new Rectangle(frameWidth * 2, frameHeight * 3, frameWidth, frameHeight)
                },
                Direction.Right => new Rectangle[]
                {
            isSushiAttacking
                ? (useSushiAttackFrame
                    ? new Rectangle(frameWidth * 4, frameHeight, frameWidth, frameHeight)
                    : new Rectangle(frameWidth * 5, frameHeight, doubleWidth, frameHeight))
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

            if (useBlinkingFrame && !isSushiAttacking)
            {
                baseRectangles[2] = new Rectangle(frameWidth * 3, baseRectangles[2].Y, frameWidth, frameHeight);
            }

            return baseRectangles;
        }
    }
}