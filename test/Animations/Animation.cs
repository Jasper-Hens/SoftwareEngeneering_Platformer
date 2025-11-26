using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace test.Animations
{
    public abstract class Animation
    {
        public List<Rectangle> Frames { get; protected set; }
        public Texture2D Texture { get; protected set; }
        public int CurrentFrame { get; private set; }
        public double FrameSpeed { get; protected set; } = 120;

        private double _timer = 0;

        public Animation(Texture2D texture)
        {
            Texture = texture;
            Frames = new List<Rectangle>();
        }

        public void Update(GameTime gameTime)
        {
            _timer += gameTime.ElapsedGameTime.TotalMilliseconds;

            if (_timer >= FrameSpeed)
            {
                CurrentFrame++;
                if (CurrentFrame >= Frames.Count)
                    CurrentFrame = 0;

                _timer = 0;
            }
        }

        public void Draw(SpriteBatch sb, Vector2 position, bool facingRight)
        {
            var flip = facingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            sb.Draw(Texture,position,Frames[CurrentFrame],Color.White,0f,Vector2.Zero,1f,flip,0f);
        }
    }
}
