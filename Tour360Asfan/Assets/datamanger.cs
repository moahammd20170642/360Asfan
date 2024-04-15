using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class datamanger : MonoBehaviour
{
   
    public PositionsManaager pm;

    public List<GameObject> Hotspots;
    public List<Texture2D> images;
 
    public MeshRenderer meshRenderer;

    public CameraControlle cameraControlle;


    

    public void changeImages(int index)
    {
        meshRenderer.material.mainTexture = images[index];
        pm.ActivatePoint(index);
    }

    private void Start()
    {
        applyImages(0);
        changeImages(0);
    }

    public void Rcamera()
    {
        cameraControlle.ResetCamera();

    }
    public void applyImages(int imageindex)
    {
        foreach (var hotspot in Hotspots)
        {
            hotspot.SetActive(false);
        }

        Hotspots[imageindex].SetActive(true);   

    }
}
