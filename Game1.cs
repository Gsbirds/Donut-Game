using System;
using System.ComponentModel;
using System.IO.Pipes;
using System.Timers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace monogame
{

    public class Game1 : IGameState
    {
        Texture2D charaset;
        Texture2D nacho;
        Vector2 ballPosition;
        Texture2D cheeseLaunch;
        Texture2D nachoMouth;
        Vector2 nachoPosition;

        Vector2 cheesePosition;

        Texture2D sombreroWallpaper;
        float ballSpeed;
        float nachoSpeed = 40f;
        float nachoRotation = 0f;
        bool rotatingRight = true;
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
        bool showSplashCheese = false;
        float splashCheeseTimer = 0f;
        const float splashCheeseDuration = 1f;
        Vector2 splashPosition;

        private Direction currentDirectionNacho2 = Direction.Down;
        private float nachoDirectionDelayTimer = 0f;
        private const float NachoDirectionDelayDuration = 1f;

        private bool usePostHitFrame = false;
        private float postHitAnimationTimer = 0f;
        private const float postHitAnimationDuration = 0.5f;

        private bool nachoDefeated = false;
        private GraphicsDevice _graphicsDevice;
        private MainGame _mainGame;
        private Texture2D empanada;
        private Vector2 empanadaPosition = new Vector2(200, 200);
        private bool isEmpanadaAttacking = false;
        private float empanadaAttackCooldown = 1.5f;
        private float empanadaAttackTimer = 0f;
        private float empanadaSpeed = 60f;
        const float MinDistanceBetweenNachoAndEmpanada = 170f;

        private float donutTimer = 0f;
        private float empanadaTimer = 0f;
        private bool empanadaMoving;
        private byte currentAnimationIndexEmpanada;
        private Texture2D sombrero;
        private Texture2D background;
        private Texture2D weed;
        private Texture2D pipe;
        private Texture2D mushroom;
        private Texture2D churroTree;
        private Vector2 sombreroPosition = new Vector2(300, 300);
        private bool donutMovingToSombrero = false;
        private float donutJumpSpeed = 100f;
        private bool hasJumped = false;
        private float pipeAnimationTimer = 0f;
        private float pipeFrameDuration = 0.2f;
        private int currentPipeFrameIndex = 0;
        private Texture2D[] pipes;
        private Vector2[] pipePositions;
        private int[] currentPipeFrameIndices;
        private float[] pipeAnimationTimers;
        private Texture2D puprmushSpritesheet;
        private Rectangle[] puprmushFrames;
        private int currentPuprmushFrame;
        private float puprmushFrameTimer;
        private const float PuprmushFrameDuration = 0.2f;
        bool isJumping = false;
        float jumpTimer = 0f;
        float jumpDuration = 1.0f;
        float jumpHeight = 50f;
        float jumpStartY = 0f;
        private Direction nachoFacingDirection = Direction.Down;
        private Direction empanadaFacingDirection = Direction.Down;


        public Game1(MainGame mainGame, SpriteBatch spriteBatch)
        {
            _graphicsDevice = mainGame.GraphicsDevice;
            _spriteBatch = spriteBatch;
            mainGame.Content.RootDirectory = "Content";
            mainGame.IsMouseVisible = true;
            _mainGame = mainGame;
        }


        public void LoadContent()
        {
            charaset = _mainGame.Content.Load<Texture2D>("donutsprites20");
            nacho = _mainGame.Content.Load<Texture2D>("nachosprites4");
            font = _mainGame.Content.Load<SpriteFont>("DefaultFont1");
            cheeseLaunch = _mainGame.Content.Load<Texture2D>("cheeselaunch");
            nachoMouth = _mainGame.Content.Load<Texture2D>("openmountnacho2");
            sombreroWallpaper = _mainGame.Content.Load<Texture2D>("sombrerosetting");
            splashCheese = _mainGame.Content.Load<Texture2D>("splashcheese");
            empanada = _mainGame.Content.Load<Texture2D>("empanadasprites11");
            sombrero = _mainGame.Content.Load<Texture2D>("Sombrero");
            background = _mainGame.Content.Load<Texture2D>("pink_purp_background");
            weed = _mainGame.Content.Load<Texture2D>("weed");
            churroTree = _mainGame.Content.Load<Texture2D>("churroTree");
            pipe = _mainGame.Content.Load<Texture2D>("pipe2");
            pipes = new Texture2D[3];
            pipes[0] = pipe;
            pipes[1] = _mainGame.Content.Load<Texture2D>("pipe2");
            pipes[2] = _mainGame.Content.Load<Texture2D>("pipe2");

            pipePositions = new Vector2[]
            {
            new Vector2(670, 570),
            new Vector2(250, 300),
            new Vector2(465, 300),
            };

            currentPipeFrameIndices = new int[3];
            pipeAnimationTimers = new float[3];

            health = 4;

            timer = 0;
            threshold = 150;
            nachoPosition = new Vector2(100, 100);
            ballPosition = new Vector2(
            _graphicsDevice.Viewport.Width - 96,
            _graphicsDevice.Viewport.Height - 128
            );

            ballSpeed = 100f;
            lastDonutPosition = ballPosition;

            nachoHealth = 4;

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

            puprmushSpritesheet = _mainGame.Content.Load<Texture2D>("Puprmush");
            int frameWidth = puprmushSpritesheet.Width / 5;
            int frameHeight = puprmushSpritesheet.Height;

            puprmushFrames = new Rectangle[5];
            for (int i = 0; i < 5; i++)
            {
                puprmushFrames[i] = new Rectangle(i * frameWidth, 0, frameWidth, frameHeight);
            }
            currentPuprmushFrame = 0;
            puprmushFrameTimer = 0f;

        }

        private Vector2 GetNachoMouthPosition()
        {
            Rectangle currentRect = GetCurrentRectanglesNacho()[currentAnimationIndex];

            Vector2 mouthOffset = new Vector2(currentRect.Width / 2, (currentRect.Height / 2) - 70);

            float sin = (float)Math.Sin(nachoRotation);
            float cos = (float)Math.Cos(nachoRotation);

            Vector2 rotatedOffset = new Vector2(
                mouthOffset.X * cos - mouthOffset.Y * sin,
                mouthOffset.X * sin + mouthOffset.Y * cos
            );

            return nachoPosition + rotatedOffset;
        }

        private void CheckEmpanadaAttack(float elapsedTime)
        {
            float attackRange = 100f;
            float distanceToDonut = Vector2.Distance(empanadaPosition, ballPosition);

            if (isEmpanadaAttacking)
            {
                empanadaAttackTimer += elapsedTime;
                if (empanadaAttackTimer >= empanadaAttackCooldown)
                {
                    isEmpanadaAttacking = false;
                    empanadaAttackTimer = 0f;
                }
            }

            if (distanceToDonut <= attackRange && !isEmpanadaAttacking)
            {
                isEmpanadaAttacking = true;
                empanadaAttackTimer = 0f;
                health -= 0.5f;
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
                    cheeseRotation = (float)Math.Atan2(directionToDonut.Y, directionToDonut.X);
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

            Vector2 nextPosition = ballPosition + movement * updatedBallSpeed;


            Rectangle donutRect = new Rectangle(
                (int)nextPosition.X,
                (int)nextPosition.Y,
                96,
                128
            );


            if (movement != Vector2.Zero)
            {
                movement.Normalize();
                ballPosition += movement * updatedBallSpeed;

                float halfScreenHeight = _graphicsDevice.Viewport.Height / 2 - 100;
                if (ballPosition.Y < halfScreenHeight)
                {
                    ballPosition.Y = halfScreenHeight;
                }
            }

            nachoDirectionDelayTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (nachoDirectionDelayTimer >= NachoDirectionDelayDuration)
            {
                currentDirectionNacho2 = currentDirection;
                nachoDirectionDelayTimer = 0f;
            }

            return isMoving;
        }


        private void nachoRotater()
        {
            if (rotatingRight)
            {
                nachoRotation += 0.01f;

                if (nachoRotation >= 0.1f)
                {
                    rotatingRight = false;
                }
            }
            else
            {
                nachoRotation -= 0.01f;
                if (nachoRotation <= -0.1f)
                {
                    rotatingRight = true;
                }
            }
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

                        useBlinkingFrame = animationCycleCount % 3 == 0;
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

            if (empanadaMoving)
            {
                if (empanadaTimer > threshold)
                {
                    currentAnimationIndexEmpanada = (byte)((currentAnimationIndexEmpanada + 1) % 3);

                    if (currentAnimationIndexEmpanada == 0)
                    {
                        animationCycleCount++;
                        useBlinkingFrame = animationCycleCount % 3 == 0;
                    }

                    empanadaTimer = 0f;
                }
                else
                {
                    empanadaTimer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                }
            }
            else
            {
                currentAnimationIndexEmpanada = 1;
            }
        }

        private void LeftClickAttack(GameTime gameTime, MouseState currentMouseState)
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

            if (currentMouseState.LeftButton == ButtonState.Pressed)
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


        public void Update(GameTime gameTime)
        {

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))

                if (nachoDefeated)
                {
                    return;
                }

            if (nachoHealth <= 0)
            {
                nachoDefeated = true;
                _mainGame.SwitchGameState(MainGame.GameStateType.Game2);
                return;
            }


            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            float updatedNachoSpeed = nachoSpeed * elapsedTime;

            KeyboardState currentKeyboardState = Keyboard.GetState();
            MouseState currentMouseState = Mouse.GetState();

            for (int i = 0; i < pipes.Length; i++)
            {
                pipeAnimationTimers[i] += elapsedTime;
                if (pipeAnimationTimers[i] >= pipeFrameDuration)
                {
                    pipeAnimationTimers[i] -= pipeFrameDuration;
                    currentPipeFrameIndices[i] = (currentPipeFrameIndices[i] + 1) % 3;
                }
            }


            LeftClickAttack(gameTime, currentMouseState);

            if (usePostHitFrame)
            {
                postHitAnimationTimer += elapsedTime;
                if (postHitAnimationTimer >= postHitAnimationDuration)
                {
                    usePostHitFrame = false;
                    postHitAnimationTimer = 0f;
                }
            }

            Rectangle currentRect = GetCurrentRectangles()[currentAnimationIndex];
            // ballPosition.X = MathHelper.Clamp(ballPosition.X, currentRect.Width / 2, _graphics.PreferredBackBufferWidth - currentRect.Width / 2);
            // ballPosition.Y = MathHelper.Clamp(ballPosition.Y, currentRect.Height / 2, _graphics.PreferredBackBufferHeight - currentRect.Height / 2);

            Vector2 adjustedDonutPosition = new Vector2(ballPosition.X, ballPosition.Y + 60f);

            // --- Update Nacho ---
            Vector2 directionToDonut = adjustedDonutPosition - nachoPosition;
            if (directionToDonut != Vector2.Zero)
            {
                directionToDonut.Normalize();
                nachoPosition += directionToDonut * updatedNachoSpeed;

                if (Math.Abs(directionToDonut.X) > Math.Abs(directionToDonut.Y))
                    nachoFacingDirection = directionToDonut.X > 0 ? Direction.Right : Direction.Left;
                else
                    nachoFacingDirection = directionToDonut.Y > 0 ? Direction.Down : Direction.Up;
            }

            Vector2 directionToDonutFromEmpanada = adjustedDonutPosition - empanadaPosition;
            if (directionToDonutFromEmpanada != Vector2.Zero)
            {
                empanadaMoving = true;
                directionToDonutFromEmpanada.Normalize();
                empanadaPosition += directionToDonutFromEmpanada * empanadaSpeed * elapsedTime;

                if (Math.Abs(directionToDonutFromEmpanada.X) > Math.Abs(directionToDonutFromEmpanada.Y))
                    empanadaFacingDirection = directionToDonutFromEmpanada.X > 0 ? Direction.Right : Direction.Left;
                else
                    empanadaFacingDirection = directionToDonutFromEmpanada.Y > 0 ? Direction.Down : Direction.Up;
            }
            else
            {
                empanadaMoving = false;

            }



            MaintainMinimumDistance(ref nachoPosition, empanadaPosition, nachoSpeed, elapsedTime);
            MaintainMinimumDistance(ref empanadaPosition, nachoPosition, empanadaSpeed, elapsedTime);

            CheckEmpanadaAttack(elapsedTime);

            bool isMoving = keyboardTracker(elapsedTime, gameTime);

            nachoRotater();
            animationBlinker(isMoving, gameTime);
            cheeseLauncher(updatedNachoSpeed, gameTime);

            PurpleMushUpdate(gameTime);

            if (currentKeyboardState.IsKeyDown(Keys.Space) && !previousKeyboardState.IsKeyDown(Keys.Space) && !isJumping)
            {
                isJumping = true;
                jumpTimer = 0f;
                jumpStartY = ballPosition.Y;
            }

            if (isJumping)
            {
                jumpTimer += elapsedTime;
                float progress = jumpTimer / jumpDuration;
                float offset = jumpHeight * (float)Math.Sin(Math.PI * progress);
                ballPosition.Y = jumpStartY - offset;

                if (jumpTimer >= jumpDuration)
                {
                    ballPosition.Y = jumpStartY;
                    isJumping = false;
                    jumpTimer = 0f;
                }
            }
            previousKeyboardState = currentKeyboardState;

        }

        private void PurpleMushUpdate(GameTime gameTime)
        {
            puprmushFrameTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (puprmushFrameTimer >= PuprmushFrameDuration)
            {
                puprmushFrameTimer -= PuprmushFrameDuration;
                currentPuprmushFrame = (currentPuprmushFrame + 1) % puprmushFrames.Length;
            }
        }


        private void MaintainMinimumDistance(ref Vector2 movingPosition, Vector2 otherPosition, float adjustmentSpeed, float elapsedTime)
        {
            float distance = Vector2.Distance(movingPosition, otherPosition);
            if (distance < MinDistanceBetweenNachoAndEmpanada)
            {
                Vector2 directionAway = movingPosition - otherPosition;
                if (directionAway != Vector2.Zero)
                {
                    directionAway.Normalize();
                    movingPosition += directionAway * adjustmentSpeed * elapsedTime;
                }
            }
        }



        public void Draw(GameTime gameTime)
        {
            if (nachoDefeated)
            {
                _graphicsDevice.Clear(Color.Black);
                string defeatMessage = "Nacho Defeated";
                Vector2 textSize = font.MeasureString(defeatMessage);
                Vector2 textPosition = new Vector2(
                    100,
                    100
                );
                _spriteBatch.DrawString(font, defeatMessage, textPosition, Color.White);
                return;
            }

            _spriteBatch.Draw(
                background,
                new Rectangle(0, 0, 850, 850), Color.White
            );

            _spriteBatch.Draw(
                churroTree,
                new Rectangle(500, 350, 400, 400), Color.White
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

            for (int i = 0; i < pipes.Length; i++)
            {
                Rectangle[] pipeFrames = GetCurrentRectanglePipe();
                _spriteBatch.Draw(
                    pipes[i],
                    pipePositions[i],
                    pipeFrames[currentPipeFrameIndices[i]],
                    Color.White
                );
            }


            Rectangle[] empanadaFrames = GetCurrentRectangleEmpanada();
            int frameIndex = currentAnimationIndexEmpanada % empanadaFrames.Length;

            _spriteBatch.Draw(
                empanada,
                empanadaPosition,
                empanadaFrames[frameIndex],
                Color.White,
                0f,
                new Vector2(70, 66),
                1.0f,
                SpriteEffects.None,
                0f
            );

            _spriteBatch.Draw(
                nacho,
                nachoPosition,
                GetCurrentRectanglesNacho()[currentAnimationIndex],
                Color.White,
                nachoRotation,
                new Vector2(downRectangles[0].Width / 2, downRectangles[0].Height / 2),
                1.0f,
                SpriteEffects.None,
                0f
            );

            _spriteBatch.Draw(charaset, ballPosition, GetCurrentRectangles()[currentAnimationIndex], Color.White);

            if (Vector2.Distance(nachoPosition, ballPosition) <= 150)
            {
                useOpenMouthFrame = true;
            }
            else
            {
                useOpenMouthFrame = false;
            }

            if (showSplashCheese)
            {
                Vector2 splashOffset = new Vector2(30, 40);
                _spriteBatch.Draw(

                    splashCheese,
                    splashPosition + splashOffset,
                    null,
                    Color.White,
                    0f,
                    new Vector2(splashCheese.Width / 2, splashCheese.Height / 2),
                    1.0f,
                    SpriteEffects.None,
                    0f
                );
            }

            if (cheeseVisible && !showSplashCheese)
            {
                _spriteBatch.Draw(
                    cheeseLaunch,
                    cheesePosition,
                    null,
                    Color.White,
                    cheeseRotation,
                    new Vector2(cheeseLaunch.Width / 2, cheeseLaunch.Height / 2),
                    1.0f,
                    SpriteEffects.None,
                    0f
                );
            }

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
            Vector2 donutHealthPosition = new Vector2(530, 10);
            _spriteBatch.DrawString(font, donutHealthText, donutHealthPosition, Color.Black);

        }



        private Rectangle[] GetCurrentRectanglesNacho()
        {
            Rectangle[] baseRectangles = nachoFacingDirection switch
            {
                Direction.Up => new Rectangle[]
                {
            new Rectangle(0, 0, 96, 128),
            new Rectangle(0, 0, 96, 128),
            new Rectangle(0, 0, 96, 128)
                },
                Direction.Down => new Rectangle[]
                {
            usePostHitFrame ? new Rectangle(96, 512, 96, 128)
                            : (useOpenMouthFrame ? new Rectangle(96, 128, 96, 128)
                                                 : new Rectangle(96, 256, 96, 128)),
            usePostHitFrame ? new Rectangle(96, 512, 96, 128)
                            : (useOpenMouthFrame ? new Rectangle(96, 128, 96, 128)
                                                 : new Rectangle(96, 256, 96, 128)),
            usePostHitFrame ? new Rectangle(96, 512, 96, 128)
                            : (useOpenMouthFrame ? new Rectangle(96, 128, 96, 128)
                                                 : new Rectangle(96, 256, 96, 128))
                },
                Direction.Left => new Rectangle[]
                {
            usePostHitFrame ? new Rectangle(0, 512, 96, 128)
                            : (useOpenMouthFrame ? new Rectangle(192, 128, 96, 128)
                                                 : new Rectangle(192, 256, 96, 128)),
            usePostHitFrame ? new Rectangle(0, 512, 96, 128)
                            : (useOpenMouthFrame ? new Rectangle(192, 128, 96, 128)
                                                 : new Rectangle(192, 256, 96, 128)),
            usePostHitFrame ? new Rectangle(0, 512, 96, 128)
                            : (useOpenMouthFrame ? new Rectangle(192, 128, 96, 128)
                                                 : new Rectangle(192, 256, 96, 128))
                },
                Direction.Right => new Rectangle[]
                {
            usePostHitFrame ? new Rectangle(192, 512, 96, 128)
                            : (useOpenMouthFrame ? new Rectangle(0, 128, 96, 128)
                                                 : new Rectangle(0, 256, 96, 128)),
            usePostHitFrame ? new Rectangle(192, 512, 96, 128)
                            : (useOpenMouthFrame ? new Rectangle(0, 128, 96, 128)
                                                 : new Rectangle(0, 256, 96, 128)),
            usePostHitFrame ? new Rectangle(192, 512, 96, 128)
                            : (useOpenMouthFrame ? new Rectangle(0, 128, 96, 128)
                                                 : new Rectangle(0, 256, 96, 128))
                },
                _ => new Rectangle[]
                {
            new Rectangle(0, 384, 96, 128),
            new Rectangle(96, 384, 96, 128),
            new Rectangle(192, 384, 96, 128)
                },
            };

            // (Optional) Adjust for blinking if needed.
            if (useBlinkingFrame && !usePostHitFrame && nachoFacingDirection != Direction.Up)
            {
                baseRectangles[2] = new Rectangle(baseRectangles[2].X, 384, 96, 128);
            }

            return baseRectangles;
        }



        private Rectangle[] GetCurrentRectangleEmpanada()
        {
            int frameWidth = 110;
            int frameHeight = 133;

            if (isEmpanadaAttacking)
            {
                return empanadaFacingDirection switch
                {
                    Direction.Up => new Rectangle[]
                    {
                new Rectangle(frameWidth * 4, 0, frameWidth, frameHeight),
                new Rectangle(frameWidth * 5, 0, frameWidth * 2, frameHeight)
                    },
                    Direction.Down => new Rectangle[]
                    {
                new Rectangle(frameWidth * 4, frameHeight * 2, frameWidth, frameHeight),
                new Rectangle(frameWidth * 5, frameHeight * 2, frameWidth * 2, frameHeight)
                    },
                    Direction.Left => new Rectangle[]
                    {
                new Rectangle(475, frameHeight * 3, frameWidth * (3/2), frameHeight),
                new Rectangle(605, frameHeight * 3, frameWidth, frameHeight)
                    },
                    Direction.Right => new Rectangle[]
                    {
                new Rectangle(frameWidth * 4, frameHeight, frameWidth, frameHeight),
                new Rectangle(frameWidth * 5, frameHeight, frameWidth * 2, frameHeight)
                    },
                    _ => new Rectangle[]
                    {
                new Rectangle(frameWidth * 4, frameHeight, frameWidth, frameHeight),
                new Rectangle(frameWidth * 5, frameHeight, frameWidth * 2, frameHeight)
                    }
                };
            }
            else
            {
                return empanadaFacingDirection switch
                {
                    Direction.Up => new Rectangle[]
                    {
                new Rectangle(0, 0, frameWidth, frameHeight),
                new Rectangle(frameWidth, 0, frameWidth, frameHeight),
                new Rectangle(frameWidth * 2, 0, frameWidth, frameHeight)
                    },
                    Direction.Down => new Rectangle[]
                    {
                new Rectangle(0, frameHeight * 2, frameWidth, frameHeight),
                new Rectangle(frameWidth, frameHeight * 2, frameWidth, frameHeight),
                new Rectangle(frameWidth * 2, frameHeight * 2, frameWidth, frameHeight)
                    },
                    Direction.Left => new Rectangle[]
                    {
                new Rectangle(0, frameHeight * 3, frameWidth, frameHeight),
                new Rectangle(frameWidth, frameHeight * 3, frameWidth, frameHeight),
                new Rectangle(frameWidth * 2, frameHeight * 3, frameWidth, frameHeight)
                    },
                    Direction.Right => new Rectangle[]
                    {
                new Rectangle(0, frameHeight, frameWidth, frameHeight),
                new Rectangle(frameWidth, frameHeight, frameWidth, frameHeight),
                new Rectangle(frameWidth * 2, frameHeight, frameWidth, frameHeight)
                    },
                    _ => new Rectangle[]
                    {
                new Rectangle(0, frameHeight, frameWidth, frameHeight),
                new Rectangle(frameWidth, frameHeight, frameWidth, frameHeight),
                new Rectangle(frameWidth * 2, frameHeight, frameWidth, frameHeight)
                    }
                };
            }
        }


        private Rectangle[] GetCurrentRectanglePipe()
        {
            int frameWidth = 200;
            int frameHeight = 200;

            return new Rectangle[]
            {
        new Rectangle(0, 0, frameWidth, frameHeight),
        new Rectangle(frameWidth, 0, frameWidth, frameHeight),
        new Rectangle(frameWidth * 2, 0, frameWidth, frameHeight)
            };
        }


        private Rectangle[] GetCurrentRectangles()
        {
            if (isJumping)
            {
                Rectangle jumpFrame;
                switch (currentDirection)
                {
                    case Direction.Up:
                        jumpFrame = new Rectangle(288, 0, 96, 128); break;
                    case Direction.Down:
                        jumpFrame = new Rectangle(288, 256, 96, 128); break;
                    case Direction.Left:
                        jumpFrame = new Rectangle(288, 384, 96, 128); break;
                    case Direction.Right:
                        jumpFrame = new Rectangle(288, 128, 96, 128); break;
                    default:
                        jumpFrame = new Rectangle(288, 128, 96, 128); break;
                }
                return new Rectangle[] { jumpFrame, jumpFrame, jumpFrame };
            }

            Rectangle[] baseRectangles = currentDirection switch
            {
                Direction.Up => new Rectangle[]
                {
            isSpacebarAnimationActive
                ? (useSpacebarFrame ? new Rectangle(80, 0, doubleWidth, 128)
                                    : new Rectangle(384, 0, 96, 128))
                : new Rectangle(0, 0, 96, 128),
            new Rectangle(96, 0, 96, 128),
            new Rectangle(192, 0, 96, 128)
                },
                Direction.Down => new Rectangle[]
                {
            isSpacebarAnimationActive
                ? (useSpacebarFrame ? new Rectangle(480, 256, doubleWidth, 128)
                                    : new Rectangle(384, 256, 96, 128))
                : new Rectangle(0, 256, 96, 128),
            new Rectangle(96, 256, 96, 128),
            new Rectangle(192, 256, 96, 128)
                },
                Direction.Left => new Rectangle[]
                {
            isSpacebarAnimationActive
                ? (useSpacebarFrame ? new Rectangle(480, 384, doubleWidth, 128)
                                    : new Rectangle(384, 384, 96, 128))
                : new Rectangle(0, 384, 96, 128),
            new Rectangle(96, 384, 96, 128),
            new Rectangle(192, 384, 96, 128)
                },
                Direction.Right => new Rectangle[]
                {
            isSpacebarAnimationActive
                ? (useSpacebarFrame ? new Rectangle(480, 128, doubleWidth, 128)
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
