namespace MovementComponents
{
    public interface IMovement
    {
        void TryMove(float horizontal);
        void TurnRight();
        void TurnLeft();
        void LongJump();
        void HighJump();
    }
}