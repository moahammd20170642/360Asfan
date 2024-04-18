using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class imagLoader : MonoBehaviour
{
    public string adress;
    // Start is called before the first frame update
    private void OnEnable()
    {
        LoadImage(adress);
    }
    public void LoadImage(string path)
    {

        Addressables.LoadAssetAsync<Texture2D>(path).Completed += handle => onchanged(handle);
    }



    public void onchanged(AsyncOperationHandle<Texture2D> handle)
    {


        //meshRenderer.material.mainTexture = images[index];
        //pm.ActivatePoint(index);

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            Texture2D image = handle.Result;
            GetComponent<MeshRenderer>().material.mainTexture = image;
           
        }
        else
        {
            Debug.LogError("Failed to load image: " + handle.OperationException);
        }
    }
}
