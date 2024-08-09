using System;
using UnityEngine;

public class DestroyNotifier : MonoBehaviour
{
    public static event Action<GameObject> OnObjectDestroyed;

    void OnDestroy()
    {
        OnObjectDestroyed?.Invoke(gameObject);
    }
}
