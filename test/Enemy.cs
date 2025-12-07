using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace test
{
    public class Enemy
    {
        public Vector2 Position;
        public Texture2D Texture;
        public bool IsDead = false;
        public int Health = 3;

        // Hit timer om te voorkomen dat hij 60x per seconde damage krijgt
        private double _invincibilityTimer = 0;

        public Rectangle Hitbox
        {
            get
            {
                // Pas de grootte aan naar jouw sprite (bijv. 50x50)
                return new Rectangle((int)Position.X, (int)Position.Y, 50, 50);
            }
        }

        public Enemy(Texture2D texture, Vector2 position)
        {
            Texture = texture;
            Position = position;
        }

        public void Update(GameTime gameTime)
        {
            if (_invincibilityTimer > 0)
                _invincibilityTimer -= gameTime.ElapsedGameTime.TotalMilliseconds;
        }

        public void TakeDamage(int damage)
        {
            // Alleen damage doen als we niet 'onsterfelijk' zijn (korte cooldown)
            if (_invincibilityTimer <= 0)
            {
                Health -= damage;
                _invincibilityTimer = 500; // 0.5 seconde onsterfelijk na een klap

                if (Health <= 0)
                {
                    IsDead = true;
                }
            }
        }

        public void Draw(SpriteBatch sb)
        {
            if (!IsDead)
            {
                // Als hij hit is, teken hem rood, anders wit
                Color color = (_invincibilityTimer > 0) ? Color.Red : Color.White;
                sb.Draw(Texture, Position, color);
            }
        }
    }
}