// using System;
// using Microsoft.Xna.Framework;
// using Microsoft.Xna.Framework.Graphics;
// using Microsoft.Xna.Framework.Input;

// namespace monogame
// {

//     public class Game2 : IGameState
//     {
//         Texture2D charaset;
//         Texture2D nacho;
//         Texture2D sushi;
//         Vector2 ballPosition = new Vector2(300, 300);
//         Texture2D cheeseLaunch;
//         Texture2D nachoMouth;
//         Vector2 nachoPosition;

//         Vector2 cheesePosition;

//         Texture2D sombreroWallpaper;
//         Texture2D sushiWallpaper;
//         float ballSpeed;
//         float nachoSpeed = 40f;
//         float nachoRotation = 0f;
//         bool rotatingRight = true;
//         private GraphicsDeviceManager _graphics;
//         private SpriteBatch _spriteBatch;
//         float health;
//         float timer;
//         int threshold;
//         private SpriteFont font;

//         Rectangle[] downRectangles;
//         Rectangle[] upRectangles;
//         Rectangle[] leftRectangles;
//         Rectangle[] rightRectangles;

//         Vector2 lastDonutPosition;

//         bool cheeseVisible = true;

//         byte currentAnimationIndex;


//         enum Direction { Down, Up, Left, Right }
//         Direction currentDirection;

//         bool hasCheeseDealtDamage = false;
//         float cheeseRotation = 0f;

//         int animationCycleCount = 0;
//         bool useBlinkingFrame = false;

//         bool useSpacebarFrame = false;

//         bool useOpenMouthFrame = false;

//         private bool isSpacebarAnimationActive = false;
//         private float spacebarAnimationTimer = 0f;
//         private float spacebarFirstFrameDuration = 0.5f;
//         private float spacebarSecondFrameDuration = 0.8f;
//         private int doubleWidth = 192;
//         float nachoHealth;
//         private bool nachoDamagedThisCycle = false;
//         private KeyboardState previousKeyboardState;

//         float cheeseVisibilityTimer = 0f;

//         Texture2D splashCheese;
//         private Texture2D mainmenu;
//         bool showSplashCheese = false;
//         float splashCheeseTimer = 0f;
//         const float splashCheeseDuration = 1f;
//         Vector2 splashPosition;
//         Vector2 sushiPosition;

//         private Direction currentDirectionNacho2 = Direction.Down;
//         private float nachoDirectionDelayTimer = 0f;
//         private const float NachoDirectionDelayDuration = 2f;

//         private bool usePostHitFrame = false;
//         private float postHitAnimationTimer = 0f;
//         private const float postHitAnimationDuration = 0.5f;

//         private bool nachoDefeated = false;
//         private float nachoDefeatedTimer = 0f;
//         private const float nachoDefeatedDuration = 3f;

//         private Direction currentDirectionSushi = Direction.Down;
//         private float sushiDirectionDelayTimer = 0f;
//         private const float SushiDirectionDelayDuration = 1f;
//         private GraphicsDevice _graphicsDevice;
//         private MainGame _mainGame;


//         public Game2(MainGame mainGame, SpriteBatch spriteBatch)
//         {
//             _mainGame = mainGame;
//             _spriteBatch = spriteBatch;
//             _graphicsDevice = mainGame.GraphicsDevice;

//             ballPosition = new Vector2(300, 300);
//             nachoPosition = new Vector2(150, 150);
//             sushiPosition = new Vector2(200, 200);
//             ballSpeed = 100f;
//             nachoSpeed = 80f;
//             downRectangles = new Rectangle[3]
//           {
//                 new Rectangle(0, 256, 96, 128),
//                 new Rectangle(96, 256, 96, 128),
//                 new Rectangle(192, 256, 96, 128)
//           };

//             upRectangles = new Rectangle[3]
//             {
//                 new Rectangle(0, 0, 96, 128),
//                 new Rectangle(96, 0, 96, 128),
//                 new Rectangle(192, 0, 96, 128)
//             };

//             rightRectangles = new Rectangle[3]
//             {
//                 new Rectangle(0, 128, 96, 128),
//                 new Rectangle(96, 128, 96, 128),
//                 new Rectangle(192, 128, 96, 128)
//             };

//             leftRectangles = new Rectangle[3]
//             {
//                 new Rectangle(0, 384, 96, 128),
//                 new Rectangle(96, 384, 96, 128),
//                 new Rectangle(192, 384, 96, 128)
//             };

//             currentAnimationIndex = 1;
//             currentDirection = Direction.Down; // Initialize currentDirection
//             nachoHealth = 4;
//         }


//         public void LoadContent()
//         {

//             charaset = _mainGame.Content.Load<Texture2D>("donutsprites17");
//             nacho = _mainGame.Content.Load<Texture2D>("nachosprites4");
//             sushi = _mainGame.Content.Load<Texture2D>("sushisprites10");
//             font = _mainGame.Content.Load<SpriteFont>("DefaultFont1");
//             cheeseLaunch = _mainGame.Content.Load<Texture2D>("cheeselaunch");
//             nachoMouth = _mainGame.Content.Load<Texture2D>("openmountnacho2");
//             sombreroWallpaper = _mainGame.Content.Load<Texture2D>("sombrerosetting");
//             sushiWallpaper = _mainGame.Content.Load<Texture2D>("japaneselevel2");
//             splashCheese = _mainGame.Content.Load<Texture2D>("splashcheese");
//             threshold = 150;

//         }

//         private void CheeseLauncher(GameTime gameTime)
//         {
//             float distanceToBall = Vector2.Distance(nachoPosition, ballPosition);
//             if (distanceToBall <= 150)
//             {
//                 if (!cheeseVisible)
//                 {
//                     cheesePosition = GetNachoMouthPosition();
//                     cheeseVisible = true;
//                 }

//                 Vector2 directionToBall = ballPosition - cheesePosition;
//                 directionToBall.Normalize();
//                 cheeseRotation = (float)Math.Atan2(directionToBall.Y, directionToBall.X);
//                 cheesePosition += directionToBall * nachoSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

//                 Rectangle cheeseRect = new Rectangle((int)cheesePosition.X - 10, (int)cheesePosition.Y - 10, 20, 20);
//                 Rectangle ballRect = new Rectangle((int)ballPosition.X - 48, (int)ballPosition.Y - 64, 96, 128);

//                 if (cheeseRect.Intersects(ballRect))
//                 {
//                     showSplashCheese = true;
//                     splashPosition = ballPosition;
//                     nachoHealth -= 0.5f;
//                     cheeseVisible = false;
//                 }
//             }
//         }

//         private Vector2 GetNachoMouthPosition()
//         {
//             Rectangle currentRect = downRectangles[currentAnimationIndex];
//             return nachoPosition + new Vector2(currentRect.Width / 2, -70);
//         }

//         private void KeyboardTracker(GameTime gameTime)
//         {
//             KeyboardState kstate = Keyboard.GetState();
//             Vector2 movement = Vector2.Zero;

//             if (kstate.IsKeyDown(Keys.Up))
//             {
//                 movement.Y -= 1;
//                 currentDirection = Direction.Up; // Update direction
//             }
//             if (kstate.IsKeyDown(Keys.Down))
//             {
//                 movement.Y += 1;
//                 currentDirection = Direction.Down; // Update direction
//             }
//             if (kstate.IsKeyDown(Keys.Left))
//             {
//                 movement.X -= 1;
//                 currentDirection = Direction.Left; // Update direction
//             }
//             if (kstate.IsKeyDown(Keys.Right))
//             {
//                 movement.X += 1;
//                 currentDirection = Direction.Right; // Update direction
//             }

//             if (movement != Vector2.Zero)
//             {
//                 movement.Normalize();
//                 ballPosition += movement * ballSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
//             }
//         }

//         private void spacebarAttack(GameTime gameTime, KeyboardState currentKeyboardState)
//         {
//             Rectangle donutRect = new Rectangle(
//                 (int)ballPosition.X - 48,
//                 (int)ballPosition.Y - 64,
//                 96,
//                 128
//             );

//             Rectangle nachoRect = new Rectangle(
//                 (int)nachoPosition.X - 48,
//                 (int)nachoPosition.Y - 64,
//                 96,
//                 128
//             );

//             if (currentKeyboardState.IsKeyDown(Keys.Space) && !previousKeyboardState.IsKeyDown(Keys.Space))
//             {
//                 if (!isSpacebarAnimationActive)
//                 {
//                     isSpacebarAnimationActive = true;
//                     spacebarAnimationTimer = 0f;
//                     nachoDamagedThisCycle = false;
//                 }
//             }

//             if (isSpacebarAnimationActive)
//             {
//                 spacebarAnimationTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

//                 if (spacebarAnimationTimer <= spacebarFirstFrameDuration)
//                 {
//                     useSpacebarFrame = true;
//                 }
//                 else if (spacebarAnimationTimer <= spacebarFirstFrameDuration + spacebarSecondFrameDuration)
//                 {
//                     useSpacebarFrame = false;
//                 }
//                 else
//                 {
//                     spacebarAnimationTimer = 0f;
//                     isSpacebarAnimationActive = false;
//                 }
//             }

//             if (useSpacebarFrame && donutRect.Intersects(nachoRect) && !nachoDamagedThisCycle)
//             {
//                 nachoHealth = Math.Max(0, nachoHealth - 1);
//                 nachoDamagedThisCycle = true;

//                 usePostHitFrame = true;
//                 postHitAnimationTimer = 0f;
//             }
//         }

//         private bool keyboardTracker(float elapsedTime, GameTime gameTime)
//         {

//             bool isMoving = false;
//             float updatedBallSpeed = ballSpeed * elapsedTime;

//             var kstate = Keyboard.GetState();

//             Vector2 movement = Vector2.Zero;

//             if (kstate.IsKeyDown(Keys.Up))
//             {
//                 movement.Y -= 1;
//                 currentDirection = Direction.Up;
//                 isMoving = true;
//             }
//             if (kstate.IsKeyDown(Keys.Down))
//             {
//                 movement.Y += 1;
//                 currentDirection = Direction.Down;
//                 isMoving = true;
//             }
//             if (kstate.IsKeyDown(Keys.Left))
//             {
//                 movement.X -= 1;
//                 currentDirection = Direction.Left;
//                 isMoving = true;
//             }
//             if (kstate.IsKeyDown(Keys.Right))
//             {
//                 movement.X += 1;
//                 currentDirection = Direction.Right;
//                 isMoving = true;
//             }

//             if (movement != Vector2.Zero)
//             {
//                 movement.Normalize();
//                 ballPosition += movement * updatedBallSpeed;
//             }

//             nachoDirectionDelayTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

//             if (nachoDirectionDelayTimer >= NachoDirectionDelayDuration)
//             {
//                 currentDirectionNacho2 = currentDirection;
//                 nachoDirectionDelayTimer = 0f;
//             }

//             return isMoving;
//         }

//         private void animationBlinker(bool isMoving, GameTime gameTime)
//         {
//             if (isMoving || isSpacebarAnimationActive)
//             {
//                 if (timer > threshold)
//                 {
//                     currentAnimationIndex = (byte)((currentAnimationIndex + 1) % 3);

//                     if (currentAnimationIndex == 0)
//                     {
//                         animationCycleCount++;

//                         if (animationCycleCount % 3 == 0)
//                         {
//                             useBlinkingFrame = true;
//                         }
//                         else
//                         {
//                             useBlinkingFrame = false;
//                         }
//                     }

//                     timer = 0;
//                 }
//                 else
//                 {
//                     timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
//                 }
//             }

//             else
//             {
//                 currentAnimationIndex = 1;
//             }

//         }

//         private void cheeseLauncher(float updatedNachoSpeed, GameTime gameTime)
//         {
//             int cheeseWidth = 20;
//             int cheeseHeight = 20;

//             float distanceToDonut = Vector2.Distance(nachoPosition, ballPosition);

//             cheeseVisibilityTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

//             if (showSplashCheese)
//             {
//                 splashCheeseTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
//                 if (splashCheeseTimer >= splashCheeseDuration)
//                 {
//                     showSplashCheese = false;
//                     splashCheeseTimer = 0f;
//                 }
//             }

//             if (cheeseVisible && !showSplashCheese)
//             {
//                 cheeseVisibilityTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

//                 if (cheeseVisibilityTimer >= 4f)
//                 {
//                     cheeseVisible = false;
//                     cheeseVisibilityTimer = 0f;
//                     cheesePosition = GetNachoMouthPosition();
//                     hasCheeseDealtDamage = false;

//                     if (cheeseVisible)
//                     {
//                         cheesePosition = GetNachoMouthPosition();
//                         hasCheeseDealtDamage = false;
//                     }
//                     else
//                     {
//                         splashPosition = cheesePosition;
//                     }
//                     return;
//                 }
//             }

//             if (distanceToDonut <= 150)
//             {
//                 if (!cheeseVisible && !showSplashCheese)
//                 {
//                     cheesePosition = GetNachoMouthPosition();
//                     cheeseVisible = true;
//                 }

//                 Vector2 directionToDonut = ballPosition - cheesePosition;
//                 if (directionToDonut != Vector2.Zero && directionToDonut.LengthSquared() > 1f)
//                 {
//                     directionToDonut.Normalize();
//                     cheeseRotation = (float)Math.Atan2(directionToDonut.Y, directionToDonut.X);
//                     cheesePosition += directionToDonut * updatedNachoSpeed * 2.5f;
//                 }


//                 Rectangle cheeseRect = new Rectangle(
//                     (int)cheesePosition.X - cheeseWidth / 2,
//                     (int)cheesePosition.Y - cheeseHeight / 2,
//                     cheeseWidth,
//                     cheeseHeight
//                 );

//                 Rectangle donutRect = new Rectangle(
//                     (int)ballPosition.X - 48,
//                     (int)ballPosition.Y - 64,
//                     96,
//                     128
//                 );

//                 if (cheeseRect.Intersects(donutRect) && !hasCheeseDealtDamage)
//                 {
//                     showSplashCheese = true;
//                     cheeseVisible = false;
//                     splashPosition = ballPosition;
//                     splashCheeseTimer = 0f;
//                     health -= 0.5f;
//                     hasCheeseDealtDamage = true;
//                 }
//             }
//             else
//             {
//                 cheeseVisible = false;
//                 hasCheeseDealtDamage = false;
//             }
//         }


//         public void Update(GameTime gameTime)
//         {

//             if (nachoDefeated)
//             {
//                 nachoDefeatedTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
//             }

//             if (nachoHealth <= 0)
//             {
//                 nachoDefeated = true;
//                 nachoDefeatedTimer = 0f;
//                 return;
//             }

//             float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
//             float updatedNachoSpeed = nachoSpeed * elapsedTime;

//             KeyboardState currentKeyboardState = Keyboard.GetState();

//             spacebarAttack(gameTime, currentKeyboardState);

//             if (usePostHitFrame)
//             {
//                 postHitAnimationTimer += elapsedTime;
//                 if (postHitAnimationTimer >= postHitAnimationDuration)
//                 {
//                     usePostHitFrame = false;
//                     postHitAnimationTimer = 0f;
//                 }
//             }

//             Rectangle currentRect = GetCurrentRectangles()[currentAnimationIndex];
//             // ballPosition.X = MathHelper.Clamp(ballPosition.X, currentRect.Width / 2, _graphics.PreferredBackBufferWidth - currentRect.Width / 2);
//             // ballPosition.Y = MathHelper.Clamp(ballPosition.Y, currentRect.Height / 2, _graphics.PreferredBackBufferHeight - currentRect.Height / 2);

//             Vector2 directionToDonut = ballPosition - nachoPosition;
//             if (directionToDonut != Vector2.Zero)
//             {
//                 directionToDonut.Normalize();
//                 nachoPosition += directionToDonut * updatedNachoSpeed;
//             }

//             bool isMoving = keyboardTracker(elapsedTime, gameTime);

//             animationBlinker(isMoving, gameTime);
//             cheeseLauncher(updatedNachoSpeed, gameTime);

//             previousKeyboardState = currentKeyboardState;

//             float updatedSushiSpeed = nachoSpeed * elapsedTime; // Reuse nachoSpeed for sushi speed
//             Vector2 directionToDonutFromSushi = ballPosition - sushiPosition;

//             if (directionToDonutFromSushi != Vector2.Zero)
//             {
//                 directionToDonutFromSushi.Normalize();
//                 sushiPosition += directionToDonutFromSushi * updatedSushiSpeed;
//                 currentAnimationIndex = currentAnimationIndex++;
//             }

//             sushiDirectionDelayTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

//             if (sushiDirectionDelayTimer >= SushiDirectionDelayDuration)
//             {
//                 currentDirectionSushi = currentDirection;
//                 sushiDirectionDelayTimer = 0f;
//             }
//         }


//         public void Draw(GameTime gameTime)
//         {
//             _graphicsDevice.Clear(Color.Black);

//             _spriteBatch.Draw(sushiWallpaper, new Rectangle(0, 0, 800, 600), Color.White);

//             if (showSplashCheese)
//             {
//                 _spriteBatch.Draw(splashCheese, splashPosition, null, Color.White);
//             }

//             _spriteBatch.Draw(charaset, ballPosition, GetCurrentRectangles()[currentAnimationIndex], Color.White);

//             _spriteBatch.Draw(
//            sushi,
//            sushiPosition,
//            GetCurrentRectangleSushi()[currentAnimationIndex],
//            Color.White,
//            0f,
//            new Vector2(70, 66),
//            1.0f,
//            SpriteEffects.None,
//            0f
//        );
//         }


//         private Rectangle[] GetCurrentRectangles()
//         {
//             Rectangle[] baseRectangles = currentDirection switch
//             {
//                 Direction.Up => new Rectangle[]
//                 {
//             isSpacebarAnimationActive
//                 ? (useSpacebarFrame
//                     ? new Rectangle(384, 0, 96, 128)  // Fifth column
//                     : new Rectangle(480, 0, doubleWidth, 128))  // Sixth column
//                 : new Rectangle(0, 0, 96, 128),
//             new Rectangle(96, 0, 96, 128),
//             new Rectangle(192, 0, 96, 128)
//                 },
//                 Direction.Down => new Rectangle[]
//                 {
//             isSpacebarAnimationActive
//                 ? (useSpacebarFrame
//                     ? new Rectangle(384, 256, 96, 128) // Fifth column
//                     : new Rectangle(480, 256, doubleWidth, 128)) // Sixth column
//                 : new Rectangle(0, 256, 96, 128),
//             new Rectangle(96, 256, 96, 128),
//             new Rectangle(192, 256, 96, 128)
//                 },
//                 Direction.Left => new Rectangle[]
//                 {
//             isSpacebarAnimationActive
//                 ? (useSpacebarFrame
//                     ? new Rectangle(384, 384, 96, 128) // Fifth column
//                     : new Rectangle(480, 384, doubleWidth, 128)) // Sixth column
//                 : new Rectangle(0, 384, 96, 128),
//             new Rectangle(96, 384, 96, 128),
//             new Rectangle(192, 384, 96, 128)
//                 },
//                 Direction.Right => new Rectangle[]
//                 {
//             isSpacebarAnimationActive
//                 ? (useSpacebarFrame
//                     ? new Rectangle(384, 128, 96, 128) // Fifth column
//                     : new Rectangle(480, 128, doubleWidth, 128)) // Sixth column
//                 : new Rectangle(0, 128, 96, 128),
//             new Rectangle(96, 128, 96, 128),
//             new Rectangle(192, 128, 96, 128)
//                 },
//                 _ => new Rectangle[]
//                 {
//             new Rectangle(0, 128, 96, 128),
//             new Rectangle(96, 128, 96, 128),
//             new Rectangle(192, 128, 96, 128)
//                 },
//             };

//             if (useBlinkingFrame && !isSpacebarAnimationActive)
//             {
//                 baseRectangles[2] = new Rectangle(288, baseRectangles[2].Y, 96, 128); // Blinking frame
//             }

//             return baseRectangles;
//         }
//         private Rectangle[] GetCurrentRectangleSushi()
//         {
//             int frameWidth = 110;
//             int frameHeight = 133;
//             int doubleWidth = frameWidth * 2;

//             Rectangle[] baseRectangles = currentDirection switch
//             {
//                 Direction.Up => new Rectangle[]
//                 {
//             isSpacebarAnimationActive
//                 ? (useSpacebarFrame
//                     ? new Rectangle(frameWidth * 4, 0, frameWidth, frameHeight)  // Fifth column
//                     : new Rectangle(frameWidth * 5, 0, doubleWidth, frameHeight))  // Sixth column
//                 : new Rectangle(0, 0, frameWidth, frameHeight),
//             new Rectangle(frameWidth, 0, frameWidth, frameHeight),
//             new Rectangle(frameWidth * 2, 0, frameWidth, frameHeight)
//                 },
//                 Direction.Down => new Rectangle[]
//                 {
//             isSpacebarAnimationActive
//                 ? (useSpacebarFrame
//                     ? new Rectangle(frameWidth * 4, frameHeight * 2, frameWidth, frameHeight) // Fifth column
//                     : new Rectangle(frameWidth * 5, frameHeight * 2, doubleWidth, frameHeight)) // Sixth column
//                 : new Rectangle(0, frameHeight * 2, frameWidth, frameHeight),
//             new Rectangle(frameWidth, frameHeight * 2, frameWidth, frameHeight),
//             new Rectangle(frameWidth * 2, frameHeight * 2, frameWidth, frameHeight)
//                 },
//                 Direction.Left => new Rectangle[]
//                 {
//             isSpacebarAnimationActive
//                 ? (useSpacebarFrame
//                     ? new Rectangle(frameWidth * 4, frameHeight * 3, frameWidth, frameHeight) // Fifth column
//                     : new Rectangle(frameWidth * 5, frameHeight * 3, doubleWidth, frameHeight)) // Sixth column
//                 : new Rectangle(0, frameHeight * 3, frameWidth, frameHeight),
//             new Rectangle(frameWidth, frameHeight * 3, frameWidth, frameHeight),
//             new Rectangle(frameWidth * 2, frameHeight * 3, frameWidth, frameHeight)
//                 },
//                 Direction.Right => new Rectangle[]
//                 {
//             isSpacebarAnimationActive
//                 ? (useSpacebarFrame
//                     ? new Rectangle(frameWidth * 4, frameHeight, frameWidth, frameHeight) // Fifth column
//                     : new Rectangle(frameWidth * 5, frameHeight, doubleWidth, frameHeight)) // Sixth column
//                 : new Rectangle(0, frameHeight, frameWidth, frameHeight),
//             new Rectangle(frameWidth, frameHeight, frameWidth, frameHeight),
//             new Rectangle(frameWidth * 2, frameHeight, frameWidth, frameHeight)
//                 },
//                 _ => new Rectangle[]
//                 {
//             new Rectangle(0, frameHeight, frameWidth, frameHeight),
//             new Rectangle(frameWidth, frameHeight, frameWidth, frameHeight),
//             new Rectangle(frameWidth * 2, frameHeight, frameWidth, frameHeight)
//                 },
//             };

//             if (useBlinkingFrame && !isSpacebarAnimationActive)
//             {
//                 baseRectangles[2] = new Rectangle(frameWidth * 3, baseRectangles[2].Y, frameWidth, frameHeight); // Blinking frame
//             }

//             return baseRectangles;
//         }

//     }
// }
