using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Visualization and debugging tool for ray marching.
/// </summary>
public class RayVisualization : MonoBehaviour
{
    public Transform dir;
    public Singularity singularity;

    // Approximation used with Schwarzschild radius instead of mass. This works because they scale
    // linearly, and is done to avoid use of astronomical values that cause floating point errors.
    // Actual gravitational constant is 0.000000000066743
    public float productOfConst = 1.1f;
    public float stepSize;
    public float maxSteps;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        float effectRadius = 8 * singularity.GetSchwarzschildRadius;
        Vector3 currentPos = transform.position;
        Vector3 currentDir = Vector3.Normalize(dir.transform.position - transform.position);
        Vector2 intersection = RaySphereIntersection(effectRadius,
            singularity.transform.position, currentPos, currentDir);
        if (intersection.x == Mathf.Infinity)
        {
            // Ray misses effect sphere
            Gizmos.DrawLine(currentPos, dir.position);
            return;
        }
        Gizmos.DrawLine(currentPos, currentPos + currentDir * -intersection.y);
        // Steps ray forward to effect radius
        currentPos += currentDir * -intersection.y;
        // Perform ray marching loop
        for (int i = 0; i < maxSteps; i++)
        {
            // Update ray position
            Gizmos.DrawLine(currentPos, currentPos + currentDir * stepSize);
            currentPos += currentDir * stepSize;
            Gizmos.DrawSphere(currentPos, 0.05f);
            Vector2 effectRadiusCollision = RaySphereIntersection(effectRadius, singularity.transform.position, currentPos, currentDir);
            if (effectRadiusCollision.x > 0)
            {
                // Ray left effect range. Return ray
                Gizmos.DrawLine(currentPos, currentPos + currentDir * 20);
                return;
            }
            // Get forces and update direction
            float dist = Vector3.Magnitude(currentPos - singularity.transform.position);
            // See shader for logic behind these values
            float accelerationMagnitude = productOfConst * singularity.GetSchwarzschildRadius / (dist * dist);
            Vector3 acceleration = Vector3.Normalize(singularity.transform.position - currentPos) * accelerationMagnitude;
            // Using step size instead of delta time to reduce chance of floating point errors
            currentDir = Vector3.Normalize(currentDir + acceleration * stepSize);
        }
    }

    private Vector2 RaySphereIntersection(float radius, Vector3 center, Vector3 rayOrigin, Vector3 rayDirection)
    {
        // Vector from sphere center to ray origin
        Vector3 sphereToRay = center - rayOrigin;

        // Coefficients for quadratic equation
        float a = Vector3.Dot(rayDirection, rayDirection);
        float b = 2 * Vector3.Dot(rayDirection, sphereToRay);
        float c = Vector3.Dot(sphereToRay, sphereToRay) - (radius * radius);

        float discriminant = b * b - 4 * a * c;

        // No intersection; ray misses
        if (discriminant < 0)
        {
            return new Vector2(Mathf.Infinity, 0);
        }
        // Single point of intersection
        else if (discriminant < 0.01)
        {
            return new Vector2(-b / (2 * a), 0);
        }
        // Ray passes through sphere
        else
        {
            // Handle negative values after funtion returns
            float distToSphere = (-b - Mathf.Sqrt(discriminant)) / (2 * a);
            float distThroughSphere = (-b + Mathf.Sqrt(discriminant)) / (2 * a);
            return new Vector2(distToSphere, distThroughSphere);
        }
    }
}
