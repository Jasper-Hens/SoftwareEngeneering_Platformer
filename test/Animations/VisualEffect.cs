using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace test.Effects
{
    public class VisualEffect
    {
        private Texture2D _texture;
        private List<Rectangle> _frames;
        private int _currentFrame;
        private double _timer;
        private double _frameSpeed = 60; // Snelheid van effect

        public bool IsActive { get; private set; } = false;
        public Vector2 Position { get; private set; }
        public bool FacingRight { get; private set; }

        public VisualEffect(Texture2D texture, int frameWidth, int frameHeight, int frameCount)
        {
            _texture = texture;
            _frames = new List<Rectangle>();

            // We knippen de frames automatisch (neemt aan dat ze op 1 rij staan)
            for (int i = 0; i < frameCount; i++)
            {
                _frames.Add(new Rectangle(i * frameWidth, 0, frameWidth, frameHeight));
            }
        }

        public void Play(Vector2 position, bool facingRight)
        {
            Position = position;
            FacingRight = facingRight;
            IsActive = true;
            _currentFrame = 0;
            _timer = 0;
        }

        public void Update(GameTime gameTime)
        {
            if (!IsActive) return;

            _timer += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (_timer >= _frameSpeed)
            {
                _currentFrame++;
                if (_currentFrame >= _frames.Count)
                {
                    IsActive = false; // Effect is klaar
                }
                _timer = 0;
            }
        }

        public void Draw(SpriteBatch sb)
        {
            if (!IsActive) return;

            var flip = FacingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            // Je moet misschien de positie wat finetunen (offset)
            Vector2 drawPos = Position;
            if (FacingRight) drawPos.X += 30; // Iets naar rechts schuiven
            else drawPos.X -= 30; // Iets naar links

            sb.Draw(_texture, drawPos, _frames[_currentFrame], Color.White, 0f, Vector2.Zero, 1f, flip, 0f);
        }

        // Optioneel: Geeft de hitbox van dit effect terug voor damage
        public Rectangle GetHitbox()
        {
            if (!IsActive) return Rectangle.Empty;
            return new Rectangle((int)Position.X, (int)Position.Y, _frames[0].Width, _frames[0].Height);
        }
    }
}