using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using monogame.Effects;

namespace monogame.Sprites
{

    public class FruitProjectileManager
    {
        private List<FruitProjectile> projectiles = new List<FruitProjectile>();
        private float cooldownTimer = 0f;
        private const float CooldownDuration = 10f;
        private bool isStreaming = false;
        private float streamTimer = 0f;
        private const float StreamDuration = 2.5f;
        private float spawnTimer = 0f;
        private const float SpawnInterval = 0.1f;
        private Vector2 streamDirection = Vector2.Zero;
        private DonutColor currentFruitType = DonutColor.Normal;
        private static Random random = new Random();
        
        private List<Texture2D> strawberryTextures = new List<Texture2D>();
        private List<Texture2D> blueberryTextures = new List<Texture2D>();
        private List<Texture2D> bananaTextures = new List<Texture2D>();
        
        public bool IsStreaming => isStreaming;
        public bool CanFire => cooldownTimer <= 0f;
        public List<FruitProjectile> Projectiles => projectiles;
        
        public float GetCooldownPercentage()
        {
            return 1.0f - (cooldownTimer / CooldownDuration);
        }
        
        public void ResetCooldown()
        {
            cooldownTimer = 0f;
            isStreaming = false;
        }
        
        public FruitProjectileManager(ContentManager content)
        {
            strawberryTextures.Add(content.Load<Texture2D>("strawberry"));
            blueberryTextures.Add(content.Load<Texture2D>("blueberry"));
            bananaTextures.Add(content.Load<Texture2D>("banana"));
            
            try {
                strawberryTextures.Add(content.Load<Texture2D>("_gabby/strawberry2"));
                strawberryTextures.Add(content.Load<Texture2D>("_gabby/strawberry3"));
                
                blueberryTextures.Add(content.Load<Texture2D>("_gabby/blueberry2"));
                blueberryTextures.Add(content.Load<Texture2D>("_gabby/blueberry3"));
                
                bananaTextures.Add(content.Load<Texture2D>("_gabby/banana2"));
                bananaTextures.Add(content.Load<Texture2D>("_gabby/banana3"));
            } catch (Exception ex) {
                System.Diagnostics.Debug.WriteLine("Error loading fruit variants: " + ex.Message);
            }

        }

        public void Update(GameTime gameTime, Vector2 donutPosition, DonutColor donutColor, MouseState mouseState, MouseState prevMouseState)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            if (cooldownTimer > 0)
            {
                cooldownTimer -= deltaTime;
                if (cooldownTimer < 0) cooldownTimer = 0;
            }
            
            for (int i = projectiles.Count - 1; i >= 0; i--)
            {
                projectiles[i].Update(gameTime);
                
                if (!projectiles[i].IsActive)
                {
                    projectiles.RemoveAt(i);
                }
            }
            
            bool mouseJustRightClicked = mouseState.RightButton == ButtonState.Pressed && 
                                        prevMouseState.RightButton == ButtonState.Released;
            
            if (mouseJustRightClicked && cooldownTimer <= 0 && !isStreaming)
            {
                if (donutColor == DonutColor.Normal || donutColor == DonutColor.Pink || donutColor == DonutColor.Yellow)
                {
                    Vector2 mousePosition = new Vector2(mouseState.X, mouseState.Y);
                    streamDirection = mousePosition - donutPosition;
                    streamDirection.Normalize();
                    
                    isStreaming = true;
                    streamTimer = 0;
                    spawnTimer = 0;
                    currentFruitType = donutColor;
                }
            }
            
            if (isStreaming)
            {
                streamTimer += deltaTime;
                spawnTimer += deltaTime;
                
                if (spawnTimer >= SpawnInterval && streamTimer <= StreamDuration)
                {
                    spawnTimer = 0;
                    
                    Texture2D fruitTexture;
                    if (currentFruitType == DonutColor.Normal) // Normal = Blueberry
                    {
                        fruitTexture = blueberryTextures[random.Next(blueberryTextures.Count)];
                    }
                    else if (currentFruitType == DonutColor.Pink) // Pink = Strawberry
                    {
                        fruitTexture = strawberryTextures[random.Next(strawberryTextures.Count)];
                    }
                    else if (currentFruitType == DonutColor.Yellow) // Yellow = Banana
                    {
                        fruitTexture = bananaTextures[random.Next(bananaTextures.Count)];
                    }
                    else
                    {
                        fruitTexture = blueberryTextures[random.Next(blueberryTextures.Count)];
                    }
                    
                    FruitProjectile.CreateProjectileStream(
                        projectiles,
                        donutPosition,
                        streamDirection,
                        currentFruitType,
                        fruitTexture);
                }
                
                if (streamTimer >= StreamDuration)
                {
                    isStreaming = false;
                    streamTimer = 0;
                    spawnTimer = 0;
                    cooldownTimer = CooldownDuration;
                }
            }
        }
        
        public void CheckCollisions(Sprite[] enemies, GraphicsDevice graphicsDevice)
        {
            for (int i = projectiles.Count - 1; i >= 0; i--)
            {
                var projectile = projectiles[i];
                if (projectile.IsActive)
                {
                    bool collided = false;
                    
                    foreach (var enemy in enemies)
                    {
                        if (enemy.Health <= 0) continue;
                        
                        float distance = Vector2.Distance(projectile.Position, enemy.Position);
                        if (distance < 50 && !projectile.HasDealtDamage)
                        {
                            projectile.DealDamageTo(enemy);
                            projectile.Reset();
                            projectiles.RemoveAt(i);
                            collided = true;
                            break;
                        }
                    }
                    
                    if (collided) continue;
                    
                    Vector2 position = projectile.Position;
                    if (position.X < -100 || position.Y < -100 || 
                        position.X > graphicsDevice.Viewport.Width + 100 || 
                        position.Y > graphicsDevice.Viewport.Height + 100)
                    {
                        projectile.Reset();
                        projectiles.RemoveAt(i);
                    }
                }
            }
        }
        
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var projectile in projectiles)
            {
                if (projectile.IsActive)
                {
                    projectile.Draw(spriteBatch);
                }
            }
        }
    }
    
    public class FruitProjectile : Projectile
    {
        private DonutColor fruitType;
        private float scale = 0.125f;
        private static Random random = new Random();
        
        public DonutColor FruitType => fruitType;
        public new Texture2D Texture => texture;

        public FruitProjectile(Texture2D texture, Vector2 position, float speed, DonutColor fruitType, float damage = 20f) 
            : base(texture, position, speed, damage)
        {
            this.fruitType = fruitType;
        }

        public void LaunchWithRandomVariation(Vector2 position, Vector2 direction, float randomFactor = 0.3f)
        {
            Vector2 randomizedDirection = direction;
            randomizedDirection.X += (float)(random.NextDouble() - 0.5) * randomFactor;
            randomizedDirection.Y += (float)(random.NextDouble() - 0.5) * randomFactor;
            randomizedDirection.Normalize();
            
            Launch(position, randomizedDirection);
        }
        

        public static void CreateProjectileStream(
            List<FruitProjectile> projectileList, 
            Vector2 sourcePosition, 
            Vector2 targetDirection, 
            DonutColor fruitType, 
            Texture2D fruitTexture, 
            float speed = 700f, 
            float damage = 20f)
        {
            FruitProjectile newPellet = new FruitProjectile(fruitTexture, sourcePosition, speed, fruitType, damage);
            
            newPellet.LaunchWithRandomVariation(sourcePosition, targetDirection);
            
            projectileList.Add(newPellet);
        }

        protected override void Draw(SpriteBatch spriteBatch, Rectangle sourceRectangle)
        {
            if (!IsActive) return;
            
            float currentScale = scale;
            
            if (fruitType == DonutColor.Yellow)
            {
                string textureName = texture.Name ?? "";
                if (textureName.Contains("banana2") || textureName.Contains("banana3"))
                {
                    currentScale = scale * 1.5f;
                }
                else
                {
                    currentScale = scale * 2.0f;
                }
            }
            
            spriteBatch.Draw(
                texture, 
                position, 
                sourceRectangle, 
                Color.White, 
                Rotation,
                new Vector2(sourceRectangle.Width / 2, sourceRectangle.Height / 2),
                currentScale, 
                SpriteEffects.None, 
                0f
            );
        }
    }
}
