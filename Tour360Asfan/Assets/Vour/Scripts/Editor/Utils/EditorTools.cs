using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

using UnityEngine;
using UnityEngine.Rendering;

namespace CrizGames.Vour.Editor
{
    [InitializeOnLoad]
    public static class EditorTools
    {
        static EditorTools()
        {
#if WEBXR_INPUT_PROFILES && UNITY_WEBGL && !(USING_URP || USING_HDRP )
            AddAlwaysIncludedShader("glTF/PbrMetallicRoughness");
            AddAlwaysIncludedShader("glTF/PbrSpecularGlossiness");
            AddAlwaysIncludedShader("glTF/Unlit");
#endif
        }

        // https://forum.unity.com/threads/modify-always-included-shaders-with-pre-processor.509479/#post-3509413
        public static void AddAlwaysIncludedShader(string shaderName)
        {
            var shader = Shader.Find(shaderName);
            if (shader == null)
                return;

            var graphicsSettingsObj = AssetDatabase.LoadAssetAtPath<GraphicsSettings>("ProjectSettings/GraphicsSettings.asset");
            var serializedObject = new SerializedObject(graphicsSettingsObj);
            var arrayProp = serializedObject.FindProperty("m_AlwaysIncludedShaders");
            bool hasShader = false;
            for (int i = 0; i < arrayProp.arraySize; ++i)
            {
                var arrayElem = arrayProp.GetArrayElementAtIndex(i);
                if (shader == arrayElem.objectReferenceValue)
                {
                    hasShader = true;
                    break;
                }
            }

            if (!hasShader)
            {
                int arrayIndex = arrayProp.arraySize;
                arrayProp.InsertArrayElementAtIndex(arrayIndex);
                var arrayElem = arrayProp.GetArrayElementAtIndex(arrayIndex);
                arrayElem.objectReferenceValue = shader;

                serializedObject.ApplyModifiedProperties();

                AssetDatabase.SaveAssets();
            }
        }

        /// <summary>
        /// GetPrefabPath
        /// </summary>
        public static string GetPrefabPath(string prefabName, out bool foundPrefab, params string[] pathContains)
        {
            return GetAssetPath("prefab", prefabName, out foundPrefab, pathContains);
        }

        /// <summary>
        /// GetScenePath
        /// </summary>
        public static string GetScenePath(string sceneName, out bool foundScene, params string[] pathContains)
        {
            return GetAssetPath("scene", sceneName, out foundScene, pathContains);
        }

        /// <summary>
        /// GetAssetPath
        /// </summary>
        public static string GetAssetPath(string assetType, string assetName, out bool foundAsset, params string[] pathContains)
        {
            // Search for assets with this name
            string[] assetGUIDs = AssetDatabase.FindAssets($"t:{assetType} {assetName}");
            string[] assetPaths = assetGUIDs.Select(AssetDatabase.GUIDToAssetPath).ToArray();

            // Get asset
            string assetPath = null;
            for (int i = 0; i < assetPaths.Length; i++)
            {
                string path = assetPaths[i];
                if (!path.StartsWith("Assets/Oculus") && (pathContains.Length == 0 || pathContains.Any(c => path.Contains(c))))
                {
                    if (System.IO.Path.GetFileNameWithoutExtension(path) == assetName)
                        assetPath = path;
                    break;
                }
            }

            // If no asset found
            if (string.IsNullOrEmpty(assetPath))
                foundAsset = false;
            else
                foundAsset = true;

            return assetPath;
        }

        /// <summary>
        /// GetAssetObject
        /// </summary>
        public static T GetAssetObject<T>(string path) where T : Object
        {
            return (T)AssetDatabase.LoadAssetAtPath(path, typeof(T));
        }

        /// <summary>
        /// IsInPrefabView
        /// </summary>
        public static bool IsInPrefabView(GameObject obj)
        {
            return !obj.scene.IsValid() || UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage() != null;
        }

        public static void AddTooltip(this SerializedProperty property)
        {
            GUI.Label(GUILayoutUtility.GetLastRect(), new GUIContent("", property.tooltip));
        }
    }
}