using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using test.Animations;
using test.Animations.EvilWizardAnimations;
using test.Blocks;
using test.Level;

namespace test
{
    public class EvilWizard : Enemy
    {
        private Animation _idleAnim;
        private Animation _attackAnim;
        private Animation _deathAnim;
        private Animation _currentAnim;

        private double _attackTimer;
        private double _attackInterval = 3000;
        private bool _isAttacking = false;

        private const int BODY_WIDTH = 27;
        private const int BODY_HEIGHT = 49;
        private const int ATTACK_BOX_WIDTH = 48;
        private const int ATTACK_BOX_HEIGHT = 32;

        // LET OP: Hier roepen we 'base' aan. Dit lost de CS7036 error op.
        public EvilWizard(Texture2D idleTex, Texture2D attackTex, Texture2D deathTex, Vector2 startPosition)
            : base(startPosition, 40)
        {
            _idleAnim = new EvilWizardIdleAnimation(idleTex);
            _attackAnim = new EvilWizardAttackAnimation(attackTex);
            _deathAnim = new EvilWizardDeathAnimation(deathTex);

            _currentAnim = _idleAnim;

            // Dit lost de CS0534 error op (we roepen de methode aan)
            UpdateHitbox();
        }

        // DEZE METHODE IS VERPLICHT (Lost CS0534 op)
        protected override void UpdateAI(GameTime gameTime, Hero hero, List<Block> blocks)
        {
            if (IsDead)
            {
                if (_currentAnim != _deathAnim)
                {
                    _currentAnim = _deathAnim;
                    _currentAnim.Reset();
                }
                _currentAnim.Update(gameTime);
                return;
            }

            FacingRight = hero.Position.X > Position.X;

            if (!_isAttacking)
            {
                _attackTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
                if (_attackTimer >= _attackInterval) StartAttack();
            }

            _currentAnim.Update(gameTime);

            if (_isAttacking && _currentAnim.IsFinished)
            {
                _isAttacking = false;
                _currentAnim = _idleAnim;
                _attackTimer = 0;
            }

            IsHitting = false;
            AttackHitbox = Rectangle.Empty;

            if (_isAttacking && _currentAnim.DamageFrames.Contains(_currentAnim.CurrentFrame))
            {
                IsHitting = true;
                int attackX = FacingRight ? (int)Position.X + BODY_WIDTH : (int)Position.X - ATTACK_BOX_WIDTH;
                AttackHitbox = new Rectangle(attackX, (int)Position.Y + 10, ATTACK_BOX_WIDTH, ATTACK_BOX_HEIGHT);
            }
        }

        // DEZE METHODE IS OOK VERPLICHT (Lost CS0534 op)
        protected override void UpdateHitbox()
        {
            // Dit lost de Rectangle error op
            Hitbox = new Rectangle((int)Position.X, (int)Position.Y, BODY_WIDTH, BODY_HEIGHT);
        }

        private void StartAttack()
        {
            _isAttacking = true;
            _currentAnim = _attackAnim;
            _currentAnim.Reset();
        }

        public override void Draw(SpriteBatch sb)
        {
            if (IsDead && _currentAnim.IsFinished) return;
            _currentAnim.Draw(sb, Position, FacingRight, _color);
        }
    }
}