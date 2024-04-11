using System;
using System.Collections.Generic;
using UnityEngine;

namespace Spawn
{
    [Serializable]
    public class SpawnerData
    {
        [field: SerializeField] public int TeamsNumber { get; set; } = 2;
        [field: SerializeField] public int WormsNumber { get; set; } = 4;
        [field: SerializeField] public List<Color> TeamColors { get; set; }
    }
}