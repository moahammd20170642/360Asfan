using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class closeView : MonoBehaviour
{
    private void OnMouseDown()
    {
        if (this.tag != "VideoB")
        {
            GameObject parent = transform.parent.gameObject;
            parent.SetActive(false);
        }

        if (this.tag== "VideoB")
        {
            GameObject parent = transform.parent.gameObject;
            vidPlayer vp = parent.transform.GetChild(1).GetComponent<vidPlayer>();
            parent.SetActive(false);
            vp.StopVideo();
        }
    }
}
