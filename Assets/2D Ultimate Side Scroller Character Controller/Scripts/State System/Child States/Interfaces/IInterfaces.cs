// Interfaces to clarify which state has movement capability, 1D: 1 Direction 2D: 2 Direction
// Note that these interfaces represents directions not dimensions.
namespace _2D_Ultimate_Side_Scroller_Character_Controller.Scripts.State_System.Child_States.Interfaces
{
    public interface IMove1D
    {
        void Move1D();
    }

    public interface IMove2D
    {
        void Move2D();
    }
}
