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

        // aanvallen
        private Texture2D _attack1Texture;
        private Texture2D _attack2Texture;
        private Texture2D _attack3Texture;
        private Texture2D _runAttackTexture;

        // slash animatie
        private Texture2D _slashTexture;

        
        // enemies 
        private List<Enemy> _enemies;

        // Boss Textures Variables
        private Texture2D _bossIdle, _bossWalk, _bossRun, _bossJump;
        private Texture2D _bossAtk1, _bossAtk2, _bossAtk3, _bossWalkAtk, _bossSpecial;
        private Texture2D _bossHurt, _bossDeath;
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // --- snelheid fix ---
            IsFixedTimeStep = true;
            TargetElapsedTime = System.TimeSpan.FromSeconds(1d / 60d); // Forceer 60 keer per seconde
                                                                       // --------------------------------------------

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
            _hero = new Hero(_idleTexture, _runTexture, _jumpTexture, _attack1Texture, _attack2Texture, _attack3Texture, _runAttackTexture, _slashTexture);
            _hero.Position = new Vector2(100, 300); // Start hoger dan de grond (Row 6)

            base.Initialize();
        }

        // In test/Game1.cs
        private int[,] _gameboard = new int[,]
        {
            // 50 kolommen breed (Index 0 tot 49)
            { 3,3,3,3,3,3,3,3,3,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 }, // 0
            { 3,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 }, // 1
            { 3,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 }, // 2
            { 3,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 }, // 3
            { 3,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 }, // 4
            { 3,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 }, // 5
            { 3,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 }, // 6 
            { 3,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,2,2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 }, // 7
            { 3,0,0,0,0,0,0,0,1,0,0,0,0,0,2,2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 }, // 8
            { 3,0,0,0,0,0,0,0,1,0,0,0,2,2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 }, // 9
            { 3,0,0,0,0,0,0,0,0,0,2,2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 }, // 10
            { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 }  // 11 (Onderste rij = Grond)
        };

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _idleTexture = Content.Load<Texture2D>("Idle");
            _runTexture = Content.Load<Texture2D>("Run");
            _jumpTexture = Content.Load<Texture2D>("Jump");

            // attacks
            _attack1Texture = Content.Load<Texture2D>("Attacks/Attack 1");
            _attack2Texture = Content.Load<Texture2D>("Attacks/Attack 2");
            _attack3Texture = Content.Load<Texture2D>("Attacks/Attack 3");
            _runAttackTexture = Content.Load<Texture2D>("Attacks/Run+Attack");

            // slash effect
            _slashTexture = Content.Load<Texture2D>("Attacks/SlashSprite");

            _blokTexture = new Texture2D(GraphicsDevice, 1, 1);
            _blokTexture.SetData(new[] { Color.White });

            _hero = new Hero(_idleTexture, _runTexture, _jumpTexture, _attack1Texture, _attack2Texture, _attack3Texture, _runAttackTexture, _slashTexture);

            _tilesSpriteSheet = Content.Load<Texture2D>("Tiles/tilesSpriteSheet");
            _brightBackGroundSpriteSheet = Content.Load<Texture2D>("Background/BrightBackgroundSpriteSheet");
            _paleBackGroundSpriteSheet = Content.Load<Texture2D>("Background/paleBackgroundSpriteSheet");
            _ItemSpriteSheet = Content.Load<Texture2D>("Items/ItemSpriteSheet");
            _ObjectSpriteSheet = Content.Load<Texture2D>("Objects/ObjectSpriteSheet");

            _hero = new Hero(_idleTexture, _runTexture, _jumpTexture, _attack1Texture, _attack2Texture, _attack3Texture, _runAttackTexture, _slashTexture);

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
                /////////////////////////////////////////////
                _enemies = new List<Enemy>();

                // 1. LAAD ALLE BOSS TEXTURES (Pas namen aan aan MGCB!)
                _bossIdle = Content.Load<Texture2D>("Boss/Idle");
                _bossWalk = Content.Load<Texture2D>("Boss/Walk");
                _bossRun = Content.Load<Texture2D>("Boss/Run");
                _bossJump = Content.Load<Texture2D>("Boss/Jump");

                _bossAtk1 = Content.Load<Texture2D>("Boss/Attack"); // Attack 1
                _bossAtk2 = Content.Load<Texture2D>("Boss/Attack2");
                _bossAtk3 = Content.Load<Texture2D>("Boss/Attack3");

                _bossWalkAtk = Content.Load<Texture2D>("Boss/Walk_Attack");
                _bossSpecial = Content.Load<Texture2D>("Boss/Special");

                _bossHurt = Content.Load<Texture2D>("Boss/Hurt");
                _bossDeath = Content.Load<Texture2D>("Boss/Death");

                // 2. MAAK DE BOSS AAN
                // Geef ALLE textures mee in de juiste volgorde
                KnightBoss boss = new KnightBoss(
                    new Vector2(600, 500), // Start Positie
                    _blokTexture,          // Pixel texture voor healthbar
                    _bossIdle, _bossWalk, _bossRun, _bossJump,
                    _bossAtk1, _bossAtk2, _bossAtk3,
                    _bossWalkAtk, _bossSpecial,
                    _bossHurt, _bossDeath
                );

                _enemies.Add(boss);
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

            // Update Enemies
            foreach (var enemy in _enemies)
            {
                enemy.Update(gameTime, _blocks, _hero);

                // Damage Check (Hero slaat Enemy)
                // Check of de BOSS de HERO raakt
                if (_hero.IsHitting && !enemy.IsDead)
                {
                    // Check intersection
                    if (_hero.AttackHitbox.Intersects(enemy.Hitbox))
                    {
                        // Doe damage
                        enemy.TakeDamage(10); // Of _hero.Damage variabele

                        // GEVRAAGD: Debug bericht
                        System.Diagnostics.Debug.WriteLine($"RAAK! Hero hit Boss. Boss HP: {enemy.CurrentHealth}");
                    }
                }

                // --- 2. BOSS RAAKT HERO ---
                if (enemy.IsHitting && !enemy.IsDead)
                {
                    if (enemy.AttackHitbox.Intersects(_hero.Hitbox.HitboxRect))
                    {
                        // Hier later Hero damage logic (bijv _hero.TakeDamage(20))

                        // GEVRAAGD: Debug bericht
                        System.Diagnostics.Debug.WriteLine("AU! Boss hit Hero!");
                    }
                }

                // Damage Check (Enemy raakt Hero) -> Moet je nog toevoegen in Hero!
                if (!enemy.IsDead && enemy.Hitbox.Intersects(_hero.Hitbox.HitboxRect))
                {
                    // _hero.TakeDamage(1);
                }
            }

            for (int i = _enemies.Count - 1; i >= 0; i--)
            {
                var enemy = _enemies[i];
                enemy.Update(gameTime, _blocks, _hero);

                // ... Damage checks ...

                // VERWIJDEREN ALS DOOD & ANIMATIE KLAAR IS
                if (enemy.ReadyToRemove)
                {
                    _enemies.RemoveAt(i);
                }
            }

            base.Update(gameTime);
        }

        // In Game1.Draw()

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // 1. Parallax (Achtergrond)
            _spriteBatch.Begin();
            _skyLayer.Draw(_spriteBatch, _camera.Position);
            _natureLayer.Draw(_spriteBatch, _camera.Position);
            _spriteBatch.End();

            // 2. De Wereld (Met Camera)
            _spriteBatch.Begin(transformMatrix: _camera.GetTransformMatrix());

            // Achtergrond lagen
            _wallLayer.Draw(_spriteBatch, _camera.Position);
            _floorLayer.Draw(_spriteBatch, _camera.Position); // Floor achter hero/enemies? Of ervoor?
                                                              // (Jij wilde floor achter hero/enemies in vorige prompt, dus hier is prima)

            // Pillars (achter de karakters?)
            _pillarsLayer.Draw(_spriteBatch, _camera.Position);

            // Tiles
            foreach (var block in _blocks)
                block.Draw(_spriteBatch);

            // Hero
            _hero.Draw(_spriteBatch);

            foreach (var enemy in _enemies)
            {
                enemy.Draw(_spriteBatch);
            }
            // ---------------------------

            if (_hero.IsHitting)
            {
                _spriteBatch.Draw(_blokTexture, _hero.AttackHitbox, Color.Red * 0.5f);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }

}
