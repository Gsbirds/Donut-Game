using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace monogame.Sprites
{
    public class DonutHole : Sprite
    {
        private Texture2D spriteSheet;
        private readonly Rectangle[] animationFrames;
        private int currentFrameIndex;
        private float animationTimer;
        private const float FRAME_DURATION = 0.3f;
        private Vector2 offsetFromDonut; 
        private Donut parentDonut;
        
        // Shooting properties
        private bool isShooting = false;
        private Vector2 targetPosition;
        private Vector2 shootDirection;
        private float shootSpeed = 400f;
        private bool isReturning = false;
        private float rotationAngle = 0f;
        private Sprite lastHitTarget = null;
        
        private float shootTimer = 0f;
        private float shootInterval = 3f;
        private Random random = new Random();

        public DonutHole(Texture2D texture, Donut donut, Vector2 offset, float speed) 
            : base(texture, donut.Position + offset, speed)
        {
            spriteSheet = texture;
            parentDonut = donut;
            offsetFromDonut = offset;
            
            int frameWidth = texture.Width / 6; 
            int frameHeight = texture.Height;
            
            animationFrames = new Rectangle[6];
            for (int i = 0; i < 6; i++)
            {
                animationFrames[i] = new Rectangle(i * frameWidth, 0, frameWidth, frameHeight);
            }
            
            currentFrameIndex = 0;
            animationTimer = 0f;
            
            showHealthBar = false;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            animationTimer += deltaTime;
            if (animationTimer >= FRAME_DURATION)
            {
                animationTimer = 0f;
                currentFrameIndex = (currentFrameIndex + 1) % animationFrames.Length;
            }
            
            if (!isShooting)
            {
                shootTimer += deltaTime;
                if (shootTimer >= shootInterval)
                {
                    shootTimer = 0f;
                    ShootAtRandomTarget();
                    shootInterval = 2f + (float)random.NextDouble() * 8f;
                }
            }
            
            if (isShooting)
            {
                rotationAngle += 10f * deltaTime;
                
                if (isReturning)
                {
                    Vector2 returnVector = parentDonut.Position + offsetFromDonut - position;
                    if (returnVector.Length() < 10)
                    {
                        isShooting = false;
                        isReturning = false;
                        lastHitTarget = null;
                        position = parentDonut.Position + offsetFromDonut;
                    }
                    else
                    {
                        shootDirection = Vector2.Normalize(returnVector);
                        position += shootDirection * shootSpeed * deltaTime;
                    }
                }
                else
                {
                    position += shootDirection * shootSpeed * deltaTime;
                    
                    if (Vector2.Distance(position, parentDonut.Position) > 800)
                    {
                        ReturnToDonut();
                    }
                }
            }
            else
            {
                position = parentDonut.Position + offsetFromDonut;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            float scale = 0.18f; 
            
            spriteBatch.Draw(
                texture,
                position,
                animationFrames[currentFrameIndex],
                Color.White,
                isShooting ? rotationAngle : 0f,
                new Vector2(animationFrames[currentFrameIndex].Width / 2, animationFrames[currentFrameIndex].Height / 2),
                new Vector2(scale, scale),
                SpriteEffects.None,
                0f
            );
        }
        
        public void ShootAt(Vector2 target)
        {
            if (!isShooting)
            {
                isShooting = true;
                isReturning = false;
                lastHitTarget = null;
                targetPosition = target;
                shootDirection = Vector2.Normalize(targetPosition - position);
            }
        }
        
        private List<Sprite> enemyTargets = new List<Sprite>();
        
        public void SetTargets(params Sprite[] targets)
        {
            enemyTargets.Clear();
            if (targets != null)
            {
                enemyTargets.AddRange(targets);
            }
        }
        
        private void ShootAtRandomTarget()
        {
            Sprite target = null;
            
            var validTargets = new List<Sprite>();
            foreach (var potentialTarget in enemyTargets)
            {
                if (potentialTarget != null && potentialTarget.Health > 0)
                {
                    validTargets.Add(potentialTarget);
                }
            }
                
            if (validTargets.Count > 0)
            {
                target = validTargets[random.Next(validTargets.Count)];
                
                float offsetX = -5f + (float)random.NextDouble() * 10f;
                float offsetY = -5f + (float)random.NextDouble() * 10f;
                Vector2 targetPos = target.Position + new Vector2(offsetX, offsetY);
                
                ShootAt(targetPos);
            }
        }
        
        public void ReturnToDonut()
        {
            if (isShooting && !isReturning)
            {
                isReturning = true;
            }
        }
        
        public bool CheckCollision(Sprite target)
        {
            if (isShooting && !isReturning && target.Health > 0)
            {
                Rectangle myBounds = new Rectangle(
                    (int)(position.X - 20), // Wider hit box
                    (int)(position.Y - 20), // Taller hit box
                    40,  // Fixed width 
                    40   // Fixed height
                );
                
                Rectangle targetBounds = new Rectangle(
                    (int)(target.Position.X - 30),
                    (int)(target.Position.Y - 30),
                    60,
                    60
                );
                
                if (myBounds.Intersects(targetBounds))
                {
                    if (target == lastHitTarget)
                    {
                        return false; 
                    }
                    
                    target.TakeDamage(20f);
                    
                    lastHitTarget = target;
                    
                    ReturnToDonut();
                    return true;
                }
            }
            return false;
        }
    }
}
