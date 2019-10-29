using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is the place to put all of the various steering behavior methods we're going
/// to be using. Probably best to put them all here, not in NPCController.
/// </summary>

public class SteeringBehavior : MonoBehaviour {

    // The agent at hand here, and whatever target it is dealing with
    public NPCController agent;
    public NPCController target;
    public FieldMapManager flock;

    // Below are a bunch of variable declarations that will be used for the next few
    // assignments. Only a few of them are needed for the first assignment.

    // For pursue and evade functions
    public float maxPrediction;
    public float maxAcceleration;

    // For arrive function
    public float maxSpeed;
    public float targetRadiusL;
    public float slowRadiusL;
    public float timeToTarget;

    // For Face function
    public float maxRotation;
    public float maxAngularAcceleration;
    public float targetRadiusA;
    public float slowRadiusA;

    // For wander function
    public float wanderOffset;
    public float wanderRadius;
    public float wanderRate;
    private float wanderOrientation;

    // Holds the path to follow
    public GameObject[] Path;
    public int current = 0;

    protected void Start() {
        agent = GetComponent<NPCController>();
        wanderOrientation = agent.orientation;
        timeToTarget = 1;
    }

    public float mapToRange(float rotation)
    {
        while (rotation > Mathf.PI)
        {
            rotation -= 2 * Mathf.PI;
        }
        while (rotation < -Mathf.PI)
        {
            rotation += 2 * Mathf.PI;
        }
        return rotation;
    }

    public Vector3 Seek() {
        return new Vector3(0f, 0f, 0f);
    }

    public Vector3 Flee()
    {
        return new Vector3(0f, 0f, 0f);
    }


    // Calculate the target to pursue
    public Vector3 Pursue() {
        return new Vector3(0f, 0f, 0f);
    }

    public float Face()
    {
        return 0f;
    }

    public Vector3 Separation()
    {
        Vector3 separationVector = new Vector3(0f, 0f, 0f);
        for (int i = 0; i < flock.Flock.Count; i++)

        {
            int neighbors = 0;
            if (Vector3.Distance(flock.Flock[i].transform.position, agent.transform.position) < 5)
            {
                separationVector += Vector3.Normalize((agent.position - flock.Flock[i].transform.position));
                neighbors++;
            }
            //Debug.Log(neighbors);
        }
        return Vector3.Normalize(separationVector);
    }

    public Vector3 Cohesion()
    {
        // Check for having arrived
        if (flock.position[0] - agent.position[0] < 0.2 && flock.position[2] - agent.position[2] < 0.2)
        {
            return new Vector3(0, 0, 0);
        }

        return Vector3.Normalize(flock.position - agent.position);
    }

    public Vector3 VelocityMatch()
    {
        return Vector3.Normalize(flock.velocity);
    }

    // Face places
    public float Face_Where_Im_Going(Vector3 linear)
    {
        Vector3 direction = linear;

        // Check for a zero direction, and make no change if so
        if (direction.magnitude == 0)
        {
            return 0;
        }

        // Get the naive direction to the target
        float rotation = Mathf.Atan2(direction.x, direction.z) - agent.orientation;

        // Map the result to the 
        rotation = mapToRange(rotation);
        float rotationSize = Mathf.Abs(rotation);

        // Check if we are there, return no steering
        if (rotationSize < targetRadiusA)
        {
            agent.rotation = 0;
        }

        float targetRotation = maxRotation;

        // The final target rotation 
        targetRotation *= rotation / rotationSize;

        // Acceleration tries to get to the target rotation
        float angular = targetRotation - agent.rotation;
        angular /= timeToTarget;

        // Check if the acceleration is too great
        float angularAcceleration = Mathf.Abs(angular);
        if (angularAcceleration > maxAngularAcceleration)
        {
            angular /= angularAcceleration;
            angular *= maxAngularAcceleration;
        }

        return angular;
    }

    // ETC.

}
