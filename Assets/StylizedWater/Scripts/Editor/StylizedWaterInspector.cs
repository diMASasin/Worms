// Stylized Water Shader by Staggart Creations http://u3d.as/A2R
// Online documentation can be found at http://staggart.xyz

using UnityEngine;
using UnityEditor.AnimatedValues;
using UnityEditor;
using System.Collections.Generic;

namespace StylizedWaterShader
{
    [CustomEditor(typeof(StylizedWater))]
    public class StylizedWaterInspector : Editor
    {
        //Non serialized, local
        StylizedWater stylizedWater;
        public static StylizedWaterInspector Instance;

        new SerializedObject serializedObject;

        //Variables
        #region Shader parameters
        private Shader shader;
        string shaderName = null;
        SerializedProperty shaderIndex;

        //Colors
        SerializedProperty waterColor;
        SerializedProperty waterShallowColor;
        SerializedProperty depth;
        SerializedProperty colorGradient;
        SerializedProperty enableGradient;
        SerializedProperty fresnelColor;
        SerializedProperty fresnel;
        SerializedProperty rimColor;
        SerializedProperty waveTint;

        //Surface
        SerializedProperty transparency;
        SerializedProperty edgeFade;
        SerializedProperty worldSpaceTiling;
        public string[] tilingMethodNames;
        SerializedProperty glossiness;
        SerializedProperty metallicness;
        SerializedProperty refractionAmount;

        //Normals
        SerializedProperty enableNormalMap;
        SerializedProperty normalStrength;
        SerializedProperty enableMacroNormals;
        SerializedProperty macroNormalsDistance;
        SerializedProperty normalTiling;

        //Intersection
        SerializedProperty intersectionSolver;
        SerializedProperty rimSize;
        SerializedProperty rimFalloff;
        SerializedProperty rimTiling;
        SerializedProperty rimDistortion;

        //Foam
        SerializedProperty foamSolver;
        SerializedProperty foamOpacity;
        SerializedProperty foamSize;
        SerializedProperty foamTiling;
        SerializedProperty foamDistortion;
        SerializedProperty foamSpeed;

        //Waves
        SerializedProperty waveSpeed;
        SerializedProperty waveStrength;
        SerializedProperty waveSize;
        SerializedProperty enableSecondaryWaves;
        SerializedProperty waveFoam;
        SerializedProperty waveDirectionXZ;
        private float waveDirectionX;
        private float waveDirectionZ;

        //Other
        //SerializedProperty tessellation;
        #endregion

        #region Rendering parameters
        SerializedProperty renderQueue;
        SerializedProperty enableVertexColors;
        SerializedProperty enableDepthTex;

        //Lighting
        SerializedProperty lightingMethod;
        SerializedProperty enableShadows;
        SerializedProperty shadowCaster;

        #endregion

        #region Texture parameters
        SerializedProperty useCompression;

        SerializedProperty useCustomIntersection;
        SerializedProperty useCustomNormals;
        SerializedProperty useCustomHeightmap;

        SerializedProperty intersectionStyle;
        SerializedProperty waveStyle;
        SerializedProperty waveHeightmapStyle;
        #endregion

        #region Local variables
        //Input textures
        SerializedProperty customIntersection;
        SerializedProperty customHeightmap;
        SerializedProperty customNormal;

        GameObject selected;
        public bool isMobileShader;
        #endregion

        #region Reflection
        //SerializedProperty refractionSolver;
        SerializedProperty reflectionRefraction;
        // SerializedProperty refractionRes;

        SerializedProperty enableReflectionBlur;
        SerializedProperty reflectionBlurLength;
        SerializedProperty reflectionBlurPasses;
        SerializedProperty useReflection;
        SerializedProperty clipPlaneOffset;

        public string[] reslist = new string[] { "64x64", "128x128", "256x256", "512x512", "1024x1024", "2048x2048" };
        SerializedProperty reflectLayers;
        SerializedProperty reflectionRes;
        SerializedProperty reflectionStrength;
        SerializedProperty reflectionFresnel;
        #endregion

        #region Meta
        //Section toggles
        private class Section
        {
            public bool Expanded
            {
                get { return SessionState.GetBool(id, false); }
                set { SessionState.SetBool(id, value); }
            }
            public bool showHelp;
            public const float animSpeed = 8f;
            public AnimBool anim;

            public readonly string id;
            public string title;

            public Section(string id, string title)
            {
                this.id = "SWS_" + id + "_SECTION";
                this.title = title;

                anim = new AnimBool(true);
                anim.valueChanged.AddListener(StylizedWaterInspector.Instance.Repaint);
                anim.speed = animSpeed;
            }

            public void SetTarget()
            {
                anim.target = Expanded;
            }
        }

        private Section colorSection;
        private Section lightingSection;
        private Section surfaceSection;
        private Section normalsSection;
        private Section reflectionSection;
        private Section intersectionSection;
        private Section foamSection;
        private Section wavesSection;
        private Section advancedSection;

        //Section anims
        AnimBool m_showHelp;

        //Help toggles
        private bool showHelp;

        private SerializedProperty isWaterLayer;
        private SerializedProperty hasShaderParams;
        private Shader currentShader;
#if !UNITY_5_5_OR_NEWER
        private SerializedProperty hideWireframe;
#endif
        private SerializedProperty hideMeshRenderer;

        private bool isReady = false;

        //Camera check
        private bool isOrtho;
        #endregion

        #region Third-party

        #endregion

        void OnEnable()
        {
            Instance = this;

            //Prevent OnInspectorGUI from running before these functions have been called
            isReady = false;

            selected = Selection.activeGameObject;

            if (!selected) return;

            if (!stylizedWater) stylizedWater = selected.GetComponent<StylizedWater>();

            serializedObject = new SerializedObject(stylizedWater);

            stylizedWater.Init();

            GetProperties();

            isWaterLayer.boolValue = (selected.gameObject.layer == 4) ? true : false;

            Undo.undoRedoPerformed += ApplyChanges;

            if (SceneView.currentDrawingSceneView != null) isOrtho = SceneView.currentDrawingSceneView.orthographic;

            #region Foldouts

            colorSection = new Section("COLOR", "Color");
            lightingSection = new Section("LIGHTING", "Lighting");
            surfaceSection = new Section("SURFACE", "Surface");
            normalsSection = new Section("NORMALS", "Normals");
            reflectionSection = new Section("REFLECTIONS", "Reflections");
            intersectionSection = new Section("INTERSECTION", "Intersection");
            foamSection = new Section("FOAM", "Foam");
            wavesSection = new Section("WAVES", "Waves");
            advancedSection = new Section("ADVANCED", "Advanced");

            m_showHelp = new AnimBool(true);
            m_showHelp.valueChanged.AddListener(Repaint);
            m_showHelp.speed = Section.animSpeed;

            #endregion

            isReady = true;
        }

        public override void OnInspectorGUI()
        {
            if (!isReady) return;

            //Prevent null values when component is added
            serializedObject.Update();
            stylizedWater.GetProperties(); //Refresh any material changes made externally
            GetProperties();

            Undo.RecordObject(target, "Stylized Water parameter change");

            /*-------------------*/
            DrawAllFields();
            /*-------------------*/
        }

        void ApplyChanges()
        {
            if (serializedObject.targetObject) serializedObject.ApplyModifiedProperties();
            stylizedWater.SetProperties();

            //Changes applied, grab new values

            GetProperties();
            stylizedWater.GetProperties();
        }

        //Get shader and Substance values
        void GetProperties()
        {
            //During runtime, nothing may be selected
            if (!selected) return;

            shader = stylizedWater.shader;
            shaderName = stylizedWater.shaderName;

            isMobileShader = stylizedWater.isMobileAdvanced;

            hasShaderParams = serializedObject.FindProperty("hasShaderParams");
            //Inspector
#if !UNITY_5_5_OR_NEWER
            hideWireframe = serializedObject.FindProperty("hideWireframe");
#endif
            hideMeshRenderer = serializedObject.FindProperty("hideMeshRenderer");

            //Rendering
            renderQueue = serializedObject.FindProperty("renderQueue");
            enableVertexColors = serializedObject.FindProperty("enableVertexColors");
            enableDepthTex = serializedObject.FindProperty("enableDepthTex");
            shadowCaster = serializedObject.FindProperty("shadowCaster");

            //Shader meta
            shaderIndex = serializedObject.FindProperty("shaderIndex");

            useCompression = serializedObject.FindProperty("useCompression");
            isWaterLayer = serializedObject.FindProperty("isWaterLayer");

            //Lighting
            lightingMethod = serializedObject.FindProperty("lightingMethod");
            enableShadows = serializedObject.FindProperty("enableShadows");

            //Color
            enableGradient = serializedObject.FindProperty("enableGradient");
            colorGradient = serializedObject.FindProperty("colorGradient");

            //Reflection
            enableReflectionBlur = serializedObject.FindProperty("enableReflectionBlur");
            reflectionBlurLength = serializedObject.FindProperty("reflectionBlurLength");
            reflectionBlurPasses = serializedObject.FindProperty("reflectionBlurPasses");

            useReflection = serializedObject.FindProperty("useReflection");
            clipPlaneOffset = serializedObject.FindProperty("clipPlaneOffset");
            reflectionRes = serializedObject.FindProperty("reflectionRes");
            reflectLayers = serializedObject.FindProperty("reflectLayers");
            reflectionStrength = serializedObject.FindProperty("reflectionStrength");
            reflectionRefraction = serializedObject.FindProperty("reflectionRefraction");
            reflectionFresnel = serializedObject.FindProperty("reflectionFresnel");

            #region Shader parameters
            //Color
            waterColor = serializedObject.FindProperty("waterColor");
            waterShallowColor = serializedObject.FindProperty("waterShallowColor");
            depth = serializedObject.FindProperty("depth");
            fresnelColor = serializedObject.FindProperty("fresnelColor");
            fresnel = serializedObject.FindProperty("fresnel");
            rimColor = serializedObject.FindProperty("rimColor");
            waveTint = serializedObject.FindProperty("waveTint");

            //Surface
            worldSpaceTiling = serializedObject.FindProperty("worldSpaceTiling");
            transparency = serializedObject.FindProperty("transparency");
            edgeFade = serializedObject.FindProperty("edgeFade");
            glossiness = serializedObject.FindProperty("glossiness");
            metallicness = serializedObject.FindProperty("metallicness");
            //refractionSolver = serializedObject.FindProperty("refractionSolver");
            refractionAmount = serializedObject.FindProperty("refractionAmount");
            //refractionRes = serializedObject.FindProperty("refractRes");

            //Normals
            enableNormalMap = serializedObject.FindProperty("enableNormalMap");
            normalStrength = serializedObject.FindProperty("normalStrength");
            enableMacroNormals = serializedObject.FindProperty("enableMacroNormals");
            macroNormalsDistance = serializedObject.FindProperty("macroNormalsDistance");
            normalTiling = serializedObject.FindProperty("normalTiling");

            //Foam
            waveFoam = serializedObject.FindProperty("waveFoam");
            foamSolver = serializedObject.FindProperty("foamSolver");
            foamOpacity = serializedObject.FindProperty("foamOpacity");
            foamSize = serializedObject.FindProperty("foamSize");
            foamTiling = serializedObject.FindProperty("foamTiling");
            foamDistortion = serializedObject.FindProperty("foamDistortion");
            foamSpeed = serializedObject.FindProperty("foamSpeed");

            //Intersection
            intersectionSolver = serializedObject.FindProperty("intersectionSolver");
            rimSize = serializedObject.FindProperty("rimSize");
            rimFalloff = serializedObject.FindProperty("rimFalloff");
            rimTiling = serializedObject.FindProperty("rimTiling");
            rimDistortion = serializedObject.FindProperty("rimDistortion");

            //Waves
            waveSpeed = serializedObject.FindProperty("waveSpeed");
            waveStrength = serializedObject.FindProperty("waveStrength");
            waveSize = serializedObject.FindProperty("waveSize");
            waveDirectionXZ = serializedObject.FindProperty("waveDirectionXZ");
            waveDirectionX = waveDirectionXZ.vector4Value.x;
            waveDirectionZ = waveDirectionXZ.vector4Value.z;
            enableSecondaryWaves = serializedObject.FindProperty("enableSecondaryWaves");
            #endregion

            #region Texture parameters
            intersectionStyle = serializedObject.FindProperty("intersectionStyle");
            waveStyle = serializedObject.FindProperty("waveStyle");
            waveHeightmapStyle = serializedObject.FindProperty("waveHeightmapStyle");

            useCustomNormals = serializedObject.FindProperty("useCustomNormals");
            useCustomHeightmap = serializedObject.FindProperty("useCustomHeightmap");
            useCustomIntersection = serializedObject.FindProperty("useCustomIntersection");

            //Substance input textures
            customIntersection = serializedObject.FindProperty("customIntersection");
            customNormal = serializedObject.FindProperty("customNormal");
            customHeightmap = serializedObject.FindProperty("customHeightmap");

            #endregion

        }

        private void BakeShaderMap()
        {
            stylizedWater.GetShaderMap(useCompression.boolValue, useCustomIntersection.boolValue, useCustomHeightmap.boolValue);
        }

        private void BakeNormalMap()
        {
            stylizedWater.GetNormalMap(useCompression.boolValue, useCustomNormals.boolValue);
        }

        //Draw inspector fields
        void DrawAllFields()
        {
            DoHeader();

            //If not a valid shader
            if (!stylizedWater.hasMaterial || !shaderName.Contains("StylizedWater"))
            {
                EditorGUILayout.HelpBox("Please assign a material with a \"StylizedWater\" shader to this object.", MessageType.Error);
                return;
            }

            DoInfo();

            DoColors();
            DoLighting();
            DoSurface();
            DoNormals();
            DoFoam();
            DoInterSection();
            DoWaves();
            //Desktop only
            if (!isMobileShader) DoReflection();
            DoAdvanced();

            DoFooter();
        }

        #region Fields
        void DoHeader()
        {
            EditorGUILayout.Space();

            if (UnityEngine.Rendering.GraphicsSettings.renderPipelineAsset)
            {
                EditorGUILayout.HelpBox("A scriptable render pipeline is in use, this is not supported", MessageType.Error);
            }
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Label("Version: " + StylizedWaterCore.INSTALLED_VERSION, new GUIStyle(EditorStyles.centeredGreyMiniLabel)
                {
                    alignment = TextAnchor.MiddleLeft,
                    wordWrap = true,
                    fontSize = 12,
                }, GUILayout.MaxWidth(95f));

                if (GUILayout.Button(EditorGUIUtility.IconContent(EditorGUIUtility.isProSkin ? "d_RotateTool" : "RotateTool").image, EditorStyles.label, GUILayout.MaxWidth(25f), GUILayout.MaxHeight(20f)))
                {
                    StylizedWaterCore.VersionChecking.GetLatestVersionPopup();
                }

                GUILayout.FlexibleSpace();
                showHelp = GUILayout.Toggle(showHelp, new GUIContent(" Toggle help", EditorGUIUtility.IconContent("console.infoicon.sml").image), HelpButton);
                m_showHelp.target = showHelp;
            }
            EditorGUILayout.EndHorizontal();


            if (EditorGUILayout.BeginFadeGroup(m_showHelp.faded))
            {
                EditorGUILayout.Space();

                EditorGUILayout.HelpBox("\n\nThis GUI allows you to customize the water material and bake textures containing an intersection, wave and heightmap style per material\n\nBaked textures are saved in a \"/Textures\" folder next to the Material file.\n\nPress the buttons on the right to toggle parameter descriptions.\n\n", MessageType.Info);

                EditorGUILayout.Space();

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("<b><size=12>Open online documentation</size></b>\n<i>Usage instructions and troubleshooting</i>", RichTextButton))
                {
                    Application.OpenURL(StylizedWaterCore.DOC_URL);
                }
                if (GUILayout.Button("<b><size=12>Check for update</size></b>\n<i>Download new version</i>", RichTextButton))
                {
                    StylizedWaterCore.VersionChecking.GetLatestVersionPopup();
                }
                EditorGUILayout.EndHorizontal();

            }
            EditorGUILayout.EndFadeGroup();

            EditorGUILayout.Space();

        }

        void DoInfo()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox); //Box
            {

                EditorGUILayout.BeginHorizontal(); //Colums
                {
                    //Icon
                    GUILayout.Label(SWSIconContent, SWSIconStyle);

                    EditorGUILayout.BeginVertical(); //Right colum
                    {
                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.PrefixLabel("Material:");
                            GUILayout.Label(stylizedWater.material.name);
                        }
                        EditorGUILayout.EndHorizontal();

                        EditorGUI.BeginChangeCheck();

                        EditorGUI.BeginDisabledGroup(stylizedWater.isMobilePlatform);
                        shaderIndex.intValue = EditorGUILayout.Popup("Shader:", (int)shaderIndex.intValue, stylizedWater.shaderNames, new GUILayoutOption[0]);
                        EditorGUI.EndDisabledGroup();

                        if (EditorGUI.EndChangeCheck())
                        {
                            ApplyChanges();
                        }

                        if (shader != currentShader)
                        {
                            //Shader changed, current values of material are unknown
                            //Also executed OnEnable
                            hasShaderParams.boolValue = false;
                            currentShader = shader;
                            GetProperties();
                        }

                    }
                    EditorGUILayout.EndVertical(); //Right clum
                }
                EditorGUILayout.EndHorizontal(); //Colums
            }
            EditorGUILayout.EndVertical(); //Box

            if (stylizedWater.isMobilePlatform && shaderName.Contains("Desktop"))
            {
                EditorGUILayout.HelpBox("You are using a desktop shader on a mobile platform, which is not supported.\n\nThis will be automatically corrected.", MessageType.Error);
            }

            if (stylizedWater.currentCam)
            {
                if (stylizedWater.currentCam.name != "SceneCamera"  && !stylizedWater.currentCam.name.Contains("Reflection") && stylizedWater.currentCam.depthTextureMode != DepthTextureMode.Depth && stylizedWater.currentCam.actualRenderingPath != RenderingPath.DeferredLighting && enableDepthTex.boolValue)
                {
                    EditorGUILayout.HelpBox("\"" + stylizedWater.currentCam.name + "\" is not rendering a depth texture.\n\nIntersection and depth effects will not be visible.", MessageType.Warning);

                    GUILayout.Space(-32);
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button("Fix", GUILayout.Width(60)))
                        {
                            stylizedWater.currentCam.gameObject.AddComponent<EnableDepthBuffer>();
                            Debug.Log("Added the <i>EnableDepthBuffer</i> component to <b>" + stylizedWater.currentCam.name + "</b>", stylizedWater.currentCam);
                        }
                        GUILayout.Space(8);
                    }
                    GUILayout.Space(11);
                }
            }

            if (isOrtho)
            {
                EditorGUILayout.HelpBox("When the scene view is set to orthographic, the water may not appear correct.\n\nIn the game view this is not the case", MessageType.Warning);
            }

            EditorGUILayout.Space();

        }

        void DoColors()
        {
            //Head
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button((colorSection.Expanded) ? charFoldout : charCollapsed, CollapseButton))
            {
                SwitchSection(colorSection);
            }
            if (GUILayout.Button("Colors", GroupFoldout))
            {
                SwitchSection(colorSection);
            }
            colorSection.showHelp = GUILayout.Toggle((colorSection.Expanded) ? colorSection.showHelp : false, HelpIcon, SectionHelpButtonStyle);
            colorSection.anim.target = colorSection.Expanded;
            EditorGUILayout.EndHorizontal();

            //Body
            if (EditorGUILayout.BeginFadeGroup(colorSection.anim.faded))
            {
                EditorGUILayout.Space();

                EditorGUI.BeginChangeCheck();

                //Desktop only
                if (!isMobileShader)
                {
                    EditorGUILayout.PropertyField(enableGradient, new GUIContent("Use gradient"));
                    if (colorSection.showHelp) EditorGUILayout.HelpBox("Samples the water and shallow water colors from a gradient.\n\nAlpha controls transparency.", MessageType.Info);

                    EditorGUILayout.Space();

                    EditorGUILayout.BeginHorizontal();
                    if (enableGradient.boolValue)
                    {
                        EditorGUILayout.PropertyField(colorGradient, new GUIContent("Color"));

                        if (GUILayout.Button("Apply"))
                        {
                            stylizedWater.GetGradientTex();
                        }
                    }
                    EditorGUILayout.EndHorizontal();

                }
                else
                {
                    enableGradient.boolValue = false;
                }

                if (enableGradient.boolValue == false)
                {
                    EditorGUILayout.PropertyField(waterColor, new GUIContent("Deep"));

                    EditorGUILayout.PropertyField(waterShallowColor, new GUIContent("Shallow"));
                    if (colorSection.showHelp) EditorGUILayout.HelpBox("The color where the water is shallow, alpha channel controls the opacity.\n\nThe Depth parameter has an influence on this effect.", MessageType.Info);
                }

                EditorGUILayout.PropertyField(depth, new GUIContent("Depth"));
                if (colorSection.showHelp) EditorGUILayout.HelpBox("Sets the depth of the water.", MessageType.Info);

                //Desktop only
                if (!isMobileShader)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.PropertyField(fresnelColor, new GUIContent("Horizon"));
                    if (colorSection.showHelp) EditorGUILayout.HelpBox("The color's alpha channel controls the intensity", MessageType.Info);
                    EditorGUILayout.PropertyField(fresnel, new GUIContent("Horizon distance"));
                    if (colorSection.showHelp) EditorGUILayout.HelpBox("Controls the distance of the fresnel effect. Higher values push it further towards the horizon.", MessageType.Info);
                }

                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(rimColor, new GUIContent("Intersection"));
                if (colorSection.showHelp) EditorGUILayout.HelpBox("The color's alpha channel controls the intensity", MessageType.Info);

                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(waveTint, new GUIContent("Wave tint"));
                if (colorSection.showHelp) EditorGUILayout.HelpBox("Brightens or darkerns the water, based on the chosen heightmap in the Waves options section.", MessageType.Info);

                if (EditorGUI.EndChangeCheck())
                {
                    ApplyChanges();
                }
            }
            EditorGUILayout.EndFadeGroup();

            EditorGUILayout.Space();
        }

        void DoLighting()
        {
            //Head
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button((lightingSection.Expanded) ? charFoldout : charCollapsed, CollapseButton))
            {
                SwitchSection(lightingSection);
            }
            if (GUILayout.Button("Lighting", GroupFoldout))
            {
                SwitchSection(lightingSection);
            }
            lightingSection.showHelp = GUILayout.Toggle((lightingSection.Expanded) ? lightingSection.showHelp : false, HelpIcon, SectionHelpButtonStyle);
            lightingSection.anim.target = lightingSection.Expanded;
            EditorGUILayout.EndHorizontal();

            //Body
            if (EditorGUILayout.BeginFadeGroup(lightingSection.anim.faded))
            {
                EditorGUILayout.Space();

                EditorGUI.BeginChangeCheck();

                lightingMethod.intValue = EditorGUILayout.Popup("Method", (int)lightingMethod.intValue, stylizedWater.lightingMethodNames, new GUILayoutOption[0]);

                switch (lightingMethod.intValue)
                {
                    case 0:
                        EditorGUILayout.HelpBox("Material receives no light, purely shows the water's color", MessageType.None);
                        break;
                    case 1:
                        EditorGUILayout.HelpBox("Material receives color from Directional Light and Ambient Color", MessageType.None);
                        break;
                    case 2:
                        {
                            if (isMobileShader)
                            {
                                EditorGUILayout.HelpBox("Material uses all Unity lighting features (point/spot lights excluded)", MessageType.None);
                            }
                            else
                            {
                                EditorGUILayout.HelpBox("Material uses all Unity lighting features and up to 3 points light (spot lights excluded)", MessageType.None);
                            }
                        }
                        break;
                }
                if (lightingSection.showHelp) EditorGUILayout.HelpBox("Advanced lighting enables lighting features such as GI, glossiness and skybox reflection. Turning this off can be a huge performance saver on low-end devices\n\nSimple lighting uses a custom lighting model with ambient light color and sun reflection.", MessageType.Info);

                if (!isMobileShader)
                {

                    EditorGUILayout.PropertyField(enableShadows, new GUIContent("Shadow transmission"));
                    if (lightingSection.showHelp) EditorGUILayout.HelpBox("Shows the shadows cast beneath the water surface, regardless of transparency.", MessageType.Info);


                    if (enableShadows.boolValue)
                    {
                        EditorGUILayout.PropertyField(shadowCaster, new GUIContent("Directional Light"));

                        if (shadowCaster.objectReferenceValue == null)
                        {
                            EditorGUILayout.HelpBox("This field should not be empty, water will not render correctly otherwise", MessageType.Warning);
                        }
                    }

                }

                EditorGUILayout.Space();

                if (EditorGUI.EndChangeCheck())
                {
                    ApplyChanges();
                }
            }
            EditorGUILayout.EndFadeGroup();

            EditorGUILayout.Space();
        }

        void DoSurface()
        {
            //Head
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button((surfaceSection.Expanded) ? charFoldout : charCollapsed, CollapseButton))
            {
                SwitchSection(surfaceSection);
            }
            if (GUILayout.Button(surfaceSection.title, GroupFoldout))
            {
                SwitchSection(surfaceSection);
            }
            surfaceSection.showHelp = GUILayout.Toggle((surfaceSection.Expanded) ? surfaceSection.showHelp : false, HelpIcon, SectionHelpButtonStyle);
            surfaceSection.SetTarget();
            EditorGUILayout.EndHorizontal();

            //Body
            if (EditorGUILayout.BeginFadeGroup(surfaceSection.anim.faded))
            {
                EditorGUILayout.Space();

                EditorGUI.BeginChangeCheck();

                worldSpaceTiling.floatValue = EditorGUILayout.Popup("Tiling method", (int)worldSpaceTiling.floatValue, stylizedWater.tilingMethodNames, new GUILayoutOption[0]);

                //EditorGUILayout.PropertyField(worldSpaceTiling, new GUIContent("World-space tiling"));
                if (surfaceSection.showHelp) EditorGUILayout.HelpBox("Rather than using the mesh's UV, you can opt to use world-space coordinates. Which are independent from the object's scale and position.", MessageType.Info);

                EditorGUILayout.PropertyField(transparency, new GUIContent("Transparency"));
                if (surfaceSection.showHelp) EditorGUILayout.HelpBox("Opacity is also read from the Green vertex color channel, and thus can be painted using third-party tools.", MessageType.Info);

                EditorGUILayout.PropertyField(glossiness, new GUIContent("Glossiness"));
                if (surfaceSection.showHelp) EditorGUILayout.HelpBox("Determine how glossy the material is. Higher values show more light reflection.", MessageType.Info);

                if (lightingMethod.intValue == 2)
                {
                    EditorGUILayout.PropertyField(metallicness, new GUIContent("Metallicness"));
                    if (surfaceSection.showHelp) EditorGUILayout.HelpBox("Determine how conductive the surface is. Slight metalic values can enhance the reflective nature of the water", MessageType.Info);
                }


                EditorGUILayout.PropertyField(edgeFade, new GUIContent("Edge fade"));
                if (surfaceSection.showHelp) EditorGUILayout.HelpBox("Adds an inward offset to the effect from the intersecting object.", MessageType.Info);

                //Desktop only
                if (!isMobileShader)
                {
                    /*
                    EditorGUILayout.Space();

                    EditorGUILayout.LabelField("Refraction", EditorStyles.boldLabel);
                    refractionSolver.intValue = EditorGUILayout.Popup("Configuration", (int)refractionSolver.intValue, stylizedWater.refractionSolverNames, new GUILayoutOption[0]);
                    
                    if(refractionSolver.intValue == 1)
                    {
                        refractionRes.intValue = EditorGUILayout.Popup("Resolution", refractionRes.intValue, stylizedWater.resolutionNames, new GUILayoutOption[0]);
                    }
                    */
                    EditorGUILayout.PropertyField(refractionAmount, new GUIContent("Refraction"));
                    if (surfaceSection.showHelp) EditorGUILayout.HelpBox("Bends the light passing through the water surface, creating a visual distortion.", MessageType.Info);
                }

                if (EditorGUI.EndChangeCheck())
                {
                    ApplyChanges();
                }
            }
            EditorGUILayout.EndFadeGroup();
            EditorGUILayout.Space();
        }

        void DoNormals()
        {
            //Head
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button((normalsSection.Expanded) ? charFoldout : charCollapsed, CollapseButton))
            {
                SwitchSection(normalsSection);
            }
            if (GUILayout.Button(normalsSection.title, GroupFoldout))
            {
                SwitchSection(normalsSection);
            }
            normalsSection.showHelp = GUILayout.Toggle((normalsSection.Expanded) ? normalsSection.showHelp : false, HelpIcon, SectionHelpButtonStyle);
            normalsSection.SetTarget();
            EditorGUILayout.EndHorizontal();

            //Body
            if (EditorGUILayout.BeginFadeGroup(normalsSection.anim.faded))
            {
                EditorGUILayout.Space();

                EditorGUI.BeginChangeCheck();

                if (isMobileShader)
                {
                    EditorGUILayout.PropertyField(enableNormalMap, new GUIContent("Enabled"));
                    if (normalsSection.showHelp) EditorGUILayout.HelpBox("Disabling the normals sees a performance increase, but at the cost of the sun reflection", MessageType.Info);

                    EditorGUILayout.Space();
                }

                EditorGUI.BeginDisabledGroup(!enableNormalMap.boolValue);
                {
                    EditorGUILayout.BeginHorizontal();

                    waveStyle.intValue = EditorGUILayout.Popup("Style", (int)waveStyle.intValue, stylizedWater.waveStyleNames, new GUILayoutOption[0]);

                    EditorGUI.BeginDisabledGroup(useCustomNormals.boolValue && customNormal.objectReferenceValue == null);
                    if (GUILayout.Button("Apply", EditorStyles.miniButton))
                    {
                        BakeShaderMap();
                        BakeNormalMap();
                    }
                    EditorGUI.EndDisabledGroup();

                    EditorGUILayout.EndHorizontal();
                    if (normalsSection.showHelp) EditorGUILayout.HelpBox("Choose from one of the built-in normal maps, or pick a custom one.", MessageType.Info);

                    useCustomNormals.boolValue = (waveStyle.intValue == stylizedWater.waveStyleNames.Length - 1) ? true : false;

                    if (useCustomNormals.boolValue)
                    {
                        EditorGUILayout.PropertyField(customNormal, new GUIContent("Normal map"));

                        if (customNormal.objectReferenceValue == null)
                        {
                            EditorGUILayout.HelpBox("Field cannot be empty", MessageType.Warning);
                        }
                    }

                    EditorGUILayout.Space();

                    EditorGUILayout.PropertyField(normalStrength, new GUIContent("Strength"));
                    if (normalsSection.showHelp) EditorGUILayout.HelpBox("The normal are also used for the reflection highlight, so the normal strength also has an effect on it.", MessageType.Info);

                    if (!isMobileShader)
                    {
                        EditorGUILayout.PropertyField(enableMacroNormals, new GUIContent("Tiling reduction"));
                        if (normalsSection.showHelp) EditorGUILayout.HelpBox("Blend in a additional normal map, 10x the tiling size at a far away distance.", MessageType.Info);

                        if (enableMacroNormals.boolValue)
                            EditorGUILayout.PropertyField(macroNormalsDistance, new GUIContent("Blend distance"));
                    }

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Tiling");
                    if (GUILayout.Button("<<", EditorStyles.miniButtonLeft))
                    {
                        normalTiling.floatValue -= .1f;
                    }
                    if (GUILayout.Button("<", EditorStyles.miniButtonMid))
                    {
                        normalTiling.floatValue -= .05f;
                    }
                    EditorGUILayout.PropertyField(normalTiling, new GUIContent(""), GUILayout.MaxWidth(45));
                    if (GUILayout.Button(">", EditorStyles.miniButtonMid))
                    {
                        normalTiling.floatValue += .05f;
                    }
                    if (GUILayout.Button(">>", EditorStyles.miniButtonRight))
                    {
                        normalTiling.floatValue += .1f;
                    }
                    EditorGUILayout.EndHorizontal();
                    if (normalTiling.floatValue == 0)
                    {
                        EditorGUILayout.HelpBox("Tiling value should not be 0", MessageType.Warning);
                    }
                    if (normalsSection.showHelp) EditorGUILayout.HelpBox("Sets the size of the normals at which it repeats.", MessageType.Info);

                    if (EditorGUI.EndChangeCheck())
                    {
                        ApplyChanges();
                    }
                }
                EditorGUI.EndDisabledGroup();
            }
            EditorGUILayout.EndFadeGroup();
            EditorGUILayout.Space();
        }

        void DoFoam()
        {
            //Head
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button((foamSection.Expanded) ? charFoldout : charCollapsed, CollapseButton))
            {
                SwitchSection(foamSection);
            }
            if (GUILayout.Button(foamSection.title, GroupFoldout))
            {
                SwitchSection(foamSection);
            }
            foamSection.showHelp = GUILayout.Toggle((foamSection.Expanded) ? foamSection.showHelp : false, HelpIcon, SectionHelpButtonStyle);
            foamSection.SetTarget();
            EditorGUILayout.EndHorizontal();

            //Body
            if (EditorGUILayout.BeginFadeGroup(foamSection.anim.faded))
            {
                EditorGUILayout.Space();

                EditorGUI.BeginChangeCheck();

                EditorGUI.BeginDisabledGroup(intersectionStyle.intValue == 0);

                foamSolver.intValue = EditorGUILayout.Popup("Source", (int)foamSolver.intValue, stylizedWater.foamSolverNames, new GUILayoutOption[0]);

                if (foamSection.showHelp) EditorGUILayout.HelpBox("Instead of sampling the highlight from the wave normal map, use the intersection texture.", MessageType.Info);
                EditorGUI.EndDisabledGroup();
                if (intersectionStyle.intValue == 0)
                {
                    EditorGUILayout.HelpBox("Intersection style is set to \"None\", using normal map.", MessageType.Info);
                    foamSolver.intValue = 0;
                }
                if (useCustomNormals.boolValue)
                {
                    EditorGUILayout.HelpBox("When using a custom normal map, the intersection texture must be used", MessageType.Info);
                    foamSolver.intValue = 1;
                }

                EditorGUILayout.PropertyField(foamOpacity, new GUIContent("Opacity"));
                if (foamSection.showHelp) EditorGUILayout.HelpBox("Controls the intensity of the effect.", MessageType.Info);


                if (foamSolver.intValue == 0)
                {
                    EditorGUILayout.PropertyField(foamSize, new GUIContent("Size"));
                }

                EditorGUILayout.PropertyField(waveFoam, new GUIContent("Wave mask"));
                if (foamSection.showHelp) EditorGUILayout.HelpBox("Shows the foam based on the heightmap", MessageType.Info);

                //Desktop only
                if (!isMobileShader)
                {
                    EditorGUILayout.PropertyField(foamDistortion, new GUIContent("Distortion"));
                    if (foamSection.showHelp) EditorGUILayout.HelpBox("Distorts the foam based on the heightmap", MessageType.Info);
                }

                EditorGUILayout.PropertyField(foamSpeed, new GUIContent("Speed"));
                if (foamSection.showHelp) EditorGUILayout.HelpBox("Move the foam in the wave direction", MessageType.Info);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Tiling");
                if (GUILayout.Button("<<", EditorStyles.miniButtonLeft))
                {
                    foamTiling.floatValue -= .1f;
                }
                if (GUILayout.Button("<", EditorStyles.miniButtonMid))
                {
                    foamTiling.floatValue -= .01f;
                }
                EditorGUILayout.PropertyField(foamTiling, new GUIContent(""), GUILayout.MaxWidth(45));
                if (GUILayout.Button(">", EditorStyles.miniButtonMid))
                {
                    foamTiling.floatValue += .01f;
                }
                if (GUILayout.Button(">>", EditorStyles.miniButtonRight))
                {
                    foamTiling.floatValue += .1f;
                }
                EditorGUILayout.EndHorizontal();
                if (foamTiling.floatValue == 0)
                {
                    EditorGUILayout.HelpBox("Tiling value should not be 0", MessageType.Warning);
                }
                if (foamSection.showHelp) EditorGUILayout.HelpBox("Controls the tiling size of the foam texture.", MessageType.Info);

                if (EditorGUI.EndChangeCheck())
                {
                    ApplyChanges();
                }
            }

            EditorGUILayout.EndFadeGroup();
            EditorGUILayout.Space();
        }

        void DoInterSection()
        {
            //Head
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button((intersectionSection.Expanded) ? charFoldout : charCollapsed, CollapseButton))
            {
                SwitchSection(intersectionSection);
            }
            if (GUILayout.Button(intersectionSection.title, GroupFoldout))
            {
                SwitchSection(intersectionSection);
            }
            intersectionSection.showHelp = GUILayout.Toggle((intersectionSection.Expanded) ? intersectionSection.showHelp : false, HelpIcon, SectionHelpButtonStyle);
            intersectionSection.SetTarget();
            EditorGUILayout.EndHorizontal();

            //Body
            if (EditorGUILayout.BeginFadeGroup(intersectionSection.anim.faded))
            {
                EditorGUILayout.Space();

                EditorGUI.BeginChangeCheck();

                //if (intersectionSection.showHelp) EditorGUILayout.HelpBox("If the intersection effect doesn't appear, be sure to add the \"EnableDepthBuffer\" script to your camera.", MessageType.Warning);

                EditorGUILayout.BeginHorizontal();
                var m_intersectionStyle = intersectionStyle.intValue;

                intersectionStyle.intValue = EditorGUILayout.Popup("Style", (int)intersectionStyle.intValue, stylizedWater.intersectionStyleNames, new GUILayoutOption[0]);

                EditorGUI.BeginDisabledGroup(useCustomIntersection.boolValue && customIntersection.objectReferenceValue == null);
                if (GUILayout.Button("Apply", EditorStyles.miniButton))
                {
                    BakeShaderMap();
                }
                EditorGUI.EndDisabledGroup();

                EditorGUILayout.EndHorizontal();
                if (intersectionSection.showHelp) EditorGUILayout.HelpBox("Pick one of the built-in intersection texture styles.", MessageType.Info);

                useCustomIntersection.boolValue = (intersectionStyle.intValue == stylizedWater.intersectionStyleNames.Length - 1) ? true : false;

                if (useCustomIntersection.boolValue)
                {
                    EditorGUILayout.PropertyField(customIntersection, new GUIContent("Grayscale texture"));
                    if (customIntersection.objectReferenceValue == null)
                    {
                        EditorGUILayout.HelpBox("Texture field may not be empty", MessageType.Warning);
                    }
                    if (intersectionSection.showHelp) EditorGUILayout.HelpBox("Choose a black/white texture, it will automatically be packed into the \"shadermap\" texture channel.", MessageType.Info);

                }

                EditorGUILayout.Space();

                intersectionSolver.intValue = EditorGUILayout.Popup("Solver", intersectionSolver.intValue, stylizedWater.intersectionSolverNames, new GUILayoutOption[0]);
                if (intersectionSection.showHelp) EditorGUILayout.HelpBox("Using depth differences or sample the mesh's red vertex color channel.\n\nThis can be applied using third-party vertex painting tools.", MessageType.Info);

                if (intersectionSolver.intValue == 1 && enableVertexColors.boolValue == false)
                {
                    EditorGUILayout.HelpBox("The \"Enable vertex colors\" option must be enabled in the Advanced tab", MessageType.Warning);
                }
                if (isMobileShader && intersectionSolver.intValue == 0 && enableDepthTex.boolValue == false)
                {
                    EditorGUILayout.HelpBox("Depth texture option is disabled under Advanced tab", MessageType.Error);
                }

                //EditorGUILayout.PropertyField(rimSize, new GUIContent("Size"));
                rimSize.floatValue = EditorGUILayout.Slider("Size", rimSize.floatValue, 0f, intersectionSolver.intValue == 1 ? 1 : 30);
                if (intersectionSection.showHelp) EditorGUILayout.HelpBox("Increase the size of the intersection effect.", MessageType.Info);

                EditorGUILayout.PropertyField(rimFalloff, new GUIContent("Falloff"));
                if (intersectionSection.showHelp) EditorGUILayout.HelpBox("Controls how strongly the effect should taper off.", MessageType.Info);

                //Desktop
                if (!isMobileShader)
                {
                    EditorGUILayout.PropertyField(rimDistortion, new GUIContent("Distortion"));
                    if (intersectionSection.showHelp) EditorGUILayout.HelpBox("Distorts the secondary intersection layer by the heightmap", MessageType.Info);
                }

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Tiling");
                if (GUILayout.Button("<<", EditorStyles.miniButtonLeft))
                {
                    rimTiling.floatValue -= .1f;
                }
                if (GUILayout.Button("<", EditorStyles.miniButtonMid))
                {
                    rimTiling.floatValue -= .01f;
                }
                EditorGUILayout.PropertyField(rimTiling, new GUIContent(""), GUILayout.MaxWidth(45));
                if (GUILayout.Button(">", EditorStyles.miniButtonMid))
                {
                    rimTiling.floatValue += .01f;
                }
                if (GUILayout.Button(">>", EditorStyles.miniButtonRight))
                {
                    rimTiling.floatValue += .1f;
                }
                EditorGUILayout.EndHorizontal();
                if (rimTiling.floatValue == 0)
                {
                    EditorGUILayout.HelpBox("Tiling value should not be 0", MessageType.Warning);
                }
                if (intersectionSection.showHelp) EditorGUILayout.HelpBox("Controls the tiling size of the intersection texture.\n\nThis is also affected by the \"Tiling\" value in the Surface options section.", MessageType.Info);

                if (EditorGUI.EndChangeCheck())
                {
                    ApplyChanges();
                }
            }

            EditorGUILayout.EndFadeGroup();
            EditorGUILayout.Space();
        }

        void DoReflection()
        {
            //Head
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button((reflectionSection.Expanded) ? charFoldout : charCollapsed, CollapseButton))
            {
                SwitchSection(reflectionSection);
            }
            if (GUILayout.Button("Planar reflections", GroupFoldout))
            {
                SwitchSection(reflectionSection);
            }
            reflectionSection.showHelp = GUILayout.Toggle((reflectionSection.Expanded) ? reflectionSection.showHelp : false, HelpIcon, SectionHelpButtonStyle);
            reflectionSection.SetTarget();
            EditorGUILayout.EndHorizontal();

            //Body
            if (EditorGUILayout.BeginFadeGroup(reflectionSection.anim.faded))
            {
                EditorGUILayout.Space();

                EditorGUI.BeginChangeCheck();

                if (stylizedWater.usingSinglePassRendering)
                {
                    EditorGUILayout.HelpBox("Reflections are not supported in Single-Pass Stereo Rendering", MessageType.Error);
                    EditorGUILayout.Space();
                }
                else
                {
                    if (reflectionSection.showHelp) EditorGUILayout.HelpBox("This feature creates a secondary camera, to render a reflection.\n\nThis can have a huge performance impact, be sure to make use of the layer culling option.", MessageType.Warning);

                    useReflection.boolValue = EditorGUILayout.Toggle("Enable", useReflection.boolValue);

                    if (useReflection.boolValue)
                    {
                        EditorGUILayout.Space();

                        EditorGUILayout.PropertyField(reflectLayers);
                        if (reflectionSection.showHelp) EditorGUILayout.HelpBox("Choose which layers should be reflected. The \"Water\" layer is always ignored.", MessageType.Info);

                        reflectionRes.intValue = EditorGUILayout.Popup("Resolution", reflectionRes.intValue, stylizedWater.resolutionNames, new GUILayoutOption[0]);

                        EditorGUILayout.PropertyField(reflectionStrength, new GUIContent("Strength"));
                        if (reflectionSection.showHelp) EditorGUILayout.HelpBox("The amount of reflection shown", MessageType.Info);
                        EditorGUILayout.PropertyField(reflectionRefraction, new GUIContent("Distortion"));
                        if (reflectionSection.showHelp) EditorGUILayout.HelpBox("Bends the reflected image through the waves", MessageType.Info);
                        EditorGUILayout.PropertyField(reflectionFresnel, new GUIContent("Fresnel"));
                        if (reflectionSection.showHelp) EditorGUILayout.HelpBox("Determine how much reflection should be shown at glazing angles", MessageType.Info);
                        EditorGUILayout.PropertyField(clipPlaneOffset, new GUIContent("Offset"));
                        if (reflectionSection.showHelp) EditorGUILayout.HelpBox("Normally the reflection is clipped at the water plane, but you can add an offset in case a seam shows", MessageType.Info);

                        EditorGUILayout.Space();

                        EditorGUILayout.PropertyField(enableReflectionBlur);
                        if (reflectionSection.showHelp) EditorGUILayout.HelpBox("Blur the reflection in a vertical manner. It is recommended to lower the reflection resolution for the best result.", MessageType.Info);

                        if (enableReflectionBlur.boolValue)
                        {
                            EditorGUILayout.PropertyField(reflectionBlurLength, new GUIContent("Length"));
                            EditorGUILayout.PropertyField(reflectionBlurPasses, new GUIContent("Iterations"));
                            if (reflectionSection.showHelp) EditorGUILayout.HelpBox("The amount of time the blurred image is blurred again. A higher iteration count results in a better image, but at a higher rendering cost.", MessageType.Info);
                        }


                    }
                }//VR

                if (EditorGUI.EndChangeCheck())
                {
                    ApplyChanges();
                }
            }

            EditorGUILayout.EndFadeGroup();
            EditorGUILayout.Space();
        }

        void DoWaves()
        {
            //Head
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button((wavesSection.Expanded) ? charFoldout : charCollapsed, CollapseButton))
            {
                SwitchSection(wavesSection);
            }
            if (GUILayout.Button(wavesSection.title, GroupFoldout))
            {
                SwitchSection(wavesSection);
            }
            wavesSection.showHelp = GUILayout.Toggle((wavesSection.Expanded) ? wavesSection.showHelp : false, HelpIcon, SectionHelpButtonStyle);
            wavesSection.SetTarget();

            EditorGUILayout.EndHorizontal();

            //Body
            if (EditorGUILayout.BeginFadeGroup(wavesSection.anim.faded))
            {
                EditorGUI.BeginChangeCheck();

                EditorGUILayout.Space();

                EditorGUILayout.BeginHorizontal();

                waveHeightmapStyle.intValue = EditorGUILayout.Popup("Heightmap style", (int)waveHeightmapStyle.intValue, stylizedWater.waveHeightmapNames, new GUILayoutOption[0]);

                EditorGUI.BeginDisabledGroup(useCustomHeightmap.boolValue && customHeightmap.objectReferenceValue == null);
                if (GUILayout.Button("Apply", EditorStyles.miniButton))
                {
                    BakeShaderMap();
                }
                EditorGUI.EndDisabledGroup();

                EditorGUILayout.EndHorizontal();
                if (wavesSection.showHelp) EditorGUILayout.HelpBox("Choose from one of the built-in height maps.", MessageType.Info);

                useCustomHeightmap.boolValue = (waveHeightmapStyle.intValue == stylizedWater.waveHeightmapNames.Length - 1) ? true : false;

                if (useCustomHeightmap.boolValue)
                {
                    EditorGUILayout.PropertyField(customHeightmap, new GUIContent("Heightmap"));
                    if (customHeightmap.objectReferenceValue == null)
                    {
                        EditorGUILayout.HelpBox("Texture field may not be empty", MessageType.Warning);
                    }
                }

                //Desktop only
                if (!isMobileShader)
                {
                    EditorGUILayout.PropertyField(enableSecondaryWaves, new GUIContent("Additional layer"));
                }

                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(waveSpeed, new GUIContent("Speed"));
                if (wavesSection.showHelp) EditorGUILayout.HelpBox("Overal speed of the wave animations.", MessageType.Info);


                waveDirectionX = EditorGUILayout.Slider("Direction X", waveDirectionX, -1f, 1f);
                waveDirectionZ = EditorGUILayout.Slider("Direction Z", waveDirectionZ, -1f, 1f);

                waveDirectionXZ.vector4Value = new Vector4(waveDirectionX, 0f, waveDirectionZ, 0f);

                EditorGUILayout.PropertyField(waveStrength, new GUIContent("Height"));
                if (wavesSection.showHelp) EditorGUILayout.HelpBox("height of the waves, this can also be controlled through the Y-axis scale of the object, at least for planes.", MessageType.Info);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Tiling");
                if (GUILayout.Button("<<", EditorStyles.miniButtonLeft))
                {
                    waveSize.floatValue -= .1f;
                }
                if (GUILayout.Button("<", EditorStyles.miniButtonMid))
                {
                    waveSize.floatValue -= .01f;
                }
                EditorGUILayout.PropertyField(waveSize, new GUIContent(""), GUILayout.MaxWidth(45));
                if (GUILayout.Button(">", EditorStyles.miniButtonMid))
                {
                    waveSize.floatValue += .01f;
                }
                if (GUILayout.Button(">>", EditorStyles.miniButtonRight))
                {
                    waveSize.floatValue += .1f;
                }
                EditorGUILayout.EndHorizontal();
                if (waveSize.floatValue == 0)
                {
                    EditorGUILayout.HelpBox("Tiling value should not be 0", MessageType.Warning);
                }
                if (wavesSection.showHelp) EditorGUILayout.HelpBox("Controls the tiling size of the wave animation.\n\nThis is also affected by the \"Tiling\" value in the Surface options section.", MessageType.Info);

                if (EditorGUI.EndChangeCheck())
                {
                    ApplyChanges();
                }
            }

            EditorGUILayout.EndFadeGroup();
            EditorGUILayout.Space();
        }

        void DoAdvanced()
        {
            //Head
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button((advancedSection.Expanded) ? charFoldout : charCollapsed, CollapseButton))
            {
                SwitchSection(advancedSection);
            }
            if (GUILayout.Button("Advanced", GroupFoldout))
            {
                SwitchSection(advancedSection);
            }
            advancedSection.showHelp = GUILayout.Toggle((advancedSection.Expanded) ? advancedSection.showHelp : false, HelpIcon, SectionHelpButtonStyle);
            advancedSection.SetTarget();
            EditorGUILayout.EndHorizontal();

            //Body
            if (EditorGUILayout.BeginFadeGroup(advancedSection.anim.faded))
            {
                EditorGUI.BeginChangeCheck();

                EditorGUILayout.Space();

                EditorGUILayout.LabelField("Rendering", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(renderQueue, new GUIContent("Render queue"));

                if (isMobileShader)
                {
                    EditorGUILayout.PropertyField(enableDepthTex, new GUIContent("Enable Depth Texture"));
                    if (advancedSection.showHelp) EditorGUILayout.HelpBox("On mobile, if you intentionally don't have the \"EnableDepthBuffer\" script attached to your camera, this should also be disable. Otherwise the water turns invisible when built on the device", MessageType.Info);
                }

                EditorGUILayout.Space();

                EditorGUILayout.LabelField("Masking", EditorStyles.boldLabel);

                EditorGUILayout.PropertyField(enableVertexColors, new GUIContent("Enable vertex colors"));
                if (advancedSection.showHelp) EditorGUILayout.HelpBox("When enabled, vertex colors will be sampled for masking:\n\nRed: Intersection\nGreen: Opacity\nBlue: Wave flatterning", MessageType.Info);

                EditorGUILayout.Space();

                EditorGUILayout.LabelField("Hide", EditorStyles.boldLabel);
#if !UNITY_5_5_OR_NEWER
                EditorGUILayout.PropertyField(hideWireframe, new GUIContent("Wireframe"));
                if (advancedSection.showHelp) EditorGUILayout.HelpBox("Hides the wireframe of the selected object, for easier visual tuning.", MessageType.Info);
#endif
                EditorGUILayout.PropertyField(hideMeshRenderer, new GUIContent("Mesh Renderer"));
                if (advancedSection.showHelp) EditorGUILayout.HelpBox("Hides mesh inspector, for less clutter", MessageType.Info);

                EditorGUILayout.Space();

                EditorGUILayout.LabelField("Textures", EditorStyles.boldLabel);
                var m_useCompression = useCompression.boolValue;
                EditorGUILayout.PropertyField(useCompression, new GUIContent("Compress"));
                if (advancedSection.showHelp) EditorGUILayout.HelpBox("Bake the textures using compression, trading in quality for a smaller file size.\n\nPVRTC compression is used for mobile, DXT5 is used on other platforms.", MessageType.Info);

#if VEGETATION_STUDIO || VEGETATION_STUDIO_PRO
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Vegetation Studio", EditorStyles.boldLabel);

                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.PrefixLabel(new GUIContent("Sea level"));
                    if (GUILayout.Button("Set"))
                    {
                        stylizedWater.SetVegetationStudioWaterLevel();
                    }
                }
                EditorGUILayout.EndHorizontal();
                if (advancedSection.showHelp) EditorGUILayout.HelpBox("Use this object's Y-position to set the global water level for all Vegetation Systems", MessageType.Info);
#endif

                if (m_useCompression != useCompression.boolValue)
                {
                    BakeNormalMap();
                    BakeShaderMap();
                }

                EditorGUILayout.Space();

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Apply all settings", GUILayout.MaxHeight(35)))
                {
                    BakeShaderMap();
                    BakeNormalMap();
                }
                EditorGUILayout.EndHorizontal();

                if (EditorGUI.EndChangeCheck())
                {
                    ApplyChanges();
                }
            }

            EditorGUILayout.EndFadeGroup();
        }

        void DoFooter()
        {
            EditorGUILayout.Space();

            GUILayout.Label("- Staggart Creations -", new GUIStyle(EditorStyles.centeredGreyMiniLabel)
            {
                alignment = TextAnchor.MiddleCenter,
                wordWrap = true,
                fontSize = 12
            });

            EditorGUILayout.Space();

        }
        #endregion

        private void SwitchSection(Section s)
        {
            lightingSection.Expanded = (s == lightingSection) ? !lightingSection.Expanded : false;
            colorSection.Expanded = (s == colorSection) ? !colorSection.Expanded : false;
            surfaceSection.Expanded = (s == surfaceSection) ? !surfaceSection.Expanded : false;
            normalsSection.Expanded = (s == normalsSection) ? !normalsSection.Expanded : false;
            reflectionSection.Expanded = (s == reflectionSection) ? !reflectionSection.Expanded : false;
            intersectionSection.Expanded = (s == intersectionSection) ? !intersectionSection.Expanded : false;
            foamSection.Expanded = (s == foamSection) ? !foamSection.Expanded : false;
            wavesSection.Expanded = (s == wavesSection) ? !wavesSection.Expanded : false;
            advancedSection.Expanded = (s == advancedSection) ? !advancedSection.Expanded : false;

        }

        #region Styles
        private static string charFoldout = "−";
        private static string charCollapsed = "≡";
        private static GUIStyle _ParameterGroup;
        public static GUIStyle ParameterGroup
        {
            get
            {
                if (_ParameterGroup == null)
                {
                    _ParameterGroup = new GUIStyle(EditorStyles.helpBox)
                    {

                    };
                }

                return _ParameterGroup;
            }
        }

        private static GUIStyle _RichTextButton;
        public static GUIStyle RichTextButton
        {
            get
            {
                if (_RichTextButton == null)
                {
                    _RichTextButton = new GUIStyle(GUI.skin.button)
                    {
                        alignment = TextAnchor.MiddleLeft,
                        stretchWidth = true,
                        richText = true,
                        wordWrap = true,
                        padding = new RectOffset()
                        {
                            left = 14,
                            right = 14,
                            top = 8,
                            bottom = 8
                        }
                    };
                }

                return _RichTextButton;
            }
        }

        private static GUIContent _SWSIconContent;
        public static GUIContent SWSIconContent
        {
            get
            {
                if (_SWSIconContent == null)
                {
                    _SWSIconContent = new GUIContent()
                    {
                        image = Resources.Load("sws_icon") as Texture,

                    };
                }

                return _SWSIconContent;
            }
        }

        private static GUIStyle _SWSIconStyle;
        public static GUIStyle SWSIconStyle
        {
            get
            {
                if (_SWSIconStyle == null)
                {
                    _SWSIconStyle = new GUIStyle()
                    {
                        fixedHeight = 40f,
                        fixedWidth = 40f,
                        margin = new RectOffset()
                        {
                            right = 10
                        }
                    };
                }

                return _SWSIconStyle;
            }
        }

        private static GUIStyle _HelpButton;
        public static GUIStyle HelpButton
        {
            get
            {
                if (_HelpButton == null)
                {
                    _HelpButton = new GUIStyle(GUI.skin.button)
                    {
                        alignment = TextAnchor.MiddleCenter,
                        fixedWidth = 120f,
                        richText = true,
                        wordWrap = true
                    };
                }

                return _HelpButton;
            }
        }

        private static GUIStyle _SectionHelpButtonStyle;
        public static GUIStyle SectionHelpButtonStyle
        {
            get
            {
                if (_SectionHelpButtonStyle == null)
                {
                    _SectionHelpButtonStyle = new GUIStyle(EditorStyles.miniButtonRight)
                    {
                        fontSize = 12,
                        fontStyle = FontStyle.Normal,
                        fixedWidth = 50,
                        fixedHeight = 21.5f
                    };
                }

                return _SectionHelpButtonStyle;
            }
        }

        private static Texture _HelpIcon;
        public static Texture HelpIcon
        {
            get
            {
                if (_HelpIcon == null)
                {
                    _HelpIcon = EditorGUIUtility.FindTexture("d_UnityEditor.InspectorWindow");
                }

                return _HelpIcon;
            }
        }

        private static GUIContent _HelpButtonContent;
        public static GUIContent HelpButtonContent
        {
            get
            {
                if (_HelpButtonContent == null)
                {
                    _HelpButtonContent = new GUIContent()
                    {
                        image = HelpIcon,
                    };
                }

                return _HelpButtonContent;
            }
        }

        private static GUIStyle _CollapseButton;
        public static GUIStyle CollapseButton
        {
            get
            {
                if (_CollapseButton == null)
                {
                    _CollapseButton = new GUIStyle(EditorStyles.miniButtonLeft)
                    {
                        fontSize = 16,
                        fontStyle = FontStyle.Normal,
                        fixedWidth = 30,
                        fixedHeight = 21.5f,
                    };
                }

                return _CollapseButton;
            }
        }

        private static GUIStyle _GroupFoldout;
        public static GUIStyle GroupFoldout
        {
            get
            {
                if (_GroupFoldout == null)
                {
                    _GroupFoldout = new GUIStyle(EditorStyles.miniButtonMid)
                    {
                        fontSize = 11,
                        alignment = TextAnchor.MiddleLeft,
                        stretchWidth = true,
                        fixedHeight = 22f,
                        padding = new RectOffset()
                        {
                            left = 10,
                            top = 4,
                            bottom = 5
                        }
                    };
                }

                return _GroupFoldout;
            }
        }

        private static GUIStyle _UpdateText;
        public static GUIStyle UpdateText
        {
            get
            {
                if (_UpdateText == null)
                {
                    _UpdateText = new GUIStyle("Button")
                    {
                        //fontSize = 10,
                        alignment = TextAnchor.MiddleCenter,
                        stretchWidth = true,
                    };
                }

                return _UpdateText;
            }
        }
        #endregion

    }//Class
}//Namespace
