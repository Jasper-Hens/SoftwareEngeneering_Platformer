using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using test.Animations;

namespace test.Objects
{
    public class SpinningBlade
    {
        private Vector2 _position;
        private SimpleAnimation _animation;

        // De hitbox
        public Rectangle Hitbox => new Rectangle((int)_position.X + 10, (int)_position.Y + 10, 50, 50);

        public SpinningBlade(Texture2D texture, Vector2 position)
        {
            _position = position;

            _animation = new SimpleAnimation(texture);
            _animation.IsLooping = true;

            // --- HIER ZAT DE FOUT: Speed moest FrameSpeed zijn ---
            _animation.FrameSpeed = 50;
            // ---------------------------------------------------

            // Frames toevoegen
            _animation.Frames.Add(new Rectangle(286, 29, 71, 70));
            _animation.Frames.Add(new Rectangle(286, 158, 72, 69));
            _animation.Frames.Add(new Rectangle(287, 288, 71, 69));
        }

        public void Update(GameTime gameTime)
        {
            _animation.Update(gameTime);
        }

        public void Draw(SpriteBatch sb)
        {
            Rectangle currentRect = _animation.Frames[_animation.CurrentFrame];
            sb.Draw(_animation.Texture, _position, currentRect, Color.White);
        }
    }
}