using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;

public class Intersection : MonoBehaviour
{
    public LayerMask carLayerMask;
    public float intersectionWaitTime = 1f; // Time for cars to wait at the intersection

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Car"))
        {
            StartCoroutine(HandleCarAtIntersection(other));
        }
    }

    private IEnumerator HandleCarAtIntersection(Collider2D car)
    {
        // Wait for 1 second before processing
        yield return new WaitForSeconds(intersectionWaitTime);

        // Check if the car is still in the intersection
        if (IsCarInIntersection(car))
        {
            // Determine the order in which cars should go
            DetermineCarPriority();
        }
    }

    private bool IsCarInIntersection(Collider2D car)
    {
        // Check if the car is within the intersection bounds
        // Assuming the intersection has a BoxCollider2D to represent its area
        Collider2D intersectionCollider = GetComponent<Collider2D>();
        return intersectionCollider.IsTouching(car);
    }

    private void DetermineCarPriority()
    {
        // Retrieve all cars currently in the intersection
        Collider2D[] carsInIntersection = Physics2D.OverlapBoxAll(transform.position, transform.localScale, 0, carLayerMask);

        // Sort cars based on their entry time into the intersection
        List<Collider2D> sortedCars = new List<Collider2D>(carsInIntersection);
        sortedCars.Sort((car1, car2) => car1.GetComponent<Car>().entryTime.CompareTo(car2.GetComponent<Car>().entryTime));

        // Allow cars to proceed based on their priority
        foreach (Collider2D car in sortedCars)
        {
            car.GetComponent<Patrol>().AllowToProceed();
        }
    }
}

public class Car : MonoBehaviour
{
    [SerializeField] public float stopDuration = 1f; // Time for the car to stop at the intersection
    [SerializeField] public float detectionDistance = 2f;
    [SerializeField] public LayerMask carLayerMask;

    private IAstarAI agent;
    [SerializeField] public float entryTime;
    private bool isWaiting = false;

    private void Awake()
    {
        agent = GetComponent<IAstarAI>();
    }

    private void Update()
    {
        if (isWaiting)
        {
            return; // Skip movement while waiting
        }

        // Existing code to handle movement and slowing down
        

        
    }
    private void SlowDown()
    {
        agent.maxSpeed = agent.maxSpeed * 0.5f; // Slow down factor
    }

    private void ResumeNormalSpeed()
    {
        agent.maxSpeed = agent.maxSpeed / 0.5f; // Resume original speed
    }
    
    public void StopAtIntersection(float waitTime)
    {
        entryTime = Time.time;
        isWaiting = true;
        agent.isStopped = true;
        Invoke("AllowToProceed", waitTime); // Allow the car to proceed after wait time
    }

    public void AllowToProceed()
    {
        isWaiting = false;
        agent.isStopped = false;
        // Resume normal movement
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Intersection"))
        {
            StopAtIntersection(other.GetComponent<Intersection>().intersectionWaitTime);
        }
    }
}
