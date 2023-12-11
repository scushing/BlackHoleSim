using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles Euler updates for position and velocity of objects
/// </summary>
public class MovingObject : MonoBehaviour
{
    public ObjectProperties center;
    public Transform f2;
    public const float G = 6.67408e-11f;
    Vector3 position;
    Vector3 velocity = new(0.0f, 0.0f, 0.0f);
    void Start()
    {
        position = transform.localPosition;
        velocity = CalculateVelocity(center.transform.localPosition, f2.transform.position, transform.localPosition);
    }

    // Perform Euler Update on the object
    void FixedUpdate()
    {
        position += velocity * Time.deltaTime;
        transform.localPosition = position;
        
        Vector3 toCenter = (center.transform.localPosition - position);
        // Newton's Law of Unversal Gravitation and Newton's 2nd Law
        float acceleration_mag = G * center.Mass / Mathf.Pow(Vector3.Magnitude(toCenter), 2);
        Vector3 acceleration = toCenter.normalized * acceleration_mag;
        velocity += acceleration * Time.deltaTime;
    }

    // Initial velocity for Keplerian Orbit
    private Vector3 CalculateVelocity(Vector3 f1, Vector3 f2, Vector3 position)
    {
        // Normal to elipse circumference at position
        Vector3 elipse_normal = -((f2 - position) + (f1 - position)).normalized;
        // Tangent to elipse circumference at position
        // Only setup to work on xz plane
        Vector3 elipse_tangent = new(-elipse_normal.z, 0, elipse_normal.x);
        // Flips tangent when necessary to ensure speed calculation is correct
        if (Vector3.Dot(elipse_tangent, f1 - position) < 0.0f) {
            elipse_tangent = -elipse_tangent;
        }
        // Semi-major axis of elipse
        float a = (Vector3.Distance(f1, position) + Vector3.Distance(f2, position)) / 2;
        // Velocity formula from Kepler's Laws
        float speed = Mathf.Sqrt(G * center.Mass * (2/Vector3.Magnitude(position - f1) - 1/a));

        return elipse_tangent * speed;
    }
}
