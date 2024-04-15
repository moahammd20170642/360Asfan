using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionsManaager : MonoBehaviour
{
   public  int currentIndex;
    // Start is called before the first frame update


    public List<GameObject> images = new List<GameObject>();


    void Start()
    {

        ActivatePoint(currentIndex);

    }
    public void ActivatePoint(int index)
    {
        foreach (var item in images)
        {
          item.transform.GetChild(1).gameObject.SetActive(false);

        }
        images[index].transform.GetChild(1).gameObject.SetActive(true);
    }
}
