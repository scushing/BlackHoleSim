using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhsyicsSim : MonoBehaviour
{

    public GameObject sphereGameObject;

    // User-defined public variables.
    public float mass = 0.1f;
    public float scale = 1;

    private List<IForce> _forces;
    private List<Matter> _spheres;

    // Start is called before the first frame update
    void Start()
    {
        _forces = new List<IForce>();
        _spheres = FindObjectOfType<Matter>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {

    }
}
public class Sphere
{
    public float Mass;
    public float Scale;
    public Vector3 Position;
    public Vector3 Velocity;
    public GameObject SphereGameObject;

    public Sphere(float mass, float scale, Vector3 position, Vector3 velocity, GameObject sphere)
    {
        Mass = mass;
        Scale = scale;
        Position = position;
        Velocity = velocity;
        SphereGameObject = Object.Instantiate(sphere, Position, Quaternion.identity);
        PhysicsSimulation.SetWorldScale(sphere.transform, new Vector3(scale, scale, scale));
    }
}

public interface IForce
{
    public abstract Vector3 GetForce(Sphere p);
}

public class ConstantForce : IForce
{
    private Vector3 _force;

    public ConstantForce(Vector3 force)
    {
        _force = force;
    }

    public Vector3 GetForce(Sphere p)
    {
        return _force;
    }

    public void SetForce(Vector3 force)
    {
        _force = force;
    }
}
