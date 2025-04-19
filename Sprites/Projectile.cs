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
        private float damageAmount;

        public bool IsActive => isActive;
        public bool HasDealtDamage => hasDealtDamage;
        public new Vector2 Position => position;
        public float Rotation => rotation;

        public Projectile(Texture2D texture, Vector2 position, float speed, float damage = 10f) 
            : base(texture, position, speed)
        {
            rotationSpeed = MathHelper.TwoPi;
            damageAmount = damage;
            Reset();
            showHealthBar = false;
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

            if (position.X < -50 || position.Y < -50 || 
                position.X > 1920 || position.Y > 1080)
            {
                Reset();
            }
        }

        public void SetDealtDamage()
        {
            hasDealtDamage = true;
        }
        
        public float GetDamageAmount()
        {
            return damageAmount;
        }
        
        public bool DealDamageTo(Sprite target)
        {
            if (!isActive || hasDealtDamage) return false;
            
            bool damageDealt = target.TakeDamage(damageAmount);
            if (damageDealt)
            {
                hasDealtDamage = true;
            }
            
            return damageDealt;
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
            
            spriteBatch.Draw(texture, position, sourceRectangle, Color.White, rotation,
                new Vector2(sourceRectangle.Width / 2, sourceRectangle.Height / 2),
                Vector2.One, SpriteEffects.None, 0f);
        }
    }
}
