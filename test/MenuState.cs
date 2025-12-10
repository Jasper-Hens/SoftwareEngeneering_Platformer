using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace test.States
{
    public class MenuState : GameState
    {
        private Texture2D _buttonTexture;
        private Vector2 _buttonPosition;
        private Rectangle _buttonRect;
        private Color _buttonColor = Color.White;

        public MenuState(Game1 game, ContentManager content) : base(game, content)
        {
        }

        public override void LoadContent()
        {
            // Laad een texture voor de knop. 
            // TIP: Gebruik tijdelijk je blok texture als je nog geen knop-plaatje hebt.
            _buttonTexture = _content.Load<Texture2D>("Tiles/tilesSpriteSheet"); // Of een specifieke knop texture

            // Zet de knop in het midden
            int btnWidth = 200;
            int btnHeight = 80;
            int screenW = _game.GraphicsDevice.Viewport.Width;
            int screenH = _game.GraphicsDevice.Viewport.Height;

            _buttonPosition = new Vector2((screenW / 2) - (btnWidth / 2), (screenH / 2) - (btnHeight / 2));
            _buttonRect = new Rectangle((int)_buttonPosition.X, (int)_buttonPosition.Y, btnWidth, btnHeight);
        }

        public override void Update(GameTime gameTime)
        {
            MouseState mouse = Mouse.GetState();

            // Check of muis op de knop staat
            if (_buttonRect.Contains(mouse.Position))
            {
                _buttonColor = Color.Gray; // Hover effect

                if (mouse.LeftButton == ButtonState.Pressed)
                {
                    // === HIER SCHAKELEN WE NAAR DE GAME ===
                    _game.ChangeState(new PlayingState(_game, _content));
                }
            }
            else
            {
                _buttonColor = Color.White;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            // Teken het menu zonder camera (gewoon plat op scherm)
            spriteBatch.Begin();

            // Teken de knop
            // (Ik gebruik hier een deel van je tilesheet als voorbeeld, pas source rect aan naar wens)
            spriteBatch.Draw(_buttonTexture, _buttonRect, _buttonColor);

            spriteBatch.End();
        }
    }
}