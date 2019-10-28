using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockManager
{
    private List<GameObject> Flock;
    private Vector3 velocity;
    public Vector3 position;

    // Start is called before the first frame update
    void Start()
    {
        Flock = new List<GameObject>();
    }

    // Update calculates velocity and position
    void Update()
    {
        // Get average position

        float sum_x = 0;
        float sum_y = 0;
        float sum_z = 0;
        
        // Sum values
        for (int i = 0; i<Flock.Count; i++)
        {
            sum_x += Flock[i].gameObject.transform.position.x;
            sum_y += Flock[i].gameObject.transform.position.y;
            sum_z += Flock[i].gameObject.transform.position.z;
        }

        // Divide
        sum_x = sum_x / Flock.Count;
        sum_y = sum_y / Flock.Count;
        sum_z = sum_z / Flock.Count;

        // Set average position
        position = new Vector3(sum_x, sum_y, sum_z);

        // Get average velocity

        sum_x = 0;
        sum_y = 0;
        sum_z = 0;

        // Sum values
        for (int i = 0; i < Flock.Count; i++)
        {
            sum_x += Flock[i].gameObject.GetComponent<Rigidbody>().velocity.x;
            sum_y += Flock[i].gameObject.GetComponent<Rigidbody>().velocity.y;
            sum_z += Flock[i].gameObject.GetComponent<Rigidbody>().velocity.z;
        }

        // Divide
        sum_x = sum_x / Flock.Count;
        sum_y = sum_y / Flock.Count;
        sum_z = sum_z / Flock.Count;

        // Set average position
        velocity = new Vector3(sum_x, sum_y, sum_z);

        Debug.Log(position);
        Debug.Log(velocity);


    }

    public void Add(GameObject boid)
    {
        Flock.Add(boid);
    }

    public Vector3 GetVelocity()
    {
        return velocity;
    }

    public Vector3 GetPosition()
    {
        return position;
    }
}
