using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRevoludtion : MonoBehaviour
{
    public GameObject container;
    public Transform center;
    public float dir = 1;

    private Transform containerTransfom;
    private float radius;
    private float tInterval;
    private Vector3 rot;

    // Start is called before the first frame update
    void Start()
    {
        containerTransfom = container.transform;
        radius = containerTransfom.position.magnitude;
        tInterval = 0f;
        rot = new Vector3();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 newPos = new();
        tInterval += Time.deltaTime;
        float theta = dir * (tInterval * Mathf.PI / 20f) % (2 * Mathf.PI);
        newPos.z = Mathf.Cos(theta) * radius;
        newPos.x = Mathf.Sin(theta) * radius;
        container.transform.position = newPos;
        rot.y = theta * 180 / Mathf.PI;
        container.transform.rotation = Quaternion.Euler(rot);
    }
}
