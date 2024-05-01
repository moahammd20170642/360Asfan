using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class PositionsManaager : MonoBehaviour
{
    public bool startPostions=false;
   public  int currentIndex;
    // Start is called before the first frame update


    public List<GameObject> images = new List<GameObject>();


    private void Start()
    {
        DisaplePoints();
    }
    public void ActivatePoint(int index)
    {
        if (startPostions != false)
        {
            foreach (var item in images)
            {
                item.transform.GetChild(1).gameObject.SetActive(false);

            }
            images[index].transform.GetChild(1).gameObject.SetActive(true);
        }
        startPostions = true;

    }
    public void DisaplePoints()
    {

        foreach (var item in images)
        {
            item.transform.GetChild(1).gameObject.SetActive(false);

        }
    }
}
