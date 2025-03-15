using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace monogame.Sprites
{
    public abstract class Sprite
    {
        protected Texture2D texture;
        protected Vector2 position;
        protected float speed;
        protected float rotation;
        
        public Vector2 Position 
        { 
            get => position;
            set => position = value;
        }

        public float Speed
        {
            get => speed;
            set => speed = value;
        }

        public Texture2D Texture => texture;

        public Sprite(Texture2D texture, Vector2 position, float speed)
        {
            this.texture = texture;
            this.position = position;
            this.speed = speed;
            this.rotation = 0f;
        }

        public virtual void Update(GameTime gameTime)
        {
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            Draw(spriteBatch, new Rectangle(0, 0, texture.Width, texture.Height));
        }

        protected virtual void Draw(SpriteBatch spriteBatch, Rectangle sourceRectangle)
        {
            spriteBatch.Draw(texture, position, sourceRectangle, Color.White, rotation,
                new Vector2(sourceRectangle.Width / 2, sourceRectangle.Height / 2),
                Vector2.One, SpriteEffects.None, 0f);
        }
    }
}
