using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trigger : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnMouseDown()
    {
        // Perform your desired action here
        Debug.Log("Object Clicked!");

        // For example, let's change the object's color
        GetComponent<Renderer>().material.color = Color.red;
    }
}
