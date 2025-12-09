using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using test.Animations;
using test.Animations.BossAnimations; // Zorg dat dit klopt met je mapje
using test.Blocks;

namespace test
{
    // Specifieke States voor DEZE boss (past bij jouw bestanden)
    public enum KnightBossState
    {
        Idle,
        Walk,
        Run,
        Jump,
        Attack1,
        Attack2,
        Attack3,
        WalkAttack,
        Special,
        Hurt,
        Death
    }

    public class KnightBoss : Enemy
    {
        // Animaties
        private Animation _idleAnim, _walkAnim, _runAnim, _jumpAnim;
        private Animation _atk1Anim, _atk2Anim, _atk3Anim, _walkAtkAnim, _specialAnim;
        private Animation _hurtAnim, _deathAnim;

        private Animation _currentAnim;
        private KnightBossState _currentState;

        // AI en Timers
        private float _moveSpeed = 1.5f;
        private float _runSpeed = 3.5f;
        private double _cooldownTimer = 0;

        // Hitbox Instellingen (Jouw verzoek)
        private const int SPRITE_WIDTH = 112;  // Totale breedte sprite
        private const int SPRITE_HEIGHT = 110; // Totale hoogte sprite

        // Trim waarden
        private const int TRIM_LEFT = 10;
        private const int TRIM_RIGHT = 50;

        // Effectieve Hitbox breedte = 112 - 10 - 50 = 52
        private int _finalHitboxWidth = SPRITE_WIDTH - TRIM_LEFT - TRIM_RIGHT;
        private int _finalHitboxHeight = 110; // Of iets minder als je wilt

        private Texture2D _pixelTexture; // Voor de healthbar

        // CONSTRUCTOR: Heel veel parameters omdat je losse files hebt!
        public KnightBoss(Vector2 startPos, Texture2D pixelTex,
                          Texture2D idleTex, Texture2D walkTex, Texture2D runTex, Texture2D jumpTex,
                          Texture2D atk1Tex, Texture2D atk2Tex, Texture2D atk3Tex,
                          Texture2D walkAtkTex, Texture2D specialTex,
                          Texture2D hurtTex, Texture2D deathTex)
            : base(startPos, 200) // 200 HP
        {
            _pixelTexture = pixelTex;

            // Animaties initialiseren met hun EIGEN texture
            _idleAnim = new BossIdleAnimation(idleTex);
            _walkAnim = new BossWalkAnimation(walkTex);
            _runAnim = new BossRunAnimation(runTex);
            _jumpAnim = new BossJumpAnimation(jumpTex);

            _atk1Anim = new BossAttackOneAnimation(atk1Tex);
            _atk2Anim = new BossAttackTwoAnimation(atk2Tex);
            _atk3Anim = new BossAttackThreeAnimation(atk3Tex);

            _walkAtkAnim = new BossWalk_AttackAnimation(walkAtkTex); // Check class naam
            _specialAnim = new BossSpecialAnimation(specialTex);

            // _hurtAnim = new BossHurtAnimation(hurtTex);
            _deathAnim = new BossDeathAnimation(deathTex);

            // Zorg dat aanvallen niet loopen
            _atk1Anim.IsLooping = false;
            _atk2Anim.IsLooping = false;
            _atk3Anim.IsLooping = false;
            _specialAnim.IsLooping = false;
            _deathAnim.IsLooping = false;

            // Start State
            _currentState = KnightBossState.Idle;
            _currentAnim = _idleAnim;

            // Attack 1: Hitbox is actief tijdens sprite 3, 4 en 5
            _atk1Anim.DamageFrames = new List<int> { 5, 6, 7 };

            // Attack 2: Hitbox is actief tijdens sprite 3 en 4
            _atk2Anim.DamageFrames = new List<int> { 5, 6, 7 };

            // Attack 3: Hitbox is actief tijdens sprite 4, 5 en 6
            _atk3Anim.DamageFrames = new List<int> { 3, 5, 6, 7};

            UpdateHitbox(); // Direct hitbox zetten
        }

        protected override void UpdateHitbox()
        {
            Hitbox = new Rectangle((int)Position.X, (int)Position.Y, _finalHitboxWidth, _finalHitboxHeight);
        }

        protected override void UpdateAI(GameTime gameTime, Hero hero)
        {
            // 1. Timers & Afstand
            if (_cooldownTimer > 0) _cooldownTimer -= gameTime.ElapsedGameTime.TotalMilliseconds;

            float distanceToHero = Vector2.Distance(Position, hero.Position);
            float xDist = hero.Position.X - Position.X;

            // 2. Richting bepalen (niet draaien tijdens aanval)
            bool isAttacking = IsInAttackState();
            if (!isAttacking && !IsDead)
            {
                FacingRight = xDist > 0;
            }

            // 3. State Machine Logica (Gedrag)
            switch (_currentState)
            {
                case KnightBossState.Idle:
                case KnightBossState.Walk:
                case KnightBossState.Run:
                    if (_cooldownTimer > 0)
                    {
                        SwitchState(KnightBossState.Idle);
                        Velocity.X = 0;
                    }
                    else
                    {
                        if (Math.Abs(xDist) < 100) // Dichtbij -> Aanvallen
                        {
                            Velocity.X = 0;
                            Random rnd = new Random();
                            int choice = rnd.Next(0, 3);
                            if (choice == 0) SwitchState(KnightBossState.Attack1);
                            else if (choice == 1) SwitchState(KnightBossState.Attack2);
                            else SwitchState(KnightBossState.Attack3);
                        }
                        else if (Math.Abs(xDist) < 600) // Ver weg -> Rennen
                        {
                            SwitchState(KnightBossState.Run);
                            Velocity.X = FacingRight ? _runSpeed : -_runSpeed;
                        }
                        else
                        {
                            SwitchState(KnightBossState.Idle);
                            Velocity.X = 0;
                        }
                    }
                    break;

                case KnightBossState.Attack1:
                case KnightBossState.Attack2:
                case KnightBossState.Attack3:
                case KnightBossState.Special:
                    Velocity.X = 0;
                    if (_currentAnim.IsFinished)
                    {
                        _cooldownTimer = 1500;
                        SwitchState(KnightBossState.Idle);
                    }
                    break;

                case KnightBossState.Death:
                    Velocity.X = 0;
                    break;
            }

            // 4. HITBOX LOGICA (DIT IS HET STUK DAT JE ZOCHT!)
            // ==========================================================

            // Resetten aan het begin
            IsHitting = false;
            AttackHitbox = Rectangle.Empty;

            if (IsInAttackState())
            {
                // HIER KOMT JOUW CODE:
                if (_currentAnim.DamageFrames.Contains(_currentAnim.CurrentFrame))
                {
                    IsHitting = true;

                    // --- HIER PAS JE DE GROOTTE AAN PER AANVAL ---
                    int attackRange = 100;
                    int attackHeight = 110;

                    if (_currentState == KnightBossState.Attack1)
                    {
                        attackRange = 150;
                        attackHeight = 110;
                    }
                    else if (_currentState == KnightBossState.Attack2)
                    {
                        attackRange = 80;
                        attackHeight = 60;
                    }
                    else if (_currentState == KnightBossState.Attack3)
                    {
                        attackRange = 200;
                        attackHeight = 150;
                    }
                    // ---------------------------------------------

                    int attackX = (int)Position.X;
                    int attackY = (int)Position.Y;

                    if (FacingRight)
                    {
                        // Rechts: Begin bij de rechterkant van de body-hitbox
                        attackX += _finalHitboxWidth;
                    }
                    else
                    {
                        // Links: Begin 'range' pixels links van de boss
                        attackX -= attackRange;
                    }

                    AttackHitbox = new Rectangle(attackX, attackY, attackRange, attackHeight);
                }
            }
            // ==========================================================

            _currentAnim.Update(gameTime);
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
            // 1. SPRITE TEKENEN
            Vector2 drawPos = Position;
            SpriteEffects effect = SpriteEffects.None;

            if (FacingRight)
            {
                drawPos.X -= TRIM_LEFT;
            }
            else
            {
                drawPos.X -= TRIM_RIGHT;
                effect = SpriteEffects.FlipHorizontally;
            }

            Rectangle currentFrame = _currentAnim.Frames[_currentAnim.CurrentFrame];
            float yOffset = SPRITE_HEIGHT - currentFrame.Height;

            sb.Draw(_currentAnim.Texture, drawPos + new Vector2(0, yOffset), currentFrame, _color, 0f, Vector2.Zero, 1f, effect, 0f);

            // 2. HEALTH BAR TEKENEN
            if (!IsDead)
            {
                int barW = 100;
                int barH = 10;
                int barX = (int)Position.X + (_finalHitboxWidth / 2) - (barW / 2);
                int barY = (int)Position.Y - 20;

                sb.Draw(_pixelTexture, new Rectangle(barX, barY, barW, barH), Color.Red);
                float pct = (float)CurrentHealth / 200; // Hardcoded MaxHealth 200 voor nu
                sb.Draw(_pixelTexture, new Rectangle(barX, barY, (int)(barW * pct), barH), Color.LightGreen);
            }

            // ==========================================================
            // 3. DEBUG: HITBOXES ZICHTBAAR MAKEN (DIT ONTBRAK!)
            // ==========================================================

            // BLAUW = Body Hitbox (Waar je de boss moet raken)
            sb.Draw(_pixelTexture, Hitbox, Color.Blue * 0.5f);

            // ROOD = Attack Hitbox (Waar de boss JOU raakt)
            if (IsHitting)
            {
                sb.Draw(_pixelTexture, AttackHitbox, Color.Red * 0.5f);
            }
        }


    }
}