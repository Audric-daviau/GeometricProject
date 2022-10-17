using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace WingedEdge
{
    public class WingedEdge
    {   
        public int index;
        public Vertex startVertex;

        public Vertex endVertex;
        public Face leftFace;
        public Face rightFace;
        public WingedEdge startCWEdge;
        public WingedEdge startCCWEdge;
        public WingedEdge endCWEdge;
        public WingedEdge endCCWEdge;
    }
    
    public class Vertex
    {
        public int index;
        public Vector3 position;
        public WingedEdge edge;
    }

    public class Face
    {
        public int index;
        public WingedEdge edge;
    }

    public class WingedEdgeMesh
    {
        public List<Vertex> vertices;
        public List<WingedEdge> edges;
        public List<Face> faces;
        public WingedEdgeMesh(Mesh mesh)
    {// constructeur prenant un mesh Vertex-Face en paramètre
    // magic happens
    }
    
    public Mesh ConvertToFaceVertexMesh()
    {
        Mesh faceVertexMesh = new Mesh();
        // magic happens
        return faceVertexMesh;
    }
    
    public string ConvertToCSVFormat(string separator="\t")
    {
        string str = "";
        //magic happens
        return str;
    }
    public void DrawGizmos(bool drawVertices,bool drawEdges,bool drawFaces)

    {
        //magic happens
    }
    }
}