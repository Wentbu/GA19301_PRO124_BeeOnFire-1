using System.Collections.Generic;
using UnityEngine;

public class AIData : MonoBehaviour
{
    public List<Transform> targets;
    public Collider2D[] obstacles;

    public Transform currentTarget;
    public int GetTargetsCount() => targets == null ? 0 : targets.Count;
}

public abstract class Detector : MonoBehaviour
{
    public abstract void Detect(AIData aIData);
}

public abstract class SteeringBehaviour : MonoBehaviour
{
    public abstract (float[] danger, float[] interest)
        GetSteering(float[] danger, float[] interest, AIData aiData);
}
