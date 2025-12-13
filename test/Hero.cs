using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using test.Animations;
using test.Blocks;
using test.block_Interfaces;
using test.Effects;
using test.Animations.Attack_animations;

namespace test
{
    public class Hero
    {
        // ... (Al je bestaande animatie variabelen) ...
        private Animation _idle, _run, _jump;
        private Animation _attack1, _attack2, _attack3, _runAttack;
        private Animation _current;
        private VisualEffect _airSlashEffect;

        // Physics vars
        public Vector2 Position = new Vector2(100, 100);
        public Vector2 Velocity = Vector2.Zero;
        public bool IsRunningRight = false, IsRunningLeft = false, Jump = false, FacingRight = true;

        // Hitbox vars
        public Hitbox Hitbox { get; private set; }
        public Rectangle AttackHitbox { get; private set; }
        public bool IsHitting { get; private set; } = false;
        public bool IsAttacking { get; private set; } = false;

        private float gravity = 0.5f;
        private float moveSpeed = 4f;
        private float jumpStrength = -10f;
        private const int HITBOX_WIDTH = 45;
        private const int HITBOX_HEIGHT = 65;

        // Combo vars
        private int _comboIndex = 0;
        private double _comboTimer = 0;
        private const double COMBO_WINDOW = 1000;
        private bool _wasAttackPressed = false;

        // --- NIEUW: HEALTH SYSTEEM ---
        public int MaxHealth { get; private set; } = 6; // 3 Schildjes (want 1 schild = 2 HP)
        public int CurrentHealth { get; private set; }
        public bool IsDead { get; private set; } = false;

        private double _invincibilityTimer = 0; // Tijd dat je niet geraakt kan worden
        private Color _heroColor = Color.White; // Kleur voor knipperen

        public Hero(Texture2D idleTex, Texture2D runTex, Texture2D jumpTex, Texture2D atk1Tex, Texture2D atk2Tex, Texture2D atk3Tex, Texture2D runAtkTex, Texture2D slashTex)
        {
            // ... (Animatie initialisatie blijft hetzelfde) ...
            _idle = new IdleAnimation(idleTex);
            _run = new RunAnimation(runTex);
            _jump = new JumpAnimation(jumpTex);
            _attack1 = new AttackOneAnimation(atk1Tex);
            _attack2 = new AttackTwoAnimation(atk2Tex);
            _attack3 = new AttackThreeAnimation(atk3Tex);
            _runAttack = new RunAttackAnimation(runAtkTex);

            _attack1.IsLooping = false; _attack2.IsLooping = false; _attack3.IsLooping = false; _runAttack.IsLooping = false;

            _attack1.DamageFrames = new List<int> { 0, 1, 2, 3, 4, 5 };
            _attack2.DamageFrames = new List<int> { 2, 3 };
            _attack3.DamageFrames = new List<int> { 2, 3 };
            _runAttack.DamageFrames = new List<int> { 3, 4 };

            _airSlashEffect = new VisualEffect(slashTex, 64, 64, 4);
            _current = _idle;
            Hitbox = new Hitbox();

            // Start met vol leven
            CurrentHealth = MaxHealth;
        }

        // --- NIEUW: SCHADE METHODE ---
        public void TakeDamage(int damage)
        {
            // Als we onsterfelijk zijn of al dood, doe niks
            if (_invincibilityTimer > 0 || IsDead) return;

            CurrentHealth -= damage;
            _invincibilityTimer = 1000; // 1 seconde onsterfelijk na een klap

            // Terugslag (klein sprongetje)
            Velocity.Y = -5;
            Velocity.X = FacingRight ? -5 : 5;

            if (CurrentHealth <= 0)
            {
                CurrentHealth = 0;
                IsDead = true;
                // Hier kunnen we later een Death Animation starten
            }
        }

        public void Update(GameTime gameTime, List<Block> blocks)
        {
            // 1. Invincibility Timer & Kleur Effect
            if (_invincibilityTimer > 0)
            {
                _invincibilityTimer -= gameTime.ElapsedGameTime.TotalMilliseconds;

                // Knipper effect (Rood / Transparant / Wit)
                if (_invincibilityTimer % 200 < 100)
                    _heroColor = Color.Red * 0.7f;
                else
                    _heroColor = Color.White * 0.5f;
            }
            else
            {
                _heroColor = Color.White;
            }

            // Als je dood bent, misschien geen input meer toelaten?
            if (IsDead)
            {
                // Simpele zwaartekracht zodat hij niet blijft zweven
                Velocity.Y += gravity;
                Position.Y += Velocity.Y;
                ResolveCollisionsY(blocks);
                Hitbox.Update(Position, HITBOX_WIDTH, HITBOX_HEIGHT);
                return; // Stop de rest van de update (geen attack/movement)
            }

            // ... (Hieronder staat JOUW bestaande Movement/Attack code ongewijzigd) ...

            KeyboardState k = Keyboard.GetState();
            IsHitting = false;
            AttackHitbox = Rectangle.Empty;

            if (!IsAttacking && _comboIndex > 0)
            {
                _comboTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
                if (_comboTimer > COMBO_WINDOW) _comboIndex = 0;
            }

            bool isAttackPressed = k.IsKeyDown(Keys.Z);
            if (isAttackPressed && !_wasAttackPressed && !IsAttacking) StartAttack();
            _wasAttackPressed = isAttackPressed;

            bool movementLocked = IsAttacking && _current != _runAttack;

            Velocity.X = 0;
            if (!movementLocked)
            {
                if (IsRunningRight) { Velocity.X += moveSpeed; FacingRight = true; }
                if (IsRunningLeft) { Velocity.X -= moveSpeed; FacingRight = false; }
            }

            if (Jump && IsOnGround(blocks) && !IsAttacking) Velocity.Y = jumpStrength;

            Velocity.Y += gravity;
            Position.X += Velocity.X;
            ResolveCollisionsX(blocks);
            Position.Y += Velocity.Y;
            ResolveCollisionsY(blocks);

            if (IsAttacking)
            {
                if (_current.DamageFrames.Contains(_current.CurrentFrame))
                {
                    IsHitting = true;
                    int range = _current.AttackRange;
                    int height = _current.AttackHeight;
                    int attackX = (int)Position.X;
                    int attackY = (int)Position.Y + 10;

                    if (FacingRight) attackX += HITBOX_WIDTH;
                    else attackX -= range;

                    AttackHitbox = new Rectangle(attackX, attackY, range, height);

                    if (_current == _attack1 && _current.CurrentFrame == 5 && !_airSlashEffect.IsActive)
                        _airSlashEffect.Play(Position, FacingRight);
                }

                if (_current.IsFinished)
                {
                    IsAttacking = false;
                    _current.Reset();
                    if (_current != _runAttack)
                    {
                        _comboIndex++;
                        if (_comboIndex > 2) _comboIndex = 0;
                        _comboTimer = 0;
                    }
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

        // ... (De rest van je private methodes: StartAttack, ResolveCollision, IsOnGround blijven hetzelfde) ...
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

        public void Draw(SpriteBatch sb)
        {
            Rectangle currentFrame = _current.Frames[_current.CurrentFrame];
            float widthDifference = currentFrame.Width - HITBOX_WIDTH;
            float drawX = Position.X - (widthDifference / 2);
            Vector2 drawPosition = new Vector2(drawX, Position.Y);

            // GEEF NU DE _heroColor MEE!
            _current.Draw(sb, drawPosition, FacingRight, _heroColor);

            _airSlashEffect.Draw(sb);
        }
    }
}