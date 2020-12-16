using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbit : MonoBehaviour
{
    public Transform target;
    public float orbitSpeed;
    Vector3 offset;

    void Start()
    {
        
    }

    void Update()
    {
        transform.RotateAround(target.position,
            Vector3.up,
            orbitSpeed * Time.deltaTime);
    }
}
