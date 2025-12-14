using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace test.States
{
    public class WinState : GameState
    {
        private Texture2D _backgroundTexture;
        private Texture2D _menuButtonTexture;

        private Vector2 _buttonPosition;
        private Rectangle _buttonRect;

        public WinState(Game1 game, ContentManager content) : base(game, content)
        {
        }

        public override void LoadContent()
        {
            _game.IsMouseVisible = true;

            // 1. Laad jouw Victory PNG
            _backgroundTexture = _content.Load<Texture2D>("GameState/VictoryV1");
            _menuButtonTexture = _content.Load<Texture2D>("HomeScreen/PlayButton");

            // 2. Positie
            int screenW = _game.GraphicsDevice.Viewport.Width;
            int screenH = _game.GraphicsDevice.Viewport.Height;

            int btnWidth = 200;
            int btnHeight = 100;

            _buttonPosition = new Vector2((screenW / 2) - (btnWidth / 2), (screenH / 2) + 100);
            _buttonRect = new Rectangle((int)_buttonPosition.X, (int)_buttonPosition.Y, btnWidth, btnHeight);
        }

        public override void Update(GameTime gameTime)
        {
            MouseState mouse = Mouse.GetState();

            // Check of de muis OP de knop staat
            if (_buttonRect.Contains(mouse.Position))
            {
                // 1. Verander het plaatje naar de "ingedrukte" versie
                _menuButtonTexture = _content.Load<Texture2D>("HomeScreen/PlayButtonPressed");

                // 2. Check voor klik
                if (mouse.LeftButton == ButtonState.Pressed)
                {
                    // Ga terug naar het hoofdmenu
                    _game.ChangeState(new MenuState(_game, _content));
                }
            }
            else
            {
                // Muis is NIET op de knop: Zet het normale plaatje terug
                _menuButtonTexture = _content.Load<Texture2D>("HomeScreen/PlayButton");
            }
        }

        public override void Draw(SpriteBatch sb)
        {
            sb.Begin();
            int screenW = _game.GraphicsDevice.Viewport.Width;
            int screenH = _game.GraphicsDevice.Viewport.Height;
            sb.Draw(_backgroundTexture, new Rectangle(0, 0, screenW, screenH), Color.White);
            sb.Draw(_menuButtonTexture, _buttonRect, Color.White);
            sb.End();
        }
    }
}