using System.Collections;
using System.Text;
using System;
using System.Collections.Generic;
using HalfEdge;
using UnityEngine;
using System.IO;

public class Test_HE : MonoBehaviour
{
    // Start is called before the first frame update
    MeshFilter m_Mf;
    //Ce fichier permet de tester la classe HalfEdge
    void Start()
    {
        m_Mf = GetComponent<MeshFilter>();
        m_Mf.mesh = CreateBox(new Vector3(4, 1, 4));
        HalfEdgeMesh m_half = new HalfEdgeMesh(m_Mf.mesh);
        GUIUtility.systemCopyBuffer = m_half.ConvertToCSVFormat();

    }
    Mesh CreateBox(Vector3 halfSize)
    {
        Mesh mesh = new Mesh();
        mesh.name = "box";

        int nSegments = 6; //car 6 faces
        Vector3[] vertices = new Vector3[8]; //Code raga

        // Vector3[] vertices = new Vector3[(nSegments + 1) * 2]; code strip
        int[] quads = new int[nSegments * 4];

        //Regarder s'il y a pas une boucle possible pour faire l'initialisation des vertices ci dessous 
        vertices[0] = new Vector3(-halfSize.x, 0, -halfSize.z); //A
        vertices[1] = new Vector3(-halfSize.x, 0, halfSize.z); //B
        vertices[2] = new Vector3(halfSize.x, 0, halfSize.z); //C
        vertices[3] = new Vector3(halfSize.x, 0, -halfSize.z); //D

        vertices[4] = new Vector3(-halfSize.x, 3, -halfSize.z);
        vertices[5] = new Vector3(-halfSize.x, 3, halfSize.z);
        vertices[6] = new Vector3(halfSize.x, 3, halfSize.z);
        vertices[7] = new Vector3(halfSize.x, 3, -halfSize.z);

        // 1 boucle for pour remplir les triangles

        quads[0] = 0;
        quads[1] = 1;
        quads[2] = 2;
        quads[3] = 3;

        quads[4] = 0;
        quads[5] = 1;
        quads[6] = 5;
        quads[7] = 4;

        quads[8] = 3;
        quads[9] = 2;
        quads[10] = 6;
        quads[11] = 7;

        quads[12] = 0;
        quads[13] = 3;
        quads[14] = 7;
        quads[15] = 4;

        quads[16] = 1;
        quads[17] = 2;
        quads[18] = 6;
        quads[19] = 5;

        quads[20] = 4;
        quads[21] = 5;
        quads[22] = 6;
        quads[23] = 7;

        mesh.vertices = vertices;
        mesh.SetIndices(quads, MeshTopology.Quads, 0);

        return mesh;
    }
    // Update is called once per frame
}
