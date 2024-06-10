using System;

namespace Infrastructure.Interfaces
{
    public interface IBattleEvents
    {
        public event Action TurnStateExited;
    }
}