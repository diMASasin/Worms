using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UltimateCC
{
    public class CustomRangeAttribute : PropertyAttribute
    {
        public float[] Options { get; }

        public CustomRangeAttribute(params float[] options)
        {
            Options = options;
            Array.Sort(Options);
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(CustomRangeAttribute))]
    public class CustomRangeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            CustomRangeAttribute customRangeAttribute = (CustomRangeAttribute)attribute;

            EditorGUI.BeginChangeCheck();

            float currentValue = property.propertyType == SerializedPropertyType.Float ? property.floatValue : property.intValue;
            float newValue = EditorGUI.Slider(position, label, currentValue, customRangeAttribute.Options[0], customRangeAttribute.Options[customRangeAttribute.Options.Length - 1]);

            if (EditorGUI.EndChangeCheck())
            {
                int index = Array.BinarySearch(customRangeAttribute.Options, newValue);
                if (index < 0)
                {
                    index = ~index;
                    float leftValue = customRangeAttribute.Options[Mathf.Max(index - 1, 0)];
                    float rightValue = customRangeAttribute.Options[Mathf.Min(index, customRangeAttribute.Options.Length - 1)];
                    newValue = newValue - leftValue < rightValue - newValue ? leftValue : rightValue;
                }

                if (property.propertyType == SerializedPropertyType.Float)
                {
                    property.floatValue = newValue;
                }
                else if (property.propertyType == SerializedPropertyType.Integer)
                {
                    property.intValue = Mathf.RoundToInt(newValue);
                }
            }
        }
    }
#endif
}