using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterSurface : MonoBehaviour
{
    private Mesh mesh;
    private Vector3[] vertices;

    public float waveHeight = 0.1f;
    public float waveFrequency = 1.0f;

    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        vertices = mesh.vertices;
    }

    void Update()
    {
        AnimateWaterSurface();
    }

    void AnimateWaterSurface()
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            float x = vertices[i].x;
            float z = vertices[i].z;

            // Calculate the wave effect
            vertices[i].y = Mathf.Sin(Time.time * waveFrequency + x + z) * waveHeight;
        }

        mesh.vertices = vertices;
        mesh.RecalculateNormals();
    }
}

