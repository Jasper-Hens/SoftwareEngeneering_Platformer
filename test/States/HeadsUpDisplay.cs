using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace test.States
{
    public class HeadsUpDisplay
    {
        private Texture2D _shieldFull, _shieldHalf, _shieldEmpty;

        // NIEUW: Pixel voor de stamina balk
        private Texture2D _pixel;

        private Vector2 _startPosition = new Vector2(10, 10);
        private int _iconSize = 80;
        private int _padding = 10;

        public HeadsUpDisplay(Texture2D full, Texture2D half, Texture2D empty, Texture2D pixel)
        {
            _shieldFull = full;
            _shieldHalf = half;
            _shieldEmpty = empty;
            _pixel = pixel;
        }

        public void Draw(SpriteBatch sb, int currentHealth, int maxHealth, float currentStamina, float maxStamina)
        {
            // 1. LEVENS (Schildjes)
            int totalShields = maxHealth / 2;
            int totalWidthOfShields = (totalShields * _iconSize) + ((totalShields - 1) * _padding);

            for (int i = 0; i < totalShields; i++)
            {
                int xPos = (int)_startPosition.X + (i * (_iconSize + _padding));
                int yPos = (int)_startPosition.Y;
                Rectangle destRect = new Rectangle(xPos, yPos, _iconSize, _iconSize);

                int hpThreshold = (i + 1) * 2;
                if (currentHealth >= hpThreshold) sb.Draw(_shieldFull, destRect, Color.White);
                else if (currentHealth == hpThreshold - 1) sb.Draw(_shieldHalf, destRect, Color.White);
                else sb.Draw(_shieldEmpty, destRect, Color.White);
            }

            // 2. STAMINA BALK (Onder de schildjes)
            // 2. STAMINA BALK
            int barWidth = 250;
            int barHeight = 15;
            int barX = (int)_startPosition.X + 5;

            int barY = (int)_startPosition.Y + _iconSize + 20;

            // Teken de balk (blijft hetzelfde)
            sb.Draw(_pixel, new Rectangle(barX, barY, barWidth, barHeight), Color.Black * 0.5f);

            float percentage = currentStamina / maxStamina;
            int currentBarWidth = (int)(barWidth * percentage);
            Color staminaColor = percentage < 0.3f ? Color.Orange : Color.LimeGreen;

            sb.Draw(_pixel, new Rectangle(barX, barY, currentBarWidth, barHeight), staminaColor);
        }
    }
}