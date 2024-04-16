using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UIViewer : MonoBehaviour
{
    public void viewUI()
    {
        if (this.tag != "VideoB")
        {
            if (PlayerPrefs.GetInt("lan", 0) == 1)
            {

                GameObject arab = transform.GetChild(0).gameObject;
                arab.SetActive(true);

            }

            if (PlayerPrefs.GetInt("lan", 0) == 2)
            {
                GameObject englis = transform.GetChild(1).gameObject;
                englis.SetActive(true);

            }
        }


    }


    private void OnMouseDown()
    {
        Debug.Log("hhhhhh");
        viewUI();

        if (this.tag == "VideoB")
        {
            Debug.Log("playVideo");
            GameObject cube = transform.GetChild(0).gameObject;
            cube.SetActive(true);
            vidPlayer vp = transform.GetChild(1).GetComponent<vidPlayer>();

           vp.Playvideo();
        }
    }
}
