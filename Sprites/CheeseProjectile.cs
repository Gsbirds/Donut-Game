using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace monogame.Sprites
{
    public class CheeseProjectile
    {
        // Appearance
        public Texture2D Texture { get; private set; }
        public Texture2D SplashTexture { get; private set; }
        public Vector2 Position { get; set; }
        public float Rotation { get; set; }
        
        // Movement
        private Vector2 direction;
        private float speed = 300f;
        
        // State
        public bool IsActive { get; private set; }
        public bool HasDealtDamage { get; private set; }
        private const float DAMAGE_AMOUNT = 10f;
        
        public CheeseProjectile(Texture2D texture, Texture2D splashTexture)
        {
            Texture = texture;
            SplashTexture = splashTexture;
            Position = Vector2.Zero;
            IsActive = false;
            HasDealtDamage = false;
        }
        
        public void Launch(Vector2 startPosition, Vector2 targetDirection)
        {
            Position = startPosition;
            
            // Normalize direction and set rotation
            direction = Vector2.Normalize(targetDirection);
            Rotation = (float)Math.Atan2(direction.Y, direction.X);
            
            IsActive = true;
            HasDealtDamage = false;
        }
        
        public void Update(GameTime gameTime)
        {
            if (!IsActive) return;
            
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Position += direction * speed * deltaTime;
        }
        
        public void DealDamageTo(Donut target)
        {
            if (IsActive && !HasDealtDamage)
            {
                target.TakeDamage(DAMAGE_AMOUNT);
                HasDealtDamage = true;
            }
        }
        
        public void Reset()
        {
            IsActive = false;
            HasDealtDamage = false;
        }
    }
}
