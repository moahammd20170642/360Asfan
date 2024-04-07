using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static CrizGames.Vour.Location;

namespace CrizGames.Vour.Editor
{
    [CustomEditor(typeof(LocationManager))]
    public class LocationManagerEditor : UnityEditor.Editor
    {
        private static LocationManager manager;

        private SerializedProperty startLocation;
        private SerializedProperty locationType;

        /// <summary>
        /// OnEnable
        /// </summary>
        public void OnEnable()
        {
            startLocation = serializedObject.FindProperty("StartLocation");
            locationType = serializedObject.FindProperty("LocationType");
        }

        /// <summary>
        /// AddActivateLocationEvent
        /// </summary>
        [UnityEditor.Callbacks.DidReloadScripts]
        public static void AddActivateLocationEvent()
        {
            Selection.selectionChanged -= ActivateLocationIfChildSelected;
            Selection.selectionChanged += ActivateLocationIfChildSelected;
        }

        /// <summary>
        /// ActivateLocationIfChildSelected
        /// </summary>
        public static void ActivateLocationIfChildSelected()
        {
            if (Application.isPlaying)
                return;

            LocationManager manager;

            // Search for parent
            if (Selection.activeGameObject == null)
            {
                manager = LocationManager.GetManager();
                if (manager == null)
                    return;
                manager.DeactivateLocationTypes();
                manager.DeactivateLocations();
                return;
            }

            // Search for a parent with Location script
            Transform parent = Selection.activeGameObject.transform;
            Location l = null;
            while (parent != null && l == null)
            {
                parent = parent.parent;
                if (parent)
                    l = parent.GetComponent<Location>();
            }

            if (l != null)
            {
                manager = LocationManager.GetManager();
                if (manager == null)
                    return;

                LocationEditor.ActivateLocation(manager, l);
                LocationBase loc = LocationManager.LocationsDic[l.locationType];
                l.SetSourcesToLocation(loc);
            }
        }

        /// <summary>
        /// OnInspectorGUI
        /// </summary>
        public override void OnInspectorGUI()
        {
            manager = (LocationManager)target;

            // If prefab or something else
            GameObject locationManagerObj = manager.gameObject;
            if (EditorTools.IsInPrefabView(locationManagerObj))
            {
                DrawDefaultInspector();
                return;
            }

            EditorGUILayout.PropertyField(startLocation, new GUIContent("Start Location"));
            serializedObject.ApplyModifiedProperties();

            if (manager.StartLocation == null)
                EditorGUILayout.HelpBox("Start Location is not assigned.", MessageType.Error);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Add a Location", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(locationType, new GUIContent("Location Type"));
            serializedObject.ApplyModifiedProperties();

            if (GUILayout.Button("Add Location"))
                AddLocation();
        }

        /// <summary>
        /// AddLocationManagerToScene
        /// </summary>
        [MenuItem("Tools/Vour/Add Location Manager", false, 10), MenuItem("GameObject/Vour/Location Manager")]
        public static void AddLocationManagerToScene()
        {
            LocationManager managerInScene = FindObjectOfType<LocationManager>();
            if (managerInScene != null)
            {
                EditorUtility.DisplayDialog("Info", "A Location Manager object is already in the scene!", "Okay");
                Selection.activeObject = managerInScene.gameObject;
                return;
            }

            string path = EditorTools.GetPrefabPath("Location Manager", out bool found, "Vour");
            if (found)
            {
                GameObject prefab = EditorTools.GetAssetObject<GameObject>(path);
                GameObject manager = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                manager.name = prefab.name;
                manager.transform.SetAsFirstSibling();
                Selection.activeGameObject = manager;
            }
            else
            {
                Debug.LogError("Couldn't find Location Manager prefab in assets! Did you delete it accidentally?");
            }
        }

        /// <summary>
        /// AddLocation
        /// </summary>
        public static void AddLocation()
        {
            // Check if there is a location manager already and if there is a location in scene
            if (manager == null)
                manager = LocationManager.GetManager();

            // Add location to scene
            GameObject location = (GameObject)PrefabUtility.InstantiatePrefab(manager.Location);
            location.transform.SetAsLastSibling();
            Selection.activeGameObject = location;

            Location o = location.GetComponent<Location>();
            o.locationType = manager.LocationType;
        }

        /// <summary>
        /// AddLocationWithType
        /// </summary>
        private static void AddLocationWithType(LocationType type)
        {
            if (manager == null)
                manager = LocationManager.GetManager();

            // If it doesn't exist yet, add one
            if (manager == null)
            {
                AddLocationManagerToScene();
                manager = LocationManager.GetManager();
            }

            manager.LocationType = type;

            AddLocation();
        }

        /// <summary>
        /// AddLocationEmpty
        /// </summary>
        [MenuItem("Tools/Vour/Add Location/Empty", false, 1), MenuItem("GameObject/Vour/Location/Empty Location", false, 10)]
        public static void AddLocationEmpty()
        {
            AddLocationWithType(LocationType.Empty);
        }

        /// <summary>
        /// AddLocation
        /// </summary>
        [MenuItem("Tools/Vour/Add Location/Image", false, 1), MenuItem("GameObject/Vour/Location/Image Location", false, 10)]
        public static void AddLocationImage()
        {
            AddLocationWithType(LocationType.Image);
        }

        /// <summary>
        /// AddLocationImage3D
        /// </summary>
        [MenuItem("Tools/Vour/Add Location/Image3D", false, 1), MenuItem("GameObject/Vour/Location/Image3D Location", false, 10)]
        public static void AddLocationImage3D()
        {
            AddLocationWithType(LocationType.Image3D);
        }

        /// <summary>
        /// AddLocationImage360
        /// </summary>
        [MenuItem("Tools/Vour/Add Location/Image360", false, 1), MenuItem("GameObject/Vour/Location/Image360 Location", false, 10)]
        public static void AddLocationImage360()
        {
            AddLocationWithType(LocationType.Image360);
        }

        /// <summary>
        /// AddLocation
        /// </summary>
        [MenuItem("Tools/Vour/Add Location/Image3D360", false, 1), MenuItem("GameObject/Vour/Location/Image3D360 Location", false, 10)]
        public static void AddLocationImage3D360()
        {
            AddLocationWithType(LocationType.Image3D360);
        }

        /// <summary>
        /// AddLocationVideo
        /// </summary>
        [MenuItem("Tools/Vour/Add Location/Video", false, 1), MenuItem("GameObject/Vour/Location/Video Location", false, 10)]
        public static void AddLocationVideo()
        {
            AddLocationWithType(LocationType.Video);
        }

        /// <summary>
        /// AddLocationVideo3D
        /// </summary>
        [MenuItem("Tools/Vour/Add Location/Video3D", false, 1), MenuItem("GameObject/Vour/Location/Video3D Location", false, 10)]
        public static void AddLocationVideo3D()
        {
            AddLocationWithType(LocationType.Video3D);
        }

        /// <summary>
        /// AddLocationVideo360
        /// </summary>
        [MenuItem("Tools/Vour/Add Location/Video360", false, 1), MenuItem("GameObject/Vour/Location/Video360 Location", false, 10)]
        public static void AddLocationVideo360()
        {
            AddLocationWithType(LocationType.Video360);
        }

        /// <summary>
        /// AddLocationVideo3D360
        /// </summary>
        [MenuItem("Tools/Vour/Add Location/Video3D360", false, 1), MenuItem("GameObject/Vour/Location/Video3D360 Location", false, 10)]
        public static void AddLocationVideo3D360()
        {
            AddLocationWithType(LocationType.Video3D360);
        }

        /// <summary>
        /// AddLocationScene
        /// </summary>
        [MenuItem("Tools/Vour/Add Location/Scene", false, 1), MenuItem("GameObject/Vour/Location/Scene Location", false, 10)]
        public static void AddLocationScene()
        {
            AddLocationWithType(LocationType.Scene);
        }

        private static void AddObject(GameObject obj)
        {
            Selection.activeGameObject = (GameObject)PrefabUtility.InstantiatePrefab(obj, Selection.activeTransform);
        }

        /// <summary>
        /// AddTeleportPoint
        /// </summary>
        [MenuItem("Tools/Vour/Add Teleport Point", false, 1), MenuItem("GameObject/Vour/Teleport Point", false, 10)]
        public static void AddTeleportPoint()
        {
            AddObject(VourSettings.Instance.defaultTeleportPoint);
        }

        /// <summary>
        /// AddInfoPoint
        /// </summary>
        [MenuItem("Tools/Vour/Add Info Point", false, 1), MenuItem("GameObject/Vour/Info Point", false, 10)]
        public static void AddInfoPoint()
        {
            AddObject(VourSettings.Instance.defaultInfoPoint);
        }

        /// <summary>
        /// AddInfoPanel
        /// </summary>
        [MenuItem("Tools/Vour/Add Info Panel", false, 1), MenuItem("GameObject/Vour/Info Panel", false, 10)]
        public static void AddInfoPanel()
        {
            AddObject(VourSettings.Instance.defaultInfoPanel);
        }

        /// <summary>
        /// AddVideoPoint
        /// </summary>
        [MenuItem("Tools/Vour/Add Video Point", false, 1), MenuItem("GameObject/Vour/Video Point", false, 10)]
        public static void AddVideoPoint()
        {
            AddObject(VourSettings.Instance.defaultVideoPoint);
        }

        /// <summary>
        /// AddVideoPanel
        /// </summary>
        [MenuItem("Tools/Vour/Add Video Panel", false, 1), MenuItem("GameObject/Vour/Video Panel", false, 10)]
        public static void AddVideoPanel()
        {
            AddObject(VourSettings.Instance.defaultVideoPanel);
        }

        /// <summary>
        /// CenterEditorCam
        /// </summary>
        [MenuItem("Tools/Vour/Center Editor Camera &c", false, 3000)]
        public static void CenterEditorCam()
        {
            SceneView view = SceneView.lastActiveSceneView;
            if (view != null)
            {
                Camera target = view.camera;
                target.transform.position = Vector3.zero;
                target.transform.rotation = Quaternion.identity;
                view.AlignViewToObject(target.transform);
            }
        }

        /// <summary>
        /// CenterEditorCam
        /// </summary>
        [MenuItem("Tools/Vour/Open Online Documentation", false, 4000)]
        public static void OpenOnlineDocs()
        {
            Application.OpenURL("https://crizgames.gitbook.io/vour/");
        }
    }
}