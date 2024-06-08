using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UltimateCC
{
    // A custom attribute used to make variables non-editable but keep them visible in the inspector panel.
    public class NonEditableAttribute : PropertyAttribute
    {

    }
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(NonEditableAttribute))]
    public class NonEditableDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = true;
        }
    }
#endif
}
