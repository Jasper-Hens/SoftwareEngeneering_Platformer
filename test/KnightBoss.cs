using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using test.Animations;
using test.Animations.BossAnimations;
using test.block_Interfaces;
using test.Blocks;

namespace test
{
    public enum KnightBossState
    {
        Idle, Walk, Run, Jump,
        Attack1, Attack2, Attack3,
        WalkAttack, Special, Hurt, Death
    }

    public class KnightBoss : Enemy
    {
        private Animation _idleAnim, _walkAnim, _runAnim, _jumpAnim;
        private Animation _atk1Anim, _atk2Anim, _atk3Anim, _walkAtkAnim, _specialAnim;
        private Animation _hurtAnim, _deathAnim;
        private Animation _currentAnim;
        private KnightBossState _currentState;

        // --- SNELHEDEN ---
        private float _moveSpeed = 1.0f;
        private float _runSpeed = 3.0f; // Iets sneller rennen voor het contrast

        // --- TIMERS ---
        private double _cooldownTimer = 1000;
        private double _patrolTimer = 0;
        private double _jumpTimer = 0;
        private int _patrolDirection = 1;

        private Random _rnd = new Random();
        private int _finalHitboxWidth = 52;
        private int _finalHitboxHeight = 110;
        private Texture2D _pixelTexture;

        public KnightBoss(Vector2 startPos, Texture2D pixelTex,
                          Texture2D idleTex, Texture2D walkTex, Texture2D runTex, Texture2D jumpTex,
                          Texture2D atk1Tex, Texture2D atk2Tex, Texture2D atk3Tex,
                          Texture2D walkAtkTex, Texture2D specialTex,
                          Texture2D hurtTex, Texture2D deathTex)
            : base(startPos, 200)
        {
            _pixelTexture = pixelTex;
            _idleAnim = new BossIdleAnimation(idleTex); _walkAnim = new BossWalkAnimation(walkTex);
            _runAnim = new BossRunAnimation(runTex); _jumpAnim = new BossJumpAnimation(jumpTex);
            _atk1Anim = new BossAttackOneAnimation(atk1Tex); _atk2Anim = new BossAttackTwoAnimation(atk2Tex);
            _atk3Anim = new BossAttackThreeAnimation(atk3Tex); _walkAtkAnim = new BossWalk_AttackAnimation(walkAtkTex);
            _specialAnim = new BossSpecialAnimation(specialTex); _deathAnim = new BossDeathAnimation(deathTex);
            _hurtAnim = new BossIdleAnimation(hurtTex);

            _idleAnim.IsLooping = true; _walkAnim.IsLooping = true; _runAnim.IsLooping = true;
            _atk1Anim.IsLooping = false; _atk2Anim.IsLooping = false; _atk3Anim.IsLooping = false;
            _specialAnim.IsLooping = false; _deathAnim.IsLooping = false; _jumpAnim.IsLooping = false; // Jump niet loopen

            _atk1Anim.DamageFrames = new List<int> { 5, 6, 7 };
            _atk2Anim.DamageFrames = new List<int> { 5, 6, 7 };
            _atk3Anim.DamageFrames = new List<int> { 3, 5, 6, 7 };

            _currentState = KnightBossState.Idle;
            _currentAnim = _idleAnim;
            UpdateHitbox();
        }

        protected override void UpdateHitbox()
        {
            Hitbox = new Rectangle((int)Position.X, (int)Position.Y, _finalHitboxWidth, _finalHitboxHeight);
        }

        // Helper om te checken of boss op de grond staat (Voor jump animatie fix)
        private bool IsOnGround(List<Block> blocks)
        {
            // Check een klein strookje onder de voeten
            Rectangle footRect = new Rectangle(Hitbox.X, Hitbox.Bottom, Hitbox.Width, 2);
            foreach (var b in blocks)
            {
                if ((b is ISolid || b is IPlatform) && footRect.Intersects(b.BoundingBox))
                    return true;
            }
            return false;
        }

        protected override void UpdateAI(GameTime gameTime, Hero hero, List<Block> blocks)
        {
            if (IsDead)
            {
                SwitchState(KnightBossState.Death);
                if (_currentAnim.IsFinished) ReadyToRemove = true;
                _currentAnim.Update(gameTime);
                Velocity.X = 0;
                return;
            }

            if (_cooldownTimer > 0) _cooldownTimer -= gameTime.ElapsedGameTime.TotalMilliseconds;
            if (_jumpTimer > 0) _jumpTimer -= gameTime.ElapsedGameTime.TotalMilliseconds;
            if (_patrolTimer > 0) _patrolTimer -= gameTime.ElapsedGameTime.TotalMilliseconds;

            float xDist = hero.Position.X - Position.X;
            float absDist = Math.Abs(xDist);

            // AFSTANDEN VOOR GEDRAG
            int attackRange = 80;
            int sprintRange = 250; // Alleen sprinten als hij dichterbij is dan dit
            int stalkRange = 550;  // Tussen 250 en 550 loopt hij rustig naar je toe

            bool isBusy = IsInAttackState() || _currentState == KnightBossState.Jump;

            // Richting bepalen (Kijk naar hero als we niet patrouilleren)
            if (!isBusy)
            {
                if (_currentState == KnightBossState.Run || absDist < stalkRange)
                    FacingRight = xDist > 0;
            }

            // Voelspriet (Obstakels)
            bool obstacleInFront = false;
            int feelerX = FacingRight ? Hitbox.Right + 5 : Hitbox.Left - 45;
            Rectangle feelerRect = new Rectangle(feelerX, Hitbox.Bottom - 50, 40, 40);
            foreach (var block in blocks) { if (block is ISolid && feelerRect.Intersects(block.BoundingBox)) { obstacleInFront = true; break; } }

            switch (_currentState)
            {
                case KnightBossState.Idle:
                case KnightBossState.Walk:
                    // 1. Attack
                    if (absDist < attackRange)
                    {
                        Velocity.X = 0;
                        if (_cooldownTimer <= 0) PerformRandomAttack();
                        else SwitchState(KnightBossState.Idle);
                    }
                    // 2. Sprint (Dichtbij)
                    else if (absDist < sprintRange)
                    {
                        SwitchState(KnightBossState.Run);
                    }
                    // 3. Stalk (Middelmatige afstand) - Loop naar hero toe
                    else if (absDist < stalkRange)
                    {
                        SwitchState(KnightBossState.Walk);
                        // Loop naar de hero toe
                        FacingRight = xDist > 0;
                        Velocity.X = FacingRight ? _moveSpeed : -_moveSpeed;
                    }
                    // 4. Patrouille (Ver weg)
                    else
                    {
                        if (_patrolTimer <= 0 || obstacleInFront)
                        {
                            if (_currentState == KnightBossState.Walk)
                            {
                                SwitchState(KnightBossState.Idle);
                                Velocity.X = 0;
                                _patrolTimer = 1000;
                            }
                            else
                            {
                                SwitchState(KnightBossState.Walk);
                                if (obstacleInFront) _patrolDirection *= -1;
                                else _patrolDirection = (_rnd.Next(0, 2) == 0) ? 1 : -1;
                                FacingRight = _patrolDirection > 0;
                                _patrolTimer = 3000;
                            }
                        }
                        if (_currentState == KnightBossState.Walk) Velocity.X = _moveSpeed * _patrolDirection;
                    }
                    break;

                case KnightBossState.Run:
                    // Stop met rennen als te ver
                    if (absDist > stalkRange) { SwitchState(KnightBossState.Idle); Velocity.X = 0; break; }

                    // Attack
                    if (absDist < attackRange)
                    {
                        Velocity.X = 0;
                        if (_cooldownTimer <= 0) PerformRandomAttack();
                        else SwitchState(KnightBossState.Idle);
                    }
                    else
                    {
                        SwitchState(KnightBossState.Run);
                        Velocity.X = FacingRight ? _runSpeed : -_runSpeed;

                        // Spring logica
                        bool heroIsHigher = (hero.Position.Y < Position.Y - 100);
                        bool randomJump = (_rnd.Next(0, 300) == 0);

                        if (_jumpTimer <= 0 && (obstacleInFront || heroIsHigher || randomJump))
                        {
                            PerformJump();
                        }
                    }
                    break;

                case KnightBossState.Jump:
                    // FIX: Blijf in Jump state zolang we NIET op de grond staan.
                    // (Of als we net gesprongen zijn en nog omhoog gaan)
                    bool onGround = IsOnGround(blocks);

                    // Alleen landen als we op de grond staan én we niet net gelanceerd zijn (Vel Y < 0)
                    if (onGround && Velocity.Y >= 0)
                    {
                        SwitchState(KnightBossState.Run); // Of Idle/Walk afhankelijk van afstand
                    }
                    break;

                case KnightBossState.Attack1:
                case KnightBossState.Attack2:
                case KnightBossState.Attack3:
                case KnightBossState.Special:
                    Velocity.X = 0;
                    if (_currentAnim.IsFinished) { _cooldownTimer = 1000; SwitchState(KnightBossState.Idle); }
                    break;
            }

            IsHitting = false;
            AttackHitbox = Rectangle.Empty;
            if (IsInAttackState() && _currentAnim.DamageFrames.Contains(_currentAnim.CurrentFrame))
            {
                IsHitting = true;
                int ax = FacingRight ? (int)Position.X + (_finalHitboxWidth - 30) : (int)Position.X - (100 - 30);
                AttackHitbox = new Rectangle(ax, (int)Position.Y, 100, 110);
            }
            _currentAnim.Update(gameTime);
        }

        private void PerformJump()
        {
            SwitchState(KnightBossState.Jump);
            Velocity.Y = -12f;
            // Behoud horizontale snelheid
            float airSpeed = (_currentState == KnightBossState.Run) ? _runSpeed : _moveSpeed;
            Velocity.X = FacingRight ? airSpeed : -airSpeed;
            _jumpTimer = 1500;
        }

        private bool IsInAttackState()
        {
            return _currentState == KnightBossState.Attack1 ||
                   _currentState == KnightBossState.Attack2 ||
                   _currentState == KnightBossState.Attack3 ||
                   _currentState == KnightBossState.Special ||
                   _currentState == KnightBossState.WalkAttack;
        }

        private void SwitchState(KnightBossState newState)
        {
            if (_currentState == newState) return;

            _currentState = newState;

            switch (_currentState)
            {
                case KnightBossState.Idle:
                    _currentAnim = _idleAnim;
                    break;
                case KnightBossState.Walk:
                    _currentAnim = _walkAnim;
                    break;
                case KnightBossState.Run:
                    _currentAnim = _runAnim;
                    break;
                case KnightBossState.Jump:
                    _currentAnim = _jumpAnim;
                    break;
                case KnightBossState.Attack1:
                    _currentAnim = _atk1Anim;
                    break;
                case KnightBossState.Attack2:
                    _currentAnim = _atk2Anim;
                    break;
                case KnightBossState.Attack3:
                    _currentAnim = _atk3Anim;
                    break;
                case KnightBossState.WalkAttack:
                    _currentAnim = _walkAtkAnim;
                    break;
                case KnightBossState.Special:
                    _currentAnim = _specialAnim;
                    break;
                case KnightBossState.Hurt:
                    _currentAnim = _hurtAnim;
                    break;
                case KnightBossState.Death:
                    _currentAnim = _deathAnim;
                    break;
            }

            _currentAnim.Reset();
        }

        public override void Draw(SpriteBatch sb)
        {
            Rectangle currentFrame = _currentAnim.Frames[_currentAnim.CurrentFrame];
            Vector2 origin = new Vector2(currentFrame.Width / 2, currentFrame.Height);
            Vector2 drawPos = new Vector2(Position.X + _finalHitboxWidth / 2, Position.Y + _finalHitboxHeight);
            SpriteEffects effect = FacingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            sb.Draw(_currentAnim.Texture, drawPos, currentFrame, _color, 0f, origin, 1f, effect, 0f);

            if (!IsDead)
            {
                sb.Draw(_pixelTexture, new Rectangle((int)Position.X, (int)Position.Y - 20, 100, 10), Color.Red);
                sb.Draw(_pixelTexture, new Rectangle((int)Position.X, (int)Position.Y - 20, (int)(100 * ((float)CurrentHealth / MaxHealth)), 10), Color.Green);
            }
        }

        private void PerformRandomAttack()
        {
            int choice = _rnd.Next(0, 3);

            if (choice == 0)
                SwitchState(KnightBossState.Attack1);
            else if (choice == 1)
                SwitchState(KnightBossState.Attack2);
            else
                SwitchState(KnightBossState.Attack3);
        }
    }
}