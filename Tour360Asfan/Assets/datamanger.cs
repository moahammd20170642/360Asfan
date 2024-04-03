using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class datamanger : MonoBehaviour
{


    public  List<Texture2D> images;

    public  MeshRenderer meshRenderer;

    public  void changeImages(int index)
    {
        meshRenderer.material.mainTexture = images[index];  

    }
}
