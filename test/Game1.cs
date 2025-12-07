using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;
using test.Blocks;
using test.Level;
using static System.Formats.Asn1.AsnWriter;

namespace test
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // character sprites
        private Texture2D _idleTexture;
        private Texture2D _runTexture;
        private Texture2D _jumpTexture;
        private Texture2D _blokTexture;

        private Hero _hero;

        // sprites
        private Texture2D _tilesSpriteSheet;
        private Texture2D _brightBackGroundSpriteSheet;
        private Texture2D _paleBackGroundSpriteSheet;
        private Texture2D _ItemSpriteSheet;
        private Texture2D _ObjectSpriteSheet;
        private List<Block> _blocks;


     
        /// ///////////////// camera
        
        private Camera _camera;

        // Level afmetingen (50 kolommen x 12 rijen)
        private const int TILE_SIZE = 64;
        private int _levelCols;
        private int _levelRows;
        private int _levelWidth;
        private int _levelHeight;

        // BACKGROUND

        private BackgroundLayer _skyLayer;
        private BackgroundLayer _natureLayer;
        private BackgroundLayer _wallLayer;
        private BackgroundLayer _floorLayer;
        private BackgroundLayer _pillarsLayer;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
            _graphics.ApplyChanges();
        }

        protected override void Initialize()
        {
            // Bereken level afmetingen
            _levelRows = _gameboard.GetLength(0);
            _levelCols = _gameboard.GetLength(1);
            _levelWidth = _levelCols * TILE_SIZE;
            _levelHeight = _levelRows * TILE_SIZE;

            // Camera initialisatie:
            _camera = new Camera(
                _graphics.PreferredBackBufferWidth,
                _graphics.PreferredBackBufferHeight,
                _levelWidth,
                _levelHeight
            );

            // Zorg ervoor dat de Hero op een startpositie staat
            // (bijvoorbeeld op de eerste tile, Y=Row 6 * TILE_SIZE = 384)
            _hero = new Hero(_idleTexture, _runTexture, _jumpTexture);
            _hero.Position = new Vector2(100, 300); // Start hoger dan de grond (Row 6)

            base.Initialize();
        }

        // In test/Game1.cs
        private int[,] _gameboard = new int[,]
        {
            // 50 kolommen breed (Index 0 tot 49)
            { 3,3,3,3,3,3,3,3,3,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 }, // 0
            { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 }, // 1
            { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 }, // 2
            { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 }, // 3
            { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 }, // 4
            { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 }, // 5
            { 0,0,0,0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 }, // 6 
            { 0,0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 }, // 7
            { 0,0,0,0,0,0,0,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 }, // 8
            { 0,0,0,0,0,0,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 }, // 9
            { 0,0,0,0,0,2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 }, // 10
            { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 }  // 11 (Onderste rij = Grond)
        };

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


            int rows = _gameboard.GetLength(0);
            int cols = _gameboard.GetLength(1);

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    int blockTypeInt = _gameboard[y, x];

                    // Gebruik de BlockFactory om het juiste blok te maken
                    Block newBlock = BlockFactory.CreateBlock(blockTypeInt, x, y, _tilesSpriteSheet);

                    if (newBlock != null)
                    {
                        _blocks.Add(newBlock);
                    }
                }
            }

            // BACKGROUND
            Rectangle brightBackgroundSourceRect = new Rectangle(961, 1084, 960, 541);

            int viewportWidth = _graphics.PreferredBackBufferWidth;
            int viewportHeight = _graphics.PreferredBackBufferHeight;

            // --- 1. Definieer de Source Rectangles ---
            Rectangle skyRect = new Rectangle(0, 542, 960, 541);
            Rectangle natureRect = new Rectangle(961, 154, 960, 387);
            Rectangle wallRect = new Rectangle(961, 542, 960, 541);
            Rectangle floorRect = new Rectangle(0, 456, 960, 85);
            Rectangle pillarsRect = new Rectangle(0, 1084, 960, 541);

            // --- 2. Initialisatie van de Lagen ---

            // Parallax Lagen (ScrollRatio < 1.0)

            // SKY: Achterste laag (meest rustig)
            _skyLayer = new BackgroundLayer(
                _paleBackGroundSpriteSheet, skyRect,
                0.1f, // 10% snelheid
                viewportWidth, viewportHeight, _levelWidth
            );

            // NATURE: Iets dichterbij
            _natureLayer = new BackgroundLayer(
                _paleBackGroundSpriteSheet, natureRect,
                0.1f, // Zelfde snelheid als Sky, zoals gevraagd
                viewportWidth, viewportHeight, _levelWidth
            );

            // Voorgrond Lagen (ScrollRatio = 1.0)
            // Hoewel ze niet bewegen, gebruiken we de klasse voor de Draw-logica.

            // WALL
            _wallLayer = new BackgroundLayer(
                _paleBackGroundSpriteSheet, wallRect,
                1.0f, // Volledige snelheid
                viewportWidth, viewportHeight, _levelWidth
            );

            // FLOOR (Deze moet de vloer van je tiles overlappen)
            _floorLayer = new BackgroundLayer(
                _paleBackGroundSpriteSheet, floorRect,
                1.0f, // Volledige snelheid
                viewportWidth, viewportHeight, _levelWidth
            );

            // PILLARS: Voorste laag
            _pillarsLayer = new BackgroundLayer(
                _paleBackGroundSpriteSheet, pillarsRect,
                1.0f, // Volledige snelheid
                viewportWidth, viewportHeight, _levelWidth
            );
        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState k = Keyboard.GetState();

            _hero.IsRunningRight = k.IsKeyDown(Keys.Right);
            _hero.IsRunningLeft = k.IsKeyDown(Keys.Left);
            _hero.Jump = k.IsKeyDown(Keys.Space);

            _hero.Update(gameTime, _blocks);

            // Camera volgen
            _camera.Follow(_hero.Position);

            base.Update(gameTime);
        }

        // In Game1.Draw()

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // --- 1. Parallax Lagen (Sky, Nature) ---
            _spriteBatch.Begin();

            _skyLayer.Draw(_spriteBatch, _camera.Position);
            _natureLayer.Draw(_spriteBatch, _camera.Position);

            _spriteBatch.End();


            // --- 2. Vaste Lagen (Wall, Floor, Pillars), Tiles en Hero ---
            _spriteBatch.Begin(transformMatrix: _camera.GetTransformMatrix());

            // Teken eerst de vaste (niet-bewegende) achtergronddelen, achter de tiles
            _wallLayer.Draw(_spriteBatch, _camera.Position);
            // Teken de Floor over de Tiles en Hero heen (als voorgrond, om de tiles te bedekken)
            _floorLayer.Draw(_spriteBatch, _camera.Position);
            // Teken de Pillars over de Tiles en Hero heen (als voorgrond)
            _pillarsLayer.Draw(_spriteBatch, _camera.Position);

            // Tiles (Level geometrie)
            foreach (var block in _blocks)
                block.Draw(_spriteBatch);

            // Hero
            _hero.Draw(_spriteBatch);

            // Hero hitbox debug
            _spriteBatch.Draw(_blokTexture, _hero.Hitbox.HitboxRect, Color.Red * 0.4f);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }

}
