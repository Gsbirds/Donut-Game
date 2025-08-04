using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using monogame.Animation;
using System;

namespace monogame.Sprites
{
    public class Lomein : Sprite
    {
        private Direction lomeinFacingDirection;
        private bool isLomeinAttacking;
        private float lomeinAttackTimer;
        private const float lomeinAttackCooldown = 1.0f;
        private bool lomeinMoving;
        private byte currentAnimationIndexLomein;
        private byte currentAttackFrame;
        private float attackAnimationTimer;
        private const int frameWidth = 1200; 
        private const int frameHeight = 1200; 
        private float animationTimer;
        private const float AnimationThreshold = 300f;
        private const float MinDistanceFromDonut = 170f;
        private const float AttackRange = 100f;
        private const float AttackDamage = 10.0f;
        private Vector2 targetPosition;
        private Vector2 donutPosition;
        private float halfScreenHeight;
        public event Action<float> OnDamageDealt;
        
        private Texture2D hitTexture; 
        private bool isHit = false; 
        private float hitAnimationTimer = 0f;
        private const float HIT_ANIMATION_DURATION = 0.5f; 
        private Direction hitDirection; 

        private float periodicAttackTimer;
        private const float PeriodicAttackInterval = 1.5f;
        private bool canPeriodicAttack;

        public Direction FacingDirection => lomeinFacingDirection;
        public bool IsAttacking => isLomeinAttacking;
        public bool IsMoving
        {
            get => lomeinMoving;
            set => lomeinMoving = value;
        }

        public Lomein(Texture2D texture, Vector2 position, float speed) 
            : base(texture, position, speed)
        {
            maxHealth = 80f;
            currentHealth = maxHealth;
            
            lomeinFacingDirection = Direction.Down;
            currentAnimationIndexLomein = 0;
            currentAttackFrame = 0;
            animationTimer = 0f;
            attackAnimationTimer = 0f;
            lomeinAttackTimer = 0f;
            lomeinMoving = false;
            isLomeinAttacking = false;
            halfScreenHeight = 325f;
            periodicAttackTimer = 0f;
            canPeriodicAttack = true;
            targetPosition = new Vector2(1000, 1000);
        }

        public void Update(float deltaTime, Vector2 targetPos)
        {
            if (currentHealth <= 0)
            {
                lomeinMoving = false;
                isLomeinAttacking = false;
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
            
            targetPosition = targetPos;

            Vector2 directionToDonut = targetPosition - Position;
            float distanceToDonut = directionToDonut.Length();
            lomeinMoving = true;
            
            if (directionToDonut != Vector2.Zero)
            {
                directionToDonut.Normalize();
                
                if (distanceToDonut > 40.0f)
                {
                    Position += directionToDonut * Speed * deltaTime;
                }
                else if (distanceToDonut < 30.0f)
                {
                    Position -= directionToDonut * Speed * 0.7f * deltaTime;
                }
                
                if (!isLomeinAttacking)
                {
                    if (Math.Abs(directionToDonut.X) > Math.Abs(directionToDonut.Y))
                    {
                        lomeinFacingDirection = directionToDonut.X > 0 ? Direction.Right : Direction.Left;
                    }
                    else
                    {
                        lomeinFacingDirection = directionToDonut.Y > 0 ? Direction.Down : Direction.Up;
                    }
                }
            }

            if (lomeinMoving)
            {
                animationTimer += deltaTime * 1000f;
                if (animationTimer >= AnimationThreshold)
                {
                    animationTimer = 0f;
                    currentAnimationIndexLomein = (byte)((currentAnimationIndexLomein + 1) % 4); 
                }
            }

            periodicAttackTimer += deltaTime;
            if (periodicAttackTimer >= PeriodicAttackInterval)
            {
                canPeriodicAttack = true;
            }

            if (distanceToDonut <= AttackRange && canPeriodicAttack)
            {
                StartAttack();
            }

            if (isLomeinAttacking)
            {
                lomeinAttackTimer += deltaTime;
                if (lomeinAttackTimer >= lomeinAttackCooldown)
                {
                    isLomeinAttacking = false;
                    lomeinAttackTimer = 0f;
                }
            }

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

        public void Draw(SpriteBatch spriteBatch)
        {
            if (isHit)
            {
                Color hitColor = Color.Red;
                
                Rectangle currentFrame = new Rectangle(currentAnimationIndexLomein * frameWidth, 0, frameWidth, frameHeight);
                
                Vector2 hitOrigin = new Vector2(frameWidth / 2, frameHeight / 2);
                float hitScale = 0.3f; // Same scale as normal rendering
                
                spriteBatch.Draw(
                    texture,
                    position,
                    currentFrame,
                    hitColor,
                    0f,
                    hitOrigin,
                    hitScale,
                    SpriteEffects.None,
                    0f
                );
                
                DrawHealthBar(spriteBatch);
                return;
            }
            
            Rectangle frame = new Rectangle(currentAnimationIndexLomein * frameWidth, 0, frameWidth, frameHeight);

            Color spriteColor;
            if (currentHealth <= 0)
            {
                spriteColor = Color.Gray;
            }
            else
            {
                spriteColor = isInvulnerable && (int)(invulnerabilityTimer * 10) % 2 == 0 ? Color.Red : Color.White;
            }

            Vector2 origin = new Vector2(frameWidth / 2, frameHeight / 2);
            float scale = 0.3f; // Make Lomein sprites much larger and more visible
                
            spriteBatch.Draw(
                texture,
                position,
                frame,
                spriteColor,
                0f,
                origin,
                scale,
                SpriteEffects.None,
                0f
            );
            
            DrawHealthBar(spriteBatch);
        }

        public override Rectangle GetBounds()
        {
            int scaledWidth = (int)(frameWidth * 0.3f);
            int scaledHeight = (int)(frameHeight * 0.3f);
            return new Rectangle(
                (int)position.X - scaledWidth / 2, 
                (int)position.Y - scaledHeight / 2,
                scaledWidth,                    
                scaledHeight                     
            );
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (currentHealth <= 0)
            {
            }
        }

        public void SetFacingDirection(Direction direction)
        {
            lomeinFacingDirection = direction;
        }

        public void SetTargetPosition(Vector2 position)
        {
            targetPosition = position;
        }

        public void StartAttack()
        {
            float distanceToDonut = Vector2.Distance(Position, targetPosition);
            if (!isLomeinAttacking && canPeriodicAttack)
            {
                isLomeinAttacking = true;
                lomeinAttackTimer = 0f;
                canPeriodicAttack = false;
                periodicAttackTimer = 0f;
                
                OnDamageDealt?.Invoke(AttackDamage);
            }
        }
        
        public override bool TakeDamage(float damage)
        {
            bool damageDealt = base.TakeDamage(damage);
            
            if (damageDealt)
            {
                isHit = true;
                hitAnimationTimer = 0f;
                hitDirection = lomeinFacingDirection; 
            }
            
            return damageDealt;
        }
    }
}
