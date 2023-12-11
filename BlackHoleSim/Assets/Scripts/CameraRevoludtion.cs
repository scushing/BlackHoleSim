using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Rotates camera around center
/// </summary>
public class CameraRevoludtion : MonoBehaviour
{
    public Transform center;
    public float dir = 1;

    private float radius;
    private float tInterval;

    // Start is called before the first frame update
    void Start()
    {
        radius = (transform.position - center.transform.position).magnitude;
        tInterval = 0f;
    }

    void FixedUpdate()
    {
        Vector3 newPos = new();
        tInterval += Time.deltaTime;
        float theta = dir * (tInterval * Mathf.PI / 20f) % (2 * Mathf.PI);
        newPos.z = Mathf.Cos(theta) * radius;
        newPos.x = Mathf.Sin(theta) * radius;
        newPos.y = transform.position.y;
        transform.position = newPos;
    }
}
