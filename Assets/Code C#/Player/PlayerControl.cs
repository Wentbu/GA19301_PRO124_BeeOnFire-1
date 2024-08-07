using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Collections;

public class PlayerControl : MonoBehaviour
{

    [Header("Interface")]
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody2D rb2d;

    [Header("Movement Settings")]
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float acceleration = 50f;
    [SerializeField] private float deceleration = 100f;
    [SerializeField] private float velocityPower = 0.9f;
    [SerializeField] private float frictionAmount = 0.2f;

    [Header("NPC Interaction")]
    [SerializeField] private float interactDistance = 2f; //Khoảng cách để tương tác với npc (nếu cần)

    private Vector2 movement;
    private Vector2 smoothedMovement;
    
    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        SmoothMovement();
        Move();
        ApplyFriction();
    }

    private void Update()
    {
        UpdateAnimation();
    }

    private void OnMove(InputValue value)
    {
        movement = value.Get<Vector2>();
    }

    private void SmoothMovement()
    {
        smoothedMovement = Vector2.Lerp(smoothedMovement, movement, Time.fixedDeltaTime * 10f);
    }

    private void Move()
    {
        // Tính toán vận tốc tối đa
        Vector2 targetVelocity = smoothedMovement * maxSpeed;

        // Tính toán vận tốc hiện tại và tối đa
        Vector2 velocityDiff = targetVelocity - rb2d.velocity;

        // Tính toán lực gia tốc
        Vector2 accelerationForce = velocityDiff * acceleration;

        // Áp dụng lực vào Rigidbody2D
        rb2d.AddForce(accelerationForce);

        // Giới hạn tốc độ tối đa
        rb2d.velocity = Vector2.ClampMagnitude(rb2d.velocity, maxSpeed);
    }

    private void ApplyFriction()
    {
        // Áp dụng ma sát khi người chơi bỏ tay ra khỏi nút bấm
        if (movement.magnitude < 0.01f)
        {
            float frictionForce = Mathf.Min(rb2d.velocity.magnitude, frictionAmount);
            rb2d.AddForce(-rb2d.velocity.normalized * frictionForce, ForceMode2D.Impulse);
        }
    }

    private void UpdateAnimation()
    {
        animator.SetFloat("Horizontal", smoothedMovement.x);
        animator.SetFloat("Vertical", smoothedMovement.y);
        animator.SetFloat("Speed", smoothedMovement.sqrMagnitude);
    }

}