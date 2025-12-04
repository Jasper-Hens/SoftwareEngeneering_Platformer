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

            // Sla de breedte en hoogte op om de individuele blokken te bepalen
            int tileWidth = sourceTile.Width;   // 64
            int tileHeight = sourceTile.Height; // 50

            // --- GROND: Meerdere tiles genereren (over 800 pixels breed) ---
            int groundLevelY = 400; // De Y-positie waar de vloer begint
            int worldWidth = 800;   // Totale breedte van het scherm

            // Loop van x=0 tot de wereldbreedte, met stappen van de tile breedte
            for (int x = 0; x < worldWidth; x += tileWidth)
            {
                // Destination Rect: Nieuwe positie (x, y), met de breedte en hoogte van de tile
                Rectangle worldRect = new Rectangle(x, groundLevelY, tileWidth, tileHeight);

                // Maak een nieuw Block object aan
                _blocks.Add(new Block(worldRect, sourceTile, _tilesSpriteSheet));
            }

            // --- MIDDEN PLATFORM: Eén tile herhaald ---
            // We maken hier bijvoorbeeld 3 tiles achter elkaar op positie 200, 310
            int platformX = 200;
            int platformY = 310;
            int platformTilesCount = 3;

            for (int i = 0; i < platformTilesCount; i++)
            {
                int currentX = platformX + (i * tileWidth);
                Rectangle worldRect = new Rectangle(currentX, platformY, tileWidth, tileHeight);

                _blocks.Add(new Block(worldRect, sourceTile, _tilesSpriteSheet));
            }
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
