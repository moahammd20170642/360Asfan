using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class cameraT : MonoBehaviour
{
    public RawImage miniMapImage;
    public Camera miniMapCamera;

    private RenderTexture renderTexture;
    private Texture2D miniMapTexture;

    private void Start()
    {
        // Create a new Render Texture
        renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
        miniMapCamera.targetTexture = renderTexture;

        // Create a new Texture2D to hold the mini-map texture
        miniMapTexture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
    }

    private void LateUpdate()
    {
        // Render the camera's view onto the Render Texture
        miniMapCamera.Render();

        // Copy the Render Texture to the Texture2D
        RenderTexture.active = renderTexture;
        miniMapTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        miniMapTexture.Apply();

        // Update the Raw Image with the Texture2D
        miniMapImage.texture = miniMapTexture;
    }

    private void OnDestroy()
    {
        // Cleanup resources
        renderTexture.Release();
        Destroy(miniMapTexture);
    }
}
