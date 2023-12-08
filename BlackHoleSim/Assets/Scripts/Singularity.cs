using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singularity : MonoBehaviour
{
    [SerializeField]
    private float SchwarzschildRadius = 1.0f;
    private float SpeedOfLight = 299792458f;
    private float GravitationalConst = 6.6743f * Mathf.Pow(10, -11);

    public float GetSchwarzschildRadius { get { return SchwarzschildRadius; } }

    public float Mass { get { return (SchwarzschildRadius * Mathf.Pow(SpeedOfLight, 2) / (2 * GravitationalConst)); } }
}
