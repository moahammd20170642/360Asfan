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
            Sprite convertedSprite = Sprite.Create(image, new Rect(0, 0, image.width, image.height), Vector2.one * 0.5f);

            GetComponent<SpriteRenderer>().sprite = convertedSprite;
           
        }
        else
        {
            Debug.LogError("Failed to load image: " + handle.OperationException);
        }
    }
}
