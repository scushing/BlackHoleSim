using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Points camera towards origin
public class CenterCamera : MonoBehaviour
{
    void FixedUpdate()
    {
        this.transform.forward = -this.transform.position;
    }
}
