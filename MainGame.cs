using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using monogame.Effects;

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
        public Game1 Game1Instance { get; private set; }
        private bool inMainMenu = true;
        Vector2 ballPosition;
        Vector2 nachoPosition;
        Vector2 sushiPosition;
        
        // Game music
        private Song gameMusic;
        private Song menuMusic;
        
        public DonutColor CurrentDonutColor { get; set; } = DonutColor.Pink;
        public bool IsColorEffectActive { get; set; } = false;
        public int ColorButtonIndex { get; set; } = 0;

        public bool HasPickedUpAxe { get; set; } = false;

        public enum GameStateType
        {
            MainMenu,
            Game1,
            Game2,
            Game3
        }

        public MainGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            
            _graphics.IsFullScreen = true;
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            ballPosition = new Vector2(_graphics.PreferredBackBufferWidth / 2,
                    _graphics.PreferredBackBufferHeight / 2);
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _graphics.ApplyChanges();

            base.Initialize();
        }


        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            mainmenu = Content.Load<Texture2D>("mainmenudonut4");

            startButtonArea = new Rectangle(300, 270, 200, 50);
            endButtonArea = new Rectangle(300, 380, 200, 50);
            
            // Load music
            try {
                gameMusic = Content.Load<Song>("donut_song");
                menuMusic = Content.Load<Song>("menu_song");
                MediaPlayer.Play(menuMusic);
                MediaPlayer.IsRepeating = true;
            } catch (System.Exception e) {
                System.Console.WriteLine("Failed to load music: " + e.Message);
            }

            SwitchGameState(GameStateType.MainMenu);
        }

        public void SwitchGameState(GameStateType newState)
        {
            if (newState == GameStateType.MainMenu)
            {
                inMainMenu = true;
                currentGameState = null;
                
                MediaPlayer.Stop();
                if (menuMusic != null)
                {
                    MediaPlayer.Play(menuMusic);
                    MediaPlayer.IsRepeating = true;
                }
            }
            else
            {
                inMainMenu = false;
                switch (newState)
                {
                    case GameStateType.Game1:
                        Game1Instance = new Game1(this, _spriteBatch);
                        currentGameState = Game1Instance;
                        
                        if (gameMusic != null)
                        {
                            MediaPlayer.Play(gameMusic);
                            MediaPlayer.IsRepeating = true;
                        }
                        break;
                    case GameStateType.Game2:
                        currentGameState = new Game2(this, _spriteBatch);
                        break;
                    case GameStateType.Game3:
                        currentGameState = new Game3(this, _graphics.GraphicsDevice, _spriteBatch);
                        break;
                }
                currentGameState.LoadContent();
            }
        }

        private KeyboardState previousKeyboardState;
        
        private void ToggleFullScreen()
        {
            _graphics.IsFullScreen = !_graphics.IsFullScreen;
            
            if (_graphics.IsFullScreen)
            {
                _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            }
            else
            {
                _graphics.PreferredBackBufferWidth = 850;
                _graphics.PreferredBackBufferHeight = 850;
            }
            
            _graphics.ApplyChanges();
        }
        
        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            
            if (keyboardState.IsKeyDown(Keys.LeftAlt) && keyboardState.IsKeyDown(Keys.Enter) &&
                (!previousKeyboardState.IsKeyDown(Keys.LeftAlt) || !previousKeyboardState.IsKeyDown(Keys.Enter)))
            {
                ToggleFullScreen();
            }
            
            previousKeyboardState = keyboardState;

            if (inMainMenu)
            {
                if (keyboardState.IsKeyDown(Keys.Enter))
                {
                    SwitchGameState(GameStateType.Game1);
                }

                MouseState mouseState = Mouse.GetState();
                if (mouseState.LeftButton == ButtonState.Pressed)
                {
                    Point mousePosition = mouseState.Position;
                    if (startButtonArea.Contains(mousePosition))
                    {
                        SwitchGameState(GameStateType.Game1);
                    }
                    else if (endButtonArea.Contains(mousePosition))
                    {
                        Exit();
                    }
                }
            }
            else
            {
                if (keyboardState.IsKeyDown(Keys.Escape))
                {
                    SwitchGameState(GameStateType.MainMenu);
                }

                currentGameState?.Update(gameTime);
            }

            base.Update(gameTime);
        }



        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();

            if (inMainMenu)
            {
                int backgroundWidth = mainmenu.Width;
                int backgroundHeight = mainmenu.Height;
                int screenWidth = GraphicsDevice.Viewport.Width;
                int screenHeight = GraphicsDevice.Viewport.Height;
                
                int x = (screenWidth - backgroundWidth) / 2;
                int y = (screenHeight - backgroundHeight) / 2;
                
                _spriteBatch.Draw(mainmenu, new Rectangle(x, y, backgroundWidth, backgroundHeight), Color.White);
                
                float buttonScaleX = backgroundWidth / 800f;
                float buttonScaleY = backgroundHeight / 600f;
                startButtonArea = new Rectangle(
                    x + (int)(300 * buttonScaleX), 
                    y + (int)(270 * buttonScaleY), 
                    (int)(200 * buttonScaleX), 
                    (int)(50 * buttonScaleY));
                endButtonArea = new Rectangle(
                    x + (int)(300 * buttonScaleX), 
                    y + (int)(380 * buttonScaleY),
                    (int)(200 * buttonScaleX), 
                    (int)(50 * buttonScaleY));
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
