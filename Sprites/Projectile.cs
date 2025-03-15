using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace monogame.Sprites
{
    public class Projectile : Sprite
    {
        private Vector2 direction;
        private bool isActive;
        private float rotationSpeed;
        private bool hasDealtDamage;

        public bool IsActive => isActive;
        public bool HasDealtDamage => hasDealtDamage;

        public Projectile(Texture2D texture, Vector2 position, float speed) 
            : base(texture, position, speed)
        {
            rotationSpeed = MathHelper.TwoPi;
            Reset();
        }

        public void Launch(Vector2 startPosition, Vector2 direction)
        {
            this.position = startPosition;
            this.direction = Vector2.Normalize(direction);
            isActive = true;
            hasDealtDamage = false;
        }

        public void Reset()
        {
            isActive = false;
            hasDealtDamage = false;
            rotation = 0f;
        }

        public override void Update(GameTime gameTime)
        {
            if (!isActive) return;

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            position += direction * speed * deltaTime;

            rotation += rotationSpeed * deltaTime;
            
            rotation %= MathHelper.TwoPi;
        }

        public void SetDealtDamage()
        {
            hasDealtDamage = true;
        }

        public Rectangle GetBounds()
        {
            return new Rectangle(
                (int)position.X - texture.Width / 2,
                (int)position.Y - texture.Height / 2,
                texture.Width,
                texture.Height
            );
        }

        protected override void Draw(SpriteBatch spriteBatch, Rectangle sourceRectangle)
        {
            if (!isActive) return;
            base.Draw(spriteBatch, sourceRectangle);
        }
    }
}
