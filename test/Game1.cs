using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using test.States; // Zorg dat deze namespace klopt

namespace test
{
    public class Game1 : Game
    {
        // VARIABELEN MOETEN HIER STAAN (Binnen de class, buiten methodes)
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private GameState _currentState;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // --- snelheid fix ---
            IsFixedTimeStep = true;
            TargetElapsedTime = System.TimeSpan.FromSeconds(1d / 60d);

            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
            _graphics.ApplyChanges();
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // START MET HET MENU
            _currentState = new MenuState(this, Content);
            _currentState.LoadContent();
        }

        // Methode om van state te wisselen
        public void ChangeState(GameState newState)
        {
            _currentState = newState;
            _currentState.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();

            // Update de huidige state
            if (_currentState != null)
                _currentState.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Teken de huidige state
            if (_currentState != null)
                _currentState.Draw(_spriteBatch);

            base.Draw(gameTime);
        }
    }
}