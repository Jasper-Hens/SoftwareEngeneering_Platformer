using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using test.Animations;
using test.block_Interfaces;
using test.Blocks;

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

        // Vaste afmetingen voor de hitbox (breedste/hoogste frame van alle animaties)
        private const int HITBOX_WIDTH = 69;
        private const int HITBOX_HEIGHT = 65;

        public Hero(Texture2D idleTexture, Texture2D runTexture, Texture2D jumpTexture)
        {
            _idle = new IdleAnimation(idleTexture);
            _run = new RunAnimation(runTexture);
            _jump = new JumpAnimation(jumpTexture);

            _current = _idle;
            Hitbox = new Hitbox();
        }

        public void Update(GameTime gameTime, List<Block> blocks)
        {
            // --- Input ---
            Velocity.X = 0;
            if (IsRunningRight) { Velocity.X += moveSpeed; FacingRight = true; }
            if (IsRunningLeft) { Velocity.X -= moveSpeed; FacingRight = false; }

            // Springen
            if (Jump && IsOnGround(blocks))
            {
                Velocity.Y = jumpStrength;
            }

            // Gravity
            Velocity.Y += gravity;

            // X-beweging en Collision (zijwaartse beweging wordt alleen door ISolid blocks geblokkeerd)
            Position.X += Velocity.X;
            ResolveCollisionsX(blocks);

            // Y-beweging en Collision
            Position.Y += Velocity.Y;
            ResolveCollisionsY(blocks);

            // --- Animatie update ---
            if (!IsOnGround(blocks))
                _current = _jump;
            else if (Velocity.X != 0)
                _current = _run;
            else
                _current = _idle;

            _current.Update(gameTime);

            // --- Update hitbox ---
            Hitbox.Update(Position, HITBOX_WIDTH, HITBOX_HEIGHT);
        }

        public void Draw(SpriteBatch sb)
        {
            _current.Draw(sb, Position, FacingRight);
        }

        // Check of de hero op een platform staat
        private bool IsOnGround(List<Block> blocks)
        {
            // Tijdelijke hitbox (1px lager) om te checken of we op de grond staan
            Rectangle footCheckRect = Hitbox.HitboxRect;
            footCheckRect.Y += 1;

            foreach (var block in blocks)
            {
                // Alleen controleren op Solid en Platform blocks
                if (block is ISolid || block is IPlatform)
                {
                    Rectangle plat = block.BoundingBox;

                    if (footCheckRect.Intersects(plat))
                    {
                        // Zorg ervoor dat de hero bovenop het platform staat (Bottom moet ongeveer gelijk zijn aan Top)
                        if (Hitbox.HitboxRect.Bottom <= plat.Top + 1)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        // --- Resolutie van X-as collisions ---
        private void ResolveCollisionsX(List<Block> blocks)
        {
            Hitbox.Update(Position, HITBOX_WIDTH, HITBOX_HEIGHT);

            foreach (var block in blocks)
            {
                if (block is ISolid) // Alleen Solid blocks blokkeren zijwaartse beweging
                {
                    Rectangle plat = block.BoundingBox;

                    if (Hitbox.HitboxRect.Intersects(plat))
                    {
                        Rectangle intersection = Rectangle.Intersect(Hitbox.HitboxRect, plat);

                        // Verschuif terug op X-as
                        if (Velocity.X > 0)
                            Position.X -= intersection.Width;
                        else if (Velocity.X < 0)
                            Position.X += intersection.Width;

                        Velocity.X = 0;

                        // Update hitbox na correctie
                        Hitbox.Update(Position, HITBOX_WIDTH, HITBOX_HEIGHT);
                    }
                }
            }
        }

        // --- Resolutie van Y-as collisions ---
        private void ResolveCollisionsY(List<Block> blocks)
        {
            Hitbox.Update(Position, HITBOX_WIDTH, HITBOX_HEIGHT);

            foreach (var block in blocks)
            {
                if (block is ISolid || block is IPlatform)
                {
                    Rectangle plat = block.BoundingBox;

                    if (Hitbox.HitboxRect.Intersects(plat))
                    {
                        Rectangle intersection = Rectangle.Intersect(Hitbox.HitboxRect, plat);

                        if (intersection.Height < intersection.Width)
                        {
                            if (Velocity.Y > 0) // Naar beneden bewegen (landen/vallen op platform)
                            {
                                // IPlatform check: als we van onder komen, negeer de botsing
                                if (block is IPlatform && Hitbox.HitboxRect.Bottom - intersection.Height > plat.Top)
                                {
                                    continue; // Ga door het platform heen
                                }

                                // Solide of IPlatform van boven: Land op het blok
                                Position.Y -= intersection.Height;
                                Velocity.Y = 0;
                            }
                            else if (Velocity.Y < 0) // Naar boven bewegen (tegen onderkant platform)
                            {
                                // IPlatform blokkeert NIET van onder, ISolid blokkeert WEL
                                if (block is ISolid)
                                {
                                    Position.Y += intersection.Height;
                                    Velocity.Y = 0;
                                }
                                // IPlatform wordt genegeerd, zodat de speler er doorheen springt.
                            }

                            // Update hitbox na correctie
                            Hitbox.Update(Position, HITBOX_WIDTH, HITBOX_HEIGHT);
                        }
                    }
                }
            }
        }


    }
}
        

