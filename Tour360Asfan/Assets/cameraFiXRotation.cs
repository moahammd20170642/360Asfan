using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class cameraFiXRotation : MonoBehaviour
{
    public Transform transformO;
    // Start is called before the first frame update

    public bool itNeedsFix = false;
    public float y_axix;
    private void OnEnable()
    {
        if (itNeedsFix)
        {
            transformO.eulerAngles = new Vector3(0, y_axix, 0);
        }

    }
}
