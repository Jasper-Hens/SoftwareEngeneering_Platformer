using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace test.Objects
{
    public class Spikes
    {
        private Texture2D _texture;
        private Vector2 _position;
        private Rectangle _sourceRect = new Rectangle(390, 659, 64, 50);


        public Rectangle Hitbox => new Rectangle((int)_position.X + 4, (int)_position.Y + 10, 56, 40);

        public Spikes(Texture2D texture, Vector2 position)
        {
            _texture = texture;
            _position = position;
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(_texture, _position, _sourceRect, Color.White);
        }
    }
}