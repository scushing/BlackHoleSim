using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObject : MonoBehaviour
{
    // Start is called before the first frame update
    public ObjectProperties center;
    public Transform f2;
    public const float G = 6.67408e-11f;
    Vector3 position;
    Vector3 velocity = new Vector3(0.0f, 0.0f, 0.0f);
    public float mass = 1.0f;
    void Start()
    {
        position = transform.localPosition;
        velocity = CalculateVelocity(center.transform.localPosition, f2.transform.position, transform.localPosition);
    }

    // perform Euler Update on the object
    void FixedUpdate()
    {
        position += velocity * Time.deltaTime;

        transform.localPosition = position;
        
        // calculate the force on the object
        Vector3 toCenter = (center.transform.localPosition - position);
        float acceleration_mag = G * center.Mass / Mathf.Pow(Vector3.Magnitude(toCenter), 2);
        Vector3 acceleration = toCenter.normalized * acceleration_mag;
        velocity += acceleration * Time.deltaTime;
    }

    // Initial velocity for Keplerian Orbit
    private Vector3 CalculateVelocity(Vector3 f1, Vector3 f2, Vector3 position)
    {
        Vector3 elipse_normal = -((f2 - position) + (f1 - position)).normalized;
        Vector3 elipse_tangent = new(-elipse_normal.z, 0, elipse_normal.x);
        if (Vector3.Dot(elipse_tangent, f1 - position) < 0.0f) {
            elipse_tangent = -elipse_tangent;
        }
        Debug.Log("Mass: " + center.Mass);

        float a = (Vector3.Distance(f1, position) + Vector3.Distance(f2, position)) / 2;
        float speed = Mathf.Sqrt(G * center.Mass * (2/Vector3.Magnitude(position - f1) - 1/a));

        return elipse_tangent * speed;
    }
}
