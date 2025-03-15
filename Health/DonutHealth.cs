using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace monogame.Health
{
    public class DonutHealth
    {
        private int maxHealth;
        private int currentHealth;
        private Vector2 position;
        private float invulnerabilityTimer;
        private const float InvulnerabilityDuration = 1000f;
        private bool isInvulnerable;
        private float healthBarWidth = 200f;
        private float healthBarHeight = 20f;

        public bool IsDead => currentHealth <= 0;
        public bool IsInvulnerable => isInvulnerable;

        public event Action OnDeath;
        public event Action<int> OnHealthChanged;

        public DonutHealth(int maxHealth, Vector2 position)
        {
            this.maxHealth = maxHealth;
            this.currentHealth = maxHealth;
            this.position = position;
            this.isInvulnerable = false;
        }

        public bool TakeDamage(int damage)
        {
            if (isInvulnerable) return false;

            currentHealth = Math.Max(0, currentHealth - damage);
            OnHealthChanged?.Invoke(currentHealth);


            isInvulnerable = true;
            invulnerabilityTimer = 0f;

            if (currentHealth <= 0)
            {
                OnDeath?.Invoke();
            }

            return true;
        }

        public void Heal(int amount)
        {
            int oldHealth = currentHealth;
            currentHealth = Math.Min(maxHealth, currentHealth + amount);
            
            if (oldHealth != currentHealth)
            {
                OnHealthChanged?.Invoke(currentHealth);
            }
        }

        public void Update(GameTime gameTime, Vector2 donutPosition)
        {

            position = new Vector2(donutPosition.X, 50);


            if (isInvulnerable)
            {
                invulnerabilityTimer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (invulnerabilityTimer >= InvulnerabilityDuration)
                {
                    isInvulnerable = false;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {

            float screenCenterX = graphicsDevice.Viewport.Width / 2f;
            Vector2 healthBarPosition = new Vector2(screenCenterX - healthBarWidth / 2f, 20);


            var backgroundRect = new Rectangle(
                (int)healthBarPosition.X,
                (int)healthBarPosition.Y,
                (int)healthBarWidth,
                (int)healthBarHeight
            );
            spriteBatch.Draw(Game1.WhitePixel, backgroundRect, Color.DarkGray);


            float healthPercentage = (float)currentHealth / maxHealth;
            var healthRect = new Rectangle(
                backgroundRect.X,
                backgroundRect.Y,
                (int)(backgroundRect.Width * healthPercentage),
                backgroundRect.Height
            );


            Color healthColor = Color.Pink;
            if (isInvulnerable && (int)(invulnerabilityTimer / 100) % 2 == 0)
            {
                healthColor = Color.White;
            }

            spriteBatch.Draw(Game1.WhitePixel, healthRect, healthColor);
        }

        public void Reset()
        {
            currentHealth = maxHealth;
            isInvulnerable = false;
            invulnerabilityTimer = 0f;
            OnHealthChanged?.Invoke(currentHealth);
        }
    }
}
