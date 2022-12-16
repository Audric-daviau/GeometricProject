using System.Collections.Generic;
using UnityEditor;
using System;
using System.IO;
using UnityEngine;

namespace HalfEdge
{
    public class HalfEdge
    {
        public int index;
        public Vertex sourceVertex;
        public Face face;
        public HalfEdge prevEdge;
        public HalfEdge nextEdge;
        public HalfEdge twinEdge;
    }
    public class Vertex
    {
        public int index;
        public Vector3 position;
        public HalfEdge outgoingEdge;
    }
    public class Face
    {
        public int index;
        public HalfEdge edge;
    }
    public class HalfEdgeMesh
    {
        public List<Vertex> vertices;
        public List<HalfEdge> edges;
        public List<Face> faces;
        public HalfEdgeMesh(Mesh mesh)
        { 
            vertices = new List<Vertex>();
            edges = new List<HalfEdge>();
            faces = new List<Face>();

            Vector3[] vertPos = mesh.vertices;
            int[] quads = mesh.GetIndices(0);

            //Ajout des vertices : indices, position
            for(int i = 0; i < mesh.vertexCount; i++)
            {
                Vertex v = new Vertex();
                v.index = i;
                v.position = vertPos[i];
                vertices.Add(v);
            }

            //Liste des edges pour ajouter les edges au fur et à mesure qu'on les créee
            Dictionary<ulong, HalfEdge> dicoEdges = new Dictionary<ulong, HalfEdge>();
            HalfEdge hf;
            int cpt = 0;
            //Ajout des faces : index
            while(cpt < quads.Length / 4)
            {
                Face f = new Face();
                f.index = faces.Count;
                faces.Add(f);
                int[] quad_index = new int[4];
                for(int j = 0; j < 4; j++)
                {
                    quad_index[j] = quads[4 * cpt + j];
                }

                HalfEdge prevEdge = null;
                //Ajout des edges : index, sourceVertex, face, twin edge, previous edge, next edge
                for(int j = 0; j < quad_index.Length; j++)
                {
                    int begin = quad_index[j];
                    int end = quad_index[(j + 1) % 4];

                    ulong k = (ulong)Mathf.Min(begin, end) + ((ulong)Mathf.Max(begin, end) << 32);
                    HalfEdge newEdge = null;
                    //on vérifie si l'edge a déjà été crée et ajouté dans la liste
                    if(dicoEdges.TryGetValue(k, out hf))
                    {
                        newEdge = new HalfEdge();
                        newEdge.index = edges.Count;
                        newEdge.sourceVertex = vertices[begin];
                        newEdge.face = f;
                        newEdge.twinEdge = hf;
                        edges.Add(newEdge);
                        hf.twinEdge = newEdge;
                    }
                    else
                    {
                        newEdge = new HalfEdge();
                        newEdge.index = edges.Count;
                        newEdge.sourceVertex = vertices[begin];
                        newEdge.face = f;
                        edges.Add(newEdge);
                        dicoEdges.Add(k, newEdge);
                    }
                    if(f.edge == null)
                    {
                        f.edge = newEdge;
                    }
                    if(vertices[begin].outgoingEdge == null)
                    {
                        vertices[begin].outgoingEdge = newEdge;
                    }
                    if(prevEdge != null)
                    {
                        newEdge.prevEdge = prevEdge;
                        prevEdge.nextEdge = newEdge;
                    }

                    if(j == 3)
                    {
                        newEdge.nextEdge = edges[edges.Count - 4];
                        edges[edges.Count - 4].prevEdge = newEdge;
                    }
                    prevEdge = newEdge;
                
                }
                cpt++;
            }

        }
        public Mesh ConvertToFaceVertexMesh()
        {
            Mesh faceVertexMesh = new Mesh();

            List<Vertex> vertices = this.vertices;
            List<HalfEdge> edges = this.edges;
            List<Face> faces = this.faces;

            Vector3[] vertPos = new Vector3[vertices.Count];
            int[] quads = new int[faces.Count * 4];

            //position des vertices
            for (int i = 0; i < vertices.Count; i++)
            {
                vertPos[i] = vertices[i].position;
            }

            int index = 0;

            //ajout des index qui représentent les quads du mesh
            for (int i = 0; i < faces.Count; i++)
            {
                quads[index] = edges[index++].sourceVertex.index;
                quads[index] = edges[index++].sourceVertex.index;
                quads[index] = edges[index++].sourceVertex.index;
                quads[index] = edges[index++].sourceVertex.index;
            }

            faceVertexMesh.vertices = vertPos;
            faceVertexMesh.SetIndices(quads, MeshTopology.Quads, 0);

            return faceVertexMesh;
        }
        public string ConvertToCSVFormat(string separator = "\t")
        {
            if (this == null) return "";
            string csv = "";
            List<string> str = new List<string>();

            // Les vertices avec les index, positions et outgoing edges
            for (int i = 0; i < vertices.Count; i++)
            {
                Vector3 pos = vertices[i].position;
                str.Add(vertices[i].index + separator + pos.x.ToString() + " "+ pos.y.ToString() + " " + pos.z.ToString() + separator + vertices[i].outgoingEdge.index
                + separator + separator);
            }

            for (int i = vertices.Count; i < edges.Count; i++)
            {
                str.Add(separator + separator + separator + separator);
            }

            //Les indices, sources vertex, face, previous edge et next edge pour chaque halfedges
            for (int i = 0; i < edges.Count; i++)
            {
                str[i] += edges[i].index + separator + edges[i].sourceVertex.index + separator + edges[i].face.index + separator + edges[i].prevEdge.index + separator
                + edges[i].nextEdge.index + separator + $"{(edges[i].twinEdge != null ? $"{edges[i].twinEdge.index}" : "NULL")}" + separator + separator;
            }

            //Les indices et edges des faces
            for (int i = 0; i < faces.Count; i++)
            {
                List<int> edgesIndex = new List<int>();
                List<int> vertexIndex = new List<int>();
                str[i] += faces[i].index + separator + faces[i].edge.index + separator + string.Join(" ", edgesIndex) + separator + string.Join(" ", vertexIndex) + separator + separator;
            }

            // entêtes des colonnes
            csv = "Vertex" + separator + separator + separator + separator + "HalfEges" + separator + separator + separator + separator + separator + separator + separator + "Faces\n"
                + "Index" + separator + "Position" + separator + "outgoingEdge" + separator + separator + "Index" + separator + "sourceVertex" + separator + "Face" + separator + "prevEdge" + separator + "nextEdge" + separator + "twinEdge" + separator + separator +
                "Index" + separator + "Edge\n"
                + string.Join("\n", str);

            // écriture dans un fichier CSV
            string filename = Application.dataPath + "/HalfEdge.csv";
            TextWriter tw = new StreamWriter(filename, false);
            tw.WriteLine(csv);
            tw.Close();

            //afficher dans la console unity
            Debug.Log(csv);
            return csv;
        }

        public void DrawGizmos(bool drawVertices, bool drawEdges, bool drawFaces, Transform transform)
        {
            List<Vertex> vertices = this.vertices;
            List<HalfEdge> edges = this.edges;
            List<Face> faces = this.faces;
            List<Vector3> fp = new List<Vector3>();
            Mesh mesh = this.ConvertToFaceVertexMesh();
            int[] quads = mesh.GetIndices(0);

            Gizmos.color = Color.black;
            GUIStyle style = new GUIStyle();
            style.fontSize = 12;

            //affichage des vertices
            if (drawVertices)
            {
                style.normal.textColor = Color.red;
                for (int i = 0; i < vertices.Count; i++)
                {
                    Vector3 worldPos = transform.TransformPoint(vertices[i].position);
                    Handles.Label(worldPos, vertices[i].index.ToString(), style);
                }
            }

            //affichage des faces
            if (drawFaces)
            {
                style.normal.textColor = Color.green;
                for (int i = 0; i < faces.Count; i++)
                {
                    int i1 = quads[4 * i];
                    int i2 = quads[4 * i + 1];
                    int i3 = quads[4 * i + 2];
                    int i4 = quads[4 * i + 3];

                    Vector3 pt1 = transform.TransformPoint(vertices[i1].position);
                    Vector3 pt2 = transform.TransformPoint(vertices[i2].position);
                    Vector3 pt3 = transform.TransformPoint(vertices[i3].position);
                    Vector3 pt4 = transform.TransformPoint(vertices[i4].position);

                    Handles.Label((pt1 + pt2 + pt3 + pt4) / 4.0f, faces[i].index.ToString(), style);
                    fp.Add((pt1 + pt2 + pt3 + pt4) / 4.0f);
                }
            }

            //affichage des edges
            if (drawEdges)
            {
                style.normal.textColor = Color.blue;
                foreach (var edge in edges)
                {
                    Vector3 c = fp[edge.face.index];
                    Vector3 s = edge.sourceVertex.position;
                    Vector3 e = edge.nextEdge.sourceVertex.position;
                    Vector3 p = Vector3.Lerp(Vector3.Lerp(s, e, 0.5f), c, 0.2f);
                    Handles.Label(transform.TransformPoint(p), "e" + edge.index, style);
                }
            }
        }
    }
}