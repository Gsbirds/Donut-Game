using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace monogame.Screens
{
    public class GameOverScreen
    {
        private Texture2D gameOverTexture;
        private SpriteFont font;
        private float timer;
        private float delayBeforeTransition;
        private bool isActive;
        private GraphicsDevice graphicsDevice;
        private MainGame mainGame;

        /// <summary>
        /// Creates a new game over screen
        /// </summary>
        /// <param name="mainGame">Reference to the main game for transitioning</param>
        /// <param name="graphicsDevice">Graphics device for rendering</param>
        /// <param name="font">Font to display text</param>
        /// <param name="delayBeforeTransition">Delay in seconds before transitioning to the main menu</param>
        public GameOverScreen(MainGame mainGame, GraphicsDevice graphicsDevice, SpriteFont font, float delayBeforeTransition = 3.0f)
        {
            this.mainGame = mainGame;
            this.graphicsDevice = graphicsDevice;
            this.font = font;
            this.delayBeforeTransition = delayBeforeTransition;
            LoadContent();
        }

        /// <summary>
        /// Load the game over texture or create a basic one if it doesn't exist
        /// </summary>
        private void LoadContent()
        {
            try
            {
                gameOverTexture = mainGame.Content.Load<Texture2D>("gameoverscreen");
            }
            catch
            {
                // Create a basic game over screen if texture doesn't exist
                gameOverTexture = new Texture2D(graphicsDevice, 400, 200);
                Color[] colorData = new Color[400 * 200];
                for (int i = 0; i < colorData.Length; i++)
                    colorData[i] = Color.Black;
                gameOverTexture.SetData(colorData);
            }
        }

        /// <summary>
        /// Activate the game over screen
        /// </summary>
        public void Activate()
        {
            isActive = true;
            timer = 0f;
        }

        /// <summary>
        /// Updates the game over screen and handles transition
        /// </summary>
        /// <param name="deltaTime">Time elapsed since last update</param>
        /// <returns>True if screen is still active, false if it has transitioned away</returns>
        public bool Update(float deltaTime)
        {
            if (!isActive)
                return false;

            timer += deltaTime;
            if (timer >= delayBeforeTransition)
            {
                isActive = false;
                mainGame.SwitchGameState(MainGame.GameStateType.MainMenu);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Render the game over screen
        /// </summary>
        /// <param name="spriteBatch">Sprite batch for rendering</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            if (!isActive)
                return;

            // Draw the game over texture
            spriteBatch.Draw(
                gameOverTexture,
                new Rectangle(0, 0, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height),
                Color.White
            );

            // Draw the game over text
            string gameOverText = "GAME OVER";
            Vector2 textSize = font.MeasureString(gameOverText);
            spriteBatch.DrawString(
                font,
                gameOverText,
                new Vector2(graphicsDevice.Viewport.Width / 2 - textSize.X / 2,
                            graphicsDevice.Viewport.Height / 2 - textSize.Y / 2),
                Color.White
            );
        }

        /// <summary>
        /// Whether the game over screen is currently active
        /// </summary>
        public bool IsActive => isActive;
    }
}
