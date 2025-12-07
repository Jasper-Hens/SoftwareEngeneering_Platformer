using Microsoft.Xna.Framework;
using System;

namespace test
{
    public class Camera
    {
        // Huidige positie van de camera in de wereld
        public Vector2 Position { get; private set; }

        // Afmetingen van de viewport (scherm)
        private readonly int _viewportWidth;
        private readonly int _viewportHeight;

        // Afmetingen van het totale level
        private readonly int _levelWidth;
        private readonly int _levelHeight;

        public Camera(int viewportWidth, int viewportHeight, int levelWidth, int levelHeight)
        {
            _viewportWidth = viewportWidth;
            _viewportHeight = viewportHeight;
            _levelWidth = levelWidth;
            _levelHeight = levelHeight;
            Position = Vector2.Zero;
        }

        /// <summary>
        /// Berekent de nieuwe camera positie op basis van de character positie,
        /// en zorgt ervoor dat de camera binnen de level grenzen blijft.
        /// </summary>
        /// <param name="targetPosition">De centrale positie van de Hero in world coordinates.</param>
        public void Follow(Vector2 targetPosition)
        {
            // 1. Probeer de target (character) te centreren.
            // De camera positie is de linkerbovenhoek van het scherm.
            // We willen dat de target in het midden van het scherm staat.
            float targetX = targetPosition.X;
            float cameraX = targetX - (_viewportWidth / 2);

            // 2. Beperk de X-positie (horizontaal begrenzen).
            
            // Linker grens: Camera mag niet verder naar links dan 0.
            float minCameraX = 0;
            
            // Rechter grens: Camera stopt wanneer de rechterkant van het scherm
            // de rechterkant van het level bereikt.
            float maxCameraX = _levelWidth - _viewportWidth;

            // Zorg ervoor dat maxCameraX niet negatief is (als het level kleiner is dan de viewport)
            if (maxCameraX < minCameraX)
                maxCameraX = minCameraX; // of centreren als het level smal is

            cameraX = MathHelper.Clamp(cameraX, minCameraX, maxCameraX);

            // De Y-positie (verticaal) hoeft in een typische 2D-platformer niet te volgen, 
            // maar voor de volledigheid:
            float targetY = targetPosition.Y;
            float cameraY = targetY - (_viewportHeight / 2);
            float maxCameraY = _levelHeight - _viewportHeight;
            
            cameraY = MathHelper.Clamp(cameraY, 0, maxCameraY);

            Position = new Vector2(cameraX, cameraY);
        }

        /// <summary>
        /// Geeft de transformatiematrix terug die nodig is om de wereld te tekenen.
        /// </summary>
        /// <returns>De transformatiematrix voor SpriteBatch.Begin().</returns>
        public Matrix GetTransformMatrix()
        {
            // De transformatiematrix verschuift de wereld in de tegenovergestelde richting 
            // van de camera positie.
            return Matrix.CreateTranslation(-Position.X, -Position.Y, 0);
        }
    }
}