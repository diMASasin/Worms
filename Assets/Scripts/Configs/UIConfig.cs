using UI;
using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "UIConfig", menuName = "Config/UI", order = 0)]
    public class UIConfig : ScriptableObject
    {
        [field: SerializeField] public Sprite WeaponSelectorSprite { get; private set; }
        [field: SerializeField] public Sprite TurnTimerSprite { get; private set; }
        [field: SerializeField] public Sprite GlobalTimerSprite { get; private set; }
        [field: SerializeField] public Sprite WindViewSprite { get; private set; }
        
    }
}