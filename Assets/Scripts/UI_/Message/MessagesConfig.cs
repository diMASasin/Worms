using UnityEngine;

namespace _UI.Message
{
    [CreateAssetMenu(fileName = "MessagesConfig", menuName = "Config/Messages", order = 0)]
    public class MessagesConfig : ScriptableObject
    {
        [field: SerializeField] public string TurnStartedText { get; private set; }
        [field: SerializeField] public string GetReadyText { get; private set; }
        [field: SerializeField] public string WormDiedText { get; private set; }
    }
}