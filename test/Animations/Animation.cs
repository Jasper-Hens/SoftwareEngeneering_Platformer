using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace test.Animations
{
    public abstract class Animation
    {
        // 1. HERSTELD: We gebruiken weer 'Texture' (met hoofdletter) zodat KnightBoss en Door werken.
        public Texture2D Texture { get; protected set; }

        public List<Rectangle> Frames { get; protected set; } = new List<Rectangle>();
        public int CurrentFrame { get; set; }

        // We gebruiken double om casting errors (float vs int) te voorkomen
        public double FrameSpeed { get; set; }

        public double FrameTimer { get; set; }
        public bool IsLooping { get; set; } = true;
        public bool IsFinished { get; private set; } = false;

        // Combat data
        public List<int> DamageFrames { get; set; } = new List<int>();
        public int AttackRange { get; set; } = 0;
        public int AttackHeight { get; set; } = 0;

        public Animation(Texture2D texture)
        {
            Texture = texture;
            Frames = new List<Rectangle>();
        }

        public virtual void Update(GameTime gameTime)
        {
            // Als hij klaar is en niet mag loopen, doe niets meer
            if (!IsLooping && IsFinished) return;

            FrameTimer += gameTime.ElapsedGameTime.TotalMilliseconds;

            if (FrameTimer > FrameSpeed)
            {
                FrameTimer = 0;
                CurrentFrame++;

                if (CurrentFrame >= Frames.Count)
                {
                    if (IsLooping)
                    {
                        CurrentFrame = 0;
                    }
                    else
                    {
                        CurrentFrame = Frames.Count - 1; // Blijf op het laatste frame
                        IsFinished = true;
                    }
                }
            }
        }

        public void Reset()
        {
            CurrentFrame = 0;
            FrameTimer = 0;
            IsFinished = false;
        }

        // 2. BEHOUDEN: De Draw methode met de 'scale' parameter voor de Evil Wizard
        public virtual void Draw(SpriteBatch sb, Vector2 position, bool facingRight, Color color, float scale = 1.0f)
        {
            if (Frames.Count == 0) return; // Veiligheid

            Rectangle source = Frames[CurrentFrame];
            SpriteEffects effect = facingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            // We gebruiken hier de property 'Texture' die we hierboven hersteld hebben
            sb.Draw(Texture, position, source, color, 0f, Vector2.Zero, scale, effect, 0f);
        }
    }
}