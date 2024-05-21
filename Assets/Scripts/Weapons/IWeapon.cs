namespace Weapons
{
    public interface IWeapon
    {
        void MoveScope(float direction);
        void StartIncresePower();
        void IncreaseShotPower();
        void Shoot();
    }
}