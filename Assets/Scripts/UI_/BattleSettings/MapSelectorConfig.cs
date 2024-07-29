using System.Collections.Generic;
using _UI.BattleSettings;
using UnityEngine;

[CreateAssetMenu(fileName = "MapSelectorConfig", menuName = "Config/MapSelector")]
public class MapSelectorConfig : ScriptableObject
{
    [field: SerializeField] public List<MapSelectorItem> MapSelectorItems { get; private set; }
    
    
}
