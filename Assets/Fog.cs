using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Fog : MonoBehaviour
{
    public VisualEffect vfxRenderer;

    public GameObject target; // GameObject B mà chúng ta muốn theo dõi
    public float speed = 5f; // Tốc độ di chuyển
    public float minDistance = 1f; // Khoảng cách tối thiểu với target

    private void Awake()
    {
        target = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        if (target == null)
        {
            Debug.LogWarning("Target not set for follower!");
            return;
        }

        // Tính khoảng cách đến target
        float distance = Vector3.Distance(transform.position, target.transform.position);

        // Nếu khoảng cách lớn hơn minDistance, di chuyển về phía target
        if (distance > minDistance)
        {
            // Tính hướng đến target
            Vector3 direction = (target.transform.position - transform.position).normalized;

            // Di chuyển về phía target
            transform.position += direction * speed * Time.deltaTime;

            vfxRenderer.SetVector3("ColliderPos", transform.position);
        }
    }
}
