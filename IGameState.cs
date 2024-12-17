using Microsoft.Xna.Framework;

namespace monogame
{
    public interface IGameState
    {
        void LoadContent();
        void Update(GameTime gameTime);
        void Draw(GameTime gameTime);
    }
}
