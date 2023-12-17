using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CapsuleTest : MonoBehaviour
{
    [SerializeField, Range(1, 1000)]
    float power = 200f;

    void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.AddForce(Vector3.right * power);
    }
}
