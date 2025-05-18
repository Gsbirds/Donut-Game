using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace monogame.Effects
{
    public enum DonutColor
    {
        Normal,
        Pink,
        Yellow,
        Brown
    }
    
    public static class ColorReplacer
    {
        private static Dictionary<DonutColor, Texture2D> textureCache = new Dictionary<DonutColor, Texture2D>();

        public static Color GetDonutColor(DonutColor colorType)
        {
            switch (colorType)
            {
                case DonutColor.Pink:
                    return new Color(255, 51, 153); // Bright pink
                case DonutColor.Yellow:
                    return new Color(255, 215, 0);  // Gold yellow
                case DonutColor.Brown: 
                    return new Color(139, 69, 19);  // Brown
                default:
                    return Color.White;            // Normal
            }
        }

        public static Texture2D CreateColoredTexture(GraphicsDevice graphicsDevice, Texture2D originalTexture, DonutColor colorType)
        {
            Texture2D modifiedTexture = new Texture2D(graphicsDevice, originalTexture.Width, originalTexture.Height);
            
            Color[] originalData = new Color[originalTexture.Width * originalTexture.Height];
            originalTexture.GetData(originalData);
            
            Color[] newData = new Color[originalData.Length];
            
            Color replacementColor = GetDonutColor(colorType);
            
            for (int i = 0; i < originalData.Length; i++)
            {
                Color pixel = originalData[i];
                
                if (pixel.A > 0 && pixel.B > 100 && pixel.B > pixel.R + 30 && pixel.B > pixel.G + 30)
                {
                    newData[i] = new Color(replacementColor.R, replacementColor.G, replacementColor.B, pixel.A);
                }
                else
                {
                    newData[i] = pixel;
                }
            }
            
            modifiedTexture.SetData(newData);
            
            return modifiedTexture;
        }
    }
}
