using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectProperties : MonoBehaviour
{
    public float mass;

    public virtual float Mass { get { return mass; } }
}
