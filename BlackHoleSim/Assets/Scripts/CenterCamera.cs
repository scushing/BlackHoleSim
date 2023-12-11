using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Points camera towards origin
public class CenterCamera : MonoBehaviour
{
    public Transform center;
    void FixedUpdate()
    {
        this.transform.forward = (center.position - this.transform.position);
    }
}
