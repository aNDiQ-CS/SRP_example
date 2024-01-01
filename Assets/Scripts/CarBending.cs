using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;

public class CarBending : MonoBehaviour
{
    [SerializeField]
    private float maxMoveDelta = 0.1f; // maximum distance one vertice moves per explosion (in meters)\
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

    private Dictionary<MeshFilter, float> MeshHP = new Dictionary<MeshFilter, float>();

    private void Start()
    {
        foreach (Transform child in transform)
        {
            child.AddComponent<MeshCollider>();
            child.GetComponent<MeshCollider>().convex = true;
        }

        if (MeshList.Count == 0)
            MeshList = new List<MeshFilter>(GetComponentsInChildren<MeshFilter>());

        foreach (MeshFilter mf in MeshList)
        {
            MeshHP.Add(mf, 100f);
            float value;
            MeshHP.TryGetValue(mf, out value);
        }

        Physics.IgnoreLayerCollision(7, 8, true);
    }

    private void OnCollisionEnter(Collision collision)
    {
        MeshFilter meshFilter = collision.GetContact(0).thisCollider.transform.GetComponent<MeshFilter>();
        GetPhysicsInputs(meshFilter, collision);
        CheckHP(meshFilter, collision);
    }    

    private void GetPhysicsInputs(MeshFilter meshFilter, Collision collision)
    {
        Mesh mesh = meshFilter.mesh;

        Vector3 collisionPoint = collision.GetContact(0).point - transform.position;
        Vector3 velocity = collision.rigidbody.velocity;
        Vector3 impulse = collision.impulse;

        float velocityScalar = CalculateDistance(velocity, Vector3.zero);
        float impulseScalar = CalculateDistance(impulse, Vector3.zero);
        float collisionForce = impulseScalar * 10f / velocityScalar;
        MeshHP[meshFilter] -= collisionForce;


        Bend(mesh, collisionPoint, collisionForce);
    }
    
    private void Bend(Mesh mesh, Vector3 collisionPoint, float collisionForce)
    {
        List<Vector3> meshVertices = new List<Vector3>(mesh.vertices);
        List<Vector3> newMesh = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>(mesh.normals);

        for (int i = 0; i < meshVertices.Count; i++)
        {
            Vector3 carPartVertex = meshVertices[i];
            Vector3 carPartNormal = normals[i];

            Vector3 newVertex = CalculateBending(collisionPoint, carPartVertex, collisionForce, carPartNormal);
            newMesh.Add(newVertex);
        }

        

        mesh.vertices = newMesh.ToArray();
    }

    const float max_dif = 0.1f;

    // Calculates new Bending of a point based on physics
    private Vector3 CalculateBending(Vector3 collisionPoint, Vector3 carPartVertex, float collisionForce, Vector3 carPartNormal)
    {
        if (collisionForce < 10f)
            return carPartVertex;        

        Vector3 startVertex = collisionPoint;
        float delta = collisionForce / 500f;
        float r = CalculateDistance(collisionPoint, carPartVertex);

        if (r < delta)
        {
            carPartVertex -= carPartNormal * Mathf.Clamp(delta - r, 0f, max_dif);            
        }            
        return carPartVertex;
    }

    private float CalculateDistance(Vector3 p1, Vector3 p2)
    {
        Vector3 dif = p1 - p2;
        return Mathf.Sqrt(Mathf.Pow(dif[0], 2) + Mathf.Pow(dif[1], 2) + Mathf.Pow(dif[2], 2));
    }

    private void CheckHP(MeshFilter meshFilter, Collision collision)
    {
        float hp;
        MeshHP.TryGetValue(meshFilter, out hp);
        if (hp <= 0f)
        {
            Transform tr = collision.GetContact(0).thisCollider.transform;
            tr.parent = null;
            tr.AddComponent<Rigidbody>();
            tr.gameObject.layer = 8;            
        }
    }
}
