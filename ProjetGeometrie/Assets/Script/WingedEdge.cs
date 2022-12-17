using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace WingedEdge
{
    public class WingedEdge
    {   
        public int index;
        public Vertex debutVertex;
        public Vertex endVertex;
        public Face leftFace;
        public Face rightFace;
        public WingedEdge startCWEdge;
        public WingedEdge startCCWEdge;
        public WingedEdge endCWEdge;
        public WingedEdge endCCWEdge;

        public WingedEdge(int ind, Vertex startV, Vertex endV, Face rightF, Face leftF, WingedEdge startCWE = null, WingedEdge startCCWE = null, WingedEdge endCW = null, WingedEdge endCCWE = null)
        {
            index = ind;
            startVertex = startV;
            endVertex = endV;
            rightFace = rightF;
            leftFace = leftF;
            startCWEdge = startCWE;
            startCCWEdge = startCCWE;
            endCWEdge = endCW;
            endCCWEdge = endCCWE;
        }

        //determine the Last endCCW with no leftFace
        public WingedEdge FindLastEndCW()
        {
            if(leftFace != null)
                return null;

            WingedEdge tempEndCCW = this.endCCWEdge;
            
            while(tempEndCCW.leftFace != null)
            {
                if(tempEndCCW.endVertex == this.endVertex)
                    tempEndCCW =  tempEndCCW.endCCWEdge;
                else 
                    tempEndCCW = tempEndCCW.startCCWEdge;

            }
            return tempEndCCW;
        }
        
        // Function to find the last startCW without leftface
        public WingedEdge FindLastStartCCW()
        {
            if (leftFace != null)
            {
                return null;
            }

            WingedEdge tmpStartCW = startCWEdge;

            // Tant que le tmpStartCW a un leftface
            while (tmpStartCW.leftFace != null)
            {   // Si le startVertex de tmpStartCW == this.startVertex
                if (tmpStartCW.startVertex == startVertex)
                {   // tmpStartCW devient l'edge pointé par son attribut startCCWEdge
                    tmpStartCW = tmpStartCW.startCCWEdge;
                }
                else
                {   //Sinon tmpStartCW devient l'edge pointé par son attribut endCWEdge
                    tmpStartCW = tmpStartCW.endCWEdge;
                }
            }
            return tmpStartCW;
        }
    }
    
    public class Vertex
    {
        public int index;
        public Vector3 position;
        public WingedEdge edge;

        public Vertex(int ind, Vector3 pos)
        {
            this.index = ind ;
            this.position = pos ;
        }

        public List<WingedEdge> GetAdjacentEdges()
        {
            List<WingedEdge> AdjacentEdges = new List<WingedEdge>();
            WingedEdge tempWingedEdge = edge;

            while (!AdjacentEdges.Contains(tempWingedEdge))
            {
                AdjacentEdges.Add(tempWingedEdge);
                if(this == tempWingedEdge.startVertex)
                    tempWingedEdge = tempWingedEdge.startCWEdge ;
                else
                    tempWingedEdge.endCWEdge;
            }
            return AdjacentEdges;
        }

        public List<Face> GetAdjacentFaces()
        {
            List<WingedEdge> adjacentEdges = GetAdjacentEdges();
            List<WingedEdge> borderEdges = GetBorderEdges();
            List<Face> adjacentFaces = new List<Face>();
            //vertex not in border => nb adjacent Edges == nb adjacent Faces
            if(borderEdges.Count == 0)
            {
                for (int i = 0; i < adjacentEdges.Count; i++)
                    adjacentFaces.Add(this == adjacentEdges[i].startVertex ? adjacentEdges[i].rightFace : adjacentEdges[i].leftFace);
            }
            //vertex in border
            else
            {
                for (int i = 0; i < adjacentEdges.Count; i++)
                    if(!adjacentFaces.Contains(adjacentEdges[i].rightFace)) adjacentFaces.Add(adjacentEdges[i].rightFace);
            }
            return adjacentFaces;
        }
        public List<WingedEdge> GetBorderEdges()
        {
            List<WingedEdge> borderEdges = new List<WingedEdge>();
            List<WingedEdge> adjacentEdges = GetAdjacentEdges();
            
            for (int i = 0; i < adjacentEdges.Count; i++)
                if (adjacentEdges[i].leftFace == null) 
                    borderEdges.Add(adjacentEdges[i]);
            
            return borderEdges;
        }
    }

    public class Face
    {
        public int index;
        public WingedEdge edge;

        public Face(int ind)
        {
            this.index = ind;
        }

        public List<WingedEdge> GetFaceEdges()
        {
            List<WingedEdge> faceEdges = new List<WingedEdge>();
            WingedEdge wingedEdge = edge;

            //Edge CW
            while (!faceEdges.Contains(wingedEdge))
            {
                faceEdges.Add(wingedEdge);
                if(this == wingedEdge.rightFace)
                    wingedEdge = wingedEdge.endCCWEdge ;
                else
                    wingedEdge = wingedEdge.startCCWEdge ;
            }
            return faceEdges;
        }

        public List<Vertex> GetFaceVertex()
        {
            List<WingedEdge> faceEdges = GetFaceEdges();
            List<Vertex> faceVertices = new List<Vertex>();
            //Vertice CW
            for (int i = 0; i < faceEdges.Count; i++)
            {
                //faceVertices.Add((faceEdges[i].rightFace == this) ? faceEdges[i].startVertex : faceEdges[i].endVertex);
                if(faceEdges[i].rightFace == this)
                    faceVertices.Add(faceEdges[i].startVertex) ;
                else
                    faceVertices.Add(faceEdges[i].endVertex) ;
            }
            return faceVertices;
        }

        public List<WingedEdge> GetBorderEdges()
        {
            List<WingedEdge> borderEdges = new List<WingedEdge>();
            List<WingedEdge> adjacentEdges = GetFaceEdges();
            for (int i = 0; i < adjacentEdges.Count; i++)
            {
                if (adjacentEdges[i].leftFace == null)
                    borderEdges.Add(adjacentEdges[i]);
            }
            return borderEdges;
        }
    }

    public class WingedEdgeMesh
    {
        public List<Vertex> vertices;
        public List<WingedEdge> edges;
        public List<Face> faces;
        
        // constructeur prenant un mesh Vertex-Face en paramètre
        public WingedEdgeMesh(Mesh mesh)
        {   
            int nbEdges = 4 ; //car quads
            vertices = new List<Vertex>() ;
            edges = new List<WingedEdge>() ;
            faces = new List<Face>() ;

            Vector3[] _vertices = mesh.vertices ;
            int[] _quads = mesh.GetIndices(0) ;

            Dictionary<ulong, WingedEdge> edgesDico = new Dictionary<ulong, WingedEdge>();
            WingedEdge wingedEdge;

            //Creation du vertex
            for (int i = 0; i < mesh.vertexCount; i++)
                vertices.Add(new Vertex(i, _vertices[i]));
            
            //Creation de la Face et de la WingedEdge
            for (int i = 0; i < _quads.Length / nbEdges; i++) 
            {
                Face face = new Face(faces.Count);
                faces.Add(face);
                
                //quad's vertices index
                int[] tmpQuadIndex = new int[nbEdges];
                for(int j = 0; j < nbEdges; j++)
                    tmpQuadIndex[j] = _quads[nbEdges * i + j];

                List<WingedEdge> faceEdges = new List<WingedEdge>();

                for(int j = 0; j < tmpQuadIndex.Length; j++)
                {
                    int debut = tmpQuadIndex[j] ;
                    int fin = tmpQuadIndex[(j+1) % nbEdges] ;
                    //ulong key = (ulong)Mathf.Min(start,end) + ((ulong)Mathf.Max(start, end) << 32);
                    ulong key = (ulong)Mathf.Min(debut,fin) + ((ulong)Mathf.Max(debut,fin) << 32) ;
                
                    //Si le mot n'est pas dans le dictionnaire
                    if(!edgesDico.TryGetValue(key, out wingedEdge))
                    {
                        wingedEdge = new WingedEdge(edges.Count, vertices[debut], vertices[fin], face, null);
                        edges.Add(wingedEdge);
                        dicoEdges.Add(key,wingedEdge);
                    }
                    else //Sinon on complete l'edge 
                        wingedEdge.leftFace = face ;

                    //Mise a jour de la vertex edge
                    if (vertices[debut].edge == null)
                        vertices[debut].edge = wingedEdge;
                    if (vertices[fin].edge == null)
                        vertices[fin].edge = wingedEdge;
                    //mise a jour de la face edge
                    if (face.edge == null)
                        face.edge = wingedEdge;

                    faceEdges.Add(wingedEdge);
                }
                //CCW and CW Edge
                for (int j = 0; j < faceEdges.Count; j++)
                {
                    if (faceEdges[j].rightFace == face)
                    {
                        faceEdges[j].startCWEdge = faceEdges[(j - 1 + faceEdges.Count) % faceEdges.Count];
                        faceEdges[j].endCCWEdge = faceEdges[(j + 1) % faceEdges.Count];
                    }
                    if (faceEdges[j].leftFace == face)
                    {
                        faceEdges[j].endCWEdge = faceEdges[(j - 1 + faceEdges.Count) % faceEdges.Count];
                        faceEdges[j].startCCWEdge = faceEdges[(j + 1) % faceEdges.Count];
                    }
                }
                //Update CCW and CW Edge for borderEdges
                for (int i = 0; i < edges.Count; i++)
                {
                    if(edges[i].leftFace == null)
                    {
                        edges[i].startCCWEdge = edges[i].FindLastStartCCW();
                        edges[i].endCWEdge = edges[i].FindLastEndCW();
                    }
                }
            }
        }
    
        public Mesh ConvertToFaceVertexMesh()
        {
            Mesh faceVertexMesh = new Mesh();
            // magic happens
            Vector3[] tmpVertices = new Vector3[vertices.Count];
            int[] tmpQuads = new int[faces.Count*4];

            //Vertices
            for (int i = 0; i < vertices.Count; i++)
                tmpVertices[i] = vertices[i].position;

            int index = 0;
            //Quads
            for (int i = 0; i < faces.Count; i++)
            {
                List<Vertex> faceVertex = faces[i].GetFaceVertex();
                for (int j = 0; j < faceVertex.Count; j++)
                    tmpQuads[index++] = faceVertex[j].index;
            }
                
            faceVertexMesh.vertices = tmpVertices;
            faceVertexMesh.SetIndices(tmpQuads, MeshTopology.Quads, 0);

            return faceVertexMesh;
        }
    
        public string ConvertToCSVFormat(string separator="\t")
        {
            string str = "";
            //magic happens
            if(this == null)
                return str;

            List<string> strings = new List<string>();

            //Vertices
            for (int i = 0; i < vertices.Count; i++)
            {
                List<WingedEdge> adjacentEdges = vertices[i].GetAdjacentEdges();
                List<Face>       adjacentFaces = vertices[i].GetAdjacentFaces();
                List<WingedEdge> borderEdges = vertices[i].GetBorderEdges();

                List<int> edgesIndex = new List<int>();
                List<int> facesIndex = new List<int>();
                List<int> borderEdgesIndex = new List<int>();
                    
                for (int j = 0; j < adjacentEdges.Count; j++)
                    edgesIndex.Add(adjacentEdges[j].index);

                for (int j = 0; j < adjacentFaces.Count; j++)
                    facesIndex.Add(adjacentFaces[j].index);

                for (int j = 0; j < borderEdges.Count; j++)
                    borderEdgesIndex.Add(borderEdges[j].index);

                strings.Add(vertices[i].index + separator
                            + vertices[i].position.x.ToString("N03") + " " 
                            + vertices[i].position.y.ToString("N03") + " " 
                            + vertices[i].position.z.ToString("N03") + separator
                            + vertices[i].edge.index + separator
                            + string.Join(" ", edgesIndex) + separator
                            + string.Join(" ", facesIndex) + separator
                            + string.Join(" ", borderEdgesIndex)
                            + separator + separator);
            }

            for (int i = vertices.Count; i < edges.Count; i++)
                strings.Add(separator + separator + separator + separator + separator + separator + separator);

            //Edges
            for (int i = 0; i < edges.Count; i++)
            {
                strings[i] += edges[i].index + separator
                        + edges[i].startVertex.index + separator
                        + edges[i].endVertex.index + separator
                        + $"{(edges[i].leftFace     != null ? edges[i].leftFace.index.ToString()    : "NULL")}" + separator
                        + $"{(edges[i].rightFace    != null ? edges[i].rightFace.index.ToString()   : "NULL")}" + separator
                        + $"{(edges[i].startCCWEdge != null ? edges[i].startCCWEdge.index.ToString(): "NULL")}" + separator
                        + $"{(edges[i].startCWEdge  != null ? edges[i].startCWEdge.index.ToString() : "NULL")}" + separator
                        + $"{(edges[i].endCWEdge    != null ? edges[i].endCWEdge.index.ToString()   : "NULL")}" + separator
                        + $"{(edges[i].endCCWEdge   != null ? edges[i].endCCWEdge.index.ToString()  : "NULL")}" + separator 
                        + separator;
            }

            //Faces
            for (int i = 0; i < faces.Count; i++)
            {
                List<WingedEdge> faceEdges = faces[i].GetFaceEdges();
                List<Vertex> faceVertex = faces[i].GetFaceVertex();

                List<int> edgesIndex = new List<int>();
                List<int> vertexIndex = new List<int>();
                //Edge CW
                for (int j = 0; j < faceEdges.Count; j++)
                    edgesIndex.Add(faceEdges[j].index);
                    
                //Vertice CW
                for (int j = 0; j < faceVertex.Count; j++)
                    vertexIndex.Add(faceVertex[j].index);
                
                strings[i] += faces[i].index + separator
                        + faces[i].edge.index + separator
                        + string.Join(" ", edgesIndex) + separator
                        + string.Join(" ", vertexIndex) + separator 
                        + separator;
            }

            string str  = "Vertex" + separator + separator + separator + separator + separator + separator + separator + "WingedEdges" + separator + separator + separator + separator + separator + separator + separator + separator + separator + separator + "Faces\n"
                        + "Index" + separator + "Position" + separator + "Edge" + separator + "Edges Adj" + separator + "Face Adj" + separator + "Border Edges" + separator + separator 
                        + "Index" + separator + "Start Vertex" + separator + "End Vertex" + separator + "Left Face" + separator + "Right Face" + separator + "Start CCW Edge" + separator + "Start CW Edge" + separator + "End CW Edge" + separator + "End CCW Edge" + separator + separator 
                        + "Index" + separator + "Edge" + separator + "CW Edges" + separator + "CW Vertices\n"
                        + string.Join("\n", strings);
            return str;
        }

        public void DrawGizmos(bool drawVertices,bool drawEdges,bool drawFaces)
        {
            //magic happens
            Gizmos.color = Color.red;
            GUIStyle style = new GUIStyle();
            style.fontSize = 12;

            //vertices
            if (drawVertices)
            {
                style.normal.textColor = Color.black;
                for (int i = 0; i < vertices.Count; i++)
                    Handles.Label(transform.TransformPoint(vertices[i].position), "V" + vertices[i].index, style);
            }

            //faces
            if (drawFaces)
            {
                style.normal.textColor = Color.blue;
                for (int i = 0; i < faces.Count; i++)
                {
                    List<Vertex> faceVertex = faces[i].GetFaceVertex();
                    Vector3 C = new Vector3();
                    for (int j = 0; j < faceVertex.Count; j++)
                    {
                        Gizmos.DrawLine(transform.TransformPoint(faceVertex[j].position), transform.TransformPoint(faceVertex[(j + 1) % faceVertex.Count].position));
                        C += faceVertex[j].position;
                    }
                    Handles.Label(transform.TransformPoint(C / 4f), "F" + faces[i].index, style);
                }
            }

            //edges
            if (drawEdges)
            {
                style.normal.textColor = Color.magenta;
                for (int i = 0; i < edges.Count; i++)
                {
                    Vector3 start = transform.TransformPoint(edges[i].startVertex.position);
                    Vector3 end = transform.TransformPoint(edges[i].endVertex.position);
                    Vector3 pos = Vector3.Lerp(start, end, 0.5f);

                    Gizmos.DrawLine(start, end);
                    Handles.Label(pos, "e" + edges[i].index, style);
                }
            }
        }
    }
}