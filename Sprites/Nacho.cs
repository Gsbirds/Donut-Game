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
        private float health;
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
        public float Health => health;
        public bool IsDefeated => isDefeated;
        public Direction FacingDirection => facingDirection;

        public Nacho(Texture2D texture, Texture2D openMouthTexture, Vector2 position, float speed) 
            : base(texture, position, speed)
        {
            this.openMouthTexture = openMouthTexture;
            health = 4f;
            rotationSpeed = MathHelper.Pi / 2; // 90 degrees per second
            rotatingRight = true;
            facingDirection = Direction.Down;
            currentAnimationIndex = 1;

            // Initialize animations for each direction
            animations = new SpriteAnimation[4]; // Up, Right, Down, Left
            for (int i = 0; i < 4; i++)
            {
                animations[i] = new SpriteAnimation(AnimationFrames.GetCurrentRectanglesNacho((Direction)i, false, false, false), 0.1f);
            }
        }

        protected Nacho(Texture2D texture, Vector2 position, float speed)
            : base(texture, position, speed)
        {
            health = 4f;
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
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Update(deltaTime, targetPosition);
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

        public void TakeDamage(float damage)
        {
            health = MathHelper.Max(0, health - damage);
            if (health <= 0)
            {
                isDefeated = true;
            }
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
            
            spriteBatch.Draw(texture, position, currentFrame, Color.White, rotation,
                new Vector2(currentFrame.Width / 2, currentFrame.Height / 2),
                Vector2.One, SpriteEffects.None, 0f);
        }
    }
}
