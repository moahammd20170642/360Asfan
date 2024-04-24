using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CrizGames.Vour
{
    public interface IInteractable
    {
        /// <summary>
        /// Interact
        /// </summary>
        void Interact();

        /// <summary>
        /// OnPointerOverObjEnter
        /// </summary>
        void OnPointerOverObjEnter();

        /// <summary>
        /// OnPointerOverObjExit
        /// </summary>
        void OnPointerOverObjExit();
    }
}