using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace monogame.Animation
{
    public class SpriteAnimation
    {
        private Rectangle[] frames;
        private int currentFrame;
        private float frameTimer;
        private readonly float frameDuration;
        private bool isLooping;

        public SpriteAnimation(Rectangle[] frames, float frameDuration, bool isLooping = true)
        {
            this.frames = frames;
            this.frameDuration = frameDuration;
            this.isLooping = isLooping;
            Reset();
        }

        public SpriteAnimation()
        {
        }

        public void Update(GameTime gameTime)
        {
            frameTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            if (frameTimer >= frameDuration)
            {
                frameTimer = 0;
                if (currentFrame + 1 < frames.Length)
                {
                    currentFrame++;
                }
                else if (isLooping)
                {
                    currentFrame = 0;
                }
            }
        }

        public Rectangle GetCurrentFrame()
        {
            return frames[currentFrame];
        }

        public void Reset()
        {
            currentFrame = 0;
            frameTimer = 0;
        }
    }
}
