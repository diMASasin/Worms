﻿// Stylized Water Shader by Staggart Creations http://u3d.as/A2R
// Online documentation can be found at http://staggart.xyz

using UnityEditor;
using UnityEngine;

namespace StylizedWater.Scripts
{
    [RequireComponent(typeof(Camera))]
    [ExecuteInEditMode]
    public class EnableDepthBuffer : MonoBehaviour
    {
        private Camera cam;

        private void OnValidate()
        {
            if (!cam) cam = GetComponent<Camera>();
        }

        void OnEnable()
        {
            if (!cam) cam = GetComponent<Camera>();

            if (cam) cam.depthTextureMode |= DepthTextureMode.Depth;
        }

        private void OnDestroy()
        {
            if (cam) cam.depthTextureMode = DepthTextureMode.None;
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(EnableDepthBuffer))]
    public class EnableDepthBufferInspector : Editor
    {
        override public void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("\nWhen using Forward rendering, this component will activate the depth buffer on this camera.\n\nThis is necessary for the depth and intersection effects of the water to work.\n", MessageType.Info);
        }
    }
#endif
}
