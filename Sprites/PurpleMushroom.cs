using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace monogame.Sprites
{
    public class PurpleMushroom : Sprite
    {
        private readonly Rectangle[] frames;
        private int currentFrame;
        private float frameTimer;
        private const float FRAME_TIME = 0.2f;

        public PurpleMushroom(Texture2D texture, Vector2 position, Rectangle[] frames) 
            : base(texture, position, 0f)
        {
            this.frames = frames;
            this.currentFrame = 0;
            this.frameTimer = 0f;
        }

        public void Update(float deltaTime)
        {
            frameTimer += deltaTime;
            if (frameTimer >= FRAME_TIME)
            {
                currentFrame = (currentFrame + 1) % frames.Length;
                frameTimer = 0f;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                texture,
                position,
                frames[currentFrame],
                Color.White
            );
        }
    }
}
