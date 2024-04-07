using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CrizGames.Vour.Editor
{
    [CustomEditor(typeof(Player))]
    public class PlayerEditor : UnityEditor.Editor
    {
        private SerializedProperty centerCam;
        private SerializedProperty vrGazePointer;

        /// <summary>
        /// OnEnable
        /// </summary>
        public void OnEnable()
        {
            centerCam = serializedObject.FindProperty("CenterCamera");
            vrGazePointer = serializedObject.FindProperty("VRGazePointer");
        }

        /// <summary>
        /// OnInspectorGUI
        /// </summary>
        public override void OnInspectorGUI()
        {
            GameObject playerGO = ((Player)target).gameObject;
            // Check if in project window / prefab mode
            if (EditorTools.IsInPrefabView(playerGO))
            {
                DrawDefaultInspector();
                return;
            }

            EditorGUILayout.PropertyField(centerCam, new GUIContent("Center Camera"));
            EditorGUILayout.PropertyField(vrGazePointer, new GUIContent("Enable Gaze Pointer (VR only)"));
            vrGazePointer.AddTooltip();
            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// AddPlayer
        /// </summary>
        [MenuItem("Tools/Vour/Add Player", false, 10), MenuItem("GameObject/Vour/Player")]
        public static void AddPlayer()
        {
            Player playerInScene = FindObjectOfType<Player>();
            if (playerInScene != null)
            {
                EditorUtility.DisplayDialog("Info", "A Player object is already in the scene!", "Okay");
                Selection.activeObject = playerInScene.gameObject;
                return;
            }

            string path = EditorTools.GetPrefabPath("Player", out bool found, "Vour");
            if (found)
            {
                GameObject prefab = EditorTools.GetAssetObject<GameObject>(path);
                GameObject player = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                player.name = prefab.name;
                Selection.activeGameObject = player;
            }
        }
    }
}