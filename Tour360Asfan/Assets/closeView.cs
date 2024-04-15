using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class closeView : MonoBehaviour
{
    private void OnMouseDown()
    {
        GameObject parent = transform.parent.gameObject;
        parent.SetActive(false);
    }
}
