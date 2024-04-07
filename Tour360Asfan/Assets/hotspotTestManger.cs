using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.AI;

public class hotspotTestManger : MonoBehaviour
{
   
    public int sphereIndex;
    public GameObject table;
    datamanger datamangerf;
    public GameObject world;
    void OnMouseDown()
    {
        // Perform your desired action here
        Debug.Log("Object Clicked!");

        // For example, let's change the object's color
        GetComponent<Renderer>().material.color = Color.red;
        datamangerf.changeImages(sphereIndex - 1);

        table.SetActive(false);
        world.SetActive(true);
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.M))
        {
            world.SetActive(false);
            table.SetActive(true);
        }
    }
    private void Start()
    {
        datamangerf = GameObject.Find("DataManager").GetComponent<datamanger>();
    }
}



