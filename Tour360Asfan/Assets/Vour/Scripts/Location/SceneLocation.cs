using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CrizGames.Vour
{
    public class SceneLocation : LocationBase
    {
        /// <summary>
        /// UpdateLocation
        /// </summary>
        public override void UpdateLocation()
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
#endif
                SceneManager.LoadScene(scene.SceneName);
        }

        /// <summary>
        /// RemoveScene
        /// </summary>
        public void RemoveScene()
        {
            SceneManager.UnloadSceneAsync(scene.SceneName);
        }
    }
}