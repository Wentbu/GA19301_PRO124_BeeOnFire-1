using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxMenu : MonoBehaviour
{
    [SerializeField] private Transform Camera;
    [SerializeField] private float moveSpeed;


    // Update is called once per frame
    void Update()
    {
        transform.Translate(-1 * moveSpeed * Time.deltaTime, 0f, 0f);

        if (Camera.position.x >= transform.position.x + 84f)
        {
            transform.position = new Vector2(Camera.position.x + 84f, transform.position.y);
        }
    }
}
