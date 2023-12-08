using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereController : MonoBehaviour
{
    public GameObject self;
    public GameObject center;

    private double G;
    private double mass;
    private Vector3 velocity;

    // Start is called before the first frame update
    void Start()
    {
        G = 6.67408 * Mathf.Pow(10, -11);
        mass = 1.0f;
        velocity = Vector3.zero;
    }

    // Update is called once per frame
    //void FixedUpdate()
    //{
    //    Vector3 acceleration = GetForces() / ((float)mass);
    //    this.transform.position += this.velocity * Time.deltaTime;
    //    this.velocity += acceleration * Time.deltaTime;
    //}

    //private Vector3 GetForces()
    //{
    //    float otherMass = 100000.0f;
    //    Vector3 pos = other.transform.position;
    //    Vector3 dir = pos - this.transform.position;
    //    float dist = Vector3.Magnitude(dir);
    //    dir = dir.normalized;
    //    double magnitude = this.mass * otherMass * this.G / Mathf.Pow(dist, 2);
    //    return dir * ((float)magnitude);
    //}
}
