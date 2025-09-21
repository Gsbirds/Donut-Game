using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace monogame.Animation
{
    public class TreeAnimation
    {
        private float animationTimer;
        private float swayDuration;
        private float scaleDuration;
        private float maxSwayAngle;
        private float scaleVariation;
        private Vector2 baseScale;
        
        public TreeAnimation(float swayDuration = 3f, float scaleDuration = 4f, float maxSwayAngle = 5f, float scaleVariation = 0.1f)
        {
            this.swayDuration = swayDuration;
            this.scaleDuration = scaleDuration;
            this.maxSwayAngle = maxSwayAngle;
            this.scaleVariation = scaleVariation;
            this.baseScale = Vector2.One;
            this.animationTimer = 0f;
        }
        
        public void Update(GameTime gameTime)
        {
            animationTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
        
        public float GetCurrentRotation()
        {
            // Sine wave for smooth swaying motion
            float swayProgress = (animationTimer % swayDuration) / swayDuration;
            return MathHelper.ToRadians(maxSwayAngle * (float)Math.Sin(swayProgress * MathHelper.TwoPi));
        }
        
        public Vector2 GetCurrentScale()
        {
            // Different frequency for scale to make it look more natural
            float scaleProgress = (animationTimer % scaleDuration) / scaleDuration;
            float scaleMultiplier = 1f + scaleVariation * (float)Math.Sin(scaleProgress * MathHelper.TwoPi);
            return baseScale * scaleMultiplier;
        }
        
        public void SetBaseScale(Vector2 scale)
        {
            baseScale = scale;
        }
    }
}
