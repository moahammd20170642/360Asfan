using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class datamanger : MonoBehaviour
{

    public PositionsManaager pm;

    public List<GameObject> Hotspots;
    //public List<Texture2D> images;
    public List<string> Addresses;
    public MeshRenderer meshRenderer;

    public CameraControlle cameraControlle;

    public  int currentIndex = 0;

    public void changeImages(int index)
    {
        currentIndex = index; // Update the current index
        Addressables.LoadAssetAsync<Texture2D>(Addresses[index]).Completed += handle => onchanged(handle, index);
    }
    public void onchanged(AsyncOperationHandle<Texture2D> handle, int index)
    {


        //meshRenderer.material.mainTexture = images[index];
        //pm.ActivatePoint(index);

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            Texture2D image = handle.Result;
            meshRenderer.material.mainTexture = image;
            pm.ActivatePoint(index);
        }
        else
        {
            Debug.LogError("Failed to load image: " + handle.OperationException);
        }
    }

    private void Start()
    {
        applyImages(24);
        changeImages(24);
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