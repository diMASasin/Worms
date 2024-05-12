using System.Collections.Generic;
using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "WormsSpawner", menuName = "Config/WormsSpawner", order = 0)]
    public class WormsSpawnerConfig : ScriptableObject
    {
        [field: SerializeField] public float MaxSlope { get; private set; } = 45;
        [field: SerializeField] public List<TeamConfig> TeamConfigs { get; private set; }
        [field: SerializeField] public List<Color> TeamColors { get; private set; }

        private void OnValidate()
        {
            if (TeamColors.Count < TeamConfigs.Count)
            {
                TeamConfigs.RemoveAt(TeamConfigs.Count - 1);
                Debug.LogWarning($"{nameof(TeamColors)} count cant be less then {nameof(TeamConfigs)} count");
            }
        }
    }
}