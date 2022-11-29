using System.Collections.Generic;
using UnityEditor;
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
        { // constructeur prenant un mesh Vertex-Face en paramètre
          //magic happens
            vertices = new List<Vertex>(mesh.vertices.Length);
            edges = new List<HalfEdge>(mesh.vertices.Length + 4);
            faces = new List<Face>(mesh.vertices.Length - 2);

            //list of vertex
            for (int i = 0; i < mesh.vertices.Length; i++)
            {
                vertices[i].index = i;
                vertices[i].position = mesh.vertices[i];
            }
            //list of edges
            for(int i = 0; i < mesh.vertices.Length + 4; i++)
            {
                edges[i].index = i;
                edges[i].sourceVertex = vertices[i];
            }
            //list of faces
            int j = 0;
            for(int i = 0; i< mesh.triangles.Length / 6 ; i++)
            {
                faces[i].index = i;
                faces[i].edge = edges[j];
                j += 4;
            }

            //faire le calcul pour les next twin des edges....


        }
        public Mesh ConvertToFaceVertexMesh()
        {
            Mesh faceVertexMesh = new Mesh();
            // magic happens
            return faceVertexMesh;
        }
        public string ConvertToCSVFormat(string separator = "\t")
        {
            string str = "";
            //magic happens
            return str;
        }
        public void DrawGizmos(bool drawVertices, bool drawEdges, bool drawFaces)
        {
            //magic happens
        }
    }
}