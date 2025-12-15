using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace test.States
{
    public class GameOverState : GameState
    {
        private Texture2D _backgroundTexture;

        // Textures voor Play Again knop
        private Texture2D _playAgainTexture, _playAgainTexturePressed, _currentPlayAgainTexture;

        // Textures voor Home knop
        private Texture2D _homeTexture, _homeTexturePressed, _currentHomeTexture;

        private Rectangle _playAgainRect;
        private Rectangle _homeRect;

        public GameOverState(Game1 game, ContentManager content) : base(game, content)
        {
        }

        public override void LoadContent()
        {
            _game.IsMouseVisible = true;

            // 1. Laad de Achtergrond
            _backgroundTexture = _content.Load<Texture2D>("GameState/GameOver");

            // 2. Laad de knoppen (Let op de bestandsnamen uit je Content.mgcb)
            _playAgainTexture = _content.Load<Texture2D>("HomeScreen/PlayAgainButton - kopie");
            _playAgainTexturePressed = _content.Load<Texture2D>("HomeScreen/PlayAgainButtonPressed - kopie");

            _homeTexture = _content.Load<Texture2D>("HomeScreen/HomeButton - kopie");
            _homeTexturePressed = _content.Load<Texture2D>("HomeScreen/HomeButtonPressed - kopie");

            // Standaard textures instellen
            _currentPlayAgainTexture = _playAgainTexture;
            _currentHomeTexture = _homeTexture;

            // 3. Posities bepalen (Naast elkaar of onder elkaar)
            int screenW = _game.GraphicsDevice.Viewport.Width;
            int screenH = _game.GraphicsDevice.Viewport.Height;

            int btnWidth = 200; // Pas aan naar wens
            int btnHeight = 80;
            int spacing = 20;   // Ruimte tussen de knoppen

            // We zetten ze naast elkaar in het midden, iets onder het midden van het scherm
            int totalWidth = (btnWidth * 2) + spacing;
            int startX = (screenW - totalWidth) / 2;
            int startY = (screenH / 2) + 150;

            _playAgainRect = new Rectangle(startX, startY, btnWidth, btnHeight);
            _homeRect = new Rectangle(startX + btnWidth + spacing, startY, btnWidth, btnHeight);
        }

        public override void Update(GameTime gameTime)
        {
            MouseState mouse = Mouse.GetState();

            // --- LOGICA VOOR PLAY AGAIN KNOP ---
            if (_playAgainRect.Contains(mouse.Position))
            {
                _currentPlayAgainTexture = _playAgainTexturePressed;
                if (mouse.LeftButton == ButtonState.Pressed)
                {
                    // Herstart het spel
                    _game.ChangeState(new PlayingState(_game, _content));
                }
            }
            else
            {
                _currentPlayAgainTexture = _playAgainTexture;
            }

            // --- LOGICA VOOR HOME KNOP ---
            if (_homeRect.Contains(mouse.Position))
            {
                _currentHomeTexture = _homeTexturePressed;
                if (mouse.LeftButton == ButtonState.Pressed)
                {
                    // Ga naar hoofdmenu
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

            // Teken achtergrond
            sb.Draw(_backgroundTexture, new Rectangle(0, 0, screenW, screenH), Color.White);

            // Teken knoppen
            sb.Draw(_currentPlayAgainTexture, _playAgainRect, Color.White);
            sb.Draw(_currentHomeTexture, _homeRect, Color.White);

            sb.End();
        }
    }
}