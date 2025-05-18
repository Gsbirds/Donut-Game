using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace monogame.Effects
{
    public class SparkleManager
    {
        private Sparkle[] sparkles;
        private Texture2D sparkleTexture;
        private Random random;
        private int maxSparkles;
        private float spawnTimer;
        private float spawnRate;
        
        public SparkleManager(Texture2D texture, int maxCount)
        {
            sparkleTexture = texture;
            maxSparkles = maxCount;
            sparkles = new Sparkle[maxSparkles];
            
            for (int i = 0; i < maxSparkles; i++)
            {
                sparkles[i] = new Sparkle();
            }
            
            random = new Random();
            spawnRate = 0.1f;
            spawnTimer = 0;
        }
        
        public void Update(GameTime gameTime, Vector2 center)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            foreach (var sparkle in sparkles)
            {
                sparkle.Update(gameTime);
            }
            
            spawnTimer -= deltaTime;
            if (spawnTimer <= 0)
            {
                SpawnSparkle(center);
                spawnTimer = spawnRate;
            }
        }
        
        private void SpawnSparkle(Vector2 center)
        {
            for (int i = 0; i < maxSparkles; i++)
            {
                if (!sparkles[i].IsActive)
                {
                    sparkles[i].Activate(center);
                    return;
                }
            }
        }
        
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var sparkle in sparkles)
            {
                sparkle.Draw(spriteBatch, sparkleTexture);
            }
        }
    }
}
