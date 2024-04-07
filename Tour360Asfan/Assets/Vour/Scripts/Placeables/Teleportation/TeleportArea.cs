using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CrizGames.Vour
{
    public class TeleportArea : MonoBehaviour, IInteractable
    {
        /// <summary>
        /// Interact
        /// </summary>
        public void Interact()
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

            PlayerBase player = PlayerBase.Instance;

            player.transform.position = player.pointerPos;
            player.SetCenterCam(player.CenterCamera);
            
            yield return manager.PlayBlinkAnim(0f);
        }

        public void OnPointerOverObjEnter()
        {

        }

        public void OnPointerOverObjExit()
        {

        }
    }
}