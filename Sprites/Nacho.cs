using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using monogame.Animation;
using System;

namespace monogame.Sprites
{
    public class Nacho : Sprite
    {
        private Direction facingDirection;
        private SpriteAnimation[] animations;
        private bool isDefeated;
        private float rotationSpeed;
        private bool rotatingRight;
        private bool useOpenMouthFrame;
        private bool usePostHitFrame;
        private bool useBlinkingFrame;
        private Texture2D openMouthTexture;
        private byte currentAnimationIndex;
        private Vector2 targetPosition;
        private new float rotation;
        public bool IsDefeated => isDefeated || currentHealth <= 0;
        public Direction FacingDirection => facingDirection;

        public Nacho(Texture2D texture, Texture2D openMouthTexture, Vector2 position, float speed) 
            : base(texture, position, speed)
        {
            this.openMouthTexture = openMouthTexture;
            maxHealth = 120f;
            currentHealth = maxHealth;
            rotationSpeed = MathHelper.Pi / 2;
            rotatingRight = true;
            facingDirection = Direction.Down;
            currentAnimationIndex = 1;

            animations = new SpriteAnimation[4];
            for (int i = 0; i < 4; i++)
            {
                animations[i] = new SpriteAnimation(AnimationFrames.GetCurrentRectanglesNacho((Direction)i, false, false, false), 0.1f);
            }
        }

        protected Nacho(Texture2D texture, Vector2 position, float speed)
            : base(texture, position, speed)
        {
            // Set nacho-specific health
            maxHealth = 120f;
            currentHealth = maxHealth;
            rotationSpeed = MathHelper.Pi / 2;
            rotatingRight = true;
            facingDirection = Direction.Down;
            currentAnimationIndex = 1;
        }

        public void SetTargetPosition(Vector2 position)
        {
            targetPosition = position;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Update(deltaTime, targetPosition);
            
            if (currentHealth <= 0)
            {
                isDefeated = true;
            }
        }

        private void Update(float deltaTime, Vector2 targetPosition)
        {
            if (isDefeated) return;

            currentAnimationIndex = (byte)((DateTime.Now.Ticks / 2000000) % 3);

            if (rotatingRight)
            {
                rotation += rotationSpeed * deltaTime;
                if (rotation >= MathHelper.PiOver4)
                {
                    rotation = MathHelper.PiOver4;
                    rotatingRight = false;
                }
            }
            else
            {
                rotation -= rotationSpeed * deltaTime;
                if (rotation <= -MathHelper.PiOver4)
                {
                    rotation = -MathHelper.PiOver4;
                    rotatingRight = true;
                }
            }

            Vector2 direction = targetPosition - position;
            if (direction != Vector2.Zero)
            {
                direction.Normalize();
                position += direction * speed * deltaTime;
            }

            if (Math.Abs(direction.X) > Math.Abs(direction.Y))
            {
                facingDirection = direction.X > 0 ? Direction.Right : Direction.Left;
            }
            else
            {
                facingDirection = direction.Y > 0 ? Direction.Down : Direction.Up;
            }
        }

        public void SetFacingDirection(Direction direction)
        {
            facingDirection = direction;
        }

        public void SetOpenMouthFrame(bool value)
        {
            useOpenMouthFrame = value;
        }

        public void SetPostHitFrame(bool value)
        {
            usePostHitFrame = value;
        }

        public void SetBlinkingFrame(bool value)
        {
            useBlinkingFrame = value;
        }

        public override bool TakeDamage(float damage)
        {
            bool damageDealt = base.TakeDamage(damage);
            
            if (damageDealt && currentHealth <= 0)
            {
                isDefeated = true;
            }
            
            return damageDealt;
        }

        public void SetOpenMouth(bool open)
        {
            useOpenMouthFrame = open;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var currentRectangles = AnimationFrames.GetCurrentRectanglesNacho(
                facingDirection,
                usePostHitFrame,
                useOpenMouthFrame,
                useBlinkingFrame
            );

            base.Draw(spriteBatch, currentRectangles[currentAnimationIndex]);
        }

        protected override void Draw(SpriteBatch spriteBatch, Rectangle sourceRectangle)
        {
            var texture = useOpenMouthFrame ? openMouthTexture : this.texture;
            var currentFrame = animations[(int)facingDirection].GetCurrentFrame();
            
            Color spriteColor = isInvulnerable && (int)(invulnerabilityTimer * 10) % 2 == 0 ? Color.Red : Color.White;
            
            spriteBatch.Draw(texture, position, currentFrame, spriteColor, rotation,
                new Vector2(currentFrame.Width / 2, currentFrame.Height / 2),
                Vector2.One, SpriteEffects.None, 0f);
                
            DrawHealthBar(spriteBatch);
        }
    }
}
