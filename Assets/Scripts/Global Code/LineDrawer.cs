using UnityEngine;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;

public class LineDrawer : MonoBehaviour{

    public Material mat;

    private List<Vector2> vertices = new List<Vector2>();
    
    public void DrawLine(Vector2 point1, Vector2 point2)
    {
        vertices.Add(point1);
        vertices.Add(point2);
    }
    
    public void clearVertices()
    {
        vertices.Clear();

    }
    
    void OnPostRender()
    {
        //vertices = new List<Vector2>();

        if (!mat)
        {
            Debug.Log("Mat null");
            return;
        }

        mat.SetPass(0);

        GL.PushMatrix();

        GL.Begin(GL.LINES);
        GL.Color(Color.red);
        for(int i = 0; i < vertices.Count; i += 2)
        {
            GL.Vertex(MiscHelperFuncs.convertToVec3(vertices[i]) + new Vector3(0, 0, 1f));
            GL.Vertex(MiscHelperFuncs.convertToVec3(vertices[i + 1]) + new Vector3(0, 0, 1f));
        }
        GL.End();

        GL.PopMatrix();

        Debug.Log(1.0/Time.deltaTime);

        //clear vertices
        //vertices = new List<Vector2>();
    }

   
}
