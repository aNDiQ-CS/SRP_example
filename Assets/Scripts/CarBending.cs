using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;

public class CarBending : MonoBehaviour
{
    [SerializeField]
    private float maxMoveDelta = 0.5f; // maximum distance one vertice moves per explosion (in meters)\
    [SerializeField]
    private float maxCollisionStrength = 50.0f;
    [SerializeField, Range(0f, 1f)] 
    private float YforceDamp = 0.1f;
    [SerializeField] 
    private float demolutionRange = 0.5f;
    [SerializeField] 
    private float impactDirManipulator = 0.0f;

    [SerializeField]
    private List<MeshFilter> MeshList; 

    private void Start()
    {
        foreach (Transform child in transform)
        {
            child.AddComponent<MeshCollider>();
            child.GetComponent<MeshCollider>().convex = true;
        }

        if (MeshList.Count == 0)
            MeshList = new List<MeshFilter>(GetComponentsInChildren<MeshFilter>());
    }

    private void OnCollisionEnter(Collision collision)
    {
        Mesh mesh = collision.GetContact(0).thisCollider.transform.GetComponent<MeshFilter>().mesh;
        GetPhysicsInputs(mesh, collision);
    }

    private void GetPhysicsInputs(Mesh mesh, Collision collision)
    {
        print(collision.transform.name);
        Vector3 point = collision.GetContact(0).point - transform.position;
        Vector3 velocity = collision.rigidbody.velocity;
        Vector3 impulse = collision.impulse;
        float mass = collision.rigidbody.mass;

        print("Point - " + point);
        print("Velocity - " + velocity);
        print("Impulse - " + impulse);
        print("Mass - " + mass);
        
        Bend(mesh, mesh.vertices, -impulse, collision);
    }
    
    private void Bend(Mesh mesh, Vector3[] points, Vector3 impulse, Collision collision)
    {
        List<Vector3> new_mesh = new List<Vector3>();

        for (int i = 0; i < mesh.vertices.Length; i++)
        {
            Vector3 point = points[i];
            Vector3 carPartPoint = mesh.vertices[i];

            Vector3 newPoint = CalculateBending(point, carPartPoint, impulse);
            float rs = CalculateDistance(point, newPoint);
            if (rs > max_dif)
            {
                Transform tr = collision.GetContact(0).thisCollider.transform;
                tr.parent = null;
                return;
            }
            new_mesh.Add(newPoint);
        }

        mesh.vertices = new_mesh.ToArray();
    }

    const float delta = 0.2f, max_dif = 0.5f;

    // Calculates new Bending of a point based on physics
    private Vector3 CalculateBending(Vector3 point, Vector3 carPartPoint, Vector3 impulse)
    {                
        Vector3 startPoint = point;
        float r = CalculateDistance(point, carPartPoint);
        if (r < delta)
        {
            point += impulse;
            
        }
            
        return point;
    }

    private float CalculateDistance (Vector3 p1, Vector3 p2)
    {
        Vector3 dif = p1 - p2;
        return Mathf.Sqrt(Mathf.Pow(dif[0], 2) + Mathf.Pow(dif[1], 2) + Mathf.Pow(dif[2], 2));
    }
}
