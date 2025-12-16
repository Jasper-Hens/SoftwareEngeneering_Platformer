using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace test.Items
{
    public class HeartItem : Item
    {
        private List<Rectangle> _frames;
        private int _currentFrame = 0;
        private double _timer;
        private double _animSpeed = 100; // Snelheid van animatie (lager = sneller)

        public HeartItem(Texture2D texture, Vector2 position)
            : base(texture, position, new Rectangle(68, 4, 27, 24)) // Start met frame 1
        {
            // Omdat 27x24 best klein is, maken we hem hier iets groter (1.5x)
            Scale = 1.5f;

            // De sprite coördinaten die jij gaf:
            _frames = new List<Rectangle>
            {
                new Rectangle(68, 4, 27, 24),
                new Rectangle(68, 37, 27, 24),
                new Rectangle(68, 70, 27, 24),
                new Rectangle(2, 103, 27, 24),
                new Rectangle(35, 103, 27, 24),
                new Rectangle(71, 103, 22, 24),
                new Rectangle(107, 4, 16, 24),
                new Rectangle(108, 37, 14, 24),
                new Rectangle(107, 70, 16, 24),
                new Rectangle(104, 103, 22, 24)
            };
        }

        public override void Update(GameTime gameTime)
        {
            if (!IsActive) return;

            // Animatie afspelen
            _timer += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (_timer > _animSpeed)
            {
                _timer = 0;
                _currentFrame++;
                if (_currentFrame >= _frames.Count) _currentFrame = 0; // Loop terug naar start

                // Update de rechthoek die getekend wordt
                _sourceRect = _frames[_currentFrame];
            }
        }

        public override void OnPickup(Hero hero)
        {
            // Check: Hebben we healing nodig?
            if (hero.CurrentHealth < hero.MaxHealth)
            {
                // Genees 2 punten (is gelijk aan 1 heel hartje/schildje)
                hero.Heal(2);

                // Item verdwijnt na gebruik
                IsActive = false;
            }
        }
    }
}