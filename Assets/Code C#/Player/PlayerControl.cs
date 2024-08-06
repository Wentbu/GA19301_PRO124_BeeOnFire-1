using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerControl : MonoBehaviour
{
    [Header("Interface")]
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody2D rb2d;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float interactDistance = 2f;

    private Vector2 movement;
    private List<GameObject> nearbyNPCs = new List<GameObject>();

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Update()
    {
        UpdateAnimation();
    }

    private void OnMove(InputValue value)
    {
        movement = value.Get<Vector2>();
    }

    private void OnInteract()
    {
        if (nearbyNPCs.Count > 0)
        {
            InteractWithNPC(nearbyNPCs[0]);
        }
    }

    private void Move()
    {
        rb2d.MovePosition(rb2d.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    private void UpdateAnimation()
    {
        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
        animator.SetFloat("Speed", movement.sqrMagnitude);
    }

    private void InteractWithNPC(GameObject npc)
    {
        Debug.Log("Đang tương tác với NPC");
        // Thêm logic tương tác với NPC ở đây
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("NPC"))
        {
            nearbyNPCs.Add(other.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("NPC"))
        {
            nearbyNPCs.Remove(other.gameObject);
        }
    }
}