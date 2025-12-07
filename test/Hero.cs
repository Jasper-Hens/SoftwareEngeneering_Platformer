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
        private const int HITBOX_WIDTH = 45;
        private const int HITBOX_HEIGHT = 65;

        // Combo variabelen
        private int _comboIndex = 0;
        private double _comboTimer = 0;
        private const double COMBO_WINDOW = 1000;
        private bool _wasAttackPressed = false;

        public Hero(Texture2D idleTex, Texture2D runTex, Texture2D jumpTex,
                    Texture2D atk1Tex, Texture2D atk2Tex, Texture2D atk3Tex, Texture2D runAtkTex,
                    Texture2D slashTex)
        {
            _idle = new IdleAnimation(idleTex);
            _run = new RunAnimation(runTex);
            _jump = new JumpAnimation(jumpTex);

            _attack1 = new AttackOneAnimation(atk1Tex);
            _attack2 = new AttackTwoAnimation(atk2Tex);
            _attack3 = new AttackThreeAnimation(atk3Tex);
            _runAttack = new RunAttackAnimation(runAtkTex);

            // BELANGRIJK: Zorg dat ze NIET loopen!
            _attack1.IsLooping = false;
            _attack2.IsLooping = false;
            _attack3.IsLooping = false;
            _runAttack.IsLooping = false;

            // Damage Frames instellen (voorbeeld waarden)
            ((AttackOneAnimation)_attack1).DamageFrames = new List<int> { 5 };
            ((AttackTwoAnimation)_attack2).DamageFrames = new List<int> { 2, 3 };
            ((AttackThreeAnimation)_attack3).DamageFrames = new List<int> { 2, 3 };
            ((RunAttackAnimation)_runAttack).DamageFrames = new List<int> { 3, 4 };

            _airSlashEffect = new VisualEffect(slashTex, 64, 64, 4);
            _current = _idle;
            Hitbox = new Hitbox();
        }

        public void Update(GameTime gameTime, List<Block> blocks)
        {
            KeyboardState k = Keyboard.GetState();

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
            // Mag alleen bewegen als je NIET aanvalt, OF als het een RunAttack is
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
            ResolveCollisionsX(blocks); // <-- HIER ZIT DE FIX VOOR HET LOPEN

            Position.Y += Velocity.Y;
            ResolveCollisionsY(blocks);

            // 5. Animatie Status
            if (IsAttacking)
            {
                // Check hier of de animatie in Animation.cs op IsFinished is gezet
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

                // Trigger Slash Effect logic
                if (_current == _attack1 && _current.CurrentFrame == 5 && !_airSlashEffect.IsActive)
                    _airSlashEffect.Play(Position, FacingRight);
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

        private void StartAttack()
        {
            IsAttacking = true;
            if (Velocity.X != 0) _current = _runAttack;
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

            // Maak een 'Skinny' hitbox voor collision checks.
            // We maken hem iets kleiner aan de boven- en onderkant.
            // Hierdoor botst hij NIET tegen de vloer waar je op staat.
            Rectangle skinnyRect = Hitbox.HitboxRect;
            skinnyRect.Y += 4;       // 4 pixels lager beginnen
            skinnyRect.Height -= 8;  // 8 pixels kleiner (4 boven eraf, 4 onder eraf)

            foreach (var block in blocks)
            {
                if (block is ISolid)
                {
                    // Check tegen de skinny rect!
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
            Rectangle currentFrame = _current.Frames[_current.CurrentFrame];
            float widthDifference = currentFrame.Width - HITBOX_WIDTH;
            float drawX = Position.X - (widthDifference / 2);
            Vector2 drawPosition = new Vector2(drawX, Position.Y);

            _current.Draw(sb, drawPosition, FacingRight);
            _airSlashEffect.Draw(sb);
        }
    }
}