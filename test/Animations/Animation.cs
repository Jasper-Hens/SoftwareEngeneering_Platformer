using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace test.Animations
{
    public abstract class Animation
    {

        public const int MAX_FRAME_HEIGHT = 65; // Hoogste frame is 65px
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
            Rectangle currentFrameRect = Frames[CurrentFrame];

            // Bereken de verschuiving: Max hoogte (65) - Huidige hoogte.
            float yOffset = MAX_FRAME_HEIGHT - currentFrameRect.Height;

            // Pas de positie aan met de offset
            Vector2 drawPosition = new Vector2(position.X, position.Y + yOffset);

            sb.Draw(Texture, drawPosition, currentFrameRect, Color.White, 0f, Vector2.Zero, 1f, flip, 0f);
        }
    }
}
