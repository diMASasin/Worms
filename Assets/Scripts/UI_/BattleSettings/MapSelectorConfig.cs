using System.Collections.Generic;
using UnityEngine;

namespace UI_.BattleSettings
{
    [CreateAssetMenu(fileName = "MapSelectorConfig", menuName = "Config/MapSelector")]
    public class MapSelectorConfig : ScriptableObject
    {
        [field: SerializeField] public List<MapSelectorItem> MapSelectorItems { get; private set; }
    
    
    }
}
