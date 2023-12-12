using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles Euler updates for position and velocity of objects
/// </summary>
public class MovingObject : MonoBehaviour
{
    public List<ObjectProperties> otherObjects;
    public Transform f2;
    public const float G = 6.67408e-11f;

    Vector3 position;
    Vector3 velocity = new(0.0f, 0.0f, 0.0f);
    void Start()
    {
        position = transform.position;
        foreach (ObjectProperties obj in otherObjects)
        {
            velocity += CalculateVelocity(obj, f2.transform.position, transform.position);
        }
    }

    // Perform Euler Update on the object
    void FixedUpdate()
    {
        position += velocity * Time.deltaTime;
        transform.position = position;

        Vector3 acceleration = new();
        foreach (ObjectProperties obj in  otherObjects)
        {
            Vector3 toCenter = (obj.transform.position - position);
            // Newton's Law of Unversal Gravitation and Newton's 2nd Law
            float acceleration_mag = G * obj.Mass / Mathf.Pow(Vector3.Magnitude(toCenter), 2);
            acceleration += toCenter.normalized * acceleration_mag;
        }
        velocity += acceleration * Time.deltaTime;
    }

    /// <summary>
    /// Calculates initial velocity for Keplerian Orbit
    ///
    /// </summary>
    /// <param name="f1">First focus of elipse, and in this case the object at the center of the orbit</param>
    /// <param name="f2">Second focus of elipse</param>
    /// <param name="position">Starting position of this object</param>
    /// <returns></returns>
    private Vector3 CalculateVelocity(ObjectProperties f1, Vector3 f2, Vector3 position)
    {
        // Normal to elipse circumference at position
        Vector3 elipse_normal = -((f2 - position) + (f1.transform.position - position)).normalized;
        // Tangent to elipse circumference at position
        // Only setup to work on xz plane
        Vector3 elipse_tangent = new(-elipse_normal.z, 0, elipse_normal.x);
        // Flips tangent when necessary to ensure speed calculation is correct
        if (Vector3.Dot(elipse_tangent, f1.transform.position - position) < 0.0f) {
            elipse_tangent = -elipse_tangent;
        }
        // Semi-major axis of elipse
        float a = (Vector3.Distance(f1.transform.position, position) + Vector3.Distance(f2, position)) / 2;
        // Velocity formula from Kepler's Laws
        float speed = Mathf.Sqrt(G * f1.Mass * (2/Vector3.Magnitude(position - f1.transform.position) - 1/a));

        return elipse_tangent * speed;
    }
}
