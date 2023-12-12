using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Points camera towards defined center
/// </summary>
public class CenterCamera : MonoBehaviour
{
    public Transform center;
    void FixedUpdate()
    {
        this.transform.forward = (center.position - this.transform.position);
    }
}
