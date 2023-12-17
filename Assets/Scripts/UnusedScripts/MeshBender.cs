using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEditor.UIElements;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class MeshBender : MonoBehaviour
{
    private Mesh mesh;
    private void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        List<Vector3> new_mesh = new List<Vector3> ();
/*
        for (int i = 0; i < mesh.vertices.Length; i++)
        {
            print(mesh.vertices[i]);
            var t = new Vector3(mesh.vertices[i][0], mesh.vertices[i][1], mesh.vertices[i][2]);
            new_mesh.Add(t);
            print(new_mesh[i]);
        }

        mesh.vertices = new_mesh.ToArray();
        GetComponent <MeshFilter>().mesh = mesh;*/
    }

    public void OnCollisionEnter(Collision collision)
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

        BendMesh(point, velocity, impulse, mass);
    }

    private void BendMesh(Vector3 point, Vector3 velocity, Vector3 impulse, float mass)
    {
        Dictionary<Vector3, bool> closestPoints = FindClosestMeshPoints(point);
        Bend(closestPoints, -impulse);
    }

    private void Bend(Dictionary<Vector3, bool> points, Vector3 impulse)
    {
        List<Vector3> new_mesh = new List<Vector3>();

        foreach (var t in points) 
        {
            Vector3 point = t.Key;
            if (t.Value) 
            {
                point = Vector3.zero + point + impulse ;
            }
            new_mesh.Add(point);
        }

        mesh.vertices = new_mesh.ToArray();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    const float delta = 0.2f;
    private Dictionary<Vector3, bool> FindClosestMeshPoints(Vector3 point)
    {
        Dictionary<Vector3, bool> meshPoints = new Dictionary<Vector3, bool>();

        for (int i = 0; i < mesh.vertices.Length; i++)
        {
            Vector3 dif = (point - mesh.vertices[i]);
            float r = Mathf.Sqrt(Mathf.Pow(dif[0], 2) + Mathf.Pow(dif[1], 2) + Mathf.Pow(dif[2], 2));
            //print("Radius = " + r + " Sqr = " + dif.sqrMagnitude);
            bool isBending = false;
            if (r < delta)
            {
                //print(mesh.vertices[i]);
                var t = new Vector3(mesh.vertices[i][0], mesh.vertices[i][1], mesh.vertices[i][2]);
                isBending = true;
            }
            if (!meshPoints.ContainsKey(mesh.vertices[i]))
                meshPoints.Add(mesh.vertices[i], isBending);
        }
        return meshPoints;
    }
}
