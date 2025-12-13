using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace test.States
{
    public abstract class GameState
    {
        protected Game1 _game;
        protected ContentManager _content;

        public GameState(Game1 game, ContentManager content)
        {
            _game = game;
            _content = content;
        }

        // Elke state MOET deze methodes hebben
        public abstract void LoadContent();
        public abstract void Update(GameTime gameTime);
        public abstract void Draw(SpriteBatch spriteBatch);
    }
}