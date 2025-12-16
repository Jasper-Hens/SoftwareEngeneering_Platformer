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

        // AANGEPAST: Hitbox matcht nu met de grootte van je sprite (77x83)
        public Rectangle Hitbox => new Rectangle((int)_position.X, (int)_position.Y, 77, 83);

        // We slaan het rechthoekje van de dichte deur op om herhaling te voorkomen
        private Rectangle _closedFrameRect = new Rectangle(25, 409, 77, 83);

        public Door(Texture2D texture, Texture2D animSheet, Vector2 position)
        {
            _texture = texture;
            _position = position;

            // Maak de animatie aan
            _openAnimation = new SimpleAnimation(animSheet);
            _openAnimation.IsLooping = false;

            // De frames die je zelf had opgegeven:
            _openAnimation.Frames.Add(new Rectangle(25, 409, 77, 83));  // Frame 1 (Dicht/Start)
            _openAnimation.Frames.Add(new Rectangle(154, 409, 77, 83)); // Frame 2
            _openAnimation.Frames.Add(new Rectangle(283, 409, 77, 83)); // Frame 3 (Open)
        }

        public void Update(GameTime gameTime, Hero hero)
        {
            if (_isAnimating)
            {
                _openAnimation.Update(gameTime);
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
                // Zorg dat inventory niet null is en key heeft
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

        public void Draw(SpriteBatch sb)
        {
            if (_isAnimating || IsOpen)
            {
                // Teken de animatie
                _openAnimation.Draw(sb, _position, true, Color.White);
            }
            else
            {
                // HIER ZAT HET PROBLEEM:
                // We tekenen nu het specifieke stukje uit de sheet (25, 409) in plaats van (0,0)
                sb.Draw(_texture, _position, _closedFrameRect, Color.White);
            }
        }
    }
}