using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using test.Animations;
using test.Animations.EvilWizardAnimations;
using test.Blocks;

namespace test
{
    public class EvilWizard : Enemy
    {
        public static Texture2D DebugPixel; // Voor hitbox debuggen

        private Animation _idleAnim, _attackAnim, _deathAnim, _currentAnim;
        private double _attackTimer;
        private double _attackInterval = 3000;
        private bool _isAttacking = false;

        // Cooldown zodat hij niet elke frame damage doet als hij aanvalt
        private double _damageCooldown = 0;

        private const int BODY_WIDTH = 54;
        private const int BODY_HEIGHT = 98;
        private const int ATTACK_BOX_WIDTH = 96;
        private const int ATTACK_BOX_HEIGHT = 64;

        public EvilWizard(Texture2D idleTex, Texture2D attackTex, Texture2D deathTex, Vector2 startPosition)
            : base(startPosition, 40) // 40 HP
        {
            _idleAnim = new EvilWizardIdleAnimation(idleTex);
            _attackAnim = new EvilWizardAttackAnimation(attackTex);
            _deathAnim = new EvilWizardDeathAnimation(deathTex);

            _currentAnim = _idleAnim;
            IsStompable = true; // Je kan op zijn hoofd springen!
            UpdateHitbox();
        }

        // SOLID: Deze methode wordt aangeroepen door CollisionManager
        public override void OnPlayerHit()
        {
            // Als we de speler raken, zetten we een cooldown zodat we niet 60x per seconde damage doen
            _damageCooldown = 1000;
        }

        protected override void UpdateAI(GameTime gameTime, Hero hero, List<Block> blocks)
        {
            if (_damageCooldown > 0) _damageCooldown -= gameTime.ElapsedGameTime.TotalMilliseconds;

            if (IsDead)
            {
                HandleDeathAnimation(gameTime);
                return;
            }

            // Kijk naar de speler
            FacingRight = hero.Position.X > Position.X;

            // Attack logica
            if (!_isAttacking)
            {
                _attackTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
                if (_attackTimer >= _attackInterval) StartAttack();
            }

            _currentAnim.Update(gameTime);

            if (_isAttacking)
            {
                HandleAttackState();
            }
            else
            {
                IsHitting = false;
                AttackHitbox = Rectangle.Empty;
            }
        }

        private void HandleDeathAnimation(GameTime gameTime)
        {
            if (_currentAnim != _deathAnim)
            {
                _currentAnim = _deathAnim;
                _currentAnim.Reset();
                IsHitting = false;
                AttackHitbox = Rectangle.Empty;
            }
            _currentAnim.Update(gameTime);

            // Als de animatie klaar is, mag hij weg uit het level
            if (_currentAnim.IsFinished) ReadyToRemove = true;
        }

        private void HandleAttackState()
        {
            if (_currentAnim.IsFinished)
            {
                _isAttacking = false;
                _currentAnim = _idleAnim;
                _attackTimer = 0;
                IsHitting = false;
                AttackHitbox = Rectangle.Empty;
            }
            else
            {
                // Alleen damage doen als cooldown klaar is
                IsHitting = (_damageCooldown <= 0);

                int attackX = FacingRight ? (int)Position.X + BODY_WIDTH : (int)Position.X - ATTACK_BOX_WIDTH;
                AttackHitbox = new Rectangle(attackX, (int)Position.Y + 20, ATTACK_BOX_WIDTH, ATTACK_BOX_HEIGHT);
            }
        }

        private void StartAttack()
        {
            _isAttacking = true;
            _currentAnim = _attackAnim;
            _currentAnim.Reset();
        }

        protected override void UpdateHitbox()
        {
            Hitbox = new Rectangle((int)Position.X, (int)Position.Y, BODY_WIDTH, BODY_HEIGHT);
        }

        public override void Draw(SpriteBatch sb)
        {
            // Teken logic... (Hetzelfde als voorheen)
            if (IsDead && _currentAnim.IsFinished) return;

            Rectangle currentFrame = _currentAnim.Frames[_currentAnim.CurrentFrame];
            float scaledWidth = currentFrame.Width * 2.0f;
            Vector2 drawPos = Position;
            drawPos.Y -= 10;

            if (FacingRight) drawPos.X -= 5;
            else drawPos.X = Position.X + BODY_WIDTH - scaledWidth + 5;

            _currentAnim.Draw(sb, drawPos, FacingRight, _color, 2.0f);

            // Debug optie
            if (DebugPixel != null)
            {
                // sb.Draw(DebugPixel, Hitbox, Color.Red * 0.5f);
                // if (IsHitting) sb.Draw(DebugPixel, AttackHitbox, Color.Orange * 0.5f);
            }
        }
    }
}