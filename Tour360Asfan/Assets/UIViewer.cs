using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UIViewer : MonoBehaviour
{
    public void viewUI()
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

    
    private void OnMouseDown()
    {
        viewUI();
    }
}
