using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using monogame.Animation;
using System;

namespace monogame.Sprites
{
    public class Empanada : Sprite
    {
        private Rectangle[] GetCurrentRectangleEmpanada()
        {

            return empanadaFacingDirection switch
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
        }
        private Direction empanadaFacingDirection;
        private bool isEmpanadaAttacking;
        private float empanadaAttackTimer;
        private const float empanadaAttackCooldown = 1.5f;
        private bool empanadaMoving;
        private byte currentAnimationIndexEmpanada;
        private const int frameWidth = 110;
        private const int frameHeight = 133;
        private float animationTimer;
        private const float AnimationThreshold = 300f;
        private int animationCycleCount;
        private bool useBlinkingFrame;
        private const float MinDistanceFromNacho = 170f;
        private const float AttackRange = 50f;
        private const float AttackDamage = 0.5f;
        private Vector2 targetPosition;
        private Vector2 nachoPosition;
        private float halfScreenHeight;
        public event Action<float> OnDamageDealt;

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
            empanadaFacingDirection = Direction.Down;
            currentAnimationIndexEmpanada = 0;
            animationTimer = 0f;
            empanadaAttackTimer = 0f;
            empanadaMoving = false;
            isEmpanadaAttacking = false;
            useBlinkingFrame = false;
            animationCycleCount = 0;
            halfScreenHeight = 325f;
        }

        public void Update(float deltaTime, Vector2 targetPos, Vector2 nachoPos)
        {
            targetPosition = targetPos;
            nachoPosition = nachoPos;

            Vector2 directionToDonut = targetPosition - Position;
            empanadaMoving = true;
            
            if (directionToDonut != Vector2.Zero)
            {
                directionToDonut.Normalize();
                Position += directionToDonut * Speed * deltaTime;

                if (Math.Abs(directionToDonut.X) > Math.Abs(directionToDonut.Y))
                    empanadaFacingDirection = directionToDonut.X > 0 ? Direction.Right : Direction.Left;
                else
                    empanadaFacingDirection = directionToDonut.Y > 0 ? Direction.Down : Direction.Up;
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
                }
            }

            if (distanceToTarget <= AttackRange && !isEmpanadaAttacking)
            {
                isEmpanadaAttacking = true;
                empanadaAttackTimer = 0f;
                OnDamageDealt?.Invoke(AttackDamage);
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
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Rectangle[] frames = GetCurrentRectangleEmpanada();
            Rectangle currentFrame = frames[currentAnimationIndexEmpanada];

            spriteBatch.Draw(
                texture,
                position,
                currentFrame,
                Color.White,
                0f,
                new Vector2(70, 66),
                1.0f,
                SpriteEffects.None,
                0f
            );
        }

        public override void Update(GameTime gameTime)
        {
            Update((float)gameTime.ElapsedGameTime.TotalSeconds, position, position);
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
            if (!isEmpanadaAttacking)
            {
                isEmpanadaAttacking = true;
                empanadaAttackTimer = 0f;
            }
        }
    }
}
