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
    // Specifieke States voor DEZE boss
    public enum KnightBossState
    {
        Idle, Walk, Run, Jump,
        Attack1, Attack2, Attack3,
        WalkAttack, Special, Hurt, Death
    }

    public class KnightBoss : Enemy
    {
        // Animaties
        private Animation _idleAnim, _walkAnim, _runAnim, _jumpAnim;
        private Animation _atk1Anim, _atk2Anim, _atk3Anim, _walkAtkAnim, _specialAnim;
        private Animation _hurtAnim, _deathAnim;

        private Animation _currentAnim;
        private KnightBossState _currentState;

        // --- AI INSTELLINGEN (Zorg dat deze niet 0 zijn!) ---
        private float _moveSpeed = 0.5f; // Snelheid tijdens patrouille
        private float _runSpeed = 1.5f;  // Snelheid tijdens achtervolgen
        // Timers
        private double _cooldownTimer = 0;
        private double _patrolTimer = 0;
        private double _jumpTimer = 0;
        private int _patrolDirection = 1;

        // Random Generator (1x aanmaken om haperen te voorkomen)
        private Random _rnd = new Random();

        // Hitbox Instellingen
        private const int SPRITE_WIDTH = 112;
        private const int SPRITE_HEIGHT = 110;
        private const int TRIM_LEFT = 10;
        private const int TRIM_RIGHT = 50;

        private int _finalHitboxWidth = SPRITE_WIDTH - TRIM_LEFT - TRIM_RIGHT;
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

            // Animaties laden
            _idleAnim = new BossIdleAnimation(idleTex);
            _walkAnim = new BossWalkAnimation(walkTex);
            _runAnim = new BossRunAnimation(runTex);
            _jumpAnim = new BossJumpAnimation(jumpTex);

            _atk1Anim = new BossAttackOneAnimation(atk1Tex);
            _atk2Anim = new BossAttackTwoAnimation(atk2Tex);
            _atk3Anim = new BossAttackThreeAnimation(atk3Tex);

            _walkAtkAnim = new BossWalk_AttackAnimation(walkAtkTex);
            _specialAnim = new BossSpecialAnimation(specialTex);
            _deathAnim = new BossDeathAnimation(deathTex);

            // Settings
            _atk1Anim.IsLooping = false;
            _atk2Anim.IsLooping = false;
            _atk3Anim.IsLooping = false;
            _specialAnim.IsLooping = false;
            _deathAnim.IsLooping = false;
            _jumpAnim.IsLooping = false;

            _deathAnim.IsLooping = false;

            // Damage Frames instellen
            _atk1Anim.DamageFrames = new List<int> { 5, 6, 7 };
            _atk2Anim.DamageFrames = new List<int> { 5, 6, 7 };
            _atk3Anim.DamageFrames = new List<int> { 3, 5, 6, 7 };

            // Start
            _currentState = KnightBossState.Idle;
            _currentAnim = _idleAnim;
            UpdateHitbox();
        }

        protected override void UpdateHitbox()
        {
            Hitbox = new Rectangle((int)Position.X, (int)Position.Y, _finalHitboxWidth, _finalHitboxHeight);
        }

        // ================================================================================================
        // UPDATE AI LOGICA (Compleet & Gefixt)
        // ================================================================================================
        // Vervang de UpdateAI methode met deze versie:

        // Vervang de UpdateAI methode met deze versie:

        protected override void UpdateAI(GameTime gameTime, Hero hero, List<Block> blocks)
        {
            // ==========================================================
            // 1. DOOD LOGICA (MOET HELEMAAL BOVENAAN STAAN!)
            // ==========================================================
            if (IsDead)
            {
                // Forceer direct de Death state
                SwitchState(KnightBossState.Death);

                // Zeker weten dat hij niet loopt (dubbele veiligheid)
                _deathAnim.IsLooping = false;

                // Als de animatie klaar is, mag de boss weg
                if (_currentAnim.IsFinished)
                {
                    ReadyToRemove = true;
                }

                // Update ALLEEN de animatie en stop de rest van de AI
                _currentAnim.Update(gameTime);
                Velocity.X = 0;
                return; // <--- CRUCIAAL: STOP HIER! Ga niet verder naar beneden.
            }

            // ==========================================================
            // 2. NORMALE AI (Wordt overgeslagen als hij dood is)
            // ==========================================================

            // Timers
            if (_cooldownTimer > 0) _cooldownTimer -= gameTime.ElapsedGameTime.TotalMilliseconds;
            if (_jumpTimer > 0) _jumpTimer -= gameTime.ElapsedGameTime.TotalMilliseconds;
            if (_patrolTimer > 0) _patrolTimer -= gameTime.ElapsedGameTime.TotalMilliseconds;

            float xDist = hero.Position.X - Position.X;

            // Direction Lock
            bool isBusy = IsInAttackState() || _currentState == KnightBossState.Jump;
            if (!isBusy)
            {
                if (_currentState == KnightBossState.Run || Math.Abs(xDist) < 600)
                    FacingRight = xDist > 0;
            }

            // STATE MACHINE
            switch (_currentState)
            {
                case KnightBossState.Idle:
                case KnightBossState.Walk:
                    if (Math.Abs(xDist) < 70)
                    {
                        Velocity.X = 0;
                        if (_cooldownTimer <= 0) PerformRandomAttack();
                        else SwitchState(KnightBossState.Idle);
                    }
                    else if (Math.Abs(xDist) < 600)
                    {
                        SwitchState(KnightBossState.Run);
                    }
                    else
                    {
                        // Patrouille logica
                        if (_patrolTimer <= 0)
                        {
                            if (_currentState == KnightBossState.Walk)
                            {
                                SwitchState(KnightBossState.Idle);
                                Velocity.X = 0;
                                _patrolTimer = 2000;
                            }
                            else
                            {
                                SwitchState(KnightBossState.Walk);
                                _patrolDirection *= -1;
                                FacingRight = _patrolDirection > 0;
                                _patrolTimer = 3000;
                            }
                        }
                        if (_currentState == KnightBossState.Walk) Velocity.X = _moveSpeed * _patrolDirection;
                    }
                    break;

                case KnightBossState.Run:
                    if (Math.Abs(xDist) > 800) { SwitchState(KnightBossState.Idle); Velocity.X = 0; break; }

                    if (Math.Abs(xDist) < 70)
                    {
                        Velocity.X = 0;
                        if (_cooldownTimer <= 0) PerformRandomAttack();
                        else SwitchState(KnightBossState.Idle);
                    }
                    else
                    {
                        SwitchState(KnightBossState.Run);
                        Velocity.X = FacingRight ? _runSpeed : -_runSpeed;

                        // Voelspriet & Jump Logic
                        bool obstacleInFront = false;
                        int feelerWidth = 40;
                        int feelerX = FacingRight ? Hitbox.Right : Hitbox.Left - feelerWidth;
                        Rectangle feelerRect = new Rectangle(feelerX, Hitbox.Bottom - 45, feelerWidth, 40);

                        foreach (var block in blocks)
                        {
                            if (block is ISolid && feelerRect.Intersects(block.BoundingBox))
                            {
                                obstacleInFront = true;
                                break;
                            }
                        }

                        bool heroIsHigher = (hero.Position.Y < Position.Y - 100);
                        if (_jumpTimer <= 0 && (obstacleInFront || heroIsHigher))
                        {
                            SwitchState(KnightBossState.Jump);
                            Velocity.Y = -12f;
                            Velocity.X = FacingRight ? _runSpeed : -_runSpeed;
                            _jumpTimer = 1500;
                        }
                    }
                    break;

                case KnightBossState.Jump:
                    if (Math.Abs(Velocity.Y) < 0.1f && _currentAnim.CurrentFrame > 2)
                        SwitchState(KnightBossState.Run);
                    break;

                case KnightBossState.Attack1:
                case KnightBossState.Attack2:
                case KnightBossState.Attack3:
                case KnightBossState.Special:
                    Velocity.X = 0;
                    if (_currentAnim.IsFinished)
                    {
                        _cooldownTimer = 1000;
                        SwitchState(KnightBossState.Idle);
                    }
                    break;

                case KnightBossState.Death:
                    // Deze case wordt nooit bereikt omdat we bovenaan al returnen, 
                    // maar laat hem staan voor de netheid.
                    Velocity.X = 0;
                    break;
            }

            // HITBOXES
            IsHitting = false;
            AttackHitbox = Rectangle.Empty;

            if (IsInAttackState() && _currentAnim.DamageFrames.Contains(_currentAnim.CurrentFrame))
            {
                IsHitting = true;
                // ... (Jouw hitbox code hier) ...
                // ... Plak je hitbox berekeningen met offset en overlap hier terug ...

                // Korte versie om errors te voorkomen in dit voorbeeld:
                int range = 100; int h = 110; int yOff = 0; int xOff = 20;
                if (_currentState == KnightBossState.Attack1) { range = 70; h = 40; yOff = 45; xOff = 30; }
                else if (_currentState == KnightBossState.Attack2) { range = 80; h = 100; yOff = 50; xOff = 30; }
                else if (_currentState == KnightBossState.Attack3) { range = 70; h = 100; xOff = 30; }

                int ax = FacingRight ? (int)Position.X + (_finalHitboxWidth - xOff) : (int)Position.X - (range - xOff);
                AttackHitbox = new Rectangle(ax, (int)Position.Y + yOff, range, h);
            }

            _currentAnim.Update(gameTime);
        }

        // Hulpmethodes
        private bool IsInAttackState()
        {
            return _currentState == KnightBossState.Attack1 || _currentState == KnightBossState.Attack2 ||
                   _currentState == KnightBossState.Attack3 || _currentState == KnightBossState.Special ||
                   _currentState == KnightBossState.WalkAttack;
        }

        private void SwitchState(KnightBossState newState)
        {
            if (_currentState == newState) return;
            _currentState = newState;

            switch (_currentState)
            {
                case KnightBossState.Idle: _currentAnim = _idleAnim; break;
                case KnightBossState.Walk: _currentAnim = _walkAnim; break;
                case KnightBossState.Run: _currentAnim = _runAnim; break;
                case KnightBossState.Jump: _currentAnim = _jumpAnim; break;
                case KnightBossState.Attack1: _currentAnim = _atk1Anim; break;
                case KnightBossState.Attack2: _currentAnim = _atk2Anim; break;
                case KnightBossState.Attack3: _currentAnim = _atk3Anim; break;
                case KnightBossState.WalkAttack: _currentAnim = _walkAtkAnim; break;
                case KnightBossState.Special: _currentAnim = _specialAnim; break;
                case KnightBossState.Hurt: _currentAnim = _hurtAnim; break;
                case KnightBossState.Death: _currentAnim = _deathAnim; break;
            }
            _currentAnim.Reset();
        }

        public override void Draw(SpriteBatch sb)
        {
            // Sprite Centreren
            Rectangle currentFrame = _currentAnim.Frames[_currentAnim.CurrentFrame];
            float hitboxCenterX = Position.X + (_finalHitboxWidth / 2);
            float hitboxBottomY = Position.Y + _finalHitboxHeight;

            Vector2 origin = new Vector2(currentFrame.Width / 2, currentFrame.Height);
            Vector2 drawPos = new Vector2(hitboxCenterX, hitboxBottomY);
            SpriteEffects effect = FacingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            sb.Draw(_currentAnim.Texture, drawPos, currentFrame, _color, 0f, origin, 1f, effect, 0f);

            // Healthbar
            if (!IsDead)
            {
                int barW = 100; int barH = 10;
                int barX = (int)Position.X + (_finalHitboxWidth / 2) - (barW / 2);
                int barY = (int)Position.Y - 20;

                sb.Draw(_pixelTexture, new Rectangle(barX, barY, barW, barH), Color.Red);
                float pct = (float)CurrentHealth / MaxHealth;
                sb.Draw(_pixelTexture, new Rectangle(barX, barY, (int)(barW * pct), barH), Color.LightGreen);
            }

            // DEBUG HITBOXES
            sb.Draw(_pixelTexture, Hitbox, Color.Blue * 0.5f);
            if (IsHitting) sb.Draw(_pixelTexture, AttackHitbox, Color.Red * 0.5f);
        }

        private void PerformRandomAttack()
        {
            // Zorg dat je bovenaan de class wel 'private Random _rnd = new Random();' hebt staan
            int choice = _rnd.Next(0, 3);

            if (choice == 0) SwitchState(KnightBossState.Attack1);
            else if (choice == 1) SwitchState(KnightBossState.Attack2);
            else SwitchState(KnightBossState.Attack3);
        }
    }
}