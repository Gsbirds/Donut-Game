using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace monogame.Sprites
{
    public abstract class Sprite
    {
        protected Texture2D texture;
        protected Vector2 position;
        protected Vector2 previousPosition;
        protected float speed;
        protected float rotation;
        
        // Health system
        protected float maxHealth;
        protected float currentHealth;
        protected bool isInvulnerable;
        protected float invulnerabilityTimer;
        protected const float InvulnerabilityDuration = 0.5f;
        protected bool showHealthBar = true;
        
        public Vector2 Position 
        { 
            get => position;
            set => position = value;
        }

        public float Speed
        {
            get => speed;
            set => speed = value;
        }

        public Texture2D Texture => texture;

        public float Health => currentHealth;
        public float MaxHealth => maxHealth;
        public bool IsInvulnerable => isInvulnerable;
        
        public Sprite(Texture2D texture, Vector2 position, float speed)
        {
            this.texture = texture;
            this.position = position;
            this.previousPosition = position;
            this.speed = speed;
            this.rotation = 0f;
            this.maxHealth = 100f;
            this.currentHealth = maxHealth;
            this.isInvulnerable = false;
            this.invulnerabilityTimer = 0f;
        }

        public virtual void Update(GameTime gameTime)
        {
            // Store previous position for collision detection
            previousPosition = position;
            
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (isInvulnerable)
            {
                invulnerabilityTimer += deltaTime;
                if (invulnerabilityTimer >= InvulnerabilityDuration)
                {
                    isInvulnerable = false;
                    invulnerabilityTimer = 0f;
                }
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            Draw(spriteBatch, new Rectangle(0, 0, texture.Width, texture.Height));
        }

        protected virtual void Draw(SpriteBatch spriteBatch, Rectangle sourceRectangle)
        {
            Color spriteColor = isInvulnerable && (int)(invulnerabilityTimer * 10) % 2 == 0 ? Color.Red : Color.White;
            
            spriteBatch.Draw(texture, position, sourceRectangle, spriteColor, rotation,
                new Vector2(sourceRectangle.Width / 2, sourceRectangle.Height / 2),
                Vector2.One, SpriteEffects.None, 0f);
                
            DrawHealthBar(spriteBatch);
        }
        
        protected void DrawHealthBar(SpriteBatch spriteBatch)
        {
            if (!showHealthBar) return;
            
            int healthBarWidth = 60;
            int healthBarHeight = 5;
            Vector2 healthBarPosition = new Vector2(
                position.X - healthBarWidth / 2,
                position.Y - 80
            );
            
            Rectangle backgroundRectangle = new Rectangle(
                (int)healthBarPosition.X,
                (int)healthBarPosition.Y,
                healthBarWidth,
                healthBarHeight
            );
            spriteBatch.Draw(Game1.WhitePixel, backgroundRectangle, Color.DarkGray);
            
            int currentHealthWidth = (int)(healthBarWidth * (currentHealth / maxHealth));
            Rectangle currentHealthRectangle = new Rectangle(
                (int)healthBarPosition.X,
                (int)healthBarPosition.Y,
                currentHealthWidth,
                healthBarHeight
            );
            
            Color healthColor = Color.Green;
            if (currentHealth < maxHealth * 0.6f)
                healthColor = Color.Yellow;
            if (currentHealth < maxHealth * 0.3f)
                healthColor = Color.Red;
                
            spriteBatch.Draw(Game1.WhitePixel, currentHealthRectangle, healthColor);
        }
        
        public virtual bool TakeDamage(float damage)
        {
            if (isInvulnerable) return false;
            
            currentHealth = MathHelper.Max(0, currentHealth - damage);
            isInvulnerable = true;
            invulnerabilityTimer = 0f;
            
            return true;
        }
        
        public virtual Rectangle GetBounds()
        {
            return new Rectangle(
                (int)(position.X - texture.Width / 4),
                (int)(position.Y - texture.Height / 4),
                texture.Width / 2,
                texture.Height / 2
            );
        }
        
        public bool CollidesWith(Sprite other)
        {
            if (other == this) return false;
            
            Rectangle thisBounds = GetBounds();
            Rectangle otherBounds = other.GetBounds();
            
            return thisBounds.Intersects(otherBounds);
        }
        
        public static void ResolveCollision(Sprite sprite1, Sprite sprite2)
        {
            if (!sprite1.CollidesWith(sprite2)) return;
            
            Rectangle bounds1 = sprite1.GetBounds();
            Rectangle bounds2 = sprite2.GetBounds();
            
            int overlapX = Math.Min(bounds1.Right, bounds2.Right) - Math.Max(bounds1.Left, bounds2.Left);
            int overlapY = Math.Min(bounds1.Bottom, bounds2.Bottom) - Math.Max(bounds1.Top, bounds2.Top);
            
            if (overlapX < overlapY)
            {
                if (bounds1.Center.X < bounds2.Center.X)
                {
                    sprite1.position.X -= overlapX / 2;
                    sprite2.position.X += overlapX / 2;
                }
                else
                {
                    sprite1.position.X += overlapX / 2;
                    sprite2.position.X -= overlapX / 2;
                }
            }
            else
            {
                if (bounds1.Center.Y < bounds2.Center.Y)
                {
                    sprite1.position.Y -= overlapY / 2;
                    sprite2.position.Y += overlapY / 2;
                }
                else
                {
                    sprite1.position.Y += overlapY / 2;
                    sprite2.position.Y -= overlapY / 2;
                }
            }
        }
    }
}
