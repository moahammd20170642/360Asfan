using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CrizGames.Vour.Editor
{
    [CustomEditor(typeof(TeleportPoint))]
    public class TeleportPointEditor : UnityEditor.Editor
    {
        SerializedProperty teleportType;
        SerializedProperty targetLocation;
        SerializedProperty resetPlayerRotation;

        /// <summary>
        /// OnEnable
        /// </summary>
        private void OnEnable()
        {
            teleportType = serializedObject.FindProperty("teleportType");
            targetLocation = serializedObject.FindProperty("TargetLocation");
            resetPlayerRotation = serializedObject.FindProperty("ResetPlayerRotation");
        }

        /// <summary>
        /// OnInspectorGUI
        /// </summary>
        public override void OnInspectorGUI()
        {
            TeleportPoint p = (TeleportPoint)target;

            EditorGUILayout.PropertyField(teleportType, new GUIContent("Teleport Type"));

            if (p.teleportType == TeleportPoint.TeleportType.SwitchLocation)
                EditorGUILayout.PropertyField(targetLocation, new GUIContent("Target Location"));
            
            EditorGUILayout.PropertyField(resetPlayerRotation, new GUIContent("Reset Player Rotation"));

            serializedObject.ApplyModifiedProperties();
        }
    }
}