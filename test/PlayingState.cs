using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using test.Blocks;
using test.Level;
using test.block_Interfaces; // Misschien nodig voor ISolid?
using System;
using Microsoft.Xna.Framework.Input;

namespace test.States
{
    public class PlayingState : GameState
    {
        // LEVEL DATA
        private int[,] _gameboard = new int[,]
        {
            { 3,3,3,3,3,3,3,3,3,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
            { 3,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
            { 3,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
            { 3,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
            { 3,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
            { 3,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
            { 3,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
            { 3,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,2,2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
            { 3,0,0,0,0,0,0,0,1,0,0,0,0,0,2,2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
            { 3,0,0,0,0,0,0,0,1,0,0,0,2,2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
            { 3,0,0,0,0,0,0,0,0,0,2,2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
            { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 }
        };

        // VARIABELEN
        private Texture2D _idleTexture, _runTexture, _jumpTexture, _blokTexture;
        private Texture2D _attack1Texture, _attack2Texture, _attack3Texture, _runAttackTexture, _slashTexture;

        // Background Textures
        private Texture2D _tilesSpriteSheet, _brightBackGroundSpriteSheet, _paleBackGroundSpriteSheet;
        private Texture2D _ItemSpriteSheet, _ObjectSpriteSheet; // Worden deze gebruikt?

        // Boss Textures
        private Texture2D _bossIdle, _bossWalk, _bossRun, _bossJump;
        private Texture2D _bossAtk1, _bossAtk2, _bossAtk3, _bossWalkAtk, _bossSpecial;
        private Texture2D _bossHurt, _bossDeath;

        // Game Objects
        private Hero _hero;
        private List<Block> _blocks;
        private List<Enemy> _enemies;
        private Camera _camera;

        // Background Layers
        private BackgroundLayer _skyLayer, _natureLayer, _wallLayer, _floorLayer, _pillarsLayer;

        // Level Settings
        private const int TILE_SIZE = 64;
        private int _levelWidth, _levelHeight;

        public PlayingState(Game1 game, ContentManager content) : base(game, content)
        {
            _blocks = new List<Block>();
            _enemies = new List<Enemy>();
        }

        public override void LoadContent()
        {
            // 1. Bereken level afmetingen
            int rows = _gameboard.GetLength(0);
            int cols = _gameboard.GetLength(1);
            _levelWidth = cols * TILE_SIZE;
            _levelHeight = rows * TILE_SIZE;

            // 2. Camera initialisatie (Gebruik _game.GraphicsDevice!)
            _camera = new Camera(
                _game.GraphicsDevice.Viewport.Width,
                _game.GraphicsDevice.Viewport.Height,
                _levelWidth,
                _levelHeight
            );

            // 3. Textures Laden (Gebruik _content!)
            _idleTexture = _content.Load<Texture2D>("Idle");
            _runTexture = _content.Load<Texture2D>("Run");
            _jumpTexture = _content.Load<Texture2D>("Jump");

            _attack1Texture = _content.Load<Texture2D>("Attacks/Attack 1");
            _attack2Texture = _content.Load<Texture2D>("Attacks/Attack 2");
            _attack3Texture = _content.Load<Texture2D>("Attacks/Attack 3");
            _runAttackTexture = _content.Load<Texture2D>("Attacks/Run+Attack");
            _slashTexture = _content.Load<Texture2D>("Attacks/SlashSprite");

            _tilesSpriteSheet = _content.Load<Texture2D>("Tiles/tilesSpriteSheet");
            _paleBackGroundSpriteSheet = _content.Load<Texture2D>("Background/paleBackgroundSpriteSheet");
            // Voeg hier de andere background sheets toe indien nodig

            // Boss Textures
            _bossIdle = _content.Load<Texture2D>("Boss/Idle");
            _bossWalk = _content.Load<Texture2D>("Boss/Walk");
            _bossRun = _content.Load<Texture2D>("Boss/Run");
            _bossJump = _content.Load<Texture2D>("Boss/Jump");
            _bossAtk1 = _content.Load<Texture2D>("Boss/Attack");
            _bossAtk2 = _content.Load<Texture2D>("Boss/Attack2");
            _bossAtk3 = _content.Load<Texture2D>("Boss/Attack3");
            _bossWalkAtk = _content.Load<Texture2D>("Boss/Walk_Attack");
            _bossSpecial = _content.Load<Texture2D>("Boss/Special");
            _bossHurt = _content.Load<Texture2D>("Boss/Hurt");
            _bossDeath = _content.Load<Texture2D>("Boss/Death");

            // 4. Blok Texture maken (Gebruik _game.GraphicsDevice!)
            _blokTexture = new Texture2D(_game.GraphicsDevice, 1, 1);
            _blokTexture.SetData(new[] { Color.White });

            // 5. Hero Aanmaken
            _hero = new Hero(_idleTexture, _runTexture, _jumpTexture, _attack1Texture, _attack2Texture, _attack3Texture, _runAttackTexture, _slashTexture);
            _hero.Position = new Vector2(100, 300);

            // 6. Level Genereren
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    int blockTypeInt = _gameboard[y, x];
                    Block newBlock = BlockFactory.CreateBlock(blockTypeInt, x, y, _tilesSpriteSheet);
                    if (newBlock != null) _blocks.Add(newBlock);
                }
            }

            // 7. Boss Aanmaken
            KnightBoss boss = new KnightBoss(
                new Vector2(600, 500),
                _blokTexture,
                _bossIdle, _bossWalk, _bossRun, _bossJump,
                _bossAtk1, _bossAtk2, _bossAtk3,
                _bossWalkAtk, _bossSpecial,
                _bossHurt, _bossDeath
            );
            _enemies.Add(boss);

            // 8. Achtergrond Lagen Instellen
            // Let op: Gebruik _game.GraphicsDevice.Viewport.Width/Height
            int vpW = _game.GraphicsDevice.Viewport.Width;
            int vpH = _game.GraphicsDevice.Viewport.Height;

            Rectangle skyRect = new Rectangle(0, 542, 960, 541);
            Rectangle natureRect = new Rectangle(961, 154, 960, 387);
            Rectangle wallRect = new Rectangle(961, 542, 960, 541);
            Rectangle floorRect = new Rectangle(0, 456, 960, 85);
            Rectangle pillarsRect = new Rectangle(0, 1084, 960, 541);

            _skyLayer = new BackgroundLayer(_paleBackGroundSpriteSheet, skyRect, 0.1f, vpW, vpH, _levelWidth);
            _natureLayer = new BackgroundLayer(_paleBackGroundSpriteSheet, natureRect, 0.1f, vpW, vpH, _levelWidth);
            _wallLayer = new BackgroundLayer(_paleBackGroundSpriteSheet, wallRect, 1.0f, vpW, vpH, _levelWidth);
            _floorLayer = new BackgroundLayer(_paleBackGroundSpriteSheet, floorRect, 1.0f, vpW, vpH, _levelWidth);
            _pillarsLayer = new BackgroundLayer(_paleBackGroundSpriteSheet, pillarsRect, 1.0f, vpW, vpH, _levelWidth);
        }

        public override void Update(GameTime gameTime)
        {
            KeyboardState k = Keyboard.GetState();

            _hero.IsRunningRight = k.IsKeyDown(Keys.Right);
            _hero.IsRunningLeft = k.IsKeyDown(Keys.Left);
            _hero.Jump = k.IsKeyDown(Keys.Space);

            _hero.Update(gameTime, _blocks);
            _camera.Follow(_hero.Position);

            // Enemies Update Loop
            for (int i = _enemies.Count - 1; i >= 0; i--)
            {
                var enemy = _enemies[i];
                enemy.Update(gameTime, _blocks, _hero);

                // HERO RAAKT BOSS
                if (_hero.IsHitting && !enemy.IsDead)
                {
                    if (_hero.AttackHitbox.Intersects(enemy.Hitbox))
                    {
                        enemy.TakeDamage(10);
                        System.Diagnostics.Debug.WriteLine($"RAAK! Boss HP: {enemy.CurrentHealth}");
                    }
                }

                // BOSS RAAKT HERO
                if (enemy.IsHitting && !enemy.IsDead)
                {
                    if (enemy.AttackHitbox.Intersects(_hero.Hitbox.HitboxRect))
                    {
                        System.Diagnostics.Debug.WriteLine("AU! Hero hit!");
                    }
                }

                if (enemy.ReadyToRemove)
                {
                    _enemies.RemoveAt(i);
                }
            }
        }

        public override void Draw(SpriteBatch sb)
        {
            // 1. Parallax
            sb.Begin();
            _skyLayer.Draw(sb, _camera.Position);
            _natureLayer.Draw(sb, _camera.Position);
            sb.End();

            // 2. Wereld
            sb.Begin(transformMatrix: _camera.GetTransformMatrix());

            _wallLayer.Draw(sb, _camera.Position);
            _floorLayer.Draw(sb, _camera.Position);
            _pillarsLayer.Draw(sb, _camera.Position);

            foreach (var block in _blocks) block.Draw(sb);

            _hero.Draw(sb);

            foreach (var enemy in _enemies) enemy.Draw(sb);

            if (_hero.IsHitting)
            {
                sb.Draw(_blokTexture, _hero.AttackHitbox, Color.Red * 0.5f);
            }

            sb.End();
        }
    }
}