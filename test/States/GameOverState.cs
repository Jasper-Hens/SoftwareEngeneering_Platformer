using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace test.States
{

    public class GameOverState : GameState
    {
        private Texture2D _backgroundTexture;
        private Texture2D _restartButtonTexture;

        private Vector2 _buttonPosition;
        private Rectangle _buttonRect;

        public GameOverState(Game1 game, ContentManager content) : base(game, content)
        {
        }

        public override void LoadContent()
        {
            _game.IsMouseVisible = true;

            // Laad de GameOver texture
            _backgroundTexture = _content.Load<Texture2D>("GameState/GameOver");
            // Laad de Restart knop (of PlayButton)
            _restartButtonTexture = _content.Load<Texture2D>("HomeScreen/PlayButton");

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
                // 1. Verander het plaatje naar de "ingedrukte" of "hover" versie
                _restartButtonTexture = _content.Load<Texture2D>("HomeScreen/PlayButtonPressed");

                // 2. Check voor klik
                if (mouse.LeftButton == ButtonState.Pressed)
                {
                    // Start het spel opnieuw
                    _game.ChangeState(new PlayingState(_game, _content));
                }
            }
            else
            {
                // Muis is NIET op de knop: Zet het normale plaatje terug
                _restartButtonTexture = _content.Load<Texture2D>("HomeScreen/PlayButton");
            }
        }

        public override void Draw(SpriteBatch sb)
        {
            sb.Begin();
            int screenW = _game.GraphicsDevice.Viewport.Width;
            int screenH = _game.GraphicsDevice.Viewport.Height;

            sb.Draw(_backgroundTexture, new Rectangle(0, 0, screenW, screenH), Color.White);
            sb.Draw(_restartButtonTexture, _buttonRect, Color.White);
            sb.End();
        }
    }
}