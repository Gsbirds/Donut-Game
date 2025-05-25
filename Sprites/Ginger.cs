using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using monogame.Animation;
using System;

namespace monogame.Sprites
{
    public class Ginger : Sprite
    {
        private Direction facingDirection = Direction.Down;
        private byte currentAnimationIndex = 0;
        private SpriteAnimation[] animations;
        private float animationTimer = 0f;
        private const float AnimationThreshold = 200f;
        
        private bool isAttacking = false;
        private float attackTimer = 0f;
        private const float AttackCooldown = 1.5f;
        private bool useAttackFrame = false;
        private float attackRange = 150f;
        private float attackDamage = 10f;
        
        private Vector2 targetPosition;
        private bool isMoving = false;
        private float moveSpeed;
        private float directionChangeTimer = 0f;
        private const float DirectionChangeDuration = 2.0f;
        
        private bool useBlinkingFrame = false;
        private float blinkingTimer = 0f;
        private const float BlinkDuration = 0.2f;
        private bool isDefeated = false;
        
        public event Action<float> OnDamageDealt;
        
        public bool IsDefeated => isDefeated;
        public bool IsAttacking => isAttacking;
        public Direction FacingDirection => facingDirection;
        
        public Ginger(Texture2D texture, Vector2 position, float speed)
            : base(texture, position, speed)
        {
            maxHealth = 100f;
            currentHealth = maxHealth;
            moveSpeed = speed;
            
            animations = new SpriteAnimation[4];
            for (int i = 0; i < 4; i++)
            {
                animations[i] = new SpriteAnimation();
            }
            
            InitializeAnimations();
        }
        
        private void InitializeAnimations()
        {
            int frameWidth = 98;
            int frameHeight = 85;
            float frameDuration = 0.2f;
            
            for (int dir = 0; dir < 4; dir++)
            {
                Rectangle[] frames = new Rectangle[2];
                for (int frame = 0; frame < 2; frame++)
                {
                    frames[frame] = new Rectangle(
                        frame * frameWidth,
                        dir * frameHeight,
                        frameWidth,
                        frameHeight
                    );
                }
                
                animations[dir] = new SpriteAnimation(frames, frameDuration, true);
            }
        }
        
        public void SetTargetPosition(Vector2 target)
        {
            targetPosition = target;
        }
        
        public void StartAttack()
        {
            if (!isAttacking && !isDefeated)
            {
                isAttacking = true;
                attackTimer = 0f;
                useAttackFrame = true;
            }
        }
        
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            if (currentHealth <= 0)
            {
                isDefeated = true;
            }
            
            if (!isDefeated)
            {
                UpdateAttack(deltaTime);
                UpdateMovement(deltaTime);
            }
            
            UpdateAnimation(deltaTime, gameTime);
        }
        
        private void UpdateAttack(float deltaTime)
        {
            if (isDefeated) return;
            
            if (isAttacking)
            {
                attackTimer += deltaTime;
                
                if (attackTimer < 0.2f)
                {
                    float distanceToTarget = Vector2.Distance(Position, targetPosition);
                    if (distanceToTarget <= attackRange)
                    {
                        OnDamageDealt?.Invoke(attackDamage);
                    }
                }
                
                if (attackTimer >= AttackCooldown)
                {
                    isAttacking = false;
                    attackTimer = 0f;
                    useAttackFrame = false;
                }
            }
        }
        
        private void UpdateMovement(float deltaTime)
        {
            if (isDefeated) return;
            
            Vector2 direction = targetPosition - Position;
            float distance = direction.Length();
            
            if (distance > 20f)
            {
                isMoving = true;
                
                if (distance > 0)
                    direction /= distance;
                
                Position += direction * moveSpeed * deltaTime;
                
                UpdateFacingDirection(direction);
            }
            else
            {
                isMoving = false;
            }
            
            directionChangeTimer += deltaTime;
            if (directionChangeTimer >= DirectionChangeDuration)
            {
                directionChangeTimer = 0f;
                
                if (Random.Shared.NextDouble() < 0.3)
                {
                    facingDirection = (Direction)Random.Shared.Next(0, 4);
                }
            }
        }
        
        private void UpdateFacingDirection(Vector2 direction)
        {
            if (Math.Abs(direction.X) > Math.Abs(direction.Y))
            {
                facingDirection = direction.X > 0 ? Direction.Right : Direction.Left;
            }
            else
            {
                facingDirection = direction.Y > 0 ? Direction.Down : Direction.Up;
            }
        }
        
        private void UpdateAnimation(float deltaTime, GameTime gameTime)
        {
            if (isMoving)
            {
                animationTimer += deltaTime * 1000;
                if (animationTimer >= AnimationThreshold)
                {
                    currentAnimationIndex = (byte)((currentAnimationIndex + 1) % 2);
                    animationTimer = 0f;
                }
            }
            
            blinkingTimer += deltaTime;
            if (blinkingTimer >= BlinkDuration * 5)
            {
                useBlinkingFrame = !useBlinkingFrame;
                blinkingTimer = 0f;
            }
            
            animations[(int)facingDirection].Update(gameTime);
        }
        
        public Rectangle GetCurrentFrame()
        {
            return GetGingerRectangle(facingDirection, currentAnimationIndex);
        }
        
        private Rectangle GetGingerRectangle(Direction direction, int animationIndex)
        {
            int frameWidth = 98;
            int frameHeight = 85;
            
            int row = direction switch
            {
                Direction.Up => 0,
                Direction.Right => 1,
                Direction.Down => 2,
                Direction.Left => 3,
                _ => 2
            };
            
            int column = animationIndex % 2;
            
            return new Rectangle(column * frameWidth, row * frameHeight, frameWidth, frameHeight);
        }
        
        public override void Draw(SpriteBatch spriteBatch)
        {
            Rectangle currentFrame = GetCurrentFrame();
            
            Color spriteColor = isInvulnerable && (int)(invulnerabilityTimer * 10) % 2 == 0 ? Color.Red : Color.White;
            
            spriteBatch.Draw(
                texture,
                position,
                currentFrame,
                spriteColor,
                rotation,
                new Vector2(currentFrame.Width / 2, currentFrame.Height / 2),
                Vector2.One,
                SpriteEffects.None,
                0f
            );
            
            DrawHealthBar(spriteBatch);
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
    }
}
