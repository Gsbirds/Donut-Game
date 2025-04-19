using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using monogame.Animation;
using System;

namespace monogame.Sprites
{
    public class Sushi : Sprite
    {
        private Direction facingDirection;
        private bool isDefeated;
        private bool isAttacking;
        private float attackTimer;
        private float attackCooldown;
        private bool useAttackFrame = false;
        private byte currentAnimationIndex = 0;
        private SpriteAnimation[] animations;
        private Vector2 targetPosition;
        private const float AttackRange = 150f;
        private const float AttackDamage = 15f;
        private float moveSpeed;
        
        public event Action<float> OnDamageDealt;

        public bool IsDefeated => isDefeated;
        public Direction FacingDirection => facingDirection;
        public bool IsAttacking => isAttacking;

        public Sushi(Texture2D texture, Vector2 position, float speed) 
            : base(texture, position, speed)
        {
            maxHealth = 100f;
            currentHealth = maxHealth;
            attackCooldown = 1.5f;
            facingDirection = Direction.Down;
            moveSpeed = speed;
            animations = new SpriteAnimation[4];
            InitializeAnimations();
        }
        
        private void InitializeAnimations()
        {
            int frameWidth = 110;
            int frameHeight = 133;
            
            for (int dir = 0; dir < 4; dir++)
            {
                Rectangle[] frames = new Rectangle[3];
                for (int frame = 0; frame < 3; frame++)
                {
                    frames[frame] = new Rectangle(
                        frame * frameWidth,
                        dir * frameHeight,
                        frameWidth,
                        frameHeight
                    );
                }
                
                animations[dir] = new SpriteAnimation(frames, 0.15f, true);
            }
        }

        public override void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            base.Update(gameTime);

            if (isAttacking)
            {
                attackTimer += deltaTime;
                
                if (attackTimer < 0.2f)
                {
                    float distanceToTarget = Vector2.Distance(Position, targetPosition);
                    if (distanceToTarget <= AttackRange)
                    {
                        OnDamageDealt?.Invoke(AttackDamage);
                    }
                }
                
                if (attackTimer >= attackCooldown)
                {
                    isAttacking = false;
                    attackTimer = 0f;
                    useAttackFrame = false;
                }
            }
            
            animations[(int)facingDirection].Update(gameTime);
            
            if (!isDefeated)
            {
                MoveTowardsTarget(deltaTime);
            }
            
            if (currentHealth <= 0)
            {
                isDefeated = true;
            }
        }
        
        private void MoveTowardsTarget(float deltaTime)
        {
            Vector2 direction = targetPosition - Position;
            float distance = direction.Length();
            
            if (distance > 20f)
            {
                if (distance > 0)
                    direction /= distance;
                
                Position += direction * moveSpeed * deltaTime;
                
                UpdateFacingDirection(direction);
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

        public void StartAttack()
        {
            if (!isAttacking && !isDefeated)
            {
                isAttacking = true;
                attackTimer = 0f;
                useAttackFrame = true;
            }
        }
        
        public void SetTargetPosition(Vector2 target)
        {
            targetPosition = target;
        }

        public void SetDirection(Direction direction)
        {
            facingDirection = direction;
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

        public override void Draw(SpriteBatch spriteBatch)
        {
            Rectangle currentFrame = animations[(int)facingDirection].GetCurrentFrame();
            
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

        public Rectangle GetBounds()
        {
            return new Rectangle(
                (int)position.X - 48,
                (int)position.Y - 64,
                96,
                128
            );
        }
    }
}
