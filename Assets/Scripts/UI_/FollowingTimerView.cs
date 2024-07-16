using Timers;
using UnityEngine;

namespace _UI
{
    public class FollowingTimerView : MonoBehaviour
    {
        [field: SerializeField] public FollowingObject FollowingObject { get; private set; }
        [field: SerializeField] public TimerView TimerView { get; private set; }
    }
}