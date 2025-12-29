using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace test.States
{
    public class WinState : GameState
    {
        private Texture2D _backgroundTexture;

        // Textures voor knoppen
        private Texture2D _playAgainTexture, _playAgainTexturePressed, _currentPlayAgainTexture;
        private Texture2D _homeTexture, _homeTexturePressed, _currentHomeTexture;

        private Rectangle _playAgainRect;
        private Rectangle _homeRect;

        public WinState(Game1 game, ContentManager content) : base(game, content)
        {
        }

        public override void LoadContent()
        {
            _game.IsMouseVisible = true;

            // 1. Laad Victory achtergrond
            _backgroundTexture = _content.Load<Texture2D>("GameState/VictoryV2");

            // 2. Laad de knoppen
            _playAgainTexture = _content.Load<Texture2D>("HomeScreen/PlayAgainButton - kopie");
            _playAgainTexturePressed = _content.Load<Texture2D>("HomeScreen/PlayAgainButtonPressed - kopie");

            _homeTexture = _content.Load<Texture2D>("HomeScreen/HomeButton - kopie");
            _homeTexturePressed = _content.Load<Texture2D>("HomeScreen/HomeButtonPressed - kopie");

            _currentPlayAgainTexture = _playAgainTexture;
            _currentHomeTexture = _homeTexture;

            // 3. Posities (Zelfde layout als GameOver)
            int screenW = _game.GraphicsDevice.Viewport.Width;
            int screenH = _game.GraphicsDevice.Viewport.Height;

            int btnWidth = 200;
            int btnHeight = 80;
            int spacing = 20;

            int totalWidth = (btnWidth * 2) + spacing;
            int startX = (screenW - totalWidth) / 2;
            int startY = (screenH / 2) + 150; // Iets lager dan het midden

            _playAgainRect = new Rectangle(startX, startY, btnWidth, btnHeight);
            _homeRect = new Rectangle(startX + btnWidth + spacing, startY, btnWidth, btnHeight);
        }

        public override void Update(GameTime gameTime)
        {
            MouseState mouse = Mouse.GetState();

            // --- PLAY AGAIN ---
            if (_playAgainRect.Contains(mouse.Position))
            {
                _currentPlayAgainTexture = _playAgainTexturePressed;
                if (mouse.LeftButton == ButtonState.Pressed)
                {
                    _game.ChangeState(new PlayingState(_game, _content));
                }
            }
            else
            {
                _currentPlayAgainTexture = _playAgainTexture;
            }

            // --- HOME ---
            if (_homeRect.Contains(mouse.Position))
            {
                _currentHomeTexture = _homeTexturePressed;
                if (mouse.LeftButton == ButtonState.Pressed)
                {
                    _game.ChangeState(new MenuState(_game, _content));
                }
            }
            else
            {
                _currentHomeTexture = _homeTexture;
            }
        }

        public override void Draw(SpriteBatch sb)
        {
            sb.Begin();
            int screenW = _game.GraphicsDevice.Viewport.Width;
            int screenH = _game.GraphicsDevice.Viewport.Height;

            sb.Draw(_backgroundTexture, new Rectangle(0, 0, screenW, screenH), Color.White);
            sb.Draw(_currentPlayAgainTexture, _playAgainRect, Color.White);
            sb.Draw(_currentHomeTexture, _homeRect, Color.White);
            sb.End();
        }
    }
}