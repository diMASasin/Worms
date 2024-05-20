using UnityEngine;

namespace InputService.Scroll
{
    public class MouseScroll : IScrollable
    {
        public float GetScrollDelta() => Input.mouseScrollDelta.y;
    }
}