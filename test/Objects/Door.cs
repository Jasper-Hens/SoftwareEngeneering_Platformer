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

        public Rectangle Hitbox => new Rectangle((int)_position.X, (int)_position.Y, 77, 83);

        private Rectangle _closedFrameRect = new Rectangle(25, 409, 77, 83);
        private Rectangle _OpeningSourceRect = new Rectangle(154, 409, 77, 83);
        private Rectangle _openFrameRect = new Rectangle(283, 409, 77, 83);

        public Door(Texture2D texture, Texture2D animSheet, Vector2 position)
        {
            _texture = texture;
            _position = position;

            _openAnimation = new SimpleAnimation(animSheet);
            _openAnimation.IsLooping = false;

            _openAnimation.Frames.Add(_closedFrameRect);
            _openAnimation.Frames.Add(_OpeningSourceRect);
            _openAnimation.Frames.Add(_openFrameRect);
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

            // FIX: hero.Hitbox is nu een Rectangle
            if (Hitbox.Intersects(hero.Hitbox))
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

        public void ForceOpen()
        {
            IsOpen = true;
            _isAnimating = false;
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