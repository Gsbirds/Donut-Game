using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace monogame.Sprites
{
    public class Axe : Sprite
    {
        private bool isVisible = false;
        private bool isPickedUp = false;
        private float bobTimer = 0f;
        private const float BobFrequency = 2f;
        private const float BobAmplitude = 5f;
        private Vector2 originalPosition;

        public bool IsVisible => isVisible;
        public bool IsPickedUp => isPickedUp;

        public Axe(Texture2D texture, Vector2 position)
            : base(texture, position, 0f)
        {
            originalPosition = position;
        }

        public void Show()
        {
            isVisible = true;
            isPickedUp = false;
        }

        public void PickUp()
        {
            isVisible = false;
            isPickedUp = true;
        }

        public override void Update(GameTime gameTime)
        {
            if (!isVisible || isPickedUp) return;

            // Make the axe bob up and down for visual effect
            bobTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            float bobOffset = (float)System.Math.Sin(bobTimer * BobFrequency) * BobAmplitude;
            position = new Vector2(originalPosition.X, originalPosition.Y + bobOffset);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!isVisible || isPickedUp) return;

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
