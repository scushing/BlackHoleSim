using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Container for object mass
/// </summary>
public class ObjectProperties : MonoBehaviour
{
    public float mass;

    public virtual float Mass { get { return mass; } }
}
