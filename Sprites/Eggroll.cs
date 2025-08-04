using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using monogame.Animation;
using System;

namespace monogame.Sprites
{
    public class Eggroll : Sprite
    {
        private Vector2 targetPosition;
        private bool isAttacking;
        private float attackTimer;
        private new float rotation;
        private float rotationSpeed;
        private Vector2 attackDirection;
        private float attackSpeed;
        private float normalSpeed;
        private float orbitRadius;
        private float orbitAngle;
        private Vector2 orbitCenter;
        private bool isOrbiting;
        private float attackCooldown;
        private float attackCooldownTimer;
        
        private int currentFrame;
        private float animationTimer;
        private const float ANIMATION_SPEED = 0.3f;
        private const int FRAME_COUNT = 2;
        private int frameWidth;
        private int frameHeight;
        private const float SCALE = 0.15f;
        
        private const float ATTACK_RANGE = 200f;
        private const float ATTACK_DURATION = 2f;
        private const float ATTACK_COOLDOWN = 3f;
        private const float ORBIT_SPEED = 2f;
        private const float ROTATION_SPEED_NORMAL = 1f;
        private const float ROTATION_SPEED_ATTACK = 8f;

        public event Action<float> OnDamageDealt;

        public Eggroll(Texture2D texture, Vector2 position, float speed) 
            : base(texture, position, speed)
        {
            maxHealth = 60f;
            currentHealth = maxHealth;
            
            normalSpeed = speed;
            attackSpeed = speed * 3f;
            rotation = 0f;
            rotationSpeed = ROTATION_SPEED_NORMAL;
            isAttacking = false;
            attackTimer = 0f;
            attackCooldownTimer = 0f;
            attackCooldown = ATTACK_COOLDOWN;
            
            currentFrame = 0;
            animationTimer = 0f;
            frameWidth = texture.Width / FRAME_COUNT;
            frameHeight = texture.Height;
            
            isOrbiting = true;
            orbitRadius = 150f;
            orbitAngle = 0f;
            orbitCenter = position;
            
            targetPosition = position;
        }

        public void Update(float deltaTime, Vector2 donutPosition)
        {
            if (currentHealth <= 0)
            {
                isAttacking = false;
                isOrbiting = false;
                return;
            }

            targetPosition = donutPosition;
            float distanceToDonut = Vector2.Distance(Position, donutPosition);

            if (attackCooldownTimer > 0)
            {
                attackCooldownTimer -= deltaTime;
            }
            if (!isAttacking && distanceToDonut <= ATTACK_RANGE && attackCooldownTimer <= 0)
            {
                StartAttack();
            }

            if (isAttacking)
            {
                UpdateAttack(deltaTime, donutPosition);
            }
            else
            {
                UpdateOrbiting(deltaTime, donutPosition);
            }

            animationTimer += deltaTime;
            if (animationTimer >= ANIMATION_SPEED)
            {
                currentFrame = (currentFrame + 1) % FRAME_COUNT;
                animationTimer = 0f;
            }
            
            rotation += rotationSpeed * deltaTime;
            if (rotation > MathHelper.TwoPi)
            {
                rotation -= MathHelper.TwoPi;
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

        private void StartAttack()
        {
            isAttacking = true;
            isOrbiting = false;
            attackTimer = 0f;
            rotationSpeed = ROTATION_SPEED_ATTACK;
            
            attackDirection = targetPosition - Position;
            if (attackDirection != Vector2.Zero)
            {
                attackDirection.Normalize();
            }
        }

        private void UpdateAttack(float deltaTime, Vector2 donutPosition)
        {
            attackTimer += deltaTime;
            
            Vector2 movement = attackDirection * attackSpeed * deltaTime;
            Position += movement;
            
            float distanceToDonut = Vector2.Distance(Position, donutPosition);
            if (attackTimer >= ATTACK_DURATION || distanceToDonut < 30f)
            {
                EndAttack();
                
                if (distanceToDonut < 50f)
                {
                    OnDamageDealt?.Invoke(15f);
                }
            }
        }

        private void UpdateOrbiting(float deltaTime, Vector2 donutPosition)
        {
            Vector2 directionToDonut = donutPosition - orbitCenter;
            if (directionToDonut.Length() > 10f)
            {
                directionToDonut.Normalize();
                orbitCenter += directionToDonut * normalSpeed * 0.3f * deltaTime;
            }
            
            orbitAngle += ORBIT_SPEED * deltaTime;
            if (orbitAngle > MathHelper.TwoPi)
            {
                orbitAngle -= MathHelper.TwoPi;
            }
            
            Vector2 orbitOffset = new Vector2(
                (float)Math.Cos(orbitAngle) * orbitRadius,
                (float)Math.Sin(orbitAngle) * orbitRadius
            );
            
            Vector2 targetOrbitPosition = orbitCenter + orbitOffset;
            
            Vector2 directionToOrbit = targetOrbitPosition - Position;
            if (directionToOrbit.Length() > 5f)
            {
                directionToOrbit.Normalize();
                Position += directionToOrbit * normalSpeed * deltaTime;
            }
        }

        private void EndAttack()
        {
            isAttacking = false;
            isOrbiting = true;
            attackTimer = 0f;
            attackCooldownTimer = attackCooldown;
            rotationSpeed = ROTATION_SPEED_NORMAL;
            
            orbitCenter = Position;
        }

        public void SetOrbitCenter(Vector2 center, float radius, float startAngle)
        {
            orbitCenter = center;
            orbitRadius = radius;
            orbitAngle = startAngle;
        }

        public override Rectangle GetBounds()
        {
            int scaledWidth = (int)(frameWidth * SCALE);
            int scaledHeight = (int)(frameHeight * SCALE);
            return new Rectangle(
                (int)position.X - scaledWidth / 2,
                (int)position.Y - scaledHeight / 2,
                scaledWidth,
                scaledHeight
            );
        }

        public new void Draw(SpriteBatch spriteBatch)
        {
            Rectangle sourceRectangle = new Rectangle(
                currentFrame * frameWidth,
                0,
                frameWidth,
                frameHeight
            );
            
            if (currentHealth <= 0)
            {
                Vector2 origin = new Vector2(frameWidth / 2, frameHeight / 2);
                spriteBatch.Draw(
                    texture,
                    position,
                    sourceRectangle,
                    Color.Gray,
                    rotation,
                    origin,
                    SCALE,
                    SpriteEffects.None,
                    0f
                );
                return;
            }

            Color spriteColor = isInvulnerable && (int)(invulnerabilityTimer * 10) % 2 == 0 ? Color.Red : Color.White;
            
            Vector2 drawOrigin = new Vector2(frameWidth / 2, frameHeight / 2);
            
            spriteBatch.Draw(
                texture,
                position,
                sourceRectangle,
                spriteColor,
                rotation,
                drawOrigin,
                SCALE,
                SpriteEffects.None,
                0f
            );
            
            DrawHealthBar(spriteBatch);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}
