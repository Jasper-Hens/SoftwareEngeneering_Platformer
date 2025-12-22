using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using test.Animations;
using test.Animations.DemonAnimations;
using test.Blocks;
using test.Level;

namespace test
{
    public class Demon2 : Enemy
    {
        private Animation _walkAnim;
        private Animation _attackAnim;
        private Animation _deathAnim;
        private Animation _currentAnim;

        private bool _isAttacking = false;
        private double _damageCooldown = 0;

        // Constanten
        private const int WIDTH = 44;
        private const int HEIGHT = 80;
        private const int ATTACK_RANGE = 40;

        // AANGEPAST: We vragen nu om 3 losse textures in plaats van 1 spritesheet
        public Demon2(Texture2D walkTex, Texture2D attackTex, Texture2D deathTex, Vector2 startPosition)
            : base(startPosition, 1) // 1 HP
        {
            _walkAnim = new Demon2WalkAnimation(walkTex);     // Gebruik de loop-texture
            _attackAnim = new Demon2AttackAnimation(attackTex); // Gebruik de attack-texture
            _deathAnim = new Demon2DeathAnimation(deathTex);    // Gebruik de death-texture

            _currentAnim = _walkAnim;

            FacingRight = true;
            UpdateHitbox();
        }

        protected override void UpdateAI(GameTime gameTime, Hero hero, List<Block> blocks)
        {
            if (IsDead)
            {
                if (_currentAnim != _deathAnim)
                {
                    _currentAnim = _deathAnim;
                    _currentAnim.Reset();
                    Velocity.X = 0;
                    IsHitting = false;
                    AttackHitbox = Microsoft.Xna.Framework.Rectangle.Empty;
                }
                _currentAnim.Update(gameTime);
                return;
            }

            if (_damageCooldown > 0) _damageCooldown -= gameTime.ElapsedGameTime.TotalMilliseconds;

            var headHitbox = new Microsoft.Xna.Framework.Rectangle(Hitbox.X + 5, Hitbox.Y - 5, Hitbox.Width - 10, 20);

            if (hero.Hitbox.HitboxRect.Intersects(headHitbox) && hero.Velocity.Y > 0)
            {
                hero.Velocity.Y = -12f;
                TakeDamage(1);
                return;
            }

            if (!_isAttacking && Hitbox.Intersects(hero.Hitbox.HitboxRect))
            {
                StartAttack();
            }

            if (_isAttacking)
            {
                Velocity.X = 0;
                _currentAnim.Update(gameTime);

                if (_currentAnim.CurrentFrame >= 2 && _currentAnim.CurrentFrame <= 4)
                {
                    IsHitting = (_damageCooldown <= 0);
                    int attackX = FacingRight ? Hitbox.Right : Hitbox.Left - ATTACK_RANGE;
                    AttackHitbox = new Microsoft.Xna.Framework.Rectangle(attackX, Hitbox.Y, ATTACK_RANGE, Hitbox.Height);
                }
                else
                {
                    IsHitting = false;
                    AttackHitbox = Microsoft.Xna.Framework.Rectangle.Empty;
                }

                if (_currentAnim.IsFinished)
                {
                    _isAttacking = false;
                    _currentAnim = _walkAnim;
                    IsHitting = false;
                }
            }
            else
            {
                _currentAnim = _walkAnim;
                _currentAnim.Update(gameTime);
                float speed = 2.0f;
                if (System.Math.Abs(Velocity.X) < 0.1f) FacingRight = !FacingRight;
                Velocity.X = FacingRight ? speed : -speed;
            }
        }

        private void StartAttack()
        {
            _isAttacking = true;
            _currentAnim = _attackAnim;
            _currentAnim.Reset();
        }

        public void OnHitSuccesful() { _damageCooldown = 1000; }

        protected override void UpdateHitbox()
        {
            Hitbox = new Microsoft.Xna.Framework.Rectangle((int)Position.X, (int)Position.Y, WIDTH, HEIGHT);
        }

        public override void Draw(SpriteBatch sb)
        {
            if (IsDead && _currentAnim.IsFinished) return;
            Vector2 drawPos = Position;
            drawPos.X -= 10;
            _currentAnim.Draw(sb, drawPos, FacingRight, _color, 1.0f);
        }
    }
}