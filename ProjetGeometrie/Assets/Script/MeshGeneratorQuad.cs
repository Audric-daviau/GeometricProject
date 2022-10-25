using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MeshGeneratorQuad : MonoBehaviour
{
    MeshFilter m_Mf;

    void Start()
    {
        m_Mf = GetComponent<MeshFilter>();
        m_Mf.mesh = CreateRegularPolygon(new Vector3(4, 1, 4), 6);
    }

    Mesh CreateStrip(int nSegments, Vector3 halfSize)
    {
        Mesh mesh = new Mesh();
        mesh.name = "strip";

        Vector3[] vertices = new Vector3[(nSegments + 1) * 2];
        int[] quads = new int[nSegments * 4];

        int index = 0;
        Vector3 leftTopPos = new Vector3(-halfSize.x, 0, halfSize.z);
        Vector3 rightTopPos = new Vector3(halfSize.x, 0, halfSize.z);

        // 1 boucle for pour remplir vertices
        for (int i = 0; i < nSegments + 1; i++)
        {
            float k = (float)i / nSegments;

            Vector3 tmpPos = Vector3.Lerp(leftTopPos, rightTopPos, k);
            vertices[index++] = tmpPos; // vertice du haut
            vertices[index++] = tmpPos - 2 * halfSize.z * Vector3.forward; // vertice du bas
        }
        // 1 boucle for pour remplir triangles
        index = 0;
        for (int i = 0; i < nSegments; i++)
        {
            quads[index++] = 2 * i;
            quads[index++] = 2 * i + 2;
            quads[index++] = 2 * i + 3;
            quads[index++] = 2 * i + 1;
        }

        mesh.vertices = vertices;
        mesh.SetIndices(quads, MeshTopology.Quads, 0);

        return mesh;
    }

    /*
    Mesh CreateGridXZ(int nSegmentsX,int nSegmentsZ, Vector3 halfSize)
    {
        Mesh mesh = new Mesh();
        mesh.name = "grid";

        Vector3[] vertices = new Vector3[???];
        int[] quads = new int[?????];

        // ???????????

        mesh.vertices = vertices;
        mesh.SetIndices(quads,MeshTopology.Quads,0);

        return mesh;
    }
*/

    private void OnDrawGizmos()
    {
        if (!(m_Mf && m_Mf.mesh)) return;

        Mesh mesh = m_Mf.mesh;
        Vector3[] vertices = mesh.vertices;
        int[] quads = mesh.GetIndices(0);

        GUIStyle style = new GUIStyle();
        style.fontSize = 15;
        style.normal.textColor = Color.red;

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 worldPos = transform.TransformPoint(vertices[i]);
            Handles.Label(worldPos, i.ToString(), style);
        }

        Gizmos.color = Color.black;
        style.normal.textColor = Color.blue;

        for (int i = 0; i < quads.Length / 4; i++)
        {
            int index1 = quads[4 * i];
            int index2 = quads[4 * i + 1];
            int index3 = quads[4 * i + 2];
            int index4 = quads[4 * i + 3];

            Vector3 pt1 = transform.TransformPoint(vertices[index1]);
            Vector3 pt2 = transform.TransformPoint(vertices[index2]);
            Vector3 pt3 = transform.TransformPoint(vertices[index3]);
            Vector3 pt4 = transform.TransformPoint(vertices[index4]);

            Gizmos.DrawLine(pt1, pt2);
            Gizmos.DrawLine(pt2, pt3);
            Gizmos.DrawLine(pt3, pt4);
            Gizmos.DrawLine(pt4, pt1);

            string str = string.Format("{0} ({1},{2},{3},{4})",
                i, index1, index2, index3, index4);

            Handles.Label((pt1 + pt2 + pt3 + pt4) / 4.0f, str, style);

        }
    }

    string ConvertToCSV(string separator)
    {
        if (!(m_Mf && m_Mf.mesh)) return "";

        Vector3[] vertices = m_Mf.mesh.vertices;
        int[] quads = m_Mf.mesh.GetIndices(0);

        List<string> strings = new List<string>();

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 pos = vertices[i];
            strings.Add(i.ToString() + separator
                + pos.x.ToString("N03") + " "
                + pos.y.ToString("N03") + " "
                + pos.z.ToString("N03") + separator + separator);
        }

        for (int i = vertices.Length; i < quads.Length / 4; i++)
            strings.Add(separator + separator + separator);

        for (int i = 0; i < quads.Length / 4; i++)
        {
            strings[i] += i.ToString() + separator
                + quads[4 * i + 0].ToString() + ","
                + quads[4 * i + 1].ToString() + ","
                + quads[4 * i + 2].ToString() + ","
                + quads[4 * i + 3].ToString();
        }

        return "Vertices" + separator + separator + separator + "Faces\n"
            + "Index" + separator + "Position" + separator + separator +
            "Index" + separator + "Indices des vertices\n"
            + string.Join("\n", strings);

        /* Mesh CreateNormalizedGridXZ(int nSegmentsX, int nSegmentsZ, ComputePosDelegate computePos = null)
         {
             Mesh mesh = new Mesh();
             mesh.name = "normalizedGrid";

             Vector3[] vertices = new Vector3[(nSegmentsX + 1) * (nSegmentsZ + 1)];
             int[] quads = new int[nSegmentsX * nSegmentsZ * 4];

             //Vertices
             int index = 0;
             for (int i = 0; i < nSegmentsZ + 1; i++)
             {
                 float kZ = (float)i / nSegmentsZ;

                 for (int j = 0; j < nSegmentsX + 1; j++)
                 {
                     float kX = (float)j / nSegmentsX;
                     vertices[index++] = computePos != null ? computePos(kX, kZ) : new Vector3(kX, 0, kZ);
                 }
             }

             index = 0;
             //Quads
             for (int i = 0; i < nSegmentsZ; i++)
             {
                 for (int j = 0; j < nSegmentsX; j++)
                 {
                     quads[index++] = i * (nSegmentsX + 1) + j;
                     quads[index++] = (i + 1) * (nSegmentsX + 1) + j;
                     quads[index++] = (i + 1) * (nSegmentsX + 1) + j + 1;
                     quads[index++] = i * (nSegmentsX + 1) + j + 1;
                 }
             }

             mesh.vertices = vertices;
             mesh.SetIndices(quads, MeshTopology.Quads, 0);

             return mesh;
         }*/


        /*void Start()
        {
            m_Mf = GetComponent<MeshFilter>();
            //m_Mf.mesh = CreateStrip(7, new Vector3(4, 1, 3));
            //m_Mf.mesh = CreateNormalizedGridXZ(7, 4);

            //Cylindre
            *//* m_Mf.mesh = CreateNormalizedGridXZ(20, 40,
                 (kX, kZ) =>
                 {
                     float rho, theta, y;

                     // coordinates mapping de (kX,kZ) -> (rho,theta,y)
                     theta = kX * 2 * Mathf.PI;
                     y = kZ * 6;
                     //rho = 3 + .25f * Mathf.Sin(kZ*2*Mathf.PI*4) ;
                     rho = m_Profile.Evaluate(kZ) * 2;
                     return new Vector3(rho * Mathf.Cos(theta), y, rho * Mathf.Sin(theta));
                     //return new Vector3(Mathf.Lerp(-1.5f, 5.5f, kX), 1, Mathf.Lerp(-2, 4, kZ));
                 }
                 );
            *//*

            // Sph�re
            m_Mf.mesh = CreateNormalizedGridXZ(10, 5,
                (kX, kZ) =>
                {
                    float rho, theta, phi;

                    // coordinates mapping de (kX,kZ) -> (rho,theta,phi)
                    theta = kX * 2 * Mathf.PI;
                    phi = kZ * Mathf.PI;
                    rho = 2 + .55f * Mathf.Cos(kX * 2 * Mathf.PI * 8)
                                    * Mathf.Sin(kZ * 2 * Mathf.PI * 6);
                    //rho = 3 + .25f * Mathf.Sin(kZ*2*Mathf.PI*4) ;
                    //rho = m_Profile.Evaluate(kZ) * 2;

                    return new Vector3(rho * Mathf.Cos(theta) * Mathf.Sin(phi),
                        rho * Mathf.Cos(phi),
                        rho * Mathf.Sin(theta) * Mathf.Sin(phi));
                    //return new Vector3(Mathf.Lerp(-1.5f, 5.5f, kX), 1, Mathf.Lerp(-2, 4, kZ));
                }
                );

            GUIUtility.systemCopyBuffer = ConvertToCSV("\t");
            Debug.Log(ConvertToCSV("\t"));
        }*/
    }
    Mesh CreateQuad(Vector3 halfSize)
    {
        Mesh mesh = new Mesh();
        mesh.name = "quad";

        Vector3[] vertices = new Vector3[4];
        int[] triangles = new int[2 * 3];

        vertices[0] = new Vector3(-halfSize.x, 0, -halfSize.z);
        vertices[1] = new Vector3(-halfSize.x, 0, halfSize.z);
        vertices[2] = new Vector3(halfSize.x, 0, halfSize.z);
        vertices[3] = new Vector3(halfSize.x, 0, -halfSize.z);

        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 2;

        triangles[3] = 0;
        triangles[4] = 2;
        triangles[5] = 3;

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        return mesh;
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

        // 1 boucle for pour remplir triangles

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

    
    Mesh CreateChips(Vector3 halfSize)
    {
        Mesh mesh = new Mesh();
        mesh.name = "chips";

        int nSegments = 6; //car 6 faces
        Vector3[] vertices = new Vector3[8]; //Code raga
        
        int[] quads = new int[nSegments * 2];

        //Regarder s'il y a pas une boucle possible pour faire l'initialisation des vertices ci dessous 
        vertices[0] = new Vector3(-halfSize.x, 0, -halfSize.z); //A
        vertices[1] = new Vector3(-halfSize.x, 0, halfSize.z); //B
        vertices[2] = new Vector3(halfSize.x, 0, halfSize.z); //C
        vertices[3] = new Vector3(halfSize.x, 0, -halfSize.z); //D

        vertices[4] = new Vector3(-halfSize.x, 3, -halfSize.z);
        vertices[5] = new Vector3(-halfSize.x, 3, halfSize.z);
        vertices[6] = new Vector3(halfSize.x, 3, halfSize.z);
        vertices[7] = new Vector3(halfSize.x, 3, -halfSize.z);

        // 1 boucle for pour remplir triangles

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

        mesh.vertices = vertices;
        mesh.SetIndices(quads, MeshTopology.Quads, 0);

        return mesh;
    }

    Mesh CreateRegularPolygon(Vector3 halfSize, int nSectors)
    {
        Mesh mesh = new Mesh();
        mesh.name = "regularPolygon";

        int nbVertices = 2*nSectors ; 
        Vector3[] vertices = new Vector3[nbVertices+1];
        vertices[nSectors*2] = new Vector3(0, 0, 0);       
        int[] quads = new int[nSectors * 4];

        for (int i = 0; i <= nbVertices; i+=2)
        {
            vertices[i] = new Vector3(Mathf.Cos(i*Mathf.PI / nSectors) * halfSize.x, 0, 
                                      Mathf.Sin(i*Mathf.PI / nSectors) * halfSize.z);
        }

        for(int i = 1; i < nbVertices; i+=2)
        {
            vertices[i] = Vector3.Lerp(vertices[i-1], vertices[i+1], 0.5f);
        }
        
        vertices[nbVertices] = Vector3.Lerp(vertices[0], vertices[nbVertices/2], 0.5f);

        // 1 boucle for pour remplir les quads
        int index = 0;
        for (int i = 0; i < nSectors ; i++)
        {
            quads[index++] = nbVertices ; //Pour relier à la vertice central

            if(i != 0)
            {
                quads[index++] = 2 * i + 1;
                quads[index++] = 2 * i;
                quads[index++] = 2 * i - 1;
            }
            else
            {
                quads[index++] = 1 ;
                quads[index++] = 0 ;
                quads[index++] = nbVertices - 1 ;
            }           
        }

        mesh.vertices = vertices;
        mesh.SetIndices(quads, MeshTopology.Quads, 0);

        return mesh;
    }

    /*
    Mesh CreatePacman(Vector3 halfSize, int nSectors, float startAngle =Mathf.PI/3, float endAngle = 5*Mathf.PI / 3)
    {

        mesh.vertices = vertices;
        mesh.SetIndices(quads, MeshTopology.Quads, 0);

        return mesh;        
    }*/
}