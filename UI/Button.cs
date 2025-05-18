using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using monogame.Effects;

namespace monogame.UI
{
    public class Button
    {
        private static readonly string[] ColorNames = { "Normal", "Pink", "Yellow", "Brown" };
        private static readonly DonutColor[] ColorCycleOrder = { 
            DonutColor.Pink,
            DonutColor.Yellow,
            DonutColor.Brown,
            DonutColor.Normal
        };
        private Rectangle bounds;
        private Texture2D texture;
        private SpriteFont font;
        private string text;
        private Color normalColor;
        private Color hoverColor;
        private Color textColor = Color.White;
        private bool isHovered;
        private MouseState currentMouseState;
        private MouseState previousMouseState;

        public bool IsClicked { get; private set; }

        private DonutColor currentButtonColor = DonutColor.Pink;
        
        public Button(Rectangle bounds, Texture2D texture, SpriteFont font, string text)
        {
            UpdateButtonColors();
            this.bounds = bounds;
            this.texture = texture;
            this.font = font;
            this.text = text;
        }

        public void Update(MouseState mouseState)
        {
            previousMouseState = currentMouseState;
            currentMouseState = mouseState;

            isHovered = bounds.Contains(mouseState.X, mouseState.Y);

            IsClicked = isHovered && 
                        currentMouseState.LeftButton == ButtonState.Released && 
                        previousMouseState.LeftButton == ButtonState.Pressed;
        }

        private int currentColorIndex = 0;
        
        public void CycleToNextColor()
        {
            currentColorIndex = (currentColorIndex + 1) % ColorCycleOrder.Length;
            currentButtonColor = ColorCycleOrder[currentColorIndex];
            
            string colorName = currentButtonColor == DonutColor.Normal ? "Blue" : ColorNames[(int)currentButtonColor];
            text = colorName + " Donut";
            
            UpdateButtonColors();
        }
        
        private void UpdateButtonColors()
        {
            if (currentButtonColor == DonutColor.Normal)
            {
                normalColor = new Color(30, 144, 255); // DodgerBlue
            }
            else
            {
                normalColor = ColorReplacer.GetDonutColor(currentButtonColor);
            }
            
            hoverColor = new Color(
                (int)(normalColor.R * 0.8f),
                (int)(normalColor.G * 0.8f), 
                (int)(normalColor.B * 0.8f));
        }
        
        public DonutColor GetCurrentColor()
        {
            return currentButtonColor;
        }
        
        private static Texture2D circleTexture;
        
        private static void CreateCircleTexture(GraphicsDevice graphicsDevice, int radius)
        {
            if (circleTexture != null) return;
            
            int diameter = radius * 2;
            circleTexture = new Texture2D(graphicsDevice, diameter, diameter);
            
            Color[] data = new Color[diameter * diameter];
            Vector2 center = new Vector2(radius, radius);
            
            for (int y = 0; y < diameter; y++)
            {
                for (int x = 0; x < diameter; x++)
                {
                    Vector2 pos = new Vector2(x, y);
                    float distance = Vector2.Distance(pos, center);
                    
                    data[y * diameter + x] = distance <= radius ? Color.White : Color.Transparent;
                }
            }
            
            circleTexture.SetData(data);
        }
        
        public void Draw(SpriteBatch spriteBatch)
        {
            if (circleTexture == null)
            {
                CreateCircleTexture(spriteBatch.GraphicsDevice, 8);
            }
            
            Color drawColor = isHovered ? hoverColor : normalColor;
            
            int cornerRadius = 8;
            
            Rectangle centerRect = new Rectangle(
                bounds.X + cornerRadius,
                bounds.Y,
                bounds.Width - 2 * cornerRadius,
                bounds.Height
            );
            spriteBatch.Draw(texture, centerRect, drawColor);
            
            Rectangle leftRect = new Rectangle(
                bounds.X,
                bounds.Y + cornerRadius,
                cornerRadius,
                bounds.Height - 2 * cornerRadius
            );
            Rectangle rightRect = new Rectangle(
                bounds.X + bounds.Width - cornerRadius,
                bounds.Y + cornerRadius,
                cornerRadius,
                bounds.Height - 2 * cornerRadius
            );
            spriteBatch.Draw(texture, leftRect, drawColor);
            spriteBatch.Draw(texture, rightRect, drawColor);
            
            spriteBatch.Draw(
                circleTexture,
                new Vector2(bounds.X, bounds.Y + cornerRadius),
                null,
                drawColor,
                0f,
                new Vector2(0, cornerRadius),
                1f,
                SpriteEffects.None,
                0f
            );
            
            spriteBatch.Draw(
                circleTexture,
                new Vector2(bounds.X + bounds.Width, bounds.Y + cornerRadius),
                null,
                drawColor,
                0f,
                new Vector2(circleTexture.Width, cornerRadius),
                1f,
                SpriteEffects.None,
                0f
            );
            
            spriteBatch.Draw(
                circleTexture,
                new Vector2(bounds.X, bounds.Y + bounds.Height - cornerRadius),
                null,
                drawColor,
                0f,
                new Vector2(0, cornerRadius),
                1f,
                SpriteEffects.None,
                0f
            );
            
            spriteBatch.Draw(
                circleTexture,
                new Vector2(bounds.X + bounds.Width, bounds.Y + bounds.Height - cornerRadius),
                null,
                drawColor,
                0f,
                new Vector2(circleTexture.Width, cornerRadius),
                1f,
                SpriteEffects.None,
                0f
            );

            if (font != null && !string.IsNullOrEmpty(text))
            {
                Vector2 textSize = font.MeasureString(text);
                Vector2 textPosition = new Vector2(
                    bounds.X + (bounds.Width - textSize.X) / 2,
                    bounds.Y + (bounds.Height - textSize.Y) / 2
                );
                spriteBatch.DrawString(font, text, textPosition, textColor);
            }
        }
    }
}
