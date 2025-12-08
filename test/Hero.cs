using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using test.Animations;
using test.Blocks;
using test.block_Interfaces;
using test.Effects;
using test.Animations.Attack_animations; // Zorg dat deze klopt met jouw namespace

namespace test
{
    public class Hero
    {
        private Animation _idle, _run, _jump;
        private Animation _attack1, _attack2, _attack3, _runAttack;
        private Animation _current;
        private VisualEffect _airSlashEffect;

        public Vector2 Position = new Vector2(100, 100);
        public Vector2 Velocity = Vector2.Zero;
        public bool IsRunningRight = false, IsRunningLeft = false, Jump = false, FacingRight = true;

        public Hitbox Hitbox { get; private set; }
        public Rectangle AttackHitbox { get; private set; }
        public bool IsHitting { get; private set; } = false;
        public bool IsAttacking { get; private set; } = false;

        private float gravity = 0.5f;
        private float moveSpeed = 4f;
        private float jumpStrength = -10f;

        // VASTE HITBOX AFMETINGEN
        private const int HITBOX_WIDTH = 45;
        private const int HITBOX_HEIGHT = 65;

        // Combo variabelen
        private int _comboIndex = 0;
        private double _comboTimer = 0;
        private const double COMBO_WINDOW = 1000;
        private bool _wasAttackPressed = false;

        public Hero(Texture2D idleTex, Texture2D runTex, Texture2D jumpTex, Texture2D atk1Tex, Texture2D atk2Tex, Texture2D atk3Tex, Texture2D runAtkTex, Texture2D slashTex)
        {
            _idle = new IdleAnimation(idleTex);
            _run = new RunAnimation(runTex);
            _jump = new JumpAnimation(jumpTex);

            _attack1 = new AttackOneAnimation(atk1Tex);
            _attack2 = new AttackTwoAnimation(atk2Tex);
            _attack3 = new AttackThreeAnimation(atk3Tex);
            _runAttack = new RunAttackAnimation(runAtkTex);

            _attack1.IsLooping = false;
            _attack2.IsLooping = false;
            _attack3.IsLooping = false;
            _runAttack.IsLooping = false;

            // Damage Frames (Zorg dat deze kloppen met jouw animaties!)
            _attack1.DamageFrames = new List<int> { 0, 1, 2, 3, 4, 5 };
            _attack2.DamageFrames = new List<int> { 2, 3 };
            _attack3.DamageFrames = new List<int> { 2, 3 };
            _runAttack.DamageFrames = new List<int> { 3, 4 };

            _airSlashEffect = new VisualEffect(slashTex, 64, 64, 4);
            _current = _idle;
            Hitbox = new Hitbox();
        }

        public void Update(GameTime gameTime, List<Block> blocks)
        {
            KeyboardState k = Keyboard.GetState();

            // FIX 1: Reset ALTIJD de hitbox status aan het begin van de update
            // Zo blijft hij nooit hangen als de animatie stopt.
            IsHitting = false;
            AttackHitbox = Rectangle.Empty;

            // 1. Combo Timer
            if (!IsAttacking && _comboIndex > 0)
            {
                _comboTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
                if (_comboTimer > COMBO_WINDOW) _comboIndex = 0;
            }

            // 2. Input (Aanval)
            bool isAttackPressed = k.IsKeyDown(Keys.Z);
            if (isAttackPressed && !_wasAttackPressed && !IsAttacking) StartAttack();
            _wasAttackPressed = isAttackPressed;

            // 3. Beweging Input
            bool movementLocked = IsAttacking && _current != _runAttack;

            Velocity.X = 0;
            if (!movementLocked)
            {
                if (IsRunningRight) { Velocity.X += moveSpeed; FacingRight = true; }
                if (IsRunningLeft) { Velocity.X -= moveSpeed; FacingRight = false; }
            }

            if (Jump && IsOnGround(blocks) && !IsAttacking) Velocity.Y = jumpStrength;

            // 4. Physics & Collision
            Velocity.Y += gravity;

            Position.X += Velocity.X;
            ResolveCollisionsX(blocks);

            Position.Y += Velocity.Y;
            ResolveCollisionsY(blocks);

            // 5. Animatie Status Logic
            if (IsAttacking)
            {
                // -- DAMAGE CHECK --
                if (_current.DamageFrames.Contains(_current.CurrentFrame))
                {
                    IsHitting = true;

                    int range = _current.AttackRange;
                    int height = _current.AttackHeight;
                    int attackX = (int)Position.X;
                    int attackY = (int)Position.Y + 10;

                    // FIX 2: Positie Berekening
                    if (FacingRight)
                    {
                        // Rechts: Start bij Position.X + Breedte van de Hero
                        attackX += HITBOX_WIDTH;
                    }
                    else
                    {
                        // Links: Start bij Position.X - Range (links van de speler)
                        attackX -= range;
                    }

                    AttackHitbox = new Rectangle(attackX, attackY, range, height);

                    // Slash Effect
                    if (_current == _attack1 && _current.CurrentFrame == 5 && !_airSlashEffect.IsActive)
                    {
                        _airSlashEffect.Play(Position, FacingRight);
                    }
                }

                // -- KLAAR CHECK --
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

                    // Schakel direct terug naar de juiste animatie om flikkeren te voorkomen
                    _current = (Velocity.X != 0) ? _run : _idle;
                }
            }
            else // Niet aan het aanvallen
            {
                if (!IsOnGround(blocks)) _current = _jump;
                else if (Velocity.X != 0) _current = _run;
                else _current = _idle;
            }

            _current.Update(gameTime);
            _airSlashEffect.Update(gameTime);

            // Altijd de hitbox updaten aan het einde
            Hitbox.Update(Position, HITBOX_WIDTH, HITBOX_HEIGHT);
        }

        private void StartAttack()
        {
            IsAttacking = true;

            // Check beweging
            if (Velocity.X != 0 || IsRunningRight || IsRunningLeft)
                _current = _runAttack;
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

            // Skinny Hitbox: Voorkomt dat we tegen de vloer botsen als we lopen
            Rectangle skinnyRect = Hitbox.HitboxRect;
            skinnyRect.Y += 4;
            skinnyRect.Height -= 8;

            foreach (var block in blocks)
            {
                if (block is ISolid)
                {
                    // FIX 3: Check collision met de SKINNY rect, niet de volledige hitbox
                    if (skinnyRect.Intersects(block.BoundingBox))
                    {
                        Rectangle intersection = Rectangle.Intersect(Hitbox.HitboxRect, block.BoundingBox);

                        if (Velocity.X > 0) Position.X -= intersection.Width;
                        else if (Velocity.X < 0) Position.X += intersection.Width;

                        Velocity.X = 0;
                        Hitbox.Update(Position, HITBOX_WIDTH, HITBOX_HEIGHT);
                    }
                }
            }
        }

        private void ResolveCollisionsY(List<Block> blocks)
        {
            Hitbox.Update(Position, HITBOX_WIDTH, HITBOX_HEIGHT);
            foreach (var block in blocks)
            {
                if (block is ISolid || block is IPlatform)
                {
                    if (Hitbox.HitboxRect.Intersects(block.BoundingBox))
                    {
                        Rectangle intersection = Rectangle.Intersect(Hitbox.HitboxRect, block.BoundingBox);
                        if (intersection.Height < intersection.Width)
                        {
                            if (Velocity.Y > 0)
                            {
                                if (block is IPlatform && Hitbox.HitboxRect.Bottom - intersection.Height > block.BoundingBox.Top)
                                    continue;
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
            // Centreer sprite logic
            Rectangle currentFrame = _current.Frames[_current.CurrentFrame];
            float widthDifference = currentFrame.Width - HITBOX_WIDTH;
            float drawX = Position.X - (widthDifference / 2);
            Vector2 drawPosition = new Vector2(drawX, Position.Y);

            _current.Draw(sb, drawPosition, FacingRight);
            _airSlashEffect.Draw(sb);
        }
    }
}