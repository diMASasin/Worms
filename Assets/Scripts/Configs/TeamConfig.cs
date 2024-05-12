using System.Collections.Generic;
using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "TeamConfig", menuName = "Config/Team", order = 0)]
    public class TeamConfig : ScriptableObject
    {
        [field: SerializeField] public string Name { get; private set; } = "Team";
        [field: SerializeField] public List<WormConfig> WormConfigs { get; private set; }
    }
}