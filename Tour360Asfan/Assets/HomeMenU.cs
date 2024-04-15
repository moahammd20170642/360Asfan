using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeMenU : MonoBehaviour
{
    // Start is called before the first frame update


    public void setArabic()
    {
        PlayerPrefs.SetInt("lan", 1);///arabic

        SceneManager.LoadScene(1);
    }

    public void setEng()
    {
        PlayerPrefs.SetInt("lan", 2);///English

        SceneManager.LoadScene(1);
    }
}
