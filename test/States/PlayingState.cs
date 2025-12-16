using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using test.Levels;
using test.Objects;

namespace test.States
{
    public class PlayingState : GameState
    {
        private Hero _hero;
        private Camera _camera;
        private HeadsUpDisplay _hud;

        // Level Management
        private BaseLevel _currentLevel;
        private int _levelIndex = 1;

        // Transition
        private bool _isTransitioning = false;
        private float _fadeOpacity = 0f;
        private Texture2D _pixel;
        private bool _fadingOut = false;

        // Victory
        private bool _showVictoryScreen = false;
        private Texture2D _victoryTexture, _playAgainBtnTex, _playAgainBtnPressedTex, _homeBtnTex, _homeBtnPressedTex;
        private Texture2D _currentPlayAgainTex, _currentHomeTex;
        private Rectangle _victoryPlayRect, _victoryHomeRect;

        // Achtergrond
        private Texture2D _paleBackGroundSpriteSheet;
        private BackgroundLayer _skyLayer, _natureLayer, _wallLayer, _floorLayer, _pillarsLayer;
        private int _levelPixelWidth;

        // Textures
        private Texture2D _idleTexture, _runTexture, _jumpTexture, _rollTexture;
        private Texture2D _attack1Texture, _attack2Texture, _attack3Texture, _runAttackTexture, _slashTexture;

        public PlayingState(Game1 game, ContentManager content) : base(game, content)
        {
        }

        public override void LoadContent()
        {
            _pixel = new Texture2D(_game.GraphicsDevice, 1, 1);
            _pixel.SetData(new[] { Color.Black });

            // 1. Textures
            _idleTexture = _content.Load<Texture2D>("Idle");
            _runTexture = _content.Load<Texture2D>("Run");
            _jumpTexture = _content.Load<Texture2D>("Jump");
            _rollTexture = _content.Load<Texture2D>("Roll");
            _attack1Texture = _content.Load<Texture2D>("Attacks/Attack 1");
            _attack2Texture = _content.Load<Texture2D>("Attacks/Attack 2");
            _attack3Texture = _content.Load<Texture2D>("Attacks/Attack 3");
            _runAttackTexture = _content.Load<Texture2D>("Attacks/Run+Attack");
            _slashTexture = _content.Load<Texture2D>("Attacks/SlashSprite");

            _hero = new Hero(
                _idleTexture, _runTexture, _jumpTexture,
                _attack1Texture, _attack2Texture, _attack3Texture,
                _runAttackTexture, _slashTexture, _rollTexture
            );

            // 2. HUD
            Texture2D keyIcon = _content.Load<Texture2D>("Items/ItemSpritesheet");
            Texture2D hudShieldFull = _content.Load<Texture2D>("PlayerHUD/PlayerHealthFullV3");
            Texture2D hudShieldHalf = _content.Load<Texture2D>("PlayerHUD/PlayerHealthHalfV3");
            Texture2D hudShieldEmpty = _content.Load<Texture2D>("PlayerHUD/PlayerHealthBrokenShieldV3");
            _hud = new HeadsUpDisplay(hudShieldFull, hudShieldHalf, hudShieldEmpty, _pixel);

            // 3. Victory Menu
            _victoryTexture = _content.Load<Texture2D>("GameState/VictoryV2");
            _playAgainBtnTex = _content.Load<Texture2D>("HomeScreen/PlayAgainButton - kopie");
            _playAgainBtnPressedTex = _content.Load<Texture2D>("HomeScreen/PlayAgainButtonPressed - kopie");
            _homeBtnTex = _content.Load<Texture2D>("HomeScreen/HomeButton - kopie");
            _homeBtnPressedTex = _content.Load<Texture2D>("HomeScreen/HomeButtonPressed - kopie");

            _currentPlayAgainTex = _playAgainBtnTex;
            _currentHomeTex = _homeBtnTex;

            int screenW = _game.GraphicsDevice.Viewport.Width;
            int screenH = _game.GraphicsDevice.Viewport.Height;
            int btnW = 200, btnH = 80, spacing = 20;
            int startX = (screenW - (btnW * 2 + spacing)) / 2;
            int startY = (screenH / 2) + 150;
            _victoryPlayRect = new Rectangle(startX, startY, btnW, btnH);
            _victoryHomeRect = new Rectangle(startX + btnW + spacing, startY, btnW, btnH);

            // 4. Achtergrond Sheet
            _paleBackGroundSpriteSheet = _content.Load<Texture2D>("Background/paleBackgroundSpriteSheet");

            // 5. Start Level
            LoadLevel(new LevelOne());
        }

        private void LoadLevel(BaseLevel level)
        {
            _currentLevel = level;
            _currentLevel.LoadContent(_content, _game.GraphicsDevice);

            _hero.Position = _currentLevel.StartPosition;
            _hero.Velocity = Vector2.Zero;

            _levelPixelWidth = _currentLevel.Width;
            _camera = new Camera(_game.GraphicsDevice.Viewport.Width, _game.GraphicsDevice.Viewport.Height, _levelPixelWidth, _currentLevel.Height);

            int vpW = _game.GraphicsDevice.Viewport.Width;
            int vpH = _game.GraphicsDevice.Viewport.Height;

            // Oude parallax settings terug
            _skyLayer = new BackgroundLayer(_paleBackGroundSpriteSheet, new Rectangle(0, 542, 960, 541), 0.1f, vpW, vpH, _levelPixelWidth);
            _natureLayer = new BackgroundLayer(_paleBackGroundSpriteSheet, new Rectangle(961, 154, 960, 387), 0.1f, vpW, vpH, _levelPixelWidth);
            _wallLayer = new BackgroundLayer(_paleBackGroundSpriteSheet, new Rectangle(961, 542, 960, 541), 1.0f, vpW, vpH, _levelPixelWidth);
            _floorLayer = new BackgroundLayer(_paleBackGroundSpriteSheet, new Rectangle(0, 456, 960, 85), 1.0f, vpW, vpH, _levelPixelWidth);
            _pillarsLayer = new BackgroundLayer(_paleBackGroundSpriteSheet, new Rectangle(0, 1084, 960, 541), 1.0f, vpW, vpH, _levelPixelWidth);
        }

        public override void Update(GameTime gameTime)
        {
            if (_showVictoryScreen)
            {
                _game.IsMouseVisible = true;
                UpdateVictoryMenu();
                return;
            }

            if (_isTransitioning)
            {
                if (_fadingOut)
                {
                    // Scherm wordt donkerder
                    _fadeOpacity += 0.02f;

                    if (_fadeOpacity >= 1.0f)
                    {
                        // BELANGRIJK: Zorg dat deze regel er staat!
                        // Anders blijft hij voor altijd in "fade out" modus (zwart scherm).
                        _fadingOut = false;

                        // Wissel van level
                        if (_levelIndex == 1)
                        {
                            _levelIndex = 2;

                            // Hier resetten we de speler (zoals je wilde)
                            _hero.Reset();

                            // Laad het nieuwe level
                            LoadLevel(new LevelTwo());
                        }
                        else
                        {
                            // Als we level 2 uitgespeeld hebben (voor later)
                            _showVictoryScreen = true;
                        }
                    }
                }
                else
                {
                    // Scherm wordt weer licht (Fade In)
                    _fadeOpacity -= 0.02f;

                    if (_fadeOpacity <= 0f)
                    {
                        _isTransitioning = false;
                        _fadeOpacity = 0f;
                    }
                }
                return; // Stop de update hier zolang we aan het faden zijn
            }

            _hero.Update(gameTime, _currentLevel.Blocks);
            _camera.Follow(_hero.Position);

            // Enemies Update Loop
            for (int i = _currentLevel.Enemies.Count - 1; i >= 0; i--)
            {
                var enemy = _currentLevel.Enemies[i];
                enemy.Update(gameTime, _currentLevel.Blocks, _hero);

                if (_hero.IsHitting && !enemy.IsDead)
                {
                    // HIER PAS JE DE DAMAGE AAN (bijv. 20 damage per klap)
                    if (_hero.AttackHitbox.Intersects(enemy.Hitbox)) enemy.TakeDamage(20);
                }
                if (enemy.IsHitting && !enemy.IsDead)
                {
                    if (enemy.AttackHitbox.Intersects(_hero.Hitbox.HitboxRect)) _hero.TakeDamage(1);
                }
                if (enemy.ReadyToRemove) _currentLevel.Enemies.RemoveAt(i);
            }

            // Items
            foreach (var item in _currentLevel.Items)
            {
                // --- NIEUW: Update aanroepen voor het zweven ---
                item.Update(gameTime);
                // -----------------------------------------------

                if (item.IsActive && _hero.Hitbox.HitboxRect.Intersects(item.Hitbox))
                {
                    item.OnPickup(_hero);
                }
            }

            // Deur
            if (_currentLevel.ExitDoor != null)
            {
                _currentLevel.ExitDoor.Update(gameTime, _hero);
                if (_currentLevel.ExitDoor.IsOpen && _hero.Hitbox.HitboxRect.Intersects(_currentLevel.ExitDoor.Hitbox))
                {
                    _isTransitioning = true;
                    _fadingOut = true;
                }
            }

            // --- Check of speler van de map gevallen is ---
            if (_hero.Position.Y > _currentLevel.Height + 100)
            {
                _game.ChangeState(new GameOverState(_game, _content));
            }

            if (_hero.IsDead) _game.ChangeState(new GameOverState(_game, _content));

            // --- VICTORY CHECK (HIER ZAT DE FOUT) ---
            if (_levelIndex == 1 && _currentLevel.Enemies.Count == 0 && !_showVictoryScreen)
            {
                // Zodra de enemy dood is, toon victory scherm
                _showVictoryScreen = true;
            }
        }

        private void UpdateVictoryMenu()
        {
            MouseState mouse = Mouse.GetState();
            if (_victoryPlayRect.Contains(mouse.Position))
            {
                _currentPlayAgainTex = _playAgainBtnPressedTex;
                if (mouse.LeftButton == ButtonState.Pressed)
                {
                    _showVictoryScreen = false;
                    _levelIndex = 1;
                    LoadLevel(new LevelOne());
                }
            }
            else _currentPlayAgainTex = _playAgainBtnTex;

            if (_victoryHomeRect.Contains(mouse.Position))
            {
                _currentHomeTex = _homeBtnPressedTex;
                if (mouse.LeftButton == ButtonState.Pressed) _game.ChangeState(new MenuState(_game, _content));
            }
            else _currentHomeTex = _homeBtnTex;
        }

        public override void Draw(SpriteBatch sb)
        {
            sb.Begin();
            _skyLayer.Draw(sb, _camera.Position);
            _natureLayer.Draw(sb, _camera.Position);
            sb.End();

            sb.Begin(transformMatrix: _camera.GetTransformMatrix());
            _wallLayer.Draw(sb, _camera.Position);
            _floorLayer.Draw(sb, _camera.Position);
            _pillarsLayer.Draw(sb, _camera.Position);

            foreach (var block in _currentLevel.Blocks) block.Draw(sb);
            if (_currentLevel.ExitDoor != null) _currentLevel.ExitDoor.Draw(sb);
            foreach (var item in _currentLevel.Items) item.Draw(sb);
            foreach (var enemy in _currentLevel.Enemies) enemy.Draw(sb);
            _hero.Draw(sb);
            sb.End();

            sb.Begin();
            _hud.Draw(sb, _hero.CurrentHealth, _hero.MaxHealth, _hero.CurrentStamina, _hero.MaxStamina, _hero.Inventory);

            if (_showVictoryScreen)
            {
                int screenW = _game.GraphicsDevice.Viewport.Width;
                int screenH = _game.GraphicsDevice.Viewport.Height;
                sb.Draw(_pixel, new Rectangle(0, 0, screenW, screenH), Color.Black * 0.6f);
                sb.Draw(_victoryTexture, new Rectangle(0, 0, screenW, screenH), Color.White);
                sb.Draw(_currentPlayAgainTex, _victoryPlayRect, Color.White);
                sb.Draw(_currentHomeTex, _victoryHomeRect, Color.White);
            }

            if (_fadeOpacity > 0)
            {
                sb.Draw(_pixel, new Rectangle(0, 0, 1280, 720), Color.Black * _fadeOpacity);
            }
            sb.End();
        }
    }
}