using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace monogame.Sprites
{
    public class Axe : Sprite
    {
        private bool isVisible = false;
        private bool isPickedUp = false;
        private bool isPlayingPickupAnimation = false;
        private float bobTimer = 0f;
        private const float BobFrequency = 2f;
        private const float BobAmplitude = 5f;
        private Vector2 originalPosition;
        
        private float pickupAnimationTimer = 0f;
        private const float PICKUP_ANIMATION_DURATION = 1.5f;
        private float rotationAngle = 0f;
        private float scaleMultiplier = 1.0f;
        
        private Random random = new Random();
        private Vector2[] sparklePositions;
        private float[] sparkleScales;
        private float[] sparkleAngles;
        private float[] sparkleTimers;
        private Color[] sparkleColors;
        private const int SPARKLE_COUNT = 16;
        private const float SPARKLE_SPEED = 2.5f;
        
        private static readonly Color[] SPARKLE_COLOR_PALETTE = new Color[]
        {
            new Color(255, 105, 180), // Hot Pink
            new Color(255, 182, 193), // Light Pink
            new Color(238, 130, 238), // Violet
            new Color(218, 112, 214), // Orchid
            new Color(255, 140, 255)  // Light Purple
        };

        public bool IsVisible => isVisible;
        public bool IsPickedUp => isPickedUp;
        public bool IsPlayingPickupAnimation => isPlayingPickupAnimation;

        public Axe(Texture2D texture, Vector2 position)
            : base(texture, position, 0f)
        {
            this.originalPosition = position;
            
            sparklePositions = new Vector2[SPARKLE_COUNT];
            sparkleScales = new float[SPARKLE_COUNT];
            sparkleAngles = new float[SPARKLE_COUNT];
            sparkleTimers = new float[SPARKLE_COUNT];
            sparkleColors = new Color[SPARKLE_COUNT];
            
            InitializeSparkles();
        }
        
        private void InitializeSparkles()
        {
            for (int i = 0; i < SPARKLE_COUNT; i++)
            {
                ResetSparkle(i);
            }
        }
        
        private void ResetSparkle(int index)
        {
            float angle = (float)random.NextDouble() * MathHelper.TwoPi;
            float distance = (float)random.NextDouble() * 40f + 20f;
            
            sparklePositions[index] = new Vector2(
                (float)Math.Cos(angle) * distance,
                (float)Math.Sin(angle) * distance
            );
            
            sparkleScales[index] = (float)random.NextDouble() * 0.2f + 0.2f;
            sparkleAngles[index] = (float)random.NextDouble() * MathHelper.TwoPi;
            sparkleTimers[index] = (float)random.NextDouble() * MathHelper.TwoPi;
            sparkleColors[index] = SPARKLE_COLOR_PALETTE[random.Next(SPARKLE_COLOR_PALETTE.Length)];
        }
        
        public void Show()
        {
            isVisible = true;
            isPickedUp = false;
        }
        
        private void PickUp()
        {
            Console.WriteLine("Axe PickUp() method called");
            isPlayingPickupAnimation = true;
            pickupAnimationTimer = 0f;
            rotationAngle = 0f;
            scaleMultiplier = 1.0f;
        }
        
        public override void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            if (isPlayingPickupAnimation)
            {
                pickupAnimationTimer += deltaTime;
                
                float rotationSpeed = MathHelper.ToRadians(360) * (1.0f + pickupAnimationTimer * 2.0f);
                rotationAngle += rotationSpeed * deltaTime;
                
                float animationProgress = pickupAnimationTimer / PICKUP_ANIMATION_DURATION;
                
                if (animationProgress < 0.5f)
                {
                    scaleMultiplier = 1.0f + animationProgress * 1.0f;
                }
                else
                {
                    scaleMultiplier = 2.0f - (animationProgress - 0.5f) * 3.0f;
                }
                
                for (int i = 0; i < SPARKLE_COUNT; i++)
                {
                    sparkleTimers[i] += deltaTime * SPARKLE_SPEED * 2.0f;
                    sparkleAngles[i] += deltaTime * 3.0f;
                }
                
                // Immediately set isPickedUp flag when animation is 50% complete to trigger level transition faster
                if (pickupAnimationTimer >= PICKUP_ANIMATION_DURATION * 0.5f && !isPickedUp)
                {
                    Console.WriteLine("Axe pickup 50% complete - setting IsPickedUp to true");
                    isPickedUp = true;
                }
                
                // Animation complete
                if (pickupAnimationTimer >= PICKUP_ANIMATION_DURATION)
                {
                    Console.WriteLine("Axe animation complete");
                    isPlayingPickupAnimation = false;
                    isVisible = false;
                }
                
                return;
            }
            
            if (!isVisible || isPickedUp) return;
            
            bobTimer += deltaTime;
            float bobOffset = (float)Math.Sin(bobTimer * BobFrequency) * BobAmplitude;
            position = new Vector2(originalPosition.X, originalPosition.Y + bobOffset);
            
            for (int i = 0; i < SPARKLE_COUNT; i++)
            {
                sparkleTimers[i] += deltaTime * SPARKLE_SPEED;
                sparkleAngles[i] += deltaTime * 1.5f;
                
                if (random.NextDouble() < deltaTime * 0.8)
                {
                    ResetSparkle(i);
                }
            }
        }
        
        public override void Draw(SpriteBatch spriteBatch)
        {
            if ((!isVisible && !isPlayingPickupAnimation) || (isPickedUp && !isPlayingPickupAnimation)) return;
            
            if (isPlayingPickupAnimation)
            {
                float scale = 0.375f * scaleMultiplier;
                
                byte colorPulse = (byte)(155 + 100 * Math.Sin(pickupAnimationTimer * 15));
                Color pulsingColor = new Color(colorPulse, colorPulse, (byte)255, (byte)255);
                
                spriteBatch.Draw(
                    texture,
                    position,
                    null,
                    pulsingColor,
                    rotationAngle,
                    new Vector2(texture.Width / 2, texture.Height / 2),
                    scale,
                    SpriteEffects.None,
                    0f
                );
                
                DrawSparklesDuringAnimation(spriteBatch);
            }
            else
            {
                spriteBatch.Draw(
                    texture,
                    position,
                    null,
                    Color.White,
                    0f,
                    new Vector2(texture.Width / 2, texture.Height / 2),
                    0.375f,
                    SpriteEffects.None,
                    0f
                );
                
                DrawSparkles(spriteBatch);
            }
        }
        
        private void DrawSparkles(SpriteBatch spriteBatch)
        {
            if (!isVisible || isPickedUp) return;
            
            for (int i = 0; i < SPARKLE_COUNT; i++)
            {
                Vector2 globalPos = position + sparklePositions[i];
                
                float pulseScale = 0.3f * (float)Math.Sin(sparkleTimers[i] * 2.5f) + 1.1f;
                float finalScale = sparkleScales[i] * pulseScale;
                
                float twinkleAlpha = (float)Math.Max(0.4f, Math.Sin(sparkleTimers[i] * 1.5f + i) * 0.3f + 0.7f);
                Color adjustedColor = new Color(
                    sparkleColors[i].R, 
                    sparkleColors[i].G, 
                    sparkleColors[i].B, 
                    (byte)(sparkleColors[i].A * twinkleAlpha));
                
                DrawStar(spriteBatch, globalPos, sparkleAngles[i], finalScale, adjustedColor);
            }
        }
        
        private void DrawSparklesDuringAnimation(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < SPARKLE_COUNT; i++)
            {
                Vector2 offset = sparklePositions[i];
                
                float rotatedX = offset.X * (float)Math.Cos(rotationAngle) - offset.Y * (float)Math.Sin(rotationAngle);
                float rotatedY = offset.X * (float)Math.Sin(rotationAngle) + offset.Y * (float)Math.Cos(rotationAngle);
                
                Vector2 scaledOffset = new Vector2(rotatedX, rotatedY) * scaleMultiplier * 0.8f;
                
                Vector2 globalPos = position + scaledOffset;
                
                float pulseScale = 0.5f * (float)Math.Sin(sparkleTimers[i] * 4.0f) + 1.3f;
                float finalScale = sparkleScales[i] * pulseScale * 1.2f;
                
                Color brightColor = new Color(
                    (byte)Math.Min(255, sparkleColors[i].R + 50),
                    (byte)Math.Min(255, sparkleColors[i].G + 50),
                    (byte)Math.Min(255, sparkleColors[i].B + 80),
                    sparkleColors[i].A);
                
                DrawStar(spriteBatch, globalPos, sparkleAngles[i] + rotationAngle * 0.5f, finalScale, brightColor);
            }
        }
        
        private void DrawStar(SpriteBatch spriteBatch, Vector2 center, float angle, float scale, Color color)
        {
            const float alpha = 0.85f;
            Texture2D pixel = Game1.WhitePixel;
            
            float size = 4f * scale;
            
            float glowSize = size * 1.2f;
            Color glowColor = new Color(color.R, color.G, color.B, (byte)(color.A * 0.4f));
            
            DrawLine(spriteBatch, pixel, center - new Vector2(glowSize, 0), center + new Vector2(glowSize, 0), 1.5f * scale, glowColor);
            DrawLine(spriteBatch, pixel, center - new Vector2(0, glowSize), center + new Vector2(0, glowSize), 1.5f * scale, glowColor);
            
            DrawLine(spriteBatch, pixel, center - new Vector2(size, 0), center + new Vector2(size, 0), 1.5f * scale, color * alpha);
            DrawLine(spriteBatch, pixel, center - new Vector2(0, size), center + new Vector2(0, size), 1.5f * scale, color * alpha);
            
            float diagSize = size * 0.7f;
            float cos = (float)Math.Cos(angle);
            float sin = (float)Math.Sin(angle);
            
            Vector2 diag1Start = center + new Vector2(diagSize * cos, diagSize * sin);
            Vector2 diag1End = center - new Vector2(diagSize * cos, diagSize * sin);
            
            Vector2 diag2Start = center + new Vector2(-diagSize * sin, diagSize * cos);
            Vector2 diag2End = center - new Vector2(-diagSize * sin, diagSize * cos);
            
            DrawLine(spriteBatch, pixel, diag1Start, diag1End, 1.5f * scale, color * alpha);
            DrawLine(spriteBatch, pixel, diag2Start, diag2End, 1.5f * scale, color * alpha);
            
            DrawLine(spriteBatch, pixel, center, center, 2.0f * scale, color);
        }
        
        private void DrawLine(SpriteBatch spriteBatch, Texture2D texture, Vector2 start, Vector2 end, float thickness, Color color)
        {
            Vector2 delta = end - start;
            float angle = (float)Math.Atan2(delta.Y, delta.X);
            float length = delta.Length();
            
            spriteBatch.Draw(
                texture,
                start,
                null,
                color,
                angle,
                Vector2.Zero,
                new Vector2(length, thickness),
                SpriteEffects.None,
                0
            );
        }
        
        public bool CheckPickup(Donut donut)
        {
            if (!isVisible || isPickedUp) return false;

            if (Vector2.Distance(position, donut.Position) < 200f)
            {
                PickUp();
                return true;
            }
            
            return false;
        }
        

    }
}
