using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static CrizGames.Vour.Location;

namespace CrizGames.Vour.Editor
{
    [CustomEditor(typeof(VideoPoint))]
    public class VideoPointEditor : UnityEditor.Editor
    {
        private SerializedProperty video;
        private SerializedProperty videoURL;
        private SerializedProperty streamingAssetsVidPath;
        private SerializedProperty videoType;
        private SerializedProperty loopVideo;
        private SerializedProperty videoVolume;
        private SerializedProperty videoUI;
        private SerializedProperty videoUIAudioVolume;
        private SerializedProperty videoUILoopButton;
        private SerializedProperty rotateTowardsPlayer;

        private bool inPrefabView;

        /// <summary>
        /// OnEnable
        /// </summary>
        private void OnEnable()
        {
            video = serializedObject.FindProperty("video");
            videoURL = serializedObject.FindProperty("videoURL");
            streamingAssetsVidPath = serializedObject.FindProperty("streamingAssetsVidPath");
            videoType = serializedObject.FindProperty("videoType");
            loopVideo = serializedObject.FindProperty("loopVideo");
            videoVolume = serializedObject.FindProperty("videoVolume");
            videoUI = serializedObject.FindProperty("videoUI");
            videoUIAudioVolume = serializedObject.FindProperty("videoUIAudioVolume");
            videoUILoopButton = serializedObject.FindProperty("videoUILoopButton");
            rotateTowardsPlayer = serializedObject.FindProperty("rotateTowardsPlayer");

            if (Application.isPlaying)
                return;

            VideoPoint p = (VideoPoint)target;

            inPrefabView = EditorTools.IsInPrefabView(p.gameObject);
            if (inPrefabView)
                return;

            p.RotateTowardsPlayer();
        }

        /// <summary>
        /// OnDisable
        /// </summary>
        private void OnDisable()
        {
            if (Application.isPlaying || inPrefabView)
                return;

            VideoPoint p = (VideoPoint)target;

            if (p == null)
                return;

            p.RotateTowardsPlayer();
        }

        /// <summary>
        /// OnInspectorGUI
        /// </summary>
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            VideoPoint p = (VideoPoint)target;

            if (inPrefabView)
            {
                DrawDefaultInspector();
                return;
            }

            DrawProperties(p);

            serializedObject.ApplyModifiedProperties();
        }

        void DrawProperties(VideoPoint point)
        {
            EditorGUILayout.PropertyField(videoType, new GUIContent("Video Type"));

            switch (point.videoType)
            {
                case VideoType.Local:
                    EditorGUILayout.PropertyField(video, new GUIContent("Video"));
                    break;

                case VideoType.StreamingAssets:
                    EditorGUILayout.PropertyField(streamingAssetsVidPath, new GUIContent("Streaming Assets Video Path"));
                    break;

                case VideoType.URL:
                    EditorGUILayout.PropertyField(videoURL, new GUIContent("Video URL"));
                    break;
            }

            EditorGUILayout.PropertyField(loopVideo, new GUIContent("Loop Video"));

            EditorGUILayout.PropertyField(videoVolume, new GUIContent("Volume"));

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(videoUI, new GUIContent("Enable Video UI"));
            if (videoUI.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(videoUIAudioVolume, new GUIContent("Show Audio Button"));
                EditorGUILayout.PropertyField(videoUILoopButton, new GUIContent("Show Loop Button"));
                EditorGUI.indentLevel--;
            }
            if (EditorGUI.EndChangeCheck() && !Application.isPlaying)
            {
                var ui = point.panel.GetComponent<VideoUIController>();
                if (videoUI.boolValue)
                    ui.EnableUI(null, videoUIAudioVolume.boolValue, videoUILoopButton.boolValue);
                else
                    ui.DisableUI();
            }

            EditorGUILayout.Space();

            bool prevRot = rotateTowardsPlayer.boolValue;
            EditorGUILayout.PropertyField(rotateTowardsPlayer, new GUIContent("Rotate Panel Towards Player"));
            if (!prevRot && rotateTowardsPlayer.boolValue)
            {
                point.rotateTowardsPlayer = true;
                point.RotateTowardsPlayer();
            }
        }
    }
}