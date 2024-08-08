using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObject : MonoBehaviour
{
    public GameObject targetObject;
    public Vector3 offset = Vector3.zero;

    private void Start()
    {
        DestroyNotifier.OnObjectDestroyed += HandleTargetDestroyed;
    }

    void OnDisable()
    {
        DestroyNotifier.OnObjectDestroyed -= HandleTargetDestroyed;
    }

    void Update()
    {
        if (targetObject != null)
        {
            transform.position = targetObject.transform.position + offset;
        }
    }

    void HandleTargetDestroyed(GameObject destroyedObject)
    {
        if (destroyedObject == targetObject)
        {
            Destroy(gameObject);
        }
    }
}
