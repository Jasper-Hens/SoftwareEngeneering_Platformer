using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace test.Inputs
{
    public class KeyboardInputReader : IInputReader
    {
        private bool _wasAttackPressed = false;

        public Vector2 ReadMovement()
        {
            var k = Keyboard.GetState();
            Vector2 direction = Vector2.Zero;

            if (k.IsKeyDown(Keys.Right) || k.IsKeyDown(Keys.D)) direction.X = 1;
            if (k.IsKeyDown(Keys.Left) || k.IsKeyDown(Keys.Q)) direction.X = -1;

            return direction;
        }

        public bool IsJumpPressed()
        {
            return Keyboard.GetState().IsKeyDown(Keys.Space);
        }

        public bool IsDashPressed()
        {
            return Keyboard.GetState().IsKeyDown(Keys.LeftShift);
        }

        public bool IsAttackPressed()
        {
            var k = Keyboard.GetState();
            bool isPressed = k.IsKeyDown(Keys.Z) || k.IsKeyDown(Keys.J);

            // Zorg dat je niet 'spam' krijgt als je de knop ingedrukt houdt (optioneel, maar netjes)
            bool result = isPressed && !_wasAttackPressed;
            _wasAttackPressed = isPressed;

            return result;
        }
    }
}