using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuRain : MonoBehaviour
{
    public GameObject targetObject; 
    public float delay = 30f;      

    void Start()
    {
        
        StartCoroutine(ToggleObjectRepeatedly());
    }

    IEnumerator ToggleObjectRepeatedly()
    {
        while (true)
        {
            
            targetObject.SetActive(!targetObject.activeSelf);

            
            yield return new WaitForSeconds(delay);
        }
    }
}
