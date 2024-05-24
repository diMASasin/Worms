using UnityEngine;

namespace Weapons
{
    public interface ISpawnPoint
    {
        Transform SpawnPoint { get; }
    }
}