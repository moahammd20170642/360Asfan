using System.Collections;
using System.Collections.Generic;
using System.Runtime;
using UnityEngine;

public class ArrowManger : MonoBehaviour
{
 
    public int NextImageIndex;
    // Start is called before the first frame update

    datamanger datamangerf;
    void OnMouseDown()
    {
        // Perform your desired action here
        Debug.Log("Object Clicked!");

        // For example, let's change the object's color
        GetComponent<Renderer>().material.color = Color.red;
        datamangerf.changeImages(NextImageIndex - 1);
        datamangerf.applyImages(NextImageIndex - 1);
        //datamangerf.Rcamera();
    }
    private void Start()
    {
        datamangerf = GameObject.Find("DataManager").GetComponent<datamanger>();
    }

   
}
