using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CrizGames.Vour.Editor
{
    [CustomEditor(typeof(InfoPanel))]
    public class InfoPanelEditor : UnityEditor.Editor
    {
        private SerializedProperty customPanel;
        private SerializedProperty customPanelObject;
        private SerializedProperty title;
        private SerializedProperty image;
        private SerializedProperty text;

        private bool inPrefabView;

        /// <summary>
        /// OnEnable
        /// </summary>
        protected virtual void OnEnable()
        {
            customPanel = serializedObject.FindProperty("CustomPanel");
            customPanelObject = serializedObject.FindProperty("CustomPanelObject");
            title = serializedObject.FindProperty("Title");
            image = serializedObject.FindProperty("Image");
            text = serializedObject.FindProperty("Text");

            if (Application.isPlaying)
                return;

            InfoPanel i = (InfoPanel)target;

            inPrefabView = EditorTools.IsInPrefabView(i.gameObject);
            if (inPrefabView)
                return;

            SetPanel(true);

            if (i.transform.childCount > 0)
                i.InitPanel();
        }

        /// <summary>
        /// Set Panel Visibility
        /// </summary>
        protected virtual void SetPanel(bool value)
        {
            InfoPanel i = (InfoPanel)target;
            GameObject panel = i.panel?.gameObject;
            if (panel)
                panel.SetActive(value);
        }

        /// <summary>
        /// OnInspectorGUI
        /// </summary>
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            InfoPanel i = (InfoPanel)target;

            if (inPrefabView)
            {
                DrawDefaultInspector();
                return;
            }

            // Custom panel
            EditorGUILayout.PropertyField(customPanel, new GUIContent("Custom Panel"));
            if (i.CustomPanel)
            {
                EditorGUILayout.PropertyField(customPanelObject, new GUIContent("Custom Panel Object"));
                serializedObject.ApplyModifiedProperties();

                if (i.CustomPanelObject == null)
                    EditorGUILayout.HelpBox("You need to set Custom Panel Object!", MessageType.Warning);

                else if (!i.CustomPanelObject.activeSelf)
                    i.CustomPanelObject.SetActive(true);

                // Disable built-in panels
                SetPanel(false);
                return;
            }
            else if (i.CustomPanelObject != null && i.CustomPanelObject.activeSelf)
            {
                i.CustomPanelObject.SetActive(false);
            }

            // Built-in panels
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(title, new GUIContent("Title"));
            EditorGUILayout.PropertyField(image, new GUIContent("Image"));

            GameObject prefab = null;
            if (i.Image != null)
            {
                var prevPanelType = i.PanelType;
                i.PanelType = (InfoPoint.InfoPanelImageType)EditorGUILayout.EnumPopup("Image", i.PanelType);
                if (i.PanelType != prevPanelType)
                    Undo.RecordObject(i, $"Updated Info Point ({i.name}) Panel");

                switch (i.PanelType)
                {
                    case InfoPoint.InfoPanelImageType.LeftImage:
                        prefab = LocationManager.GetManager().InfoPanelLeftImage;
                        break;

                    case InfoPoint.InfoPanelImageType.RightImage:
                        prefab = LocationManager.GetManager().InfoPanelRightImage;
                        break;
                }
            }
            else
            {
                prefab = LocationManager.GetManager().InfoPanelTextOnly;
            }

            EditorGUILayout.PropertyField(text, new GUIContent("Text"));

            serializedObject.ApplyModifiedProperties();

            // Instantiate
            Transform panelParent = i.panelParent;
            GameObject panel = i.panel?.gameObject;

            // Enable panel
            if (panel != null && !panel.activeSelf)
                panel.SetActive(true);

            // Add / Update Panel Prefab
            if (panel == null || panel.name != prefab.name)
            {
                if (panel != null)
                    DestroyImmediate(panel);
                panel = PrefabUtility.InstantiatePrefab(prefab, panelParent) as GameObject;
                EditorUtility.SetDirty(panel);
            }

            if (EditorGUI.EndChangeCheck())
            {
                // Update text
                i.InitPanel();

                // Notify objects that they changed
                Undo.RecordObject(panel.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>(), "Updated Info Title");
                Undo.RecordObject(panel.transform.GetChild(2).GetComponent<TMPro.TextMeshProUGUI>(), "Updated Info Text");

                if (panel.transform.childCount > 3)
                    Undo.RecordObject(panel.transform.GetChild(3).GetComponent<UnityEngine.UI.Image>(), "Updated Info Image");
            }
        }
    }
}