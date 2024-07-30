using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxMenu : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float moveSpeed ;
    [SerializeField] private float positionBR = 100f;
    [SerializeField] private float smoothness;

    private Vector3 targetPosition;

    void Start()
    {
        targetPosition = transform.position;
    }

    void Update()
    {
        float newPositionX = Mathf.Lerp(transform.position.x, transform.position.x - moveSpeed * Time.deltaTime, smoothness);
        targetPosition = new Vector3(newPositionX, transform.position.y, transform.position.z);

        transform.position = targetPosition;

        if (cameraTransform.position.x >= transform.position.x + positionBR)
        {
            transform.position = new Vector3(cameraTransform.position.x + positionBR - 23f, transform.position.y, transform.position.z);
        }
    }
}
