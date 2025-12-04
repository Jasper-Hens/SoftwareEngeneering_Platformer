using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;
using test.Blocks;
using static System.Formats.Asn1.AsnWriter;

namespace test
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // texture2D
        private Texture2D _idleTexture;
        private Texture2D _runTexture;
        private Texture2D _jumpTexture;
        private Texture2D _blokTexture;

        private Hero _hero;

        private Texture2D _tilesSpriteSheet;
        private Texture2D _brightBackGroundSpriteSheet;
        private Texture2D _paleBackGroundSpriteSheet;
        private Texture2D _ItemSpriteSheet;
        private Texture2D _ObjectSpriteSheet;
        private List<Block> _blocks;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {

            base.Initialize();


        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _idleTexture = Content.Load<Texture2D>("Idle");
            _runTexture = Content.Load<Texture2D>("Run");
            _jumpTexture = Content.Load<Texture2D>("Jump");

            _blokTexture = new Texture2D(GraphicsDevice, 1, 1);
            _blokTexture.SetData(new[] { Color.White });

            _hero = new Hero(_idleTexture, _runTexture, _jumpTexture);

            _tilesSpriteSheet = Content.Load<Texture2D>("Tiles/tilesSpriteSheet");
            _brightBackGroundSpriteSheet = Content.Load<Texture2D>("Background/BrightBackgroundSpriteSheet");
            _paleBackGroundSpriteSheet = Content.Load<Texture2D>("Background/paleBackgroundSpriteSheet");
            _ItemSpriteSheet = Content.Load<Texture2D>("Items/ItemSpriteSheet");
            _ObjectSpriteSheet = Content.Load<Texture2D>("Objects/ObjectSpriteSheet");

            _hero = new Hero(_idleTexture, _runTexture, _jumpTexture);

            // INITIALISEER de blokken lijst
            _blocks = new List<Block>();

            // Definitie van de tile uitsnede (SourceRectangle): sprite12, 325, 79, 64, 50
            Rectangle sourceTile = new Rectangle(325, 79, 64, 50);

            // Definieer de World Positie/Hitbox (Destination Rectangle)
            // Tip: Maak de grootte van de BoundingBox gelijk aan de SourceRectangle hoogte/breedte 
            // zodat de tile correct wordt getekend zonder te stretchen.
            Rectangle groundRect = new Rectangle(0, 400, 800, 50); // grond (stretchen is OK hier)
            Rectangle platformRect = new Rectangle(200, 310, 128, 50); // platform midden

            // MAAK de Block objecten aan en voeg ze toe
            // Hier maak je voor het 'grond'-blok een NIEUW Block aan met de tile uitsnede
            _blocks.Add(new Block(groundRect, sourceTile, _tilesSpriteSheet));
            _blocks.Add(new Block(platformRect, sourceTile, _tilesSpriteSheet));
        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState k = Keyboard.GetState();

            _hero.IsRunningRight = k.IsKeyDown(Keys.Right);
            _hero.IsRunningLeft = k.IsKeyDown(Keys.Left);
            _hero.Jump = k.IsKeyDown(Keys.Space);

            List<Rectangle> collisionRects = _blocks
                 .Select(b => b.BoundingBox)
                 .ToList();

            _hero.Update(gameTime, collisionRects);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            // Hero
            _hero.Draw(_spriteBatch);

            // Teken alle Block objecten
            foreach (var block in _blocks)
                block.Draw(_spriteBatch);

            // Hero hitbox debug
            _spriteBatch.Draw(_blokTexture, _hero.Hitbox.HitboxRect, Color.Red * 0.4f);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
