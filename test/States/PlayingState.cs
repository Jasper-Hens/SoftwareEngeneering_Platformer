using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using test.Blocks;
using test.Level;
using test.block_Interfaces;
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
            { 3,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
            { 3,0,0,0,0,0,0,0,1,0,0,0,0,2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
            { 3,0,0,0,0,0,0,0,0,0,2,2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
            { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 }
        };

        // VARIABELEN
        private Texture2D _idleTexture, _runTexture, _jumpTexture, _blokTexture;
        private Texture2D _attack1Texture, _attack2Texture, _attack3Texture, _runAttackTexture, _slashTexture, _rollTexture;

        // Background Textures
        private Texture2D _tilesSpriteSheet, _brightBackGroundSpriteSheet, _paleBackGroundSpriteSheet;
        private Texture2D _ItemSpriteSheet, _ObjectSpriteSheet;

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

        // player HUD
        private Texture2D _texShieldFull, _texShieldHalf, _texShieldEmpty;
        private HeadsUpDisplay _hud;

        // Game Logic & State
        private double _endGameTimer = 0;
        private bool _isGameOver = false;

        // --- NIEUW: Victory Overlay Variabelen ---
        private bool _showVictoryScreen = false; // Is het victory scherm zichtbaar?
        private Texture2D _victoryTexture;

        // Knoppen voor Victory Screen
        private Texture2D _playAgainBtnTex, _playAgainBtnPressedTex;
        private Texture2D _homeBtnTex, _homeBtnPressedTex;

        // Huidige texture van knoppen (voor hover effect)
        private Texture2D _currentPlayAgainTex, _currentHomeTex;

        private Rectangle _victoryPlayRect, _victoryHomeRect;
        // -----------------------------------------

        public PlayingState(Game1 game, ContentManager content) : base(game, content)
        {
            _blocks = new List<Block>();
            _enemies = new List<Enemy>();
        }

        public override void LoadContent()
        {
            // Verberg muis tijdens spelen, we zetten hem weer aan als we winnen
            _game.IsMouseVisible = false;

            // 1. Bereken level afmetingen
            int rows = _gameboard.GetLength(0);
            int cols = _gameboard.GetLength(1);
            _levelWidth = cols * TILE_SIZE;
            _levelHeight = rows * TILE_SIZE;

            // 2. Camera initialisatie
            _camera = new Camera(
                _game.GraphicsDevice.Viewport.Width,
                _game.GraphicsDevice.Viewport.Height,
                _levelWidth,
                _levelHeight
            );

            // 3. Textures Laden
            _idleTexture = _content.Load<Texture2D>("Idle");
            _runTexture = _content.Load<Texture2D>("Run");
            _jumpTexture = _content.Load<Texture2D>("Jump");

            _attack1Texture = _content.Load<Texture2D>("Attacks/Attack 1");
            _attack2Texture = _content.Load<Texture2D>("Attacks/Attack 2");
            _attack3Texture = _content.Load<Texture2D>("Attacks/Attack 3");
            _runAttackTexture = _content.Load<Texture2D>("Attacks/Run+Attack");
            _slashTexture = _content.Load<Texture2D>("Attacks/SlashSprite");
            _rollTexture = _content.Load<Texture2D>("Roll");

            _tilesSpriteSheet = _content.Load<Texture2D>("Tiles/tilesSpriteSheet");
            _paleBackGroundSpriteSheet = _content.Load<Texture2D>("Background/paleBackgroundSpriteSheet");

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

            // 4. Blok Texture
            _blokTexture = new Texture2D(_game.GraphicsDevice, 1, 1);
            _blokTexture.SetData(new[] { Color.White });

            // HUD
            _texShieldFull = _content.Load<Texture2D>("PlayerHUD/PlayerHealthFullV3");
            _texShieldHalf = _content.Load<Texture2D>("PlayerHUD/PlayerHealthHalfV3");
            _texShieldEmpty = _content.Load<Texture2D>("PlayerHUD/PlayerHealthBrokenShieldV3");

            _hud = new HeadsUpDisplay(_texShieldFull, _texShieldHalf, _texShieldEmpty, _blokTexture);

            // --- NIEUW: Laad Victory Assets ---
            _victoryTexture = _content.Load<Texture2D>("GameState/VictoryV2");

            _playAgainBtnTex = _content.Load<Texture2D>("HomeScreen/PlayAgainButton - kopie");
            _playAgainBtnPressedTex = _content.Load<Texture2D>("HomeScreen/PlayAgainButtonPressed - kopie");

            _homeBtnTex = _content.Load<Texture2D>("HomeScreen/HomeButton - kopie");
            _homeBtnPressedTex = _content.Load<Texture2D>("HomeScreen/HomeButtonPressed - kopie");

            _currentPlayAgainTex = _playAgainBtnTex;
            _currentHomeTex = _homeBtnTex;

            // Posities bepalen voor Victory knoppen (Midden van scherm)
            int screenW = _game.GraphicsDevice.Viewport.Width;
            int screenH = _game.GraphicsDevice.Viewport.Height;
            int btnW = 200;
            int btnH = 80;
            int spacing = 20;
            int startX = (screenW - (btnW * 2 + spacing)) / 2;
            int startY = (screenH / 2) + 150; // Iets onder het midden

            _victoryPlayRect = new Rectangle(startX, startY, btnW, btnH);
            _victoryHomeRect = new Rectangle(startX + btnW + spacing, startY, btnW, btnH);
            // ----------------------------------

            // 5. Hero Aanmaken
            _hero = new Hero(_idleTexture, _runTexture, _jumpTexture, _attack1Texture, _attack2Texture, _attack3Texture, _runAttackTexture, _slashTexture, _rollTexture);
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

            // 8. Achtergrond Lagen
            int vpW = _game.GraphicsDevice.Viewport.Width;
            int vpH = _game.GraphicsDevice.Viewport.Height;
            // Let op: controleer of deze rectangles kloppen met je spritesheet
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
            // --- NIEUW: Als Victory Overlay aanstaat, pauzeer de game en doe alleen menu logica ---
            if (_showVictoryScreen)
            {
                _game.IsMouseVisible = true; // Muis weer tonen!
                UpdateVictoryMenu();
                return; // Stop hier, zodat de hero/enemies niet meer bewegen
            }

            // --- NORMALE GAME LOOP ---

            // 1. HERO UPDATE & INPUT
            KeyboardState k = Keyboard.GetState();

            _hero.IsRunningRight = k.IsKeyDown(Keys.Right);
            _hero.IsRunningLeft = k.IsKeyDown(Keys.Left);
            _hero.Jump = k.IsKeyDown(Keys.Space);

            _hero.Update(gameTime, _blocks);
            _camera.Follow(_hero.Position);

            // 2. ENEMY LOOP
            for (int i = _enemies.Count - 1; i >= 0; i--)
            {
                var enemy = _enemies[i];
                enemy.Update(gameTime, _blocks, _hero);

                // Interacties
                if (_hero.IsHitting && !enemy.IsDead)
                {
                    if (_hero.AttackHitbox.Intersects(enemy.Hitbox))
                    {
                        enemy.TakeDamage(100);
                    }
                }

                if (enemy.IsHitting && !enemy.IsDead)
                {
                    if (enemy.AttackHitbox.Intersects(_hero.Hitbox.HitboxRect))
                    {
                        _hero.TakeDamage(1);
                    }
                }

                if (enemy.ReadyToRemove)
                {
                    _enemies.RemoveAt(i);
                }
            }

            // CHECK A: IS HERO DOOD? -> Game Over State
            if (_hero.IsDead && !_isGameOver)
            {
                _isGameOver = true;
                _endGameTimer = 2000;
            }

            // CHECK B: ZIJN ALLE VIJANDEN VERSLAGEN? -> Victory Overlay
            if (_enemies.Count == 0 && !_isGameOver)
            {
                _isGameOver = true;
                _endGameTimer = 1000;
            }

            // TIMER AFHANDELING
            if (_isGameOver)
            {
                _endGameTimer -= gameTime.ElapsedGameTime.TotalMilliseconds;

                if (_endGameTimer <= 0)
                {
                    if (_hero.IsDead)
                    {
                        // Dood = Nog steeds apart scherm (of je kan dit ook als overlay doen als je wilt)
                        _game.ChangeState(new GameOverState(_game, _content));
                    }
                    else
                    {
                        // Gewonnen = Overlay aanzetten!
                        _showVictoryScreen = true;
                    }
                }
            }
        }

        // --- NIEUW: Helper methode voor Victory Knoppen ---
        private void UpdateVictoryMenu()
        {
            MouseState mouse = Mouse.GetState();

            // Play Again Knop
            if (_victoryPlayRect.Contains(mouse.Position))
            {
                _currentPlayAgainTex = _playAgainBtnPressedTex;
                if (mouse.LeftButton == ButtonState.Pressed)
                {
                    // Herstart Level
                    _game.ChangeState(new PlayingState(_game, _content));
                }
            }
            else
            {
                _currentPlayAgainTex = _playAgainBtnTex;
            }

            // Home Knop
            if (_victoryHomeRect.Contains(mouse.Position))
            {
                _currentHomeTex = _homeBtnPressedTex;
                if (mouse.LeftButton == ButtonState.Pressed)
                {
                    // Naar Menu
                    _game.ChangeState(new MenuState(_game, _content));
                }
            }
            else
            {
                _currentHomeTex = _homeBtnTex;
            }
        }

        public override void Draw(SpriteBatch sb)
        {
            // 1. Parallax Achtergrond
            sb.Begin();
            _skyLayer.Draw(sb, _camera.Position);
            _natureLayer.Draw(sb, _camera.Position);
            sb.End();

            // 2. Wereld & Game Characters (MET CAMERA)
            sb.Begin(transformMatrix: _camera.GetTransformMatrix());

            _wallLayer.Draw(sb, _camera.Position);
            _floorLayer.Draw(sb, _camera.Position);
            _pillarsLayer.Draw(sb, _camera.Position);
            foreach (var block in _blocks) block.Draw(sb);

            _hero.Draw(sb);
            foreach (var enemy in _enemies) enemy.Draw(sb);

            sb.End();

            // 3. UI Layer (HUD + Victory Overlay) - ZONDER CAMERA
            sb.Begin();

            // HUD Altijd tekenen
            _hud.Draw(sb, _hero.CurrentHealth, _hero.MaxHealth, _hero.CurrentStamina, _hero.MaxStamina);

            // --- NIEUW: Victory Overlay tekenen als we gewonnen hebben ---
            if (_showVictoryScreen)
            {
                int screenW = _game.GraphicsDevice.Viewport.Width;
                int screenH = _game.GraphicsDevice.Viewport.Height;

                // Optioneel: Een half-transparant zwart vlak tekenen om de game iets te dimmen
                // sb.Draw(_blokTexture, new Rectangle(0,0,screenW,screenH), Color.Black * 0.5f);

                // Teken Victory Plaatje (Volledig scherm of gecentreerd, afhankelijk van je plaatje)
                sb.Draw(_victoryTexture, new Rectangle(0, 0, screenW, screenH), Color.White);

                // Teken de knoppen eroverheen
                sb.Draw(_currentPlayAgainTex, _victoryPlayRect, Color.White);
                sb.Draw(_currentHomeTex, _victoryHomeRect, Color.White);
            }

            sb.End();
        }
    }
}