using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using test.Animations;
using System.Collections.Generic;

namespace test
{
    public class Hero
    {
        private Animation _idle;
        private Animation _run;
        private Animation _jump;
        private Animation _current;

        public Vector2 Position = new Vector2(100, 100);
        public Vector2 Velocity = Vector2.Zero;

        public bool IsRunningRight = false;
        public bool IsRunningLeft = false;
        public bool Jump = false;
        public bool FacingRight = true;

        public Hitbox Hitbox { get; private set; }

        private float gravity = 0.5f;
        private float moveSpeed = 4f;
        private float jumpStrength = -10f;

        public Hero(Texture2D idleTexture, Texture2D runTexture, Texture2D jumpTexture)
        {
            _idle = new IdleAnimation(idleTexture);
            _run = new RunAnimation(runTexture);
            _jump = new JumpAnimation(jumpTexture);

            _current = _idle;
            Hitbox = new Hitbox();
        }

        public void Update(GameTime gameTime, List<Rectangle> platforms)
        {
            // --- Input ---
            Velocity.X = 0;
            if (IsRunningRight) { Velocity.X += moveSpeed; FacingRight = true; }
            if (IsRunningLeft) { Velocity.X -= moveSpeed; FacingRight = false; }

            // Springen
            if (Jump && IsOnGround(platforms))
            {
                Velocity.Y = jumpStrength;
            }

            // Gravity
            Velocity.Y += gravity;

            // --- Position updaten ---
            Position += Velocity;

            // --- Collision check ---
            ResolveCollisions(platforms);

            // --- Animatie update ---
            if (!IsOnGround(platforms))
                _current = _jump;
            else if (Velocity.X != 0)
                _current = _run;
            else
                _current = _idle;

            _current.Update(gameTime);

            // --- Update hitbox ---
            Rectangle currentFrame = _current.Frames[_current.CurrentFrame];
            Hitbox.Update(Position, currentFrame);
        }

        public void Draw(SpriteBatch sb)
        {
            _current.Draw(sb, Position, FacingRight);
        }

        // Check of de hero op een platform staat
        private bool IsOnGround(List<Rectangle> platforms)
        {
            foreach (var plat in platforms)
            {
                if (Hitbox.HitboxRect.Bottom + 1 >= plat.Top &&
                    Hitbox.HitboxRect.Bottom <= plat.Top + 5 &&
                    Hitbox.HitboxRect.Right > plat.Left &&
                    Hitbox.HitboxRect.Left < plat.Right)
                {
                    return true;
                }
            }
            return false;
        }

        // --- Resolutie van collisions ---
        private void ResolveCollisions(List<Rectangle> platforms)
        {
            foreach (var plat in platforms)
            {
                if (Hitbox.HitboxRect.Intersects(plat))
                {
                    Rectangle intersection = Rectangle.Intersect(Hitbox.HitboxRect, plat);

                    // Resolutie op Y-as (top/bottom)
                    if (intersection.Height < intersection.Width)
                    {
                        if (Hitbox.HitboxRect.Top < plat.Top)
                        {
                            Position.Y -= intersection.Height; // bovenop platform
                            Velocity.Y = 0;
                        }
                        else
                        {
                            Position.Y += intersection.Height; // onder platform
                            Velocity.Y = 0;
                        }
                    }
                    // Resolutie op X-as (links/rechts)
                    else
                    {
                        if (Hitbox.HitboxRect.Left < plat.Left)
                            Position.X -= intersection.Width; // tegen linkerzijde
                        else
                            Position.X += intersection.Width; // tegen rechterzijde

                        Velocity.X = 0;
                    }

                    // Update hitbox na correctie
                    Rectangle currentFrame = _current.Frames[_current.CurrentFrame];
                    Hitbox.Update(Position, currentFrame);
                }
            }
        }
    }
}
