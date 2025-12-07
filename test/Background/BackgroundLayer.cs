using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace test
{
    public class BackgroundLayer
    {
        private readonly Texture2D _texture;
        private readonly Rectangle _sourceRect;
        public float ScrollRatio { get; set; }

        private readonly int _viewportWidth;
        private readonly int _viewportHeight;
        private readonly int _levelWidth;

        // Constructor blijft hetzelfde
        public BackgroundLayer(Texture2D texture, Rectangle sourceRect, float scrollRatio, int viewportWidth, int viewportHeight, int levelWidth)
        {
            _texture = texture;
            _sourceRect = sourceRect;
            ScrollRatio = scrollRatio;
            _viewportWidth = viewportWidth;
            _viewportHeight = viewportHeight;
            _levelWidth = levelWidth;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 cameraPosition)
        {
            int backgroundWidth = _sourceRect.Width;
            int backgroundHeight = _sourceRect.Height;

            float drawY;
            int finalHeight;

            // --- 1. Verticale Uitlijning & Scaling Bepaling ---

            // De Floor is de enige uitzondering die NIET mag stretchen en een vaste Y-positie heeft.
            if (backgroundHeight == 85) // Dit is de Floor laag
            {
                finalHeight = backgroundHeight; // Houd 85px hoogte
                drawY = 718; // Houd de handmatig gefixte Y-positie aan
            }
            else // Alle andere lagen (Sky, Nature, Wall, Pillars)
            {
                // Alle andere lagen worden verticaal uitgerekt om de viewport te vullen.
                finalHeight = _viewportHeight; // 720px hoogte

                // Lagen die stretchen beginnen altijd op Y=0, zowel in Screen als World Coordinates.
                drawY = 0;
            }


            // --- 2. Horizontale Bepaling (Parallax vs. Fixed) ---

            float parallaxCameraX = cameraPosition.X * ScrollRatio;

            float worldStartX;
            float parallaxOffset;

            if (ScrollRatio < 1.0f)
            {
                // Parallax: Gebruikt de vertraagde X-positie
                worldStartX = parallaxCameraX;
                parallaxOffset = parallaxCameraX;
            }
            else
            {
                // Vaste Lagen: Gebruikt de echte camera X-positie
                worldStartX = cameraPosition.X;
                parallaxOffset = 0;
            }


            // --- 3. Herhaling en Tekenen met Destination Rectangle ---

            int startTileIndex = (int)Math.Floor(worldStartX / backgroundWidth);
            int tilesToDraw = (_viewportWidth / backgroundWidth) + 3;

            for (int i = startTileIndex; i < startTileIndex + tilesToDraw; i++)
            {
                float drawX = (i * backgroundWidth) - parallaxOffset;

                // Definieer de Destination Rectangle
                Rectangle destinationRect = new Rectangle(
                    (int)drawX,
                    (int)drawY,
                    backgroundWidth,
                    finalHeight // Gebruik de berekende hoogte
                );

                spriteBatch.Draw(
                    _texture,
                    destinationRect, // Gebruik de Destination Rectangle
                    _sourceRect,
                    Color.White
                );
            }
        }

    }
}