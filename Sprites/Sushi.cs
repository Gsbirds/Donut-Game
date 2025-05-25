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
        private bool hasDealtDamageThisAttack = false; 
        private SpriteAnimation[] animations;
        private Vector2 targetPosition;
        private const float AttackRange = 150f;
        private const float AttackDamage = 15f;
        private float moveSpeed;
        
        private Texture2D hitTexture; 
        private bool isHit = false; 
        private float hitAnimationTimer = 0f; 
        private const float HIT_ANIMATION_DURATION = 0.5f; 
        private Direction hitDirection; 

        public event Action<float> OnDamageDealt;

        public bool IsDefeated => isDefeated;
        public Direction FacingDirection => facingDirection;
        public bool IsAttacking => isAttacking;

        public Sushi(Texture2D texture, Texture2D hitTexture, Vector2 position, float speed) 
            : base(texture, position, speed)
        {
            maxHealth = 100f;
            currentHealth = maxHealth;
            attackCooldown = 3.0f; 
            facingDirection = Direction.Down;
            moveSpeed = speed;
            animations = new SpriteAnimation[4];
            this.hitTexture = hitTexture;
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
            
            if (currentHealth <= 0)
            {
                isDefeated = true;
                isAttacking = false;
                return;
            }

            if (isHit)
            {
                hitAnimationTimer += deltaTime;
                if (hitAnimationTimer >= HIT_ANIMATION_DURATION)
                {
                    isHit = false;
                    hitAnimationTimer = 0f;
                }
            }

            if (isAttacking)
            {
                attackTimer += deltaTime;
                
                if (attackTimer >= 0.5f && attackTimer <= 0.7f && !hasDealtDamageThisAttack)
                {
                    float distanceToTarget = Vector2.Distance(Position, targetPosition);
                    if (distanceToTarget <= AttackRange)
                    {
                        OnDamageDealt?.Invoke(AttackDamage);
                        hasDealtDamageThisAttack = true;
                    }
                }
                
                if (attackTimer >= attackCooldown)
                {
                    isAttacking = false;
                    attackTimer = 0f;
                    useAttackFrame = false;
                    hasDealtDamageThisAttack = false; 
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
            
            if (distance <= AttackRange && !isAttacking)
            {
                StartAttack();
            }
            
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
                facingDirection = direction.X > 0 ? Direction.Left : Direction.Right;
            }
            else
            {
                facingDirection = direction.Y > 0 ? Direction.Up : Direction.Down;
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
            
            if (damageDealt)
            {
                isHit = true;
                hitAnimationTimer = 0f;
                hitDirection = facingDirection; 
                
                if (currentHealth <= 0)
                {
                    isDefeated = true;
                }
            }
            
            return damageDealt;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (isHit)
            {
                int frameWidth = 110;
                int frameHeight = 133;
                
                int row = 0;
                
                Direction directionToUse = hitDirection; 
                
                if (directionToUse == Direction.Right)
                {
                    row = 1; 
                }
                else if (directionToUse == Direction.Left)
                {
                    row = 3; 
                }
                else if (directionToUse == Direction.Down)
                {
                    row = 2; 
                }
                else if (directionToUse == Direction.Up)
                {
                    row = 0; 
                }
                
                int frameX = (int)(3.75 * frameWidth);
                
                Rectangle hitFrame = new Rectangle(
                    frameX - 5,      
                    (row * frameHeight) + (int)(frameHeight * 1.3f), 
                    frameWidth + 15, 
                    frameHeight
                );
                
                Vector2 origin = new Vector2(frameWidth / 2, frameHeight / 2);
                
                spriteBatch.Draw(
                    hitTexture,
                    position,
                    hitFrame,
                    Color.White,
                    rotation,
                    origin,
                    Vector2.One,
                    SpriteEffects.None,
                    0f
                );
                DrawHealthBar(spriteBatch);
                return;
            }
            else
            {
                Rectangle currentFrame;
                
                if (isAttacking)
                {
                    int frameWidth = 110;
                    int frameHeight = 133;
                    
                    int attackFrame = attackTimer < 0.5f ? 3 : 4; 
                    
                    currentFrame = new Rectangle(
                        attackFrame * frameWidth,
                        (int)facingDirection * frameHeight,
                        frameWidth,
                        frameHeight
                    );
                }
                else
                {
                    currentFrame = animations[(int)facingDirection].GetCurrentFrame();
                }
                
                Color spriteColor;
                if (isDefeated || currentHealth <= 0)
                {
                    spriteColor = Color.Gray;
                }
                else if (isInvulnerable && (int)(invulnerabilityTimer * 10) % 2 == 0)
                {
                    spriteColor = Color.Red;
                }
                else
                {
                    spriteColor = Color.White;
                }
                
                Vector2 origin = new Vector2(currentFrame.Width / 2, currentFrame.Height / 2);
                
                spriteBatch.Draw(
                    texture,
                    position,
                    currentFrame,
                    spriteColor,
                    rotation,
                    origin,
                    Vector2.One,
                    SpriteEffects.None,
                    0f
                );
                
                DrawHealthBar(spriteBatch);
            }
        }

        public override Rectangle GetBounds()
        {
            return new Rectangle(
                (int)position.X - 15,
                (int)position.Y - 18,
                30,
                36
            );
        }
    }
}
