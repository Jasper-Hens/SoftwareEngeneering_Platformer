using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic; // Nodig voor List

namespace test.States
{
    public class HeadsUpDisplay
    {
        private Texture2D _shieldFull, _shieldHalf, _shieldEmpty;
        private Texture2D _pixel;
        // _keyTexture is weggehaald, want die zit nu in de inventory items zelf!

        private Vector2 _startPosition = new Vector2(10, 10);
        private int _iconSize = 80;
        private int _padding = 10;

        // Constructor aangepast: geen keyTex meer nodig
        public HeadsUpDisplay(Texture2D full, Texture2D half, Texture2D empty, Texture2D pixel)
        {
            _shieldFull = full;
            _shieldHalf = half;
            _shieldEmpty = empty;
            _pixel = pixel;
        }

        // Draw aangepast: we vragen nu om de hele Inventory in plaats van alleen 'hasKey'
        public void Draw(SpriteBatch sb, int currentHealth, int maxHealth, float currentStamina, float maxStamina, Inventory inventory)
        {
            // 1. LEVENS
            int totalShields = maxHealth / 2;
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

            // 2. STAMINA
            int barWidth = 250;
            int barHeight = 15;
            int barX = (int)_startPosition.X + 5;
            int barY = (int)_startPosition.Y + _iconSize + 20;

            sb.Draw(_pixel, new Rectangle(barX, barY, barWidth, barHeight), Color.Black * 0.5f);
            float percentage = currentStamina / maxStamina;
            int currentBarWidth = (int)(barWidth * percentage);
            Color staminaColor = percentage < 0.3f ? Color.Orange : Color.LimeGreen;
            sb.Draw(_pixel, new Rectangle(barX, barY, currentBarWidth, barHeight), staminaColor);

            // 3. INVENTORY ITEMS (NIEUW & GENERIEK)
            if (inventory != null && inventory.Items.Count > 0)
            {
                int startItemX = barX;
                int startItemY = barY + barHeight + 15; // Iets onder de stamina balk
                int itemSpacing = 10; // Ruimte tussen items

                // Loop door alle items in je inventory
                for (int i = 0; i < inventory.Items.Count; i++)
                {
                    InventoryItem item = inventory.Items[i];

                    // Bepaal grootte (bijv. 50% van origineel)
                    float scale = 0.5f;
                    int width = (int)(item.SourceRect.Width * scale);
                    int height = (int)(item.SourceRect.Height * scale);

                    // Bepaal positie (elk volgend item schuift op naar rechts)
                    int x = startItemX + (i * (width + itemSpacing));
                    int y = startItemY;

                    Rectangle destRect = new Rectangle(x, y, width, height);

                    // Teken het item
                    sb.Draw(item.Texture, destRect, item.SourceRect, Color.White);
                }
            }
        }
    }
}