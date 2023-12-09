using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObject : MonoBehaviour
{
    // Start is called before the first frame update
    public Singularity singularity;
    public Transform f2;
    public const float G = 6.67408e-11f;
    public Transform self;
    Vector3 position;
    Vector3 velocity = new Vector3(0.0f, 0.0f, 0.0f);
    public float mass = 1.0f;
    void Start()
    {
        position = transform.position;
        velocity = calculateVelocity(singularity.transform.position, f2.transform.position, transform.position);
        //Debug.Log("Velocity: " + velocity);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // perform Euler Updates on the object
        position += velocity * Time.deltaTime;
        Debug.Log("Velcity: " + velocity);

        transform.position = position;
        
        // calculate the force on the object
        Vector3 toCenter = (singularity.transform.position - position);
        float acceleration_mag = G * singularity.Mass / Mathf.Pow(Vector3.Magnitude(toCenter), 2);
        Vector3 acceleration = toCenter.normalized * acceleration_mag;
        velocity += acceleration * Time.deltaTime;
    }

    private Vector3 calculateVelocity(Vector3 f1, Vector3 f2, Vector3 position)
    {
        Vector3 v1, v2;
        v1 = f1 - position;
        v2 = f2 - position;

        // if (Vector3.Magnitude(v2 - v1) < 0.01f) {
        //     v1.x += 0.01f;
        //     v2.y += 0.01f;
        // }
        // Vector3 normal = Vector3.Cross(v1, v2);
        // normal = normal.normalized;
        // Debug.Log("Normal: " + normal);

        // Vector2 pos2D = ConvertTo2DCoordinates(position, f1, normal);
        // Vector2 f1_2D = ConvertTo2DCoordinates(f1, f1, normal);
        // Vector2 f2_2D = ConvertTo2DCoordinates(f2, f1, normal);
        // Debug.Log("Position 2D: " + pos2D);
        // Debug.Log("F1 2D: " + f1_2D);
        // Debug.Log("F2 2D: " + f2_2D);

        Vector3 elipse_normal = -((f2 - position) + (f1 - position)).normalized;
        Vector3 elipse_tangent = new Vector3(-elipse_normal.z, 0, elipse_normal.x);
        if (Vector3.Dot(elipse_tangent, f1 - position) < 0.0f) {
            elipse_tangent = -elipse_tangent;
        }
        Debug.Log("Elipse Tangent: " + elipse_tangent);

        float a = Vector3.Distance(f1, position) + Vector3.Distance(f2, position);
        float speed = Mathf.Sqrt(G * singularity.Mass * (2/Vector3.Magnitude(position - f1) - 1/a));

        // elipse_tangent_3D = ConvertTo3DCoordinates(elipse_tangent, f1, normal);
        Debug.Log("Elipse Tangent 3D: " + elipse_tangent);
        Debug.Log("Speed: " + speed);
        return elipse_tangent * speed;
    }
    public static Vector2 ConvertTo2DCoordinates(Vector3 position, Vector3 planeOrigin, Vector3 planeNormal)
    {
        // Calculate the basis vectors of the plane
        Vector3 u, v;
        if (Mathf.Abs(planeNormal.z) > Mathf.Sqrt(2.0f)/2.0f) {
            u = new Vector3(planeNormal.y, -planeNormal.x, 0.0f);
        } else {
            u = new Vector3(0.0f, planeNormal.z, -planeNormal.y);
        }

        u = u.normalized;
        v = Vector3.Cross(planeNormal, u);

        // Find the 2D coordinates of the point in the plane
        Vector3 d = position - planeOrigin;
        float x = Vector3.Dot(d, u);
        float y = Vector3.Dot(d, v);

        return new Vector2(x, y);
    }

    public static Vector3 ConvertTo3DCoordinates(Vector2 position, Vector3 planeOrigin, Vector3 planeNormal)
    {
        // Calculate the basis vectors of the plane
        Vector3 u, v;
        if (Mathf.Abs(planeNormal.z) > Mathf.Sqrt(2.0f)/2.0f) {
            u = new Vector3(planeNormal.y, -planeNormal.x, 0.0f);
        } else {
            u = new Vector3(0.0f, planeNormal.z, -planeNormal.y);
        }

        u = u.normalized;
        v = Vector3.Cross(planeNormal, u);

        // Find the 3D coordinates of the point in the plane
        Vector3 point = planeOrigin + u * position.x + v * position.y;
        return point;
    }
}
