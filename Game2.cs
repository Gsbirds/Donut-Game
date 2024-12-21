// using System;
// using Microsoft.Xna.Framework;
// using Microsoft.Xna.Framework.Graphics;
// using Microsoft.Xna.Framework.Input;

// namespace monogame
// {
//     public class Game2 : IGameState
//     {
//         private Texture2D charaset, nacho, sushi, cheeseLaunch, nachoMouth, sombreroWallpaper, sushiWallpaper, splashCheese;
//         private Vector2 ballPosition, nachoPosition, cheesePosition, splashPosition;
//         private float ballSpeed, nachoHealth, nachoSpeed = 40f, nachoRotation = 0f, cheeseRotation = 0f;
//         private bool cheeseVisible = true, showSplashCheese = false, nachoDefeated = false;
//         private byte currentAnimationIndex;
//         private float nachoDefeatedTimer = 0f;
//         private const float splashCheeseDuration = 1f, nachoDefeatedDuration = 3f;

//         private Rectangle[] downRectangles, upRectangles, leftRectangles, rightRectangles;

//         private SpriteBatch _spriteBatch;
//         private GraphicsDevice _graphicsDevice;
//         private SpriteFont font;
//         private MainGame _mainGame;

//         private Direction currentDirection; // Added this field for tracking direction

//         private enum Direction { Down, Up, Left, Right }

//         public Game2(MainGame mainGame, SpriteBatch spriteBatch)
//         {
//             _mainGame = mainGame;
//             _spriteBatch = spriteBatch;
//             _graphicsDevice = mainGame.GraphicsDevice;
//         }

//         public void LoadContent()
//         {
//             charaset = _mainGame.Content.Load<Texture2D>("donutsprites17");
//             nacho = _mainGame.Content.Load<Texture2D>("nachosprites4");
//             sushi = _mainGame.Content.Load<Texture2D>("nachosprites4");
//             font = _mainGame.Content.Load<SpriteFont>("DefaultFont1");
//             cheeseLaunch = _mainGame.Content.Load<Texture2D>("cheeselaunch");
//             nachoMouth = _mainGame.Content.Load<Texture2D>("openmountnacho2");
//             sombreroWallpaper = _mainGame.Content.Load<Texture2D>("sombrerosetting");
//             sushiWallpaper = _mainGame.Content.Load<Texture2D>("japaneselevel2");
//             splashCheese = _mainGame.Content.Load<Texture2D>("splashcheese");

//             ballPosition = new Vector2(300, 300);
//             nachoPosition = new Vector2(100, 100);
//             ballSpeed = 100f;

//             downRectangles = new Rectangle[3]
//             {
//                 new Rectangle(0, 256, 96, 128),
//                 new Rectangle(96, 256, 96, 128),
//                 new Rectangle(192, 256, 96, 128)
//             };

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

//         public void Update(GameTime gameTime)
//         {
//             if (nachoDefeated)
//             {
//                 nachoDefeatedTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
//                 if (nachoDefeatedTimer >= nachoDefeatedDuration)
//                 {
//                     // Handle game transition here
//                 }
//                 return;
//             }

//             KeyboardTracker(gameTime);
//             CheeseLauncher(gameTime);

//             if (nachoHealth <= 0)
//             {
//                 nachoDefeated = true;
//                 nachoDefeatedTimer = 0f;
//             }
//         }

//         public void Draw(GameTime gameTime)
//         {
//             _graphicsDevice.Clear(Color.Black);

//             _spriteBatch.Draw(sushiWallpaper, new Rectangle(0, 0, 800, 600), Color.White);

//             _spriteBatch.Draw(
//                 nacho,
//                 nachoPosition,
//                 downRectangles[currentAnimationIndex],
//                 Color.White,
//                 nachoRotation,
//                 new Vector2(48, 64),
//                 1.0f,
//                 SpriteEffects.None,
//                 0f
//             );

//             if (showSplashCheese)
//             {
//                 _spriteBatch.Draw(splashCheese, splashPosition, null, Color.White);
//             }

//             _spriteBatch.Draw(charaset, ballPosition, null, Color.White);
//         }
//     }
// }
