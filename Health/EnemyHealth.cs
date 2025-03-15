using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace monogame.Health
{
    public class EnemyHealth
    {
        private int maxHealth;
        private int currentHealth;
        private Vector2 position;
        private bool isVisible;
        private float displayTimer;
        private const float DisplayDuration = 2000f;

        public bool IsDead => currentHealth <= 0;
        
        public EnemyHealth(int maxHealth, Vector2 position)
        {
            this.maxHealth = maxHealth;
            this.currentHealth = maxHealth;
            this.position = position;
            this.isVisible = false;
        }

        public void TakeDamage(int damage)
        {
            currentHealth = Math.Max(0, currentHealth - damage);
            isVisible = true;
            displayTimer = 0f;
        }

        public void Heal(int amount)
        {
            currentHealth = Math.Min(maxHealth, currentHealth + amount);
        }

        public void Update(GameTime gameTime, Vector2 enemyPosition)
        {
            position = enemyPosition + new Vector2(0, -50);


            if (isVisible)
            {
                displayTimer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (displayTimer >= DisplayDuration)
                {
                    isVisible = false;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!isVisible) return;


            var backgroundRect = new Rectangle((int)position.X - 25, (int)position.Y - 5, 50, 10);
            spriteBatch.Draw(Game1.WhitePixel, backgroundRect, Color.DarkGray);


            float healthPercentage = (float)currentHealth / maxHealth;
            var healthRect = new Rectangle(
                backgroundRect.X,
                backgroundRect.Y,
                (int)(backgroundRect.Width * healthPercentage),
                backgroundRect.Height
            );
            spriteBatch.Draw(Game1.WhitePixel, healthRect, Color.Red);
        }

        public void SetPosition(Vector2 newPosition)
        {
            position = newPosition;
        }
    }
}
