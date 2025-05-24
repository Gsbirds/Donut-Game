using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace monogame.Sprites
{
    public class DonutHole : Sprite
    {
        private Texture2D spriteSheet;
        private readonly Rectangle[] animationFrames;
        private int currentFrameIndex;
        private float animationTimer;
        private const float FRAME_DURATION = 0.3f; // Slower animation speed
        private Vector2 offsetFromDonut; 
        private Donut parentDonut; 

        public DonutHole(Texture2D texture, Donut donut, Vector2 offset, float speed) 
            : base(texture, donut.Position + offset, speed)
        {
            spriteSheet = texture;
            parentDonut = donut;
            offsetFromDonut = offset;
            
            int frameWidth = texture.Width / 6; 
            int frameHeight = texture.Height;
            
            animationFrames = new Rectangle[6];
            for (int i = 0; i < 6; i++)
            {
                animationFrames[i] = new Rectangle(i * frameWidth, 0, frameWidth, frameHeight);
            }
            
            currentFrameIndex = 0;
            animationTimer = 0f;
            
            showHealthBar = false;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            animationTimer += deltaTime;
            if (animationTimer >= FRAME_DURATION)
            {
                animationTimer = 0f;
                currentFrameIndex = (currentFrameIndex + 1) % animationFrames.Length;
            }
            
            position = parentDonut.Position + offsetFromDonut;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            float scale = 0.18f; 
            
            spriteBatch.Draw(
                texture,
                position,
                animationFrames[currentFrameIndex],
                Color.White,
                0f,
                new Vector2(animationFrames[currentFrameIndex].Width / 2, animationFrames[currentFrameIndex].Height / 2),
                new Vector2(scale, scale),
                SpriteEffects.None,
                0f
            );
        }
    }
}
