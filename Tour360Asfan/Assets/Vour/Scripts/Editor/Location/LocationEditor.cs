using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Video;
using static CrizGames.Vour.Location;

namespace CrizGames.Vour.Editor
{
    [CustomEditor(typeof(Location))]
    public class LocationEditor : UnityEditor.Editor
    {
        private SerializedProperty locationType;
        private SerializedProperty texture;
        private SerializedProperty video;
        private SerializedProperty videoURL;
        private SerializedProperty streamingAssetsVidPath;
        private SerializedProperty videoType;
        private SerializedProperty prepareVideo;
        private SerializedProperty scaleToFullscreen;
        private SerializedProperty lockCamera;
        private SerializedProperty loopVideo;
        private SerializedProperty videoVolume;
        private SerializedProperty videoUI;
        private SerializedProperty videoUIAudioVolume;
        private SerializedProperty videoUILoopButton;
        private SerializedProperty scene;
        private SerializedProperty _3DLayout;
        private SerializedProperty rotOffset;

        private static GameObject previewPlayerObj;
        private VideoPlayer previewPlayer;

        /// <summary>
        /// OnEnable
        /// </summary>
        private void OnEnable()
        {
            locationType = serializedObject.FindProperty("locationType");
            texture = serializedObject.FindProperty("Texture");
            video = serializedObject.FindProperty("Video");
            videoURL = serializedObject.FindProperty("VideoURL");
            streamingAssetsVidPath = serializedObject.FindProperty("StreamingAssetsVidPath");
            videoType = serializedObject.FindProperty("videoType");
            prepareVideo = serializedObject.FindProperty("prepareVideo");
            scaleToFullscreen = serializedObject.FindProperty("scaleToFullscreen");
            lockCamera = serializedObject.FindProperty("lockCamera");
            loopVideo = serializedObject.FindProperty("loopVideo");
            videoVolume = serializedObject.FindProperty("videoVolume");
            videoUI = serializedObject.FindProperty("videoUI");
            videoUIAudioVolume = serializedObject.FindProperty("videoUIAudioVolume");
            videoUILoopButton = serializedObject.FindProperty("videoUILoopButton");
            scene = serializedObject.FindProperty("scene");
            _3DLayout = serializedObject.FindProperty("_3D_Layout");
            rotOffset = serializedObject.FindProperty("rotOffset");

            //Location l = (Location)target;
            //switch (l.locationType)
            //{
            //    case LocationType.Video:
            //    case LocationType.Video3D:
            //    case LocationType.Video360:
            //    case LocationType.Video3D360:
            //        if (l.videoType == VideoType.URL)
            //            break;

            //        if (previewPlayerObj == null)
            //        {
            //            previewPlayerObj = new GameObject("VideoPreview");
            //            previewPlayerObj.hideFlags = HideFlags.DontSave;
            //            //previewPlayerObj.hideFlags = HideFlags.HideAndDontSave;
            //            previewPlayerObj.AddComponent<VideoPlayer>();
            //        }

            //        previewPlayer = previewPlayerObj.GetComponent<VideoPlayer>();

            //        VideoPlayer.FrameReadyEventHandler frameReadyHandler = null;
            //        bool oldSendFrameReadyEvents = previewPlayer.sendFrameReadyEvents;
            //        frameReadyHandler = (source, index) => {
            //            previewPlayer.sendFrameReadyEvents = oldSendFrameReadyEvents;
            //            previewPlayer.frameReady -= frameReadyHandler;
            //            // Callback
            //            FirstVideoFramePreview();
            //        };

            //        previewPlayer.frameReady += frameReadyHandler;
            //        previewPlayer.sendFrameReadyEvents = true;

            //        if (l.videoType == VideoType.Local)
            //            previewPlayer.clip = l.Video;
            //        else if (l.videoType == VideoType.StreamingAssets)
            //            previewPlayer.url = l.VideoURL;

            //        previewPlayer.Prepare();
            //        previewPlayer.StepForward();

            //        break;
            //}
        }

        /// <summary>
        /// OnDisable
        /// </summary>
        private void OnDisable()
        {
            if (Application.isPlaying)
                return;

            Location l = (Location)target;

            if (!l)
                return;

            Transform tResult = null;
            if (Selection.activeTransform != null)
                tResult = FindDeepChild(l.transform, Selection.activeTransform.name);

            if (tResult == null)
            {
                LocationManager manager = LocationManager.GetManager();

                if (manager == null)
                    return;

                manager.DeactivateLocationTypes();

                if (l != null && l.gameObject.activeSelf)
                    l.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// FirstVideoFramePreview
        /// </summary>
        private void FirstVideoFramePreview()
        {
            //previewPlayer.
        }

        /// <summary>
        /// FindDeepChild (Breadth-first search)
        /// </summary>
        public static Transform FindDeepChild(Transform parent, string name)
        {
            Queue<Transform> queue = new Queue<Transform>();
            queue.Enqueue(parent);
            while (queue.Count > 0)
            {
                Transform child = queue.Dequeue();

                // Return if found
                if (child.name == name)
                    return child;

                // Add childs children to search queue
                foreach (Transform childsChild in child)
                    queue.Enqueue(childsChild);
            }

            return null;
        }

        /// <summary>
        /// OnInspectorGUI
        /// </summary>
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            Location l = (Location)target;
            LocationManager manager = LocationManager.GetManager();

            if (manager == null)
            {
                EditorGUILayout.HelpBox("There is no Location Manager! You need to have one in your scene!", MessageType.Error);
                if (GUILayout.Button("Add Location Manager to Scene"))
                    LocationManagerEditor.AddLocationManagerToScene();

                return;
            }

            if (!Application.isPlaying)
                ActivateLocation(manager, l);

            EditorGUI.BeginChangeCheck();

            // Location Type
            EditorGUILayout.PropertyField(locationType, new GUIContent("Location Type"));
            serializedObject.ApplyModifiedProperties();

            // File source
            DrawFileProperties(l);

            bool propertiesChanged = EditorGUI.EndChangeCheck();

            LocationBase locBase = LocationManager.LocationsDic[l.locationType];

            // Display media in scene
            if (propertiesChanged || !Application.isPlaying)
            {
                l.SetSourcesToLocation(locBase);

                if (!Application.isPlaying)
                    manager.SetCurrentLocation(l.locationType);
            }

            // Add teleport point & info point buttons
            if (!Application.isPlaying && l.locationType != LocationType.Scene)
            {
                void InstantiateObj(GameObject obj)
                {
                    Selection.activeGameObject = (GameObject)PrefabUtility.InstantiatePrefab(obj, l.transform);
                }

                EditorGUILayout.Space();

                // Teleport point button
                if (GUILayout.Button("Add Teleport Point"))
                    InstantiateObj(VourSettings.Instance.defaultTeleportPoint);

                // Place buttons next to each other
                GUILayout.BeginHorizontal();

                // Info point button
                if (GUILayout.Button("Add Info Point"))
                    InstantiateObj(VourSettings.Instance.defaultInfoPoint);

                // Info panel button
                if (GUILayout.Button("Add Info Panel"))
                    InstantiateObj(VourSettings.Instance.defaultInfoPanel);

                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();

                // Video point button
                if (GUILayout.Button("Add Video Point"))
                    InstantiateObj(VourSettings.Instance.defaultVideoPoint);

                // Video panel button
                if (GUILayout.Button("Add Video Panel"))
                    InstantiateObj(VourSettings.Instance.defaultVideoPanel);

                GUILayout.EndHorizontal();
            }
        }

        /// <summary>
        /// DrawFileProperties
        /// </summary>
        private void DrawFileProperties(Location loc)
        {
            // 3D Only
            bool show3DLayout = false;
            switch (loc.locationType)
            {
                case LocationType.Image3D:
                case LocationType.Image3D360:
                    if (loc.Texture != null)
                        show3DLayout = true;
                    break;

                case LocationType.Video3D:
                case LocationType.Video3D360:
                    if (loc.Video != null)
                        show3DLayout = true;
                    break;
            }

            // IMAGE
            if (loc.locationType.IsImage())
            {
                EditorGUILayout.PropertyField(texture, new GUIContent("Texture"));

                if (show3DLayout)
                    EditorGUILayout.PropertyField(_3DLayout, new GUIContent("3D Layout"));
            }
            // VIDEO
            else if (loc.locationType.IsVideo())
            {
                if (show3DLayout)
                    EditorGUILayout.PropertyField(_3DLayout, new GUIContent("3D Layout"));

                EditorGUILayout.PropertyField(videoType, new GUIContent("Video Type"));

                switch (loc.videoType)
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

                EditorGUILayout.PropertyField(videoUI, new GUIContent("Enable Video UI"));
                if (loc.videoUI)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(videoUIAudioVolume, new GUIContent("Show Audio Button"));
                    EditorGUILayout.PropertyField(videoUILoopButton, new GUIContent("Show Loop Button"));
                    EditorGUI.indentLevel--;
                }
            }
            // SCENE
            else if (loc.locationType == LocationType.Scene)
            {
                EditorGUILayout.PropertyField(scene, new GUIContent("Scene"));
            }

            // 360 Only
            if (loc.locationType.Is360())
            {
                EditorGUILayout.PropertyField(rotOffset, new GUIContent("Rotation"));
            }
            // Not 360 only
            else
            {
                if (loc.locationType != LocationType.Empty)
                {
                    EditorGUILayout.PropertyField(scaleToFullscreen, new GUIContent("Scale to Fullscreen"));
                    scaleToFullscreen.AddTooltip();
                }
                EditorGUILayout.PropertyField(lockCamera, new GUIContent("Lock Camera (Non-VR Only)"));
            }

            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// ActivateLocation
        /// </summary>
        public static void ActivateLocation(LocationManager manager, Location loc)
        {
            manager.FillLocationsDic();

            manager.DeactivateLocationTypes();
            manager.DeactivateLocations();

            // Activate this location
            if (!loc.gameObject.activeSelf)
                loc.gameObject.SetActive(true);
            LocationManager.LocationsDic[loc.locationType].gameObject.SetActive(true);
        }

        /// <summary>
        /// SceneExists
        /// </summary>
        public static bool SceneExists(string sceneName)
        {
            EditorTools.GetScenePath(sceneName, out bool found);
            return found;
        }
    }
}