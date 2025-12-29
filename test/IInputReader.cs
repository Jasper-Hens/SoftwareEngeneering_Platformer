using Microsoft.Xna.Framework;

namespace test.Inputs
{
    public interface IInputReader
    {
        Vector2 ReadMovement();
        bool IsJumpPressed();
        bool IsDashPressed();
        bool IsAttackPressed();
    }
}
