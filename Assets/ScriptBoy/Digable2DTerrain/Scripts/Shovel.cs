using UnityEngine;

namespace ScriptBoy.Digable2DTerrain.Scripts
{
    public class Shovel : Dll.D2DT.Shovel
    {
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!UnityEditor.Selection.Contains(gameObject))
            {
                Gizmos.DrawIcon(transform.position, "ScriptBoy/Digable2DTerrain/Shovel Scene Icon.png");
            }
        }
#endif
    }
}