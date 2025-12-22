using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using test.Blocks;
using test.block_Interfaces;
using test.Effects;
// Zorg dat deze namespaces kloppen met jouw mappen!
using test.Animations;
using test.Animations.HeroAnimations;

namespace test
{
    public class Hero
    {
        // Animaties
        private Animation _idle, _run, _jump;
        private Animation _attack1, _attack2, _attack3, _runAttack;
        private Animation _rollAnim;

        private Animation _current;
        private VisualEffect _airSlashEffect;

        // Physics
        public Vector2 Position = new Vector2(100, 100);
        public Vector2 Velocity = Vector2.Zero;
        public bool IsRunningRight = false, IsRunningLeft = false, Jump = false, FacingRight = true;

        // Hitbox & Stats
        public Hitbox Hitbox { get; private set; }
        public Rectangle AttackHitbox { get; private set; }
        public bool IsHitting { get; private set; } = false;
        public bool IsAttacking { get; private set; } = false;

        private float gravity = 0.5f;
        private float moveSpeed = 4f;
        private float jumpStrength = -12f; // Iets hoger springen
        private const int HITBOX_WIDTH = 45;
        private const int HITBOX_HEIGHT = 65;

        // Combo
        private int _comboIndex = 0;
        private double _comboTimer = 0;
        private const double COMBO_WINDOW = 1000;
        private bool _wasAttackPressed = false;

        // Health & Stamina
        public int MaxHealth { get; private set; } = 6;
        public int CurrentHealth { get; private set; }
        public bool IsDead { get; private set; } = false;
        public float MaxStamina { get; private set; } = 100f;
        public float CurrentStamina { get; private set; }

        // --- AANPASSING HIER ---
        public bool IsDashing { get; private set; } = false;
        // We voegen deze regel toe zodat 'IsRolling' hetzelfde betekent als 'IsDashing'
        public bool IsRolling => IsDashing;
        // -----------------------

        private double _invincibilityTimer = 0;
        private Color _heroColor = Color.White;

        // Dash Settings
        private float _dashCost = 30f;
        private float _staminaRecharge = 0.5f;
        private double _dashCooldown = 0;
        private float _dashSpeed = 9f;

        // Inventory
        public Inventory Inventory { get; private set; }

        public Hero(Texture2D idleTex, Texture2D runTex, Texture2D jumpTex,
                    Texture2D atk1Tex, Texture2D atk2Tex, Texture2D atk3Tex,
                    Texture2D runAtkTex, Texture2D slashTex,
                    Texture2D rollTex)
        {
            Inventory = new Inventory();

            _idle = new IdleAnimation(idleTex);
            _run = new RunAnimation(runTex);
            _jump = new JumpAnimation(jumpTex);
            _attack1 = new AttackOneAnimation(atk1Tex);
            _attack2 = new AttackTwoAnimation(atk2Tex);
            _attack3 = new AttackThreeAnimation(atk3Tex);
            _runAttack = new RunAttackAnimation(runAtkTex);
            _rollAnim = new RollAnimation(rollTex); // Zorg dat RollAnimation in test.Animations staat

            _attack1.IsLooping = false; _attack2.IsLooping = false; _attack3.IsLooping = false;
            _runAttack.IsLooping = false; _rollAnim.IsLooping = false;

            _attack1.DamageFrames = new List<int> { 0, 1, 2, 3, 4, 5 };
            _attack2.DamageFrames = new List<int> { 2, 3 };
            _attack3.DamageFrames = new List<int> { 2, 3 };
            _runAttack.DamageFrames = new List<int> { 3, 4 };

            _airSlashEffect = new VisualEffect(slashTex, 64, 64, 4);
            _current = _idle;
            Hitbox = new Hitbox();
            CurrentHealth = MaxHealth;
            CurrentStamina = MaxStamina;
        }

        public void TakeDamage(int damage)
        {
            if (IsDashing || _invincibilityTimer > 0 || IsDead) return;

            CurrentHealth -= damage;
            _invincibilityTimer = 1000;
            Velocity.Y = -5;
            Velocity.X = FacingRight ? -5 : 5;

            if (CurrentHealth <= 0) { CurrentHealth = 0; IsDead = true; }
        }

        public void Update(GameTime gameTime, List<Block> blocks)
        {
            if (IsDead) return;

            // 1. INPUT CHECK
            KeyboardState k = Keyboard.GetState();

            // Zet flags op basis van input
            IsRunningRight = k.IsKeyDown(Keys.Right) || k.IsKeyDown(Keys.D);
            IsRunningLeft = k.IsKeyDown(Keys.Left) || k.IsKeyDown(Keys.Q);
            Jump = k.IsKeyDown(Keys.Space);

            // 2. Stamina
            if (!IsDashing && CurrentStamina < MaxStamina)
            {
                CurrentStamina += _staminaRecharge;
                if (CurrentStamina > MaxStamina) CurrentStamina = MaxStamina;
            }
            if (_dashCooldown > 0) _dashCooldown -= gameTime.ElapsedGameTime.TotalMilliseconds;

            // 3. Invincibility Blink
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
                Hitbox.Update(Position, HITBOX_WIDTH, HITBOX_HEIGHT);
                return;
            }

            // 5. DASH STARTEN
            if (k.IsKeyDown(Keys.LeftShift) && !IsDashing && !IsAttacking && _dashCooldown <= 0)
            {
                if (CurrentStamina >= _dashCost && IsOnGround(blocks))
                {
                    StartDash();
                }
            }

            IsHitting = false;
            AttackHitbox = Rectangle.Empty;

            // 6. ATTACK INPUT
            if (!IsAttacking && _comboIndex > 0)
            {
                _comboTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
                if (_comboTimer > COMBO_WINDOW) _comboIndex = 0;
            }

            bool isAttackPressed = k.IsKeyDown(Keys.Z) || k.IsKeyDown(Keys.J); // Z of J voor attack
            if (isAttackPressed && !_wasAttackPressed && !IsAttacking) StartAttack();
            _wasAttackPressed = isAttackPressed;

            // 7. BEWEGING TOEPASSEN
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

            // 8. ANIMATIE STATE MACHINE
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
            Hitbox.Update(Position, HITBOX_WIDTH, HITBOX_HEIGHT);
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

        private bool IsOnGround(List<Block> blocks)
        {
            Rectangle footCheckRect = Hitbox.HitboxRect;
            footCheckRect.Y += 1;
            foreach (var block in blocks)
            {
                if ((block is ISolid || block is IPlatform) && footCheckRect.Intersects(block.BoundingBox))
                {
                    if (Hitbox.HitboxRect.Bottom <= block.BoundingBox.Top + 1) return true;
                }
            }
            return false;
        }

        private void ResolveCollisionsX(List<Block> blocks)
        {
            Hitbox.Update(Position, HITBOX_WIDTH, HITBOX_HEIGHT);
            Rectangle skinnyRect = Hitbox.HitboxRect;
            skinnyRect.Y += 4; skinnyRect.Height -= 8;

            foreach (var block in blocks)
            {
                if (block is ISolid && skinnyRect.Intersects(block.BoundingBox))
                {
                    Rectangle intersection = Rectangle.Intersect(Hitbox.HitboxRect, block.BoundingBox);
                    if (Velocity.X > 0) Position.X -= intersection.Width;
                    else if (Velocity.X < 0) Position.X += intersection.Width;
                    Velocity.X = 0;
                    Hitbox.Update(Position, HITBOX_WIDTH, HITBOX_HEIGHT);
                }
            }
        }

        private void ResolveCollisionsY(List<Block> blocks)
        {
            Hitbox.Update(Position, HITBOX_WIDTH, HITBOX_HEIGHT);
            foreach (var block in blocks)
            {
                if ((block is ISolid || block is IPlatform) && Hitbox.HitboxRect.Intersects(block.BoundingBox))
                {
                    Rectangle intersection = Rectangle.Intersect(Hitbox.HitboxRect, block.BoundingBox);
                    if (intersection.Height < intersection.Width)
                    {
                        if (Velocity.Y > 0)
                        {
                            if (block is IPlatform && Hitbox.HitboxRect.Bottom - intersection.Height > block.BoundingBox.Top) continue;
                            Position.Y -= intersection.Height;
                            Velocity.Y = 0;
                        }
                        else if (Velocity.Y < 0 && block is ISolid)
                        {
                            Position.Y += intersection.Height;
                            Velocity.Y = 0;
                        }
                        Hitbox.Update(Position, HITBOX_WIDTH, HITBOX_HEIGHT);
                    }
                }
            }
        }

        public void Reset()
        {
            CurrentHealth = MaxHealth; // Levens vol
            CurrentStamina = MaxStamina; // Stamina vol
            IsDead = false;

            // Maak de inventory leeg
            if (Inventory != null)
            {
                Inventory.Clear();
            }
        }
        public void Heal(int amount)
        {
            CurrentHealth += amount;

            // Zorg dat we niet meer levens krijgen dan het maximum
            if (CurrentHealth > MaxHealth)
            {
                CurrentHealth = MaxHealth;
            }
        }

        public void Draw(SpriteBatch sb)
        {
            Rectangle currentFrame = _current.Frames[_current.CurrentFrame];

            // 1. Horizontaal centreren (zoals je al had)
            float widthDifference = currentFrame.Width - HITBOX_WIDTH;
            float drawX = Position.X - (widthDifference / 2);

            // 2. NIEUW: Verticaal uitlijnen op de grond (Onderkant hitbox)
            // Dit zorgt ervoor dat als je rolt (klein plaatje), je netjes op de grond blijft
            // en niet zweeft of zakt.
            float heightDifference = HITBOX_HEIGHT - currentFrame.Height;
            float drawY = Position.Y + heightDifference;

            Vector2 drawPosition = new Vector2(drawX, drawY);

            // Teken de held
            _current.Draw(sb, drawPosition, FacingRight, _heroColor);

            _airSlashEffect.Draw(sb);
        }
    }
}