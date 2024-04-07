using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static CrizGames.Vour.Location;

namespace CrizGames.Vour
{
    public class LocationManager : MonoBehaviour
    {
        private static LocationManager instance;
        
        public GameObject Location;
        [Space]
        public GameObject InfoPanelTextOnly;
        public GameObject InfoPanelLeftImage;
        public GameObject InfoPanelRightImage;
        [Space]
        [SerializeField] private Material blinkMat;
        [SerializeField] private Transform blinkMesh;
        [SerializeField] private Image blinkPanel;
        private readonly int alpha = Shader.PropertyToID("_Alpha");
        [Space]
        [SerializeField] private GameObject videoDesktopUI;
        [SerializeField] private GameObject videoVRUI;
        [Space]
        public Location StartLocation;
        public LocationType LocationType;

        private static LocationType currentLocation;

        public static Dictionary<LocationType, LocationBase> LocationsDic;

        /// <summary>
        /// Start
        /// </summary>
        private void Start()
        {
            if (StartLocation == null)
            {
                Debug.LogError("Start location is not assigned.");
                return;
            }

            instance = this;

            SetBlinkState(1f);

            FillLocationsDic();

            DeactivateLocationTypes();
            DeactivateLocations();

            bool hasVideoLocation = false;

            // Init all locations
            foreach (Location l in FindObjectsOfType<Location>(true))
            {
                l.Init();

                if (l.locationType.IsVideo())
                    hasVideoLocation = true;
            }

            // Add VideoUI
            if (hasVideoLocation)
            {
                GameObject videoUIObj = Instantiate(Player.GetPlayerPlatform().IsAnyVR() ? videoVRUI : videoDesktopUI);
                videoUIObj.SetActive(false);
                videoUIObj.transform.root.SetAsLastSibling();
            }

            // Activate StartLocation
            LocationBase startL = LocationsDic[StartLocation.locationType];
            StartLocation.SetSourcesToLocation(startL);
            startL.gameObject.SetActive(true);
            currentLocation = startL.locationType;
            StartLocation.gameObject.SetActive(true);
            StartLocation.PreloadLinkedVideos();

            PlayBlinkAnim(0f);
        }

        /// <summary>
        /// DeactivateLocationTypes
        /// </summary>
        public void DeactivateLocationTypes()
        {
            for (int i = 3; i < transform.childCount; i++)
                transform.GetChild(i).gameObject.SetActive(false);
        }

        /// <summary>
        /// DeactivateLocations
        /// </summary>
        public void DeactivateLocations()
        {
            Location[] ls = FindObjectsOfType<Location>(false);
            foreach (var l in ls)
                l.gameObject.SetActive(false);
        }

        /// <summary>
        /// FillLocationsDic
        /// </summary>
        public void FillLocationsDic()
        {
            LocationsDic = new Dictionary<LocationType, LocationBase>();
            for (int i = 2; i < transform.childCount; i++)
            {
                LocationBase l = transform.GetChild(i).GetComponent<LocationBase>();
                if (l != null && !LocationsDic.ContainsKey(l.locationType))
                    LocationsDic.Add(l.locationType, l);
            }
        }

        /// <summary>
        /// SetCurrentLocation
        /// </summary>
        public void SetCurrentLocation(LocationType type)
        {
            LocationsDic[currentLocation].gameObject.SetActive(false);
            LocationsDic[type].gameObject.SetActive(true);
            currentLocation = type;
        }

        /// <summary>
        /// GetManager
        /// </summary>
        public static LocationManager GetManager()
        {
            if (instance != null)
                return instance;
            
            GameObject managerGO = GameObject.FindGameObjectWithTag("LocationManager");
            if (!managerGO)
            {
#if UNITY_EDITOR
                if (Application.isPlaying || GameObject.FindGameObjectWithTag("Location") != null)
#endif
                    Debug.LogError("Could not find a Location Manager in scene. You have to add one to your scene!");
                return null;
            }

            return managerGO.GetComponent<LocationManager>();
        }

        /// <summary>
        /// PlayBlinkAnim
        /// </summary>
        public Coroutine PlayBlinkAnim(float targetAlpha)
        {
            StopCoroutine(nameof(DoPlayBlinkAnim));
            return StartCoroutine(DoPlayBlinkAnim(targetAlpha));
        }
        
        private IEnumerator DoPlayBlinkAnim(float targetAlpha)
        {
            if (PlayerBase.Instance != null)
                blinkMesh.position = PlayerBase.Instance.transform.position;

            float startAlpha = blinkMat.GetFloat(alpha);
            
            const float animDuration = 0.1f;
            float time = 0f;
            Color color = new Color(0,0,0, startAlpha);
            while (time < animDuration)
            {
                float a = color.a = Mathf.Lerp(startAlpha, targetAlpha, time / animDuration);
                blinkMat.SetFloat(alpha, a);
                blinkPanel.color = color;
                
                time += Time.deltaTime;
                yield return null;
            }

            SetBlinkState(targetAlpha);
        }

        private void SetBlinkState(float a)
        {
            blinkMat.SetFloat(alpha, a);
            blinkPanel.color = new Color(0,0,0, a);
        }

        private void OnApplicationQuit()
        {
            blinkMat.SetFloat(alpha, 0);
        }
    }
}