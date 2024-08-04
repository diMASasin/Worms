using UnityEngine;

/// <summary>
/// Code created by Gaskellgames
/// </summary>

namespace Gaskellgames.Shared_Resources.Utilities.InspectorUtility.Attributes
{
    public class StringDropdownAttribute : PropertyAttribute
    {
        public string[] list { get; private set; }
        
        public StringDropdownAttribute(params string[] list)
        {
            this.list = list;
        }
        
    } // class end
}
