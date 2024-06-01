// Interfaces to clarify which state has movement capability, 1D: 1 Direction 2D: 2 Direction
// Note that these interfaces represents directions not dimensions.
namespace UltimateCC
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
