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
            { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 }, // 0
            { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 }, // 1
            { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 }, // 2
            { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 }, // 3
            { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 }, // 4
            { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 }, // 5
            { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,2,2,2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1 }, // 6 (Platform 2, Grond 1)
            { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 }, // 7
            { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 }, // 8
            { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 }, // 9
            { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 }, // 10
            { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 }  // 11
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

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // 1. Teken de wereld (Blocks en Hero) met de camera transformatie
            _spriteBatch.Begin(transformMatrix: _camera.GetTransformMatrix());

            // Hero
            _hero.Draw(_spriteBatch);

            // Blocks
            foreach (var block in _blocks)
                block.Draw(_spriteBatch);

            // Hero hitbox debug
            _spriteBatch.Draw(_blokTexture, _hero.Hitbox.HitboxRect, Color.Red * 0.4f);

            _spriteBatch.End();

            // 2. Optioneel: Teken UI ZONDER camera transformatie
            // Dit zorgt ervoor dat de UI (score, health, etc.) stil blijft staan op het scherm
            /*
            _spriteBatch.Begin();
            // Teken hier UI elementen
            _spriteBatch.End();
            */

            base.Draw(gameTime);
        }
    }

}
