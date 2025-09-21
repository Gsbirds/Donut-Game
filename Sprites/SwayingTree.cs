using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using monogame.Animation;

namespace monogame.Sprites
{
    public enum TreeColor
    {
        Original,
        Pink,
        Purple
    }

    public class SwayingTree
    {
        private Texture2D texture;
        private Vector2 position;
        private TreeAnimation animation;
        private Vector2 origin;
        private TreeColor treeColor;
        
        public Vector2 Position => position;
        public TreeColor Color => treeColor;
        
        public SwayingTree(Texture2D texture, Vector2 position, TreeColor color = TreeColor.Original)
        {
            this.texture = texture;
            this.position = position;
            this.treeColor = color;
            this.animation = new TreeAnimation(
                swayDuration: 6f,
                scaleDuration: 8f,
                maxSwayAngle: 1.5f,
                scaleVariation: 0.02f
            );
            
            this.animation.SetBaseScale(new Vector2(0.3f, 0.3f));
            
            this.origin = new Vector2(texture.Width / 2f, texture.Height);
        }
        
        public void Update(GameTime gameTime)
        {
            animation.Update(gameTime);
        }
        
        public void Draw(SpriteBatch spriteBatch)
        {
            float rotation = animation.GetCurrentRotation();
            Vector2 scale = animation.GetCurrentScale();
            
            spriteBatch.Draw(
                texture,
                position,
                null,
                Microsoft.Xna.Framework.Color.White,
                rotation,
                origin,
                scale,
                SpriteEffects.None,
                0.2f        
            );
        }
        
        public Rectangle GetBounds()
        {
            Vector2 scale = animation.GetCurrentScale();
            return new Rectangle(
                (int)(position.X - (texture.Width * scale.X) / 2),
                (int)(position.Y - texture.Height * scale.Y),
                (int)(texture.Width * scale.X),
                (int)(texture.Height * scale.Y)
            );
        }
    }
}
