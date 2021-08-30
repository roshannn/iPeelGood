using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FruitController : MonoBehaviour
{
    public SceneLoader sceneLoader;
    public GameObject levelOver;
    public Button restartButton;
    public float rotationPerSecond;

    private void Start()
    {
        restartButton.onClick.AddListener(sceneLoader.RestartLevel);
    }
    void Update()
    {
        RotateFruit();
        //GetInput();
        Peel();
        LevelOver();

    }

    private void LevelOver()
    {
        if(GetComponent<MeshFilter>().mesh.triangles.Length < 2000)
        {
            levelOver.SetActive(true);
            
        }
    }

    private void Peel()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            RaycastHit raycastHit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out raycastHit, Mathf.Infinity))
            {
                DeleteTriangle(raycastHit.triangleIndex);
            }
        }
        if (Input.GetKey(KeyCode.Mouse1))
        {
            //Debug.Log("Input Detected");
            RaycastHit raycastHit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out raycastHit, Mathf.Infinity))
            {
                int hitTri1 = raycastHit.triangleIndex;
                int[] triangles = GetComponent<MeshFilter>().mesh.triangles;
                Vector3[] vertices = GetComponent<MeshFilter>().mesh.vertices;
                Vector3 p0 = vertices[triangles[hitTri1 * 3 + 0]];
                Vector3 p1 = vertices[triangles[hitTri1 * 3 + 1]];
                Vector3 p2 = vertices[triangles[hitTri1 * 3 + 2]];

                float edge1 = Vector3.Distance(p0, p1);
                float edge2 = Vector3.Distance(p0, p2);
                float edge3 = Vector3.Distance(p1, p2);

                Vector3 shared1;
                Vector3 shared2;
                if (edge1 > edge2 && edge1 > edge3)
                {
                    shared1 = p0;
                    shared2 = p1;
                }
                else if (edge2 > edge1 && edge2 > edge3)
                {
                    shared1 = p0;
                    shared2 = p2;
                }
                else
                {
                    shared1 = p1;
                    shared2 = p2;
                }

                int v1 = FindVertex(shared1);
                int v2 = FindVertex(shared2);

                DeleteSquare(hitTri1,FindTriangle(vertices[v1],vertices[v2], hitTri1));
            }

        }
    }

    private int FindTriangle(Vector3 v1, Vector3 v2, int notTriIndex)
    {
        int[] triangles = GetComponent<MeshFilter>().mesh.triangles;
        Vector3[] vertices = GetComponent<MeshFilter>().mesh.vertices;
        int j = 0;
        while (j < triangles.Length)
        {
            if (j / 3 != notTriIndex)
            {
                if (vertices[triangles[j]] == v1 && (vertices[triangles[j + 1]] == v2 || vertices[triangles[j + 2]] == v2))
                {
                    return j / 3;
                }
                else if (vertices[triangles[j]] == v2 && (vertices[triangles[j + 1]] == v1 || vertices[triangles[j + 2]] == v1))
                {
                    return j / 3;
                }
                else if (vertices[triangles[j + 1]] == v2 && (vertices[triangles[j]] == v1 || vertices[triangles[j + 2]] == v1))
                {
                    return j / 3;
                }
                else if (vertices[triangles[j + 1]] == v1 && (vertices[triangles[j]] == v2 || vertices[triangles[j + 2]] == v2))
                {
                    return j / 3;
                }
            }
            j += 3;
        }
        return -1;
    }

    private int FindVertex(Vector3 v)
    {
        Vector3[] vertices = GetComponent<MeshFilter>().mesh.vertices;
        for(int i = 0; i < vertices.Length; i++)
        {
            if (vertices[i] == v)
            {
                return i;
            }
        }
        return -1;
    }

    private void DeleteTriangle(int triangleIndex)
    {
        Destroy(this.GetComponent<MeshCollider>());
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        int[] oldTriangles = mesh.triangles;
        int[] newTriangles = new int[mesh.triangles.Length - 3];

        int i = 0;
        int j = 0;
        while (j < mesh.triangles.Length)
        {
            if (j != triangleIndex * 3)
            {
                newTriangles[i++] = oldTriangles[j++];
                newTriangles[i++] = oldTriangles[j++];
                newTriangles[i++] = oldTriangles[j++];
            }
            else
            {
                j += 3;
            }
        }
        GetComponent<MeshFilter>().mesh.triangles = newTriangles;
        this.gameObject.AddComponent<MeshCollider>();
    }
    private void DeleteSquare(int triangleIndex1, int triangleIndex2)
    {
        Destroy(this.GetComponent<MeshCollider>());
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        int[] oldTriangles = mesh.triangles;
        int[] newTriangles = new int[mesh.triangles.Length - 6];

        int i = 0;
        int j = 0;
        while (j < mesh.triangles.Length)
        {
            if (j != triangleIndex1 * 3 && j != triangleIndex2 * 3)
            {
                newTriangles[i++] = oldTriangles[j++];
                newTriangles[i++] = oldTriangles[j++];
                newTriangles[i++] = oldTriangles[j++];
            }
            else
            {
                j += 6;
            }
        }
        mesh.triangles = newTriangles;
        this.gameObject.AddComponent<MeshCollider>();
    }



    private void RotateFruit()
    {
        transform.Rotate(0, rotationPerSecond * Time.deltaTime, 0);
    }
}
