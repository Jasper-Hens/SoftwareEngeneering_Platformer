using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace test.States
{
    public class HeadsUpDisplay
    {
        private Texture2D _shieldFull;
        private Texture2D _shieldHalf;
        private Texture2D _shieldEmpty;

        // INSTELLINGEN: Pas dit aan naar wens
        private Vector2 _startPosition = new Vector2(10, 10); // Helemaal linksboven (10px van de rand)
        private int _iconSize = 80;   // Hoe groot wil je het schildje? (40x40 pixels is klein)
        private int _padding = 10;     // Hoeveel ruimte tussen de schildjes?

        public HeadsUpDisplay(Texture2D full, Texture2D half, Texture2D empty)
        {
            _shieldFull = full;
            _shieldHalf = half;
            _shieldEmpty = empty;
        }

        public void Draw(SpriteBatch sb, int currentHealth, int maxHealth)
        {
            // 1 Schildje = 2 HP
            int totalShields = maxHealth / 2;

            for (int i = 0; i < totalShields; i++)
            {
                // BEREKEN DE NIEUWE POSITIE EN GROOTTE
                // We maken een rechthoek (Rectangle) waar het plaatje IN geperst moet worden.

                int xPos = (int)_startPosition.X + (i * (_iconSize + _padding));
                int yPos = (int)_startPosition.Y;

                Rectangle destRect = new Rectangle(xPos, yPos, _iconSize, _iconSize);

                // LOGICA (Hetzelfde als eerst)
                int hpThreshold = (i + 1) * 2;

                if (currentHealth >= hpThreshold)
                {
                    // Vol schild
                    sb.Draw(_shieldFull, destRect, Color.White);
                }
                else if (currentHealth == hpThreshold - 1)
                {
                    // Half schild
                    sb.Draw(_shieldHalf, destRect, Color.White);
                }
                else
                {
                    // Leeg schild
                    sb.Draw(_shieldEmpty, destRect, Color.White);
                }
            }
        }
    }
}