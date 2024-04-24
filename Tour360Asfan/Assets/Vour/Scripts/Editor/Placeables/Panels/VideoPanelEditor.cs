using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static CrizGames.Vour.Location;

namespace CrizGames.Vour.Editor
{
    [CustomEditor(typeof(VideoPanel))]
    public class VideoPanelEditor : UnityEditor.Editor
    {
        private SerializedProperty video;
        private SerializedProperty videoURL;
        private SerializedProperty streamingAssetsVidPath;
        private SerializedProperty videoType;
        private SerializedProperty playAtStart;
        private SerializedProperty loopVideo;
        private SerializedProperty videoVolume;
        private SerializedProperty videoUI;
        private SerializedProperty videoUIAudioVolume;
        private SerializedProperty videoUILoopButton;

        private bool inPrefabView;

        /// <summary>
        /// OnEnable
        /// </summary>
        protected virtual void OnEnable()
        {
            video = serializedObject.FindProperty("video");
            videoURL = serializedObject.FindProperty("videoURL");
            streamingAssetsVidPath = serializedObject.FindProperty("streamingAssetsVidPath");
            videoType = serializedObject.FindProperty("videoType");
            playAtStart = serializedObject.FindProperty("playAtStart");
            loopVideo = serializedObject.FindProperty("loopVideo");
            videoVolume = serializedObject.FindProperty("videoVolume");
            videoUI = serializedObject.FindProperty("videoUI");
            videoUIAudioVolume = serializedObject.FindProperty("videoUIAudioVolume");
            videoUILoopButton = serializedObject.FindProperty("videoUILoopButton");

            if (Application.isPlaying)
                return;

            VideoPanel p = (VideoPanel)target;

            inPrefabView = EditorTools.IsInPrefabView(p.gameObject);
        }

        /// <summary>
        /// OnInspectorGUI
        /// </summary>
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            VideoPanel p = (VideoPanel)target;

            if (inPrefabView)
            {
                DrawDefaultInspector();
                return;
            }

            DrawProperties(p);

            serializedObject.ApplyModifiedProperties();
        }

        void DrawProperties(VideoPanel point)
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

            EditorGUILayout.PropertyField(playAtStart, new GUIContent("Play at Start"));

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
        }
    }
}