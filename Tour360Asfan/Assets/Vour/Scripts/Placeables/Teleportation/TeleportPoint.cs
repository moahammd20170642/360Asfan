using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CrizGames.Vour
{
    public class TeleportPoint : MonoBehaviour, IInteractable
    {
        public Location TargetLocation;

        public bool ResetPlayerRotation = false;
        
        private SpriteRenderer bottom;

        private Color startColor;

        private Location parentLocation;

        public enum TeleportType
        {
            SwitchLocation,
            UpdatePosition
        }
        public TeleportType teleportType;

        /// <summary>
        /// Awake
        /// </summary>
        public virtual void Awake()
        {
            bottom = GetComponentInChildren<SpriteRenderer>();
            parentLocation = GetComponentInParent<Location>();

            startColor = bottom.color;
        }

        /// <summary>
        /// Interact
        /// </summary>
        public void Interact()
        {
            Teleport();
        }

        /// <summary>
        /// Teleport
        /// </summary>
        public void Teleport()
        {
            StartCoroutine(TeleportIE());
        }

        /// <summary>
        /// TeleportIE
        /// </summary>
        private IEnumerator TeleportIE()
        {
            LocationManager manager = LocationManager.GetManager();
            
            yield return manager.PlayBlinkAnim(1f);
            
            if (ResetPlayerRotation)
                PlayerBase.Instance.ResetRotation();
            
            if(teleportType == TeleportType.SwitchLocation)
            {
                // Set location type
                manager.SetCurrentLocation(TargetLocation.locationType);

                // Update Source
                LocationBase l = LocationManager.LocationsDic[TargetLocation.locationType];
                TargetLocation.SetSourcesToLocation(l);
                
                // Unload & preload videos
                parentLocation.UnloadLinkedVideos(TargetLocation);
                TargetLocation.PreloadLinkedVideos();

                // Wait until target location is ready
                if(!l.IsReady && VourSettings.Instance.blinkWaitUntilLocationIsReady) yield return new WaitUntil(() => l.IsReady);

                // Set Locations
                parentLocation.gameObject.SetActive(false);
                TargetLocation.gameObject.SetActive(true);
            }
            else
            {
                PlayerBase.Instance.transform.position = transform.position + Vector3.up * PlayerBase.Instance.yOffset;
            }

            yield return manager.PlayBlinkAnim(0f);
        }

        /// <summary>
        /// OnPointerOverObjEnter
        /// </summary>
        public void OnPointerOverObjEnter()
        {
            bottom.color = new Color(0.6f, 0.6f, 0.6f);
        }

        /// <summary>
        /// OnPointerOverObjExit
        /// </summary>
        public void OnPointerOverObjExit()
        {
            bottom.color = startColor;
        }
    }
}