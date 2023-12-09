using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class RayVisualization : MonoBehaviour
{
    public Transform dir;
    public Singularity singularity;
    public float stepSize;
    public float maxSteps;

    const float speedOfLight = 299792458;
    const float gravitationalConst = 0.000000000066743f;
    private void OnDrawGizmos()
    {
        // Origin
        Gizmos.color = Color.green;
        Vector3 currentPos = transform.position;
        Vector3 currentDir = Vector3.Normalize(dir.transform.position - transform.position);
        Vector2 intersection = RaySphereIntersection(5 * singularity.GetSchwarzschildRadius,
            singularity.transform.position, currentPos, currentDir);
        if (intersection.x == Mathf.Infinity)
        {
            // Ray misses effect sphere
            return;
        }
        Gizmos.DrawLine(currentPos, currentPos + currentDir * -intersection.y);
        currentPos += currentDir * -intersection.y;

        float deltaT = stepSize / speedOfLight;
        // Perform ray marching loop
        for (int i = 0; i < maxSteps; i++)
        {
            // Update ray position
            Gizmos.DrawLine(currentPos, currentPos + currentDir * stepSize);
            currentPos = currentPos + currentDir * stepSize;
            Gizmos.DrawSphere(currentPos, 0.05f);
            // Check for collision with event horizon
            Vector2 eventHorizonCollision = RaySphereIntersection(singularity.GetSchwarzschildRadius, singularity.transform.position, currentPos, currentDir);
            Vector2 effectRadiusCollision = RaySphereIntersection(5 * singularity.GetSchwarzschildRadius, singularity.transform.position, currentPos, currentDir);
            if (effectRadiusCollision.x > 0)
            {
                // Ray left effect range. Return ray
                Gizmos.DrawLine(currentPos, currentDir * 20);
                return;
            }
            // Get forces and update direction
            float dist = Vector3.Magnitude(currentPos - singularity.transform.position);
            float accelerationMagnitude = 0.5f / (dist * dist);
            Vector3 acceleration = Vector3.Normalize(singularity.transform.position - currentPos) * accelerationMagnitude;
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
