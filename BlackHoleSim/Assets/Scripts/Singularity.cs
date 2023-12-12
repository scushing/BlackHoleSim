using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Conatiner for Schwarzchild radius and calculates mass from it
/// </summary>
public class Singularity : ObjectProperties
{
    [SerializeField]
    private float SchwarzschildRadius = 1.0f;
    private float SpeedOfLight = 299792458f;

    // Gravitational Constant is increased here to reduce mass of singularity, allowing slower and closer orbits
    // which are easier to see. Actual is 6.6743f * Mathf.Pow(10, -11);
    private float GravitationalConst = 0.39f;

    public float GetSchwarzschildRadius { get { return SchwarzschildRadius; } }

    public override float Mass { get { return (SchwarzschildRadius * Mathf.Pow(SpeedOfLight, 2) / (2 * GravitationalConst)); } }
}
