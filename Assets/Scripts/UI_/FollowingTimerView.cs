using Timers;
using UnityEngine;

namespace UI_
{
    public class FollowingTimerView : MonoBehaviour
    {
        [field: SerializeField] public FollowingObject_.FollowingObject FollowingObject { get; private set; }
        [field: SerializeField] public TimerView TimerView { get; private set; }
    }
}