using Cinemachine.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Samples.Cinemachine._2._9._5.Cinemachine_Example_Scenes.Shared.Scripts
{
    public class InvokeEventFromInspector : MonoBehaviour
    {
        public UnityEvent Event = new UnityEvent();
        public void Invoke() { Event.Invoke(); }
    }

    #if UNITY_EDITOR
    [CustomEditor(typeof(InvokeEventFromInspector))]
    public class GenerateEventEditor : BaseEditor<InvokeEventFromInspector>
    {
        public override void OnInspectorGUI()
        {
            BeginInspector();
            Rect rect = EditorGUILayout.GetControlRect(true);
            if (GUI.Button(rect, "Invoke", "Button"))
                Target.Invoke();
            DrawRemainingPropertiesInInspector();
        }
    }
    #endif
}
