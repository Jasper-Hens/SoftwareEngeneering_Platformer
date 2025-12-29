using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace test
{
    internal interface IGameObject
    {
        public void Update(GameTime gameTime);

        public void Draw(SpriteBatch SpriteBatch);
    }
}
