using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPHManager : MonoBehaviour
{
    [Header("Particle Data")]
    public List<SPHParticle> particles = new List<SPHParticle>();
    public GameObject particlePrefab;
    public Material waterMaterial;

    public float smoothingRadius = 1.0f; // how far the particles need to be before they stop interacting
    public float gasConstant = 2f; // How stiff the fluid is - stiffer == less compressible - higher value == stiffer
    public float restDensity = 10f; // Density - Dichte
    public float viscosity = 10f; // Dickflüssigkeit - je höher der Wert, desto niedriger die Dickflüssigkeit
    public Vector3 gravity = new Vector3(0, -9.81f, 0);
    public int particleCount = 124; // Sauberen Würfel instantiieren
    public float particleMass = 1.0f;
    public float particleRadius = 1.0f;
    public float deltaTime = 0.001f; // needs to be small to ensure stability
    public float containerSize = 5.0f;
    public float spacing = 0.5f;

    [Header("Box Data")]
    public Vector3 boxCenter = Vector3.zero; // Center of the box
    public Vector3 boxSize = new Vector3(3f, 5f, 3f); // Size of the box

    [Header("Compute")]
    public ComputeShader shader;
    public Mesh particleMesh;

    private ComputeBuffer _argsBuffer;
    public ComputeBuffer _particlesBuffer;

    private int num = 0;

    // Start is called before the first frame update
    void Start()
    {
        InitializeParticles();
        Debug.Log("Start");
    }

    void InitializeParticles()
	{
        // Starting position for particle placement
        Vector3 startPosition = transform.position;
        // Cube root of particles because we want to distribute them in a 3D-space
        int particlesPerRow = Mathf.CeilToInt(Mathf.Pow(particleCount, 1f / 3f));  // Particles per row for a cube-like structure

        for (int x = 0; x < particlesPerRow; x++)
        {
            for (int y = 0; y < particlesPerRow; y++)
            {
                for (int z = 0; z < particlesPerRow; z++)
                {
                    if (particles.Count >= particleCount) return;  // Stop if we reach the desired count

                    // Calculate position of the new particle
                    Vector3 particlePosition = startPosition + new Vector3(x * spacing, y * spacing, z * spacing);

                    // Instantiate the particle and set its properties
                    GameObject particleObj = Instantiate(particlePrefab, particlePosition, Quaternion.identity);
                    SPHParticle newParticle = particleObj.GetComponent<SPHParticle>();
                    newParticle.GetComponent<Renderer>().material = waterMaterial;
                    // Check if the prefab has the correct material
                    Renderer rend = newParticle.GetComponent<Renderer>();
                    if (rend != null)
                    {
                        rend.material = waterMaterial;  // Ensure the metaball material is assigned
                    }

                    // Set boundaries
                    newParticle.boxCenter = boxCenter;
                    newParticle.boxSize = boxSize;

                    // Initialize particle properties
                    newParticle.position = particlePosition;
                    newParticle.velocity = Vector3.zero;  // Initial velocity is zero
                    newParticle.density = 0;
                    newParticle.pressure = 0;
                    newParticle.mass = 1f;  // You can change the mass value as per the requirement
                    newParticle.radius = particleRadius;

                    // Add the particle to the list
                    particles.Add(newParticle);
                }
            }
        }
    }

    // Draw the boundary box
    private void OnDrawGizmos()
    {
        // Set the color of the Gizmos
        Gizmos.color = Color.red; // You can change the color

        // Calculate the bounds of the box
        Vector3 halfSize = boxSize / 2f;
        Vector3 min = boxCenter - halfSize;
        Vector3 max = boxCenter + halfSize;

        // Draw the wire cube representing the boundaries
        Gizmos.DrawWireCube(boxCenter, boxSize);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float deltaTime = Time.deltaTime;

        // Send particle data to the shader
        /*Vector4[] particlePositions = new Vector4[particles.Count];
        float[] particleRadii = new float[particles.Count];

        for (int i = 0; i < particles.Count; i++)
        {
            particlePositions[i] = particles[i].position;
            particleRadii[i] = particles[i].radius;
        }

        // Pass particle data to the material
        waterMaterial.SetInt("_ParticleCount", particles.Count);
        waterMaterial.SetVectorArray("_ParticlePositions", particlePositions);
        waterMaterial.SetFloatArray("_ParticleRadii", particleRadii);
        */
        // Step 1: Find neighbors and calculate density/pressure
        foreach (SPHParticle particle in particles)
        {
            FindNeighbors(particle);         // Neighbor search
            ComputeDensity(particle);        // Density and pressure calculation
        }

        // Step 2: Compute forces and integrate particles
        foreach (SPHParticle particle in particles)
        {
            ComputeForces(particle);         // Force calculation
            Integrate(particle, deltaTime);  // Update positions and velocities
            HandleCollisions(particle);      // Collision handling with boundaries
        }

    }


    void FindNeighbors(SPHParticle particle)
    {
        particle.neighbors.Clear();

        foreach (SPHParticle other in particles)
        {
            if (other == particle) continue;

            float distance = Vector3.Distance(particle.position, other.position);
            if (distance < smoothingRadius)
            {
                particle.neighbors.Add(other);
            }
        }
    }

    // For each particle, the density is computed by summing up the contributions from neighboring particles
    // using a smoothing kernel (e.g., the Poly6 kernel). The pressure is then calculated based on the
    // density using the equation of state.
    void ComputeDensity(SPHParticle particle)
    {
        float density = 0f;

        foreach (SPHParticle neighbor in particle.neighbors)
        {
            float distance = Vector3.Distance(particle.position, neighbor.position);
            if (distance < smoothingRadius)
            {
                float weight = Poly6Kernel(distance, smoothingRadius);
                density += neighbor.mass * weight;
            }
        }


        particle.density = density;
        particle.pressure = gasConstant * (particle.density - restDensity);  // Equation of state
    }

    float Poly6Kernel(float r, float h)
    {
        if (r >= 0 && r <= h)
        {
            float term = (h * h - r * r);
            return (315.0f / (64.0f * Mathf.PI * Mathf.Pow(h, 9))) * Mathf.Pow(term, 3);
        }
        return 0.0f;
    }

    // Three main forces are computed in SPH:
    // Pressure Force: Particles are pushed apart due to pressure differences.
    // Viscosity Force: Fluid friction that smooths the velocities.
    // External Forces: These include gravity and other external interactions.
    void ComputeForces(SPHParticle particle)
    {
        Vector3 pressureForce = Vector3.zero;
        Vector3 viscosityForce = Vector3.zero;

        foreach (SPHParticle neighbor in particle.neighbors)
        {
            float distance = Vector3.Distance(particle.position, neighbor.position);
            if (distance < smoothingRadius && distance > 0)
            {
                // Pressure force
                float pressureTerm = (particle.pressure + neighbor.pressure) / (2 * neighbor.density);
                Vector3 direction = (particle.position - neighbor.position).normalized;
                pressureForce += -direction * pressureTerm * SpikyKernelGradient(distance, smoothingRadius);

                // Viscosity force
                Vector3 velocityDiff = neighbor.velocity - particle.velocity;
                viscosityForce += (viscosity * (velocityDiff / neighbor.density) * ViscosityKernelLaplacian(distance, smoothingRadius));
            }
        }

        // Apply gravity force
        Vector3 gravityForce = gravity * particle.mass;

        // Sum of all forces
        particle.force = pressureForce + viscosityForce + gravityForce;
    }

    float SpikyKernelGradient(float r, float h)
    {
        if (r >= 0 && r <= h)
        {
            float term = (h - r);
            return -45.0f / (Mathf.PI * Mathf.Pow(h, 6)) * term * term;
        }
        return 0.0f;
    }

    float ViscosityKernelLaplacian(float r, float h)
    {
        if (r >= 0 && r <= h)
        {
            return 45.0f / (Mathf.PI * Mathf.Pow(h, 6)) * (h - r);
        }
        return 0.0f;
    }

    void Integrate(SPHParticle particle, float deltaTime)
    {
        if (particle.density == 0)
		{
            Debug.Log("Particle.density = 0");
            //particle.velocity += deltaTime * particle.force;

        } else
		{
            // Update velocity
            particle.velocity += deltaTime * (particle.force / particle.density);
        }


        // Update position
        particle.position += deltaTime * particle.velocity;

        // Update the transform position (visual update)
        particle.transform.position = particle.position;
    }

    void HandleCollisions(SPHParticle particle)
    {
        Vector3 pos = particle.position;

        // Assuming a box boundary from -containerSize to containerSize in all axes
        if (pos.x < -containerSize || pos.x > containerSize) particle.velocity.x *= -1;
        if (pos.y < -containerSize || pos.y > containerSize) particle.velocity.y *= -1;
        if (pos.z < -containerSize || pos.z > containerSize) particle.velocity.z *= -1;

        // Make sure the particle stays within the boundary
        particle.position = new Vector3(
            Mathf.Clamp(particle.position.x, -containerSize, containerSize),
            Mathf.Clamp(particle.position.y, -containerSize, containerSize),
            Mathf.Clamp(particle.position.z, -containerSize, containerSize)
        );
    }

}
