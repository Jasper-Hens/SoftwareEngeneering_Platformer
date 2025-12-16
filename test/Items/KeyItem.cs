using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace test.Items
{
    public class KeyItem : Item
    {
        // HIER STAAN JOUW CIJFERS: X=568, Y=276, Breedte=58, Hoogte=129
        private static Rectangle _keySourceRect = new Rectangle(568, 276, 58, 129);

        public KeyItem(Texture2D texture, Vector2 position)
            : base(texture, position, _keySourceRect) // We geven dit door aan de Item-klasse
        {
        }

        public override void OnPickup(Hero hero)
        {
            if (hero.Inventory != null)
            {
                hero.Inventory.AddKey();
            }
            IsActive = false; // Verdwijn na oppakken
        }
    }
}