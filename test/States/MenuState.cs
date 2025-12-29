using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace test.States
{
    public class MenuState : GameState
    {
        private Texture2D _backgroundTexture;
        private Texture2D _playButtonTexture;

        private Vector2 _buttonPosition;
        private Rectangle _buttonRect;
        private Color _buttonColor = Color.White;

        public MenuState(Game1 game, ContentManager content) : base(game, content)
        {
        }

        public override void LoadContent()
        {

            // ZORG DAT DE MUIS ZICHTBAAR IS
            _game.IsMouseVisible = true;

            // 1. ACHTERGROND

            _backgroundTexture = _content.Load<Texture2D>("HomeScreen/HomeScreenPlatformer");

            // 2PLAY KNOP
            _playButtonTexture = _content.Load<Texture2D>("HomeScreen/PlayButton");

            // 3. POSITIE BEPALEN (Midden van scherm)
            int screenW = _game.GraphicsDevice.Viewport.Width;
            int screenH = _game.GraphicsDevice.Viewport.Height;

            // Hoe groot wil je de knop? (Pas dit aan aan de grootte van je plaatje)
            int btnWidth = 246;
            int btnHeight = 102;

            _buttonPosition = new Vector2((screenW / 2) - (btnWidth / 2), (screenH / 1) - 70 - (btnHeight / 2));
            _buttonRect = new Rectangle((int)_buttonPosition.X, (int)_buttonPosition.Y, btnWidth, btnHeight);
        }

        public override void Update(GameTime gameTime)
        {
            MouseState mouse = Mouse.GetState();

            // Check of muis op de knop staat
            if (_buttonRect.Contains(mouse.Position))
            {
                //_buttonColor = Color.Red; // Maak knop donkerder als je erop zweeft (Hover effect)
                _playButtonTexture = _content.Load<Texture2D>("HomeScreen/PlayButtonPressed");

                if (mouse.LeftButton == ButtonState.Pressed)
                {
                    // START DE GAME
                    _game.ChangeState(new PlayingState(_game, _content));
                }
            }
            else
            {
                _buttonColor = Color.White; // Normale kleur
                _playButtonTexture = _content.Load<Texture2D>("HomeScreen/PlayButton");
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            // TEKENd ACHTERGROND (Scalen naar schermgrootte)
            // Dit zorgt ervoor dat het plaatje altijd het hele scherm vult, ongeacht de resolutie.
            int screenW = _game.GraphicsDevice.Viewport.Width;
            int screenH = _game.GraphicsDevice.Viewport.Height;
            Rectangle destRect = new Rectangle(0, 0, screenW, screenH);

            spriteBatch.Draw(_backgroundTexture, destRect, Color.White);

            // 2. TEKEN DE PLAY KNOP
            spriteBatch.Draw(_playButtonTexture, _buttonRect, _buttonColor);

            spriteBatch.End();
        }
    }
}