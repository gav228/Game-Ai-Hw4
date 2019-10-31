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
    public float avoidDistance;
    public float lookAhead;

    protected void Start() {
        agent = GetComponent<NPCController>();
        wanderOrientation = agent.orientation;
        timeToTarget = 1;
        lookAhead = 0.1f;
        avoidDistance = 5f;
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

    public Vector3 Separation2()
    {
        Vector3 separationVector = new Vector3(0f, 0f, 0f);
        for (int i = 0; i < flock.Flock2.Count; i++)

        {
            int neighbors = 0;
            if (Vector3.Distance(flock.Flock2[i].transform.position, agent.transform.position) < 5)
            {
                separationVector += Vector3.Normalize((agent.position - flock.Flock2[i].transform.position));
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

    public Vector3 Cohesion2()
    {
        // Check for having arrived
        if (flock.position2[0] - agent.position[0] < 0.2 && flock.position2[2] - agent.position[2] < 0.2)
        {
            return new Vector3(0, 0, 0);
        }

        return Vector3.Normalize(flock.position2 - agent.position);
    }

    public Vector3 VelocityMatch()
    {
        return Vector3.Normalize(flock.velocity);
    }

    public Vector3 VelocityMatch2()
    {
        return Vector3.Normalize(flock.velocity2);
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

    // Calculate the Path to follow
    public Vector3 PathFollow()
    {
        // Setup direction variable
        Vector3 direction = new Vector3(0, 0, 0);

        if (current >= Path.Length)
        {
            return direction;
        }

        // Look for next path location
        if ((Path[current].transform.position - agent.position).magnitude > targetRadiusL)
        {
            // Get the direction to the target
            direction = Path[current].transform.position - agent.position;

            // Give full acceleration along this direction
            direction.Normalize();
            direction *= maxAcceleration;
        }
        else
        {
            //increment path location
            current++;
        }
        return direction;
    }

    public Vector3 ConeCheck()
    {
        // First check all around me
        Collider[] hitColliders = Physics.OverlapSphere(agent.position, 8);
        
        int i = 0;
        while (i < hitColliders.Length)
        {
            if (flock.Flock2.Contains(hitColliders[i].gameObject)) { 
                float angle = Vector3.Angle(agent.transform.forward, hitColliders[i].transform.position - transform.position);

                if (angle < 20.0f)
                {
                    return Vector3.Normalize(agent.position - hitColliders[i].transform.position);
                }
            }
            i++;
        }
        return new Vector3(0, 0, 0);
    }

    public Vector3 ConeCheck2()
    {
        // First check all around me
        Collider[] hitColliders = Physics.OverlapSphere(agent.position, 8);
        
        int i = 0;
        while (i < hitColliders.Length)
        {
            if (flock.Flock.Contains(hitColliders[i].gameObject))
            {
                float angle = Vector3.Angle(agent.transform.forward, hitColliders[i].transform.position - transform.position);

                if (angle < 20.0f)
                {
                    return Vector3.Normalize(agent.position - hitColliders[i].transform.position);
                }
            }
            i++;
        }
        return new Vector3(0, 0, 0);
    }

    public Vector3 CollisionPrediction() // Based off the textbook
    {

        // 1. Find the target that's closest to collision

        // store the first collision time
        float shortestTime = float.MaxValue;

        // Store the target that collides then, and other data
        // that we will need and can avoid recalculating
        GameObject firstTarget = null;
        float firstMinSeparation = 0;
        float firstDistance = 0;
        Vector3 firstRelativePos = Vector3.zero;
        Vector3 firstRelativeVel = Vector3.zero;
        Vector3 relativePos = Vector3.zero;
        Vector3 relativeVel = Vector3.zero;
        float distance = 0;

        // First check all around me
        Collider[] hitColliders = Physics.OverlapSphere(agent.position, 3);

        // Loop through each target
        int i = 0;
        while (i < hitColliders.Length)
        {
            // Check them only if valid obstacle (oposing flock)
            if (flock.Flock2.Contains(hitColliders[i].gameObject))
            {
                // Calculate time to collision
                relativePos = hitColliders[i].gameObject.transform.position - agent.position;
                relativeVel = hitColliders[i].gameObject.gameObject.GetComponent<Rigidbody>().velocity - agent.velocity;
                float relativeSpeed = relativeVel.magnitude;
                float timeToCollision = Vector3.Dot(relativePos, relativeVel) / (relativeSpeed * relativeSpeed);

                // Check if its going to be a collision at all
                distance = relativePos.magnitude;
                float minSeparation = distance - relativeSpeed * shortestTime;
                if (minSeparation > (2 * 5))
                {
                    continue;
                }
                if (timeToCollision > 0 && timeToCollision < shortestTime)
                {
                    shortestTime = timeToCollision;
                    firstTarget = hitColliders[i].gameObject;
                    firstMinSeparation = minSeparation;
                    firstDistance = distance;
                    firstRelativePos = relativePos;
                    firstRelativeVel = relativeVel;
                }

            }
            i++;
        }

        // 2. Calculate the steering

        // if we have no first target return no steering
        if (firstTarget == null)
        {
            return new Vector3(0, 0, 0);
        }

        // If we’re going to hit exactly, or if we’re already
        // colliding, then do the steering based on current
        // position.
        if (firstMinSeparation <= 0 || distance < 2 * 3){
            relativePos = agent.position - firstTarget.transform.position ;
            
        }
        else //Otherwise calculate the future relative position
        {
            relativePos = firstRelativePos + firstRelativeVel * shortestTime;
        }
        
        return Vector3.Normalize(relativePos);



    }

    public Vector3 CollisionPrediction2() // Based off the textbook
    {

        // 1. Find the target that's closest to collision

        // store the first collision time
        float shortestTime = float.MaxValue;

        // Store the target that collides then, and other data
        // that we will need and can avoid recalculating
        GameObject firstTarget = null;
        float firstMinSeparation = 0;
        float firstDistance = 0;
        Vector3 firstRelativePos = Vector3.zero;
        Vector3 firstRelativeVel = Vector3.zero;
        Vector3 relativePos = Vector3.zero;
        Vector3 relativeVel = Vector3.zero;
        float distance = 0;

        // First check all around me
        Collider[] hitColliders = Physics.OverlapSphere(agent.position, 3);

        // Loop through each target
        int i = 0;
        while (i < hitColliders.Length)
        {
            // Check them only if valid obstacle (oposing flock)
            if (flock.Flock.Contains(hitColliders[i].gameObject))
            {
                // Calculate time to collision
                relativePos = hitColliders[i].gameObject.transform.position - agent.position;
                relativeVel = hitColliders[i].gameObject.gameObject.GetComponent<Rigidbody>().velocity - agent.velocity;
                float relativeSpeed = relativeVel.magnitude;
                float timeToCollision = Vector3.Dot(relativePos, relativeVel) / (relativeSpeed * relativeSpeed);

                // Check if its going to be a collision at all
                distance = relativePos.magnitude;
                float minSeparation = distance - relativeSpeed * shortestTime;
                if (minSeparation > (2 * 5))
                {
                    continue;
                }
                if (timeToCollision > 0 && timeToCollision < shortestTime)
                {
                    shortestTime = timeToCollision;
                    firstTarget = hitColliders[i].gameObject;
                    firstMinSeparation = minSeparation;
                    firstDistance = distance;
                    firstRelativePos = relativePos;
                    firstRelativeVel = relativeVel;
                }

            }
            i++;
        }

        // 2. Calculate the steering

        // if we have no first target return no steering
        if (firstTarget == null)
        {
            return new Vector3(0, 0, 0);
        }

        // If we’re going to hit exactly, or if we’re already
        // colliding, then do the steering based on current
        // position.
        if (firstMinSeparation <= 0 || distance < 2 * 3)
        {
            relativePos = agent.position - firstTarget.transform.position;

        }
        else //Otherwise calculate the future relative position
        {
            relativePos = firstRelativePos + firstRelativeVel * shortestTime;
        }

        return Vector3.Normalize(relativePos);



    }
    // ETC.

}
