using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "TimersConfig", menuName = "Config/Timers", order = 0)]
    public class TimersConfig : ScriptableObject
    {
        [field: SerializeField] public float TurnDuration { get; private set; }
        [field: SerializeField] public float AfterShotDuration { get; private set; }
        [field: SerializeField] public float BetweenTurnsDuration { get; private set; }
        [field: SerializeField] public float GlobalTime { get; private set; }
    }
}