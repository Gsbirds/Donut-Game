using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace monogame
{
    public class MainGame : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D mainmenu;
        private Rectangle startButtonArea;
        private Rectangle endButtonArea;
        private IGameState currentGameState;
        private bool inMainMenu = true;

        public enum GameStateType
        {
            MainMenu,
            Game1,
            Game2
        }

        public MainGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = 800;
            _graphics.PreferredBackBufferHeight = 600;
            _graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            mainmenu = Content.Load<Texture2D>("mainmenudonut4");

            startButtonArea = new Rectangle(300, 270, 200, 50);
            endButtonArea = new Rectangle(300, 380, 200, 50);

            SwitchGameState(GameStateType.MainMenu);
        }

        public void SwitchGameState(GameStateType newState)
        {
            if (newState == GameStateType.MainMenu)
            {
                inMainMenu = true;
                currentGameState = null;
            }
            else
            {
                inMainMenu = false;
                switch (newState)
                {
                    // case GameStateType.Game1:
                    //     currentGameState = new Game1(this, _spriteBatch);
                    //     break;
                    case GameStateType.Game2:
                        currentGameState = new Game2(this, _spriteBatch);
                        break;
                }
                currentGameState.LoadContent();
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (inMainMenu)
            {
                MouseState mouseState = Mouse.GetState();
                if (mouseState.LeftButton == ButtonState.Pressed)
                {
                    Point mousePosition = mouseState.Position;
                    if (startButtonArea.Contains(mousePosition))
                    {
                        SwitchGameState(GameStateType.Game2);
                    }
                    else if (endButtonArea.Contains(mousePosition))
                    {
                        Exit();
                    }
                }
            }
            else
            {
                currentGameState?.Update(gameTime);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.BlanchedAlmond);

            _spriteBatch.Begin();

            if (inMainMenu)
            {
                _spriteBatch.Draw(mainmenu, new Rectangle(60, -10, 650, 650), Color.White);
            }
            else
            {
                currentGameState?.Draw(gameTime); 
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
