using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using test.Animations;

namespace test.Objects
{
    public class Door
    {
        private Texture2D _texture;
        private Vector2 _position;
        private SimpleAnimation _openAnimation;
        private bool _isAnimating = false;

        public bool IsOpen { get; private set; } = false;

        // Hitbox matcht met de grootte van je sprite (77x83)
        public Rectangle Hitbox => new Rectangle((int)_position.X, (int)_position.Y, 77, 83);

        // --- HIER ZIJN JE SPRITE FRAMES ---
        private Rectangle _closedFrameRect = new Rectangle(25, 409, 77, 83);
        private Rectangle _OpeningSourceRect = new Rectangle(154, 409, 77, 83);
        private Rectangle _openFrameRect = new Rectangle(283, 409, 77, 83);
        // ----------------------------------

        public Door(Texture2D texture, Texture2D animSheet, Vector2 position)
        {
            _texture = texture;
            _position = position;

            // Maak de animatie aan
            _openAnimation = new SimpleAnimation(animSheet);
            _openAnimation.IsLooping = false;

            // Frames toevoegen
            _openAnimation.Frames.Add(_closedFrameRect);   // Frame 0: Dicht
            _openAnimation.Frames.Add(_OpeningSourceRect); // Frame 1: Half open
            _openAnimation.Frames.Add(_openFrameRect);     // Frame 2: Open
        }

        public void Update(GameTime gameTime, Hero hero)
        {
            if (_isAnimating)
            {
                _openAnimation.Update(gameTime);

                // Als de animatie klaar is, blijft hij op het laatste frame staan (Open)
                if (_openAnimation.IsFinished)
                {
                    IsOpen = true;
                    _isAnimating = false;
                }
                return;
            }

            if (IsOpen) return;

            // Interactie check
            if (Hitbox.Intersects(hero.Hitbox.HitboxRect))
            {
                if (hero.Inventory != null && hero.Inventory.HasKey)
                {
                    KeyboardState k = Keyboard.GetState();
                    if (k.IsKeyDown(Keys.W) || k.IsKeyDown(Keys.Up))
                    {
                        Open();
                    }
                }
            }
        }

        private void Open()
        {
            _isAnimating = true;
        }

        // --- NIEUW: DEUR FORCEREN OM OPEN TE ZIJN (VOOR STARTLEVELS) ---
        public void ForceOpen()
        {
            IsOpen = true;
            _isAnimating = false;
            // Zet de animatie direct op het laatste frame (frame index 2 is de open-status)
            _openAnimation.CurrentFrame = 2;
        }

        public void Draw(SpriteBatch sb)
        {
            if (_isAnimating || IsOpen)
            {
                Rectangle currentRect = _openAnimation.Frames[_openAnimation.CurrentFrame];
                sb.Draw(_openAnimation.Texture, _position, currentRect, Color.White);
            }
            else
            {
                sb.Draw(_texture, _position, _closedFrameRect, Color.White);
            }
        }
    }
}