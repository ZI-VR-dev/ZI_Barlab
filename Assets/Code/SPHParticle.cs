using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPHParticle : MonoBehaviour
{
    // Physical state of the particle
    public Vector3 velocity = Vector3.zero;  // Initial velocity (can be zero or predefined)
    public float density = 0.0f;             // Will be calculated in the simulation
    public float pressure = 0.0f;            // Will be calculated from the equation of state
    public Vector3 force = Vector3.zero;     // Forces will be calculated and applied in each time step
    public Vector3 position = Vector3.zero;  // Set during initialization
    public float mass = 1.0f;                // Constant mass for the particle
    public float radius = 1.0f;

    public List<SPHParticle> neighbors = new List<SPHParticle>();  // List of neighbors within the smoothing radius

    public Vector3 boxCenter;
    public Vector3 boxSize;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        velocity += force * Time.deltaTime;

        CheckBoundaries();

        position += velocity * Time.deltaTime;

        transform.position = position;
    }

    private void CheckBoundaries()
    {
        // Calculate the half extents of the box
        Vector3 halfSize = boxSize / 2f;

        // Clamp position within the box boundaries
        position.x = Mathf.Clamp(position.x, boxCenter.x - halfSize.x, boxCenter.x + halfSize.x);
        position.y = Mathf.Clamp(position.y, boxCenter.y - halfSize.y, boxCenter.y + halfSize.y);
        position.z = Mathf.Clamp(position.z, boxCenter.z - halfSize.z, boxCenter.z + halfSize.z);

        // Optional: Reflect the velocity if the particle hits a wall
        if (position.x == boxCenter.x - halfSize.x || position.x == boxCenter.x + halfSize.x)
        {
            velocity.x = -velocity.x;  // Reverse the x-velocity
        }
        if (position.y == boxCenter.y - halfSize.y || position.y == boxCenter.y + halfSize.y)
        {
            velocity.y = -velocity.y;  // Reverse the y-velocity
        }
        if (position.z == boxCenter.z - halfSize.z || position.z == boxCenter.z + halfSize.z)
        {
            velocity.z = -velocity.z;  // Reverse the z-velocity
        }
    }
}
