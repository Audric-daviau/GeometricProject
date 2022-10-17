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