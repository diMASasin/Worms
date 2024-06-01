using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UltimateCC
{
    public class InformAttribute : PropertyAttribute
    {
        public string message;

        public InformAttribute(string message)
        {
            this.message = message;
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(InformAttribute))]
    public class InformDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            InformAttribute informUserAttribute = (InformAttribute)attribute;

            // Calculate the width of the property field
            float propertyWidth = EditorGUIUtility.labelWidth + EditorGUIUtility.fieldWidth;

            // Draw the property field
            Rect propertyRect = new Rect(position.x, position.y, propertyWidth, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(propertyRect, property, label, true);

            // Calculate the size of the message string
            Vector2 messageSize = GUI.skin.box.CalcSize(new GUIContent(informUserAttribute.message));

            // Check if there's enough space for the info box
            float availableSpace = position.width - propertyWidth - 4f;

            if (availableSpace >= messageSize.x)
            {
                // Draw the info box to the right of the property field
                Rect infoBoxRect = new Rect(position.x + propertyWidth + 4f, position.y, availableSpace, EditorGUIUtility.singleLineHeight);
                EditorGUI.HelpBox(infoBoxRect, informUserAttribute.message, MessageType.Info);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label);
        }
    }
#endif
}
