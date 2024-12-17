// using Microsoft.Xna.Framework;
// using Microsoft.Xna.Framework.Graphics;

// namespace monogame
// {
//     public class MainGame : Game
//     {
//         private GraphicsDeviceManager _graphics;
//         private SpriteBatch _spriteBatch;

//         private IGameState currentGameState;

//         public enum GameStateType
//         {
//             Game1,
//             Game2
//         }

//         public MainGame()
//         {
//             _graphics = new GraphicsDeviceManager(this);
//             Content.RootDirectory = "Content";
//             IsMouseVisible = true;
//         }

//         protected override void Initialize()
//         {
//             _graphics.PreferredBackBufferWidth = 800;
//             _graphics.PreferredBackBufferHeight = 600;
//             _graphics.ApplyChanges();

//             base.Initialize();
//         }

//         protected override void LoadContent()
//         {
//             _spriteBatch = new SpriteBatch(GraphicsDevice);

//             SwitchGameState(GameStateType.Game2);
//         }

//         public void SwitchGameState(GameStateType newState)
//         {
//             switch (newState)
//             {
//                 case GameStateType.Game2:
//                     currentGameState = new Game2(this, _spriteBatch);
//                     break;
//             }

//             currentGameState.LoadContent();
//         }

//         protected override void Update(GameTime gameTime)
//         {
//             base.Update(gameTime);
//             currentGameState?.Update(gameTime);
//         }

//         protected override void Draw(GameTime gameTime)
//         {
//             base.Draw(gameTime);
//             currentGameState?.Draw(gameTime);
//         }
//     }
// }
