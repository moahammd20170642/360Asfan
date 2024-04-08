using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace CrizGames.Vour.Editor
{
    // https://forum.unity.com/threads/adding-layer-by-script.41970/#post-2274824
    [InitializeOnLoad]
    public static class LayerUtils
    {
        private static KeyValuePair<string, int>[] layersToAdd = new KeyValuePair<string, int>[] {
            new KeyValuePair<string, int>("LeftEye", 9),
            new KeyValuePair<string, int>("RightEye", 10)
        };

        static LayerUtils()
        {
            CreateLayer();
        }

        static void CreateLayer()
        {
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);

            SerializedProperty layers = tagManager.FindProperty("layers");
            if (layers == null || !layers.isArray)
            {
                Debug.LogWarning("Can't set up the layers. It's possible the format of the layers and tags data has changed in this version of Unity.");
                Debug.LogWarning("Layers is null: " + (layers == null));
                return;
            }

            foreach (var layer in layersToAdd)
            {
                string layerName = layer.Key;
                int layerIdx = layer.Value;
                SerializedProperty layerSP = layers.GetArrayElementAtIndex(layerIdx);
                if (layerSP.stringValue != layerName)
                {
                    if (string.IsNullOrWhiteSpace(layerSP.stringValue))
                        Debug.Log($"Vour setting up layers: layer {layerIdx} is now called {layerName}");
                    else
                        Debug.LogWarning($"Vour setting up layers: layer \"{layerSP.stringValue}\" is now called {layerName}");

                    layerSP.stringValue = layerName;
                }
            }

            tagManager.ApplyModifiedProperties();
        }
    }
}