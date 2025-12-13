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
        public double FrameSpeed { get; protected set; } = 80;

        private double _timer = 0;

        // Eigenschappen voor combat
        public bool IsLooping { get; set; } = true;
        public bool IsFinished { get; private set; } = false;

        // Damage en Hitbox data
        public List<int> DamageFrames { get; set; } = new List<int>();
        public int AttackRange { get; set; } = 60;
        public int AttackHeight { get; set; } = 40;

        public Animation(Texture2D texture)
        {
            Texture = texture;
            Frames = new List<Rectangle>();
        }

        public void Update(GameTime gameTime)
        {
            // Als hij klaar is en niet mag loopen, stop hier.
            if (!IsLooping && IsFinished)
                return;

            _timer += gameTime.ElapsedGameTime.TotalMilliseconds;

            if (_timer >= FrameSpeed)
            {
                CurrentFrame++;

                // Einde van de animatie bereikt?
                if (CurrentFrame >= Frames.Count)
                {
                    if (IsLooping)
                    {
                        CurrentFrame = 0; // Begin opnieuw
                    }
                    else
                    {
                        // STOP!
                        CurrentFrame = Frames.Count - 1; // Blijf op het laatste frame
                        IsFinished = true;               // ZET HET VLAGGETJE AAN
                    }
                }
                _timer = 0;
            }
        }

        public void Reset()
        {
            CurrentFrame = 0;
            _timer = 0;
            IsFinished = false; // Reset het vlaggetje
        }

        // In Animation.cs
        public void Draw(SpriteBatch sb, Vector2 position, bool facingRight, Color color) // <--- NIEUW: Color parameter
        {
            var flip = facingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            const int MAX_FRAME_HEIGHT = 65;

            Rectangle currentFrameRect = Frames[CurrentFrame];
            float yOffset = MAX_FRAME_HEIGHT - currentFrameRect.Height;
            Vector2 drawPosition = new Vector2(position.X, position.Y + yOffset);

            // Gebruik hier de variabele 'color' in plaats van Color.White
            sb.Draw(Texture, drawPosition, currentFrameRect, color, 0f, Vector2.Zero, 1f, flip, 0f);
        }
    }
}