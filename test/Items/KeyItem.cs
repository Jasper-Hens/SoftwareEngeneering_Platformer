using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace test.Items
{
    public class KeyItem : Item
    {
        private static Rectangle _keySourceRect = new Rectangle(568, 276, 58, 129);

        // Variabelen voor het zweven
        private Vector2 _startPosition;
        private float _floatSpeed = 3f;
        private float _floatRange = 5f;

        public KeyItem(Texture2D texture, Vector2 position)
            : base(texture, position, _keySourceRect)
        {
            _startPosition = position;

            // --- HIER PAS JE DE GROOTTE IN HET LEVEL AAN ---
            Scale = 0.5f; // Dit maakt hem 2x zo klein (net als in de inventory)
            // -----------------------------------------------
        }

        public override void Update(GameTime gameTime)
        {
            if (!IsActive) return;

            // Zweef logica (blijft hetzelfde)
            double time = gameTime.TotalGameTime.TotalSeconds;
            float newY = _startPosition.Y + (float)Math.Sin(time * _floatSpeed) * _floatRange;
            Position = new Vector2(_startPosition.X, newY);
        }

        public override void OnPickup(Hero hero)
        {
            if (hero.Inventory != null)
            {
                // Voeg toe aan inventory
                hero.Inventory.AddKey(_texture, _keySourceRect);
            }
            IsActive = false;
        }
    }
}