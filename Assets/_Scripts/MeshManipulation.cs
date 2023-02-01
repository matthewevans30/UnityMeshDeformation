using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshManipulation : MonoBehaviour
{
    //Mesh parameter
    Mesh mesh;
    Vector3[] vertices;
    ParticleSystem ps;
    ParticleSystem.Particle[] particles;

    //Distortion parameters
    float[] distortionHeights;

    //Perlin customizations
    public float perlinScale = 1f;
    public float distortionScale = 2f;
    public float perlinStartX = 0f;
    public float perlinStartY = 0f;
    public float perlinSpeed = 1f;

    public Vector2 heightDistortionMinMax;
    public AudioAnalysis Analyzer;

    void Start() {
        mesh = GetComponent<MeshFilter>().mesh;
        vertices = mesh.vertices;

        ps = GetComponent<ParticleSystem>();
        particles = new ParticleSystem.Particle[vertices.Length];
        distortionHeights = new float[vertices.Length];

        Vector3 position;
        ParticleSystem.EmitParams ep = new ParticleSystem.EmitParams();

        for (int i = 0; i < vertices.Length; i++) {
            position = vertices[i];
            ep.position = position;
            ps.Emit(ep, 1);
        }

        ps.GetParticles(particles);
    }

    // Update is called once per frame
    void Update()
    {
        GetTargetHeights();
        UpdateParticlePositions();
        ps.SetParticles(particles);
    }

    public void UpdateParticlePositions() {
        for (int i = 0; i < vertices.Length; i++) {
            particles[i].position = vertices[i] * (distortionHeights[i]);
            print(distortionHeights[i]);
        }
    }

    public void GetTargetHeights() {
        for(int i = 0; i < vertices.Length; i++) {
            float height = Mathf.PerlinNoise((perlinStartX + i) * perlinScale, (perlinStartY + i) * perlinScale);
            height = Remap(height, 0, 1, heightDistortionMinMax.x, heightDistortionMinMax.y);

            distortionHeights[i] = height;
        }
        perlinStartX += perlinSpeed;
        perlinStartY += perlinSpeed;
    }

    public float Remap(float value, float fromA, float toA, float fromB, float toB) {
        float normal = Mathf.InverseLerp(fromA, toA, value);
        float newValue = Mathf.Lerp(fromB, toB, normal);
        return newValue;
    }
}
