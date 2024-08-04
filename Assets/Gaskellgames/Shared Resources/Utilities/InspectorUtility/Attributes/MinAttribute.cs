using UnityEngine;

/// <summary>
/// Code created by Gaskellgames
/// </summary>

namespace Gaskellgames.Shared_Resources.Utilities.InspectorUtility.Attributes
{
    public class MinAttribute : PropertyAttribute
    {
        public float min;

        public MinAttribute(float min)
        {
            this.min = min;
        }

    } // class end
}
