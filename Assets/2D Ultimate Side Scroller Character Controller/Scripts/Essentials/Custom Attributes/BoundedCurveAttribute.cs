using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UltimateCC
{
    // A custom attribute used to specify the bounds for editing an AnimationCurve.
    // bounds: xMin to xMin + xLength on x axis, yMin to yMin + yLength on y axis
    public class BoundedCurveAttribute : PropertyAttribute
    {
        public Rect bounds;
        public int height;

        public BoundedCurveAttribute(float xMin, float yMin, float xLength, float yLength, int height = 1)
        {
            this.bounds = new Rect(xMin, yMin, xLength, yLength);
            this.height = height;
        }

        public BoundedCurveAttribute(int height = 1)
        {
            this.bounds = new Rect(0, 0, 1, 1);
            this.height = height;
        }
    }
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(BoundedCurveAttribute))]
    public class BoundedCurveDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            BoundedCurveAttribute boundedCurve = (BoundedCurveAttribute)attribute;
            return EditorGUIUtility.singleLineHeight * boundedCurve.height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            BoundedCurveAttribute boundedCurve = (BoundedCurveAttribute)attribute;

            EditorGUI.BeginProperty(position, label, property);
            property.animationCurveValue = EditorGUI.CurveField(
              position,
              label,
              property.animationCurveValue,
              Color.white,
              boundedCurve.bounds
             );
            EditorGUI.EndProperty();
        }
    }
#endif
}
