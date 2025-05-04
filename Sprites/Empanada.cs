using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using monogame.Animation;
using System;

namespace monogame.Sprites
{
    public class Empanada : Sprite
    {
        private Direction empanadaFacingDirection;
        private bool isEmpanadaAttacking;
        private float empanadaAttackTimer;
        private const float empanadaAttackCooldown = 1.0f;
        private bool empanadaMoving;
        private byte currentAnimationIndexEmpanada;
        private byte currentAttackFrame;
        private float attackAnimationTimer;
        private const int frameWidth = 110;
        private const int attackFrameWidth = 160;
        private const int frameHeight = 133;
        private float animationTimer;
        private const float AnimationThreshold = 300f;
        private const float MinDistanceFromNacho = 170f;
        private const float AttackRange = 20f; 
        private const float AttackDamage = 10.0f;
        private Vector2 targetPosition;
        private Vector2 nachoPosition;
        private float halfScreenHeight;
        public event Action<float> OnDamageDealt;

        private float periodicAttackTimer;
        private const float PeriodicAttackInterval = 1.5f;
        private bool canPeriodicAttack;

        public Direction FacingDirection => empanadaFacingDirection;
        public bool IsAttacking => isEmpanadaAttacking;
        public bool IsMoving
        {
            get => empanadaMoving;
            set => empanadaMoving = value;
        }

        public Empanada(Texture2D texture, Vector2 position, float speed) 
            : base(texture, position, speed)
        {
            maxHealth = 80f;
            currentHealth = maxHealth;
            
            empanadaFacingDirection = Direction.Down;
            currentAnimationIndexEmpanada = 0;
            currentAttackFrame = 0;
            animationTimer = 0f;
            attackAnimationTimer = 0f;
            empanadaAttackTimer = 0f;
            empanadaMoving = false;
            isEmpanadaAttacking = false;
            halfScreenHeight = 325f;
            periodicAttackTimer = 0f;
            canPeriodicAttack = true;
            targetPosition = new Vector2(1000, 1000);
        }

        public void Update(float deltaTime, Vector2 targetPos, Vector2 nachoPos)
        {
            if (currentHealth <= 0)
            {
                empanadaMoving = false;
                isEmpanadaAttacking = false;
                return;
            }
            
            targetPosition = targetPos;
            nachoPosition = nachoPos;

            Vector2 directionToDonut = targetPosition - Position;
            float distanceToDonut = directionToDonut.Length();
            empanadaMoving = true;
            
            
            if (directionToDonut != Vector2.Zero)
            {
                directionToDonut.Normalize();
                
                if (distanceToDonut > 20.0f)
                {
                    Position += directionToDonut * Speed * deltaTime;
                }
                else if (distanceToDonut < 10.0f)
                {
                    Position -= directionToDonut * Speed * 0.5f * deltaTime;
                }
                if (!isEmpanadaAttacking)
                {
                    if (Math.Abs(directionToDonut.X) > Math.Abs(directionToDonut.Y))
                        empanadaFacingDirection = directionToDonut.X > 0 ? Direction.Right : Direction.Left;
                    else
                        empanadaFacingDirection = directionToDonut.Y > 0 ? Direction.Down : Direction.Up;
                }
            }

            Vector2 directionFromNacho = Position - nachoPosition;
            float distanceFromNacho = directionFromNacho.Length();
            if (distanceFromNacho < MinDistanceFromNacho && directionFromNacho != Vector2.Zero)
            {
                directionFromNacho.Normalize();
                Position += directionFromNacho * Speed * deltaTime;
            }

            float distanceToTarget = Vector2.Distance(Position, targetPosition);
            
            if (isEmpanadaAttacking)
            {
                empanadaAttackTimer += deltaTime;
                if (empanadaAttackTimer >= empanadaAttackCooldown)
                {
                    isEmpanadaAttacking = false;
                    empanadaAttackTimer = 0f;
                    
                    if (distanceToTarget <= AttackRange * 2.0f)
                    {
                        canPeriodicAttack = true;
                        
                        if (distanceToTarget <= AttackRange * 1.5f)
                        {
                            isEmpanadaAttacking = true;
                            empanadaAttackTimer = 0f;
                            
                            OnDamageDealt?.Invoke(AttackDamage * 0.8f);
                        }
                    }
                }
            }
            
            periodicAttackTimer += deltaTime;
            if (periodicAttackTimer >= PeriodicAttackInterval * 0.5f)
            {
                canPeriodicAttack = true;
                periodicAttackTimer = 0f;
            }
            
            if (distanceToTarget <= AttackRange * 2.0f && !isEmpanadaAttacking && canPeriodicAttack)
            {
                isEmpanadaAttacking = true;
                empanadaAttackTimer = 0f;
                canPeriodicAttack = false;
                
                Vector2 normalizedDirection = Vector2.Normalize(directionToDonut);
                if (Math.Abs(normalizedDirection.X) > Math.Abs(normalizedDirection.Y))
                {
                    empanadaFacingDirection = normalizedDirection.X > 0 ? Direction.Right : Direction.Left;
                }
                else
                {
                    empanadaFacingDirection = normalizedDirection.Y > 0 ? Direction.Down : Direction.Up;
                }
                
                OnDamageDealt?.Invoke(AttackDamage * 2.0f);
            }
            else if (isEmpanadaAttacking && distanceToTarget > AttackRange)
            {
                isEmpanadaAttacking = false;
                empanadaAttackTimer = 0f;
            }

            if (empanadaMoving)
            {
                animationTimer += deltaTime * 1000; 
                if (animationTimer > AnimationThreshold)
                {
                    currentAnimationIndexEmpanada = (byte)((currentAnimationIndexEmpanada + 1) % 3);
                    animationTimer = 0f;
                }
            }
            else if (animationTimer > 0)
            {
                animationTimer = 0f;
            }
            
            if (isEmpanadaAttacking || distanceToTarget <= AttackRange * 2.0f) 
            {
                attackAnimationTimer += deltaTime * 1000;
                
                if (attackAnimationTimer > 200) 
                {
                    currentAttackFrame = (byte)((currentAttackFrame + 1) % 2);
                    attackAnimationTimer = 0f;
                    
                    if (distanceToTarget <= AttackRange * 1.5f && isEmpanadaAttacking)
                    {
                        OnDamageDealt?.Invoke(AttackDamage * 2.0f);
                    }
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Rectangle[] frames;
            int frameIndex;
          
            float distanceToDonut = Vector2.Distance(Position, targetPosition);
            bool useAttackFrames = distanceToDonut <= AttackRange * 2.0f;
            
            if (useAttackFrames)
            {
                frames = empanadaFacingDirection switch
                {
                    Direction.Up => new Rectangle[]
                    {
                        new Rectangle(frameWidth * 4 - 35, 0, attackFrameWidth, frameHeight),
                        new Rectangle(frameWidth * 4 + attackFrameWidth - 35, 0, attackFrameWidth, frameHeight)
                    },
                    Direction.Down => new Rectangle[]
                    {
                        new Rectangle(frameWidth * 4, frameHeight * 2, attackFrameWidth, frameHeight),
                        new Rectangle(frameWidth * 4 + attackFrameWidth, frameHeight * 2, attackFrameWidth, frameHeight)
                    },
                    Direction.Left => new Rectangle[]
                    {
                        new Rectangle(frameWidth * 4 + 20, frameHeight * 3, attackFrameWidth, frameHeight),
                        new Rectangle(frameWidth * 4 + attackFrameWidth + 20, frameHeight * 3, attackFrameWidth, frameHeight)
                    },
                    Direction.Right => new Rectangle[]
                    {
                        new Rectangle(frameWidth * 4 - 20, frameHeight, attackFrameWidth, frameHeight),
                        new Rectangle(frameWidth * 4 + attackFrameWidth - 20, frameHeight, attackFrameWidth, frameHeight)
                    },
                    _ => new Rectangle[]
                    {
                        new Rectangle(frameWidth * 4, frameHeight, attackFrameWidth, frameHeight),
                        new Rectangle(frameWidth * 4 + attackFrameWidth, frameHeight, attackFrameWidth, frameHeight)
                    }
                };
                
                frameIndex = currentAttackFrame;
            }
            else
            {
                frames = empanadaFacingDirection switch
                {
                    Direction.Up => new Rectangle[]
                    {
                        new Rectangle(0, 0, frameWidth, frameHeight),
                        new Rectangle(frameWidth, 0, frameWidth, frameHeight),
                        new Rectangle(frameWidth * 2, 0, frameWidth, frameHeight)
                    },
                    Direction.Down => new Rectangle[]
                    {
                        new Rectangle(0, frameHeight * 2, frameWidth, frameHeight),
                        new Rectangle(frameWidth, frameHeight * 2, frameWidth, frameHeight),
                        new Rectangle(frameWidth * 2, frameHeight * 2, frameWidth, frameHeight)
                    },
                    Direction.Left => new Rectangle[]
                    {
                        new Rectangle(0, frameHeight * 3, frameWidth, frameHeight),
                        new Rectangle(frameWidth, frameHeight * 3, frameWidth, frameHeight),
                        new Rectangle(frameWidth * 2, frameHeight * 3, frameWidth, frameHeight)
                    },
                    Direction.Right => new Rectangle[]
                    {
                        new Rectangle(0, frameHeight, frameWidth, frameHeight),
                        new Rectangle(frameWidth, frameHeight, frameWidth, frameHeight),
                        new Rectangle(frameWidth * 2, frameHeight, frameWidth, frameHeight)
                    },
                    _ => new Rectangle[]
                    {
                        new Rectangle(0, frameHeight, frameWidth, frameHeight),
                        new Rectangle(frameWidth, frameHeight, frameWidth, frameHeight),
                        new Rectangle(frameWidth * 2, frameHeight, frameWidth, frameHeight)
                    }
                };
                
                frameIndex = currentAnimationIndexEmpanada;
            }
            
            Rectangle currentFrame = frames[frameIndex];

            Color spriteColor;
            if (currentHealth <= 0)
            {
                spriteColor = Color.Gray;
            }
            else
            {
                spriteColor = isInvulnerable && (int)(invulnerabilityTimer * 10) % 2 == 0 ? Color.Red : Color.White;
            }

            Vector2 origin = useAttackFrames ? 
                new Vector2(attackFrameWidth / 2.5f, frameHeight / 2) :  
                new Vector2(frameWidth / 2, frameHeight / 2);
                
            spriteBatch.Draw(
                texture,
                position,
                currentFrame,
                spriteColor,
                0f,
                origin,
                1.0f,
                SpriteEffects.None,
                0f
            );
            
            DrawHealthBar(spriteBatch);
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

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (currentHealth <= 0)
            {
                // Handle death logic if needed
            }
        }

        public void SetFacingDirection(Direction direction)
        {
            empanadaFacingDirection = direction;
        }

        public void SetTargetPosition(Vector2 position)
        {
            targetPosition = position;
        }

        public void StartAttack()
        {
            float distanceToDonut = Vector2.Distance(Position, targetPosition);
            if (!isEmpanadaAttacking && canPeriodicAttack)
            {
                isEmpanadaAttacking = true;
                empanadaAttackTimer = 0f;
                canPeriodicAttack = false;
                periodicAttackTimer = 0f;
                
                Vector2 directionToDonut = Vector2.Normalize(targetPosition - Position);
                if (Math.Abs(directionToDonut.X) > Math.Abs(directionToDonut.Y))
                {
                    empanadaFacingDirection = directionToDonut.X > 0 ? Direction.Right : Direction.Left;
                }
                else
                {
                    empanadaFacingDirection = directionToDonut.Y > 0 ? Direction.Down : Direction.Up;
                }
                
                OnDamageDealt?.Invoke(AttackDamage);
            }
        }
        
        public override bool TakeDamage(float damage)
        {
            bool damageDealt = base.TakeDamage(damage);
            
            // Optional: Add empanada-specific damage reactions here
            
            return damageDealt;
        }
    }
}
