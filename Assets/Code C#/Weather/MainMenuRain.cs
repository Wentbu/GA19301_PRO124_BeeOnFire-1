using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuRain : MonoBehaviour
{
    public GameObject targetObject; 
    public float toggleInterval = 15f; 

    void Start()
    {
        StartCoroutine(ToggleObjectCoroutine());
    }

    IEnumerator ToggleObjectCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(toggleInterval);
            targetObject.SetActive(!targetObject.activeSelf);
        }
    }
}
