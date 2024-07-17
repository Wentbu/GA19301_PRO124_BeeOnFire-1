using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeacherChasePlayer : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] Rigidbody2D rb2d;
    [SerializeField] private GameObject player;
    [SerializeField] private Vector2 movement;
    [SerializeField] private float KhoangCach;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float KhoangCachVoiNguoiChoi;

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        KhoangCach = Vector2.Distance(transform.position, player.transform.position);
        movement = player.transform.position - transform.position;
        movement.Normalize();
        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
        animator.SetFloat("Speed", movement.sqrMagnitude);
        float ToaDo = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg / -180f;

        if (KhoangCach < KhoangCachVoiNguoiChoi)
        {
            animator.SetFloat("Horizontal", movement.x);
            animator.SetFloat("Vertical", movement.y);
            animator.SetFloat("Speed", movement.sqrMagnitude);
            transform.position = Vector2.MoveTowards(this.transform.position, player.transform.position, moveSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(Vector3.forward * ToaDo);
        }
    }
    private void FixedUpdate()
    {
        rb2d.MovePosition(rb2d.position + movement * moveSpeed * Time.fixedDeltaTime);
    }
}
