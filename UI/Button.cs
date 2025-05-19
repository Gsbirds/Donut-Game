using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using monogame.Effects;

namespace monogame.UI
{
    public class Button
    {//
        private static readonly string[] FlavorNames = { "Blueberry", "Strawberry", "Banana", "Chocolate" };
        private static readonly DonutColor[] ColorCycleOrder = { 
            DonutColor.Normal, // Normal = Blueberry (index 0)
            DonutColor.Pink,   // Pink = Strawberry (index 1)
            DonutColor.Yellow, // Yellow = Banana (index 2)
            DonutColor.Brown   // Brown = Chocolate (index 3)
        };
        private Rectangle bounds;
        private Texture2D texture;
        private SpriteFont font;
        private string text;
        private Color normalColor;
        private Color hoverColor;
        private Color textColor = Color.White;
        private Color cooldownColor = new Color(0, 0, 0, 180);
        private bool isHovered;
        private MouseState currentMouseState;
        private MouseState previousMouseState;
        private float cooldownPercentage = 0f;

        public bool IsClicked { get; private set; }

        private DonutColor currentButtonColor = DonutColor.Normal;
        
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
        
        public void SetColorIndex(int index)
        {
            currentColorIndex = index % ColorCycleOrder.Length;
            currentButtonColor = ColorCycleOrder[currentColorIndex];
            
            string flavorName = FlavorNames[(int)currentButtonColor];
            text = flavorName + " Donut";
            
            UpdateButtonColors();
        }
        
        public void CycleToNextColor()
        {
            currentColorIndex = (currentColorIndex + 1) % ColorCycleOrder.Length;
            currentButtonColor = ColorCycleOrder[currentColorIndex];
            
            string flavorName = FlavorNames[(int)currentButtonColor];
            text = flavorName + " Donut";
            
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
        
        public int GetCurrentColorIndex()
        {
            return currentColorIndex;
        }
        
        public void SetCooldownPercentage(float percentage)
        {
            cooldownPercentage = Math.Clamp(percentage, 0f, 1f);
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
            
            if (cooldownPercentage < 1.0f)
            {
                int shadeWidth = (int)(bounds.Width * (1.0f - cooldownPercentage));
                
                Rectangle shadeRect = new Rectangle(
                    bounds.X,
                    bounds.Y,
                    shadeWidth,
                    bounds.Height
                );
                
                int shadeCornerRadius = 8;
                
                Rectangle centerShadeRect = new Rectangle(
                    shadeRect.X + shadeCornerRadius,
                    shadeRect.Y,
                    Math.Max(0, shadeRect.Width - 2 * shadeCornerRadius),
                    shadeRect.Height
                );
                
                if (centerShadeRect.Width > 0)
                {
                    spriteBatch.Draw(texture, centerShadeRect, cooldownColor);
                }
                
                if (shadeRect.Width > shadeCornerRadius)
                {
                    Rectangle leftShadeRect = new Rectangle(
                        shadeRect.X,
                        shadeRect.Y + shadeCornerRadius,
                        shadeCornerRadius,
                        Math.Max(0, shadeRect.Height - 2 * shadeCornerRadius)
                    );
                    
                    if (leftShadeRect.Height > 0)
                    {
                        spriteBatch.Draw(texture, leftShadeRect, cooldownColor);
                    }
                    
                    // Top-left corner
                    spriteBatch.Draw(
                        circleTexture,
                        new Vector2(shadeRect.X, shadeRect.Y + shadeCornerRadius),
                        null,
                        cooldownColor,
                        0f,
                        new Vector2(0, shadeCornerRadius),
                        1f,
                        SpriteEffects.None,
                        0f
                    );
                    
                    // Bottom-left corner
                    spriteBatch.Draw(
                        circleTexture,
                        new Vector2(shadeRect.X, shadeRect.Y + shadeRect.Height - shadeCornerRadius),
                        null,
                        cooldownColor,
                        0f,
                        new Vector2(0, shadeCornerRadius),
                        1f,
                        SpriteEffects.None,
                        0f
                    );
                }
                
                // Only draw right part if shade is covering the full width
                if (shadeWidth >= bounds.Width - shadeCornerRadius)
                {
                    Rectangle rightShadeRect = new Rectangle(
                        shadeRect.X + shadeRect.Width - shadeCornerRadius,
                        shadeRect.Y + shadeCornerRadius,
                        shadeCornerRadius,
                        Math.Max(0, shadeRect.Height - 2 * shadeCornerRadius)
                    );
                    
                    if (rightShadeRect.Height > 0 && rightShadeRect.X <= bounds.X + bounds.Width - shadeCornerRadius)
                    {
                        spriteBatch.Draw(texture, rightShadeRect, cooldownColor);
                    }
                    
                    // Top-right corner
                    spriteBatch.Draw(
                        circleTexture,
                        new Vector2(bounds.X + bounds.Width, shadeRect.Y + shadeCornerRadius),
                        null,
                        cooldownColor,
                        0f,
                        new Vector2(circleTexture.Width, shadeCornerRadius),
                        1f,
                        SpriteEffects.None,
                        0f
                    );
                    
                    // Bottom-right corner
                    spriteBatch.Draw(
                        circleTexture,
                        new Vector2(bounds.X + bounds.Width, shadeRect.Y + shadeRect.Height - shadeCornerRadius),
                        null,
                        cooldownColor,
                        0f,
                        new Vector2(circleTexture.Width, shadeCornerRadius),
                        1f,
                        SpriteEffects.None,
                        0f
                    );
                }
            }
        }
    }
}
