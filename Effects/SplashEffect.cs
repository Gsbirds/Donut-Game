using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace monogame.Effects
{
    public class SplashEffect
    {
        private readonly Texture2D texture;
        private Vector2 position;
        private float timer;
        private bool isActive;
        private const float DURATION = 1.0f;

        public bool IsActive => isActive;
        public Vector2 Position => position;

        public SplashEffect(Texture2D texture)
        {
            this.texture = texture;
            this.isActive = false;
            this.timer = 0f;
        }

        public void Activate(Vector2 position)
        {
            this.position = position;
            this.isActive = true;
            this.timer = 0f;
        }

        public void Update(float deltaTime)
        {
            if (!isActive) return;

            timer += deltaTime;
            if (timer >= DURATION)
            {
                isActive = false;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!isActive) return;

            spriteBatch.Draw(
                texture,
                new Vector2(position.X - 32, position.Y - 32),
                null,
                Color.White
            );
        }
    }
}
