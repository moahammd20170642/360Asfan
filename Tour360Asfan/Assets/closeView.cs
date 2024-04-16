using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class closeView : MonoBehaviour
{
    private void OnMouseDown()
    {
        GameObject parent = transform.parent.gameObject;
        parent.SetActive(false);

        if(this.tag== "VideoB")
        {
            vidPlayer vp = parent.transform.GetChild(1).GetComponent<vidPlayer>();
            vp.StopVideo();
        }
    }
}
