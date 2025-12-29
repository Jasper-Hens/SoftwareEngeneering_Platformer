using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using test.Animations;
using test.Animations.HeroAnimations;
using test.block_Interfaces;
using test.Blocks;
using test.Inputs; // Nieuwe namespace
using test.Interfaces; // Nieuwe namespace
using test.Effects;

namespace test
{
    public class Hero : IDamageable
    {
        private IInputReader _inputReader; // SRP: Input is apart

        // Animaties
        private Animation _idle, _run, _jump;
        private Animation _attack1, _attack2, _attack3, _runAttack;
        private Animation _rollAnim;
        private Animation _current;
        private VisualEffect _airSlashEffect;

        // Physics
        public Vector2 Position; // Public gemaakt voor eenvoud, kan property zijn
        public Vector2 Velocity = Vector2.Zero;
        public bool IsRunningRight = false, IsRunningLeft = false, Jump = false, FacingRight = true;

        // Hitbox & Stats
        public Hitbox HitboxObject { get; private set; } // Hernoemd om verwarring met Interface property te voorkomen
        public Rectangle Hitbox => HitboxObject.HitboxRect; // Implementatie IDamageable

        public Rectangle AttackHitbox { get; private set; }
        public bool IsHitting { get; private set; } = false;
        public bool IsAttacking { get; private set; } = false;

        private float gravity = 0.5f;
        private float moveSpeed = 4f;
        private float jumpStrength = -12f;
        private const int HITBOX_WIDTH = 45;
        private const int HITBOX_HEIGHT = 65;

        // Combo
        private int _comboIndex = 0;
        private double _comboTimer = 0;
        private const double COMBO_WINDOW = 1000;

        // Health & Stamina
        public int MaxHealth { get; private set; } = 6;
        public int CurrentHealth { get; private set; }
        public bool IsDead { get; private set; } = false;
        public float MaxStamina { get; private set; } = 100f;
        public float CurrentStamina { get; private set; }

        public bool IsDashing { get; private set; } = false;
        public bool IsRolling => IsDashing;

        private double _invincibilityTimer = 0;
        private Color _heroColor = Color.White;

        // Dash Settings
        private float _dashCost = 30f;
        private float _staminaRecharge = 0.5f;
        private double _dashCooldown = 0;
        private float _dashSpeed = 9f;

        public Inventory Inventory { get; private set; }

        // CONSTRUCTOR AANGEPAST: InputReader toegevoegd
        public Hero(Texture2D idleTex, Texture2D runTex, Texture2D jumpTex,
                    Texture2D atk1Tex, Texture2D atk2Tex, Texture2D atk3Tex,
                    Texture2D runAtkTex, Texture2D slashTex,
                    Texture2D rollTex, IInputReader inputReader)
        {
            _inputReader = inputReader;
            Inventory = new Inventory();

            // Zet startpositie
            Position = new Vector2(100, 100);

            _idle = new IdleAnimation(idleTex);
            _run = new RunAnimation(runTex);
            _jump = new JumpAnimation(jumpTex);
            _attack1 = new AttackOneAnimation(atk1Tex);
            _attack2 = new AttackTwoAnimation(atk2Tex);
            _attack3 = new AttackThreeAnimation(atk3Tex);
            _runAttack = new RunAttackAnimation(runAtkTex);
            _rollAnim = new RollAnimation(rollTex);

            _attack1.IsLooping = false; _attack2.IsLooping = false; _attack3.IsLooping = false;
            _runAttack.IsLooping = false; _rollAnim.IsLooping = false;

            _attack1.DamageFrames = new List<int> { 0, 1, 2, 3, 4, 5 };
            _attack2.DamageFrames = new List<int> { 2, 3 };
            _attack3.DamageFrames = new List<int> { 2, 3 };
            _runAttack.DamageFrames = new List<int> { 3, 4 };

            _airSlashEffect = new VisualEffect(slashTex, 64, 64, 4);
            _current = _idle;
            HitboxObject = new Hitbox();
            CurrentHealth = MaxHealth;
            CurrentStamina = MaxStamina;
        }

        // Interface method
        public void TakeDamage(int damage) => TakeDamage(damage, false);

        public void TakeDamage(int damage, bool ignoreDash = false)
        {
            if ((IsDashing && !ignoreDash) || _invincibilityTimer > 0 || IsDead) return;

            CurrentHealth -= damage;
            _invincibilityTimer = 1000;
            Velocity.Y = -5;
            Velocity.X = FacingRight ? -5 : 5;

            if (CurrentHealth <= 0) { CurrentHealth = 0; IsDead = true; }
        }

        public void Update(GameTime gameTime, List<Block> blocks)
        {
            if (IsDead) return;

            // 1. INPUT CHECK via Interface (Geen Keyboard meer hier!)
            Vector2 inputDir = _inputReader.ReadMovement();
            IsRunningRight = inputDir.X > 0;
            IsRunningLeft = inputDir.X < 0;
            Jump = _inputReader.IsJumpPressed();

            // 2. Stamina
            if (!IsDashing && CurrentStamina < MaxStamina)
            {
                CurrentStamina += _staminaRecharge;
                if (CurrentStamina > MaxStamina) CurrentStamina = MaxStamina;
            }
            if (_dashCooldown > 0) _dashCooldown -= gameTime.ElapsedGameTime.TotalMilliseconds;

            // 3. Invincibility
            if (_invincibilityTimer > 0)
            {
                _invincibilityTimer -= gameTime.ElapsedGameTime.TotalMilliseconds;
                if (_invincibilityTimer % 200 < 100) _heroColor = Color.Red * 0.7f;
                else _heroColor = Color.White * 0.5f;
            }
            else _heroColor = Color.White;

            // 4. DASH
            if (IsDashing)
            {
                _current = _rollAnim;
                Velocity.X = FacingRight ? _dashSpeed : -_dashSpeed;
                Velocity.Y += gravity;
                _heroColor = Color.LightSkyBlue * 0.8f;

                if (_current.IsFinished)
                {
                    IsDashing = false;
                    Velocity.X = 0;
                    _dashCooldown = 200;
                    _current.Reset();
                    _current = _idle;
                }
                MoveAndCollide(blocks);
                _current.Update(gameTime);
                HitboxObject.Update(Position, HITBOX_WIDTH, HITBOX_HEIGHT);
                return;
            }

            // 5. DASH STARTEN
            if (_inputReader.IsDashPressed() && !IsDashing && !IsAttacking && _dashCooldown <= 0)
            {
                if (CurrentStamina >= _dashCost && IsOnGround(blocks)) StartDash();
            }

            IsHitting = false;
            AttackHitbox = Rectangle.Empty;

            // 6. ATTACK INPUT
            if (!IsAttacking && _comboIndex > 0)
            {
                _comboTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
                if (_comboTimer > COMBO_WINDOW) _comboIndex = 0;
            }

            if (_inputReader.IsAttackPressed() && !IsAttacking) StartAttack();

            // 7. BEWEGING
            bool movementLocked = IsAttacking && _current != _runAttack;
            Velocity.X = 0;
            if (!movementLocked)
            {
                if (IsRunningRight) { Velocity.X += moveSpeed; FacingRight = true; }
                if (IsRunningLeft) { Velocity.X -= moveSpeed; FacingRight = false; }
            }

            if (Jump && IsOnGround(blocks) && !IsAttacking) Velocity.Y = jumpStrength;

            Velocity.Y += gravity;
            MoveAndCollide(blocks);

            // 8. ANIMATIE
            if (IsAttacking)
            {
                if (_current.DamageFrames.Contains(_current.CurrentFrame))
                {
                    IsHitting = true;
                    int range = _current.AttackRange;
                    int height = _current.AttackHeight;
                    int attackX = (int)Position.X;
                    int attackY = (int)Position.Y + 10;
                    if (FacingRight) attackX += HITBOX_WIDTH; else attackX -= range;
                    AttackHitbox = new Rectangle(attackX, attackY, range, height);
                    if (_current == _attack1 && _current.CurrentFrame == 5 && !_airSlashEffect.IsActive)
                        _airSlashEffect.Play(Position, FacingRight);
                }

                if (_current.IsFinished)
                {
                    IsAttacking = false;
                    _current.Reset();
                    if (_current != _runAttack) { _comboIndex++; if (_comboIndex > 2) _comboIndex = 0; _comboTimer = 0; }
                    _current = (Velocity.X != 0) ? _run : _idle;
                }
            }
            else
            {
                if (!IsOnGround(blocks)) _current = _jump;
                else if (Velocity.X != 0) _current = _run;
                else _current = _idle;
            }

            _current.Update(gameTime);
            _airSlashEffect.Update(gameTime);
            HitboxObject.Update(Position, HITBOX_WIDTH, HITBOX_HEIGHT);
        }

        private void StartDash()
        {
            IsDashing = true;
            CurrentStamina -= _dashCost;
            _rollAnim.Reset();
            _current = _rollAnim;
        }

        private void StartAttack()
        {
            IsAttacking = true;
            if (Velocity.X != 0 || IsRunningRight || IsRunningLeft) _current = _runAttack;
            else
            {
                if (_comboIndex == 0) _current = _attack1;
                else if (_comboIndex == 1) _current = _attack2;
                else _current = _attack3;
            }
            _current.Reset();
        }

        private void MoveAndCollide(List<Block> blocks)
        {
            Position.X += Velocity.X;
            ResolveCollisionsX(blocks);
            Position.Y += Velocity.Y;
            ResolveCollisionsY(blocks);
        }

        // ... (De rest van de private collision/draw methodes blijven hetzelfde) ...
        // Zorg alleen dat je 'Hitbox' vervangt door 'HitboxObject' waar je de class aanroept, 
        // en 'HitboxObject.HitboxRect' gebruikt waar je de Rectangle nodig hebt.

        private bool IsOnGround(List<Block> blocks)
        {
            Rectangle footCheckRect = HitboxObject.HitboxRect;
            footCheckRect.Y += 1;
            foreach (var block in blocks)
            {
                if ((block is ISolid || block is IPlatform) && footCheckRect.Intersects(block.BoundingBox))
                {
                    if (HitboxObject.HitboxRect.Bottom <= block.BoundingBox.Top + 1) return true;
                }
            }
            return false;
        }

        private void ResolveCollisionsX(List<Block> blocks)
        {
            HitboxObject.Update(Position, HITBOX_WIDTH, HITBOX_HEIGHT);
            Rectangle skinnyRect = HitboxObject.HitboxRect;
            skinnyRect.Y += 4; skinnyRect.Height -= 8;

            foreach (var block in blocks)
            {
                if (block is ISolid && skinnyRect.Intersects(block.BoundingBox))
                {
                    Rectangle intersection = Rectangle.Intersect(HitboxObject.HitboxRect, block.BoundingBox);
                    if (Velocity.X > 0) Position.X -= intersection.Width;
                    else if (Velocity.X < 0) Position.X += intersection.Width;
                    Velocity.X = 0;
                    HitboxObject.Update(Position, HITBOX_WIDTH, HITBOX_HEIGHT);
                }
            }
        }

        private void ResolveCollisionsY(List<Block> blocks)
        {
            HitboxObject.Update(Position, HITBOX_WIDTH, HITBOX_HEIGHT);
            foreach (var block in blocks)
            {
                if ((block is ISolid || block is IPlatform) && HitboxObject.HitboxRect.Intersects(block.BoundingBox))
                {
                    Rectangle intersection = Rectangle.Intersect(HitboxObject.HitboxRect, block.BoundingBox);
                    if (intersection.Height < intersection.Width)
                    {
                        if (Velocity.Y > 0)
                        {
                            if (block is IPlatform && HitboxObject.HitboxRect.Bottom - intersection.Height > block.BoundingBox.Top) continue;
                            Position.Y -= intersection.Height;
                            Velocity.Y = 0;
                        }
                        else if (Velocity.Y < 0 && block is ISolid)
                        {
                            Position.Y += intersection.Height;
                            Velocity.Y = 0;
                        }
                        HitboxObject.Update(Position, HITBOX_WIDTH, HITBOX_HEIGHT);
                    }
                }
            }
        }

        public void Reset()
        {
            CurrentHealth = MaxHealth;
            CurrentStamina = MaxStamina;
            IsDead = false;
            Inventory?.Clear();
        }

        public void Heal(int amount)
        {
            CurrentHealth += amount;
            if (CurrentHealth > MaxHealth) CurrentHealth = MaxHealth;
        }

        public void Draw(SpriteBatch sb)
        {
            Rectangle currentFrame = _current.Frames[_current.CurrentFrame];
            float widthDifference = currentFrame.Width - HITBOX_WIDTH;
            float drawX = Position.X - (widthDifference / 2);
            float heightDifference = HITBOX_HEIGHT - currentFrame.Height;
            float drawY = Position.Y + heightDifference;

            Vector2 drawPosition = new Vector2(drawX, drawY);
            _current.Draw(sb, drawPosition, FacingRight, _heroColor);
            _airSlashEffect.Draw(sb);
        }
    }
}