using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace monogame.Effects
{
    public class Sparkle
    {
        private Vector2 position;
        private float scale;
        private float alpha;
        private float rotation;
        private float bobTimer;
        private float bobSpeed;
        private float bobHeight;
        private Color color;
        private bool isActive;
        
        private static Random random = new Random();
        
        public Sparkle()
        {
            isActive = false;
        }
        
        public void Activate(Vector2 startPosition)
        {
            position = startPosition;
            position.X += (float)(random.NextDouble() * 40 - 20);
            position.Y += (float)(random.NextDouble() * 40 - 20);
            
            scale = (float)(random.NextDouble() * 0.4 + 0.2f);
            
            alpha = (float)(random.NextDouble() * 0.6 + 0.4f);
            
            rotation = (float)(random.NextDouble() * MathHelper.TwoPi);
            
            bobSpeed = (float)(random.NextDouble() * 5 + 2);
            bobHeight = (float)(random.NextDouble() * 10 + 5);
            bobTimer = (float)(random.NextDouble() * MathHelper.TwoPi);
            
            int r = random.Next(200, 255);
            int g = random.Next(100, 180); 
            int b = random.Next(170, 230);
            color = new Color(r, g, b);
            
            isActive = true;
        }
        
        public void Update(GameTime gameTime)
        {
            if (!isActive) return;
            
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            rotation += deltaTime * 2;
            
            bobTimer += deltaTime * bobSpeed;
            
            alpha -= deltaTime * 0.5f;
            
            if (alpha <= 0)
            {
                isActive = false;
            }
        }
        
        public void Draw(SpriteBatch spriteBatch, Texture2D texture)
        {
            if (!isActive) return;
            
            float bobOffset = (float)Math.Sin(bobTimer) * bobHeight;
            Vector2 drawPosition = new Vector2(position.X, position.Y + bobOffset);
            
            spriteBatch.Draw(
                texture,
                drawPosition,
                null,
                color * alpha,
                rotation,
                new Vector2(texture.Width / 2, texture.Height / 2),
                scale,
                SpriteEffects.None,
                0f
            );
        }
        
        public bool IsActive => isActive;
    }
}
