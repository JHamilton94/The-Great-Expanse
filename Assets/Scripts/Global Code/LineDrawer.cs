using UnityEngine;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;

public class LineDrawer : MonoBehaviour{

    public Material mat;

    private List<KeyValuePair<Color, Vector2>> vertices = new List<KeyValuePair<Color, Vector2>>();
    
    public void DrawLine(Vector2 point1, Vector2 point2, Color col)
    {
        vertices.Add(new KeyValuePair<Color, Vector2>(col, point1));
        vertices.Add(new KeyValuePair<Color, Vector2>(col, point2));
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
        for(int i = 0; i < vertices.Count; i += 2)
        {
            GL.Color(vertices[i].Key);
            GL.Vertex(MiscHelperFuncs.convertToVec3(vertices[i].Value) + new Vector3(0, 0, 1f));
            GL.Vertex(MiscHelperFuncs.convertToVec3(vertices[i + 1].Value) + new Vector3(0, 0, 1f));
        }
        GL.End();

        GL.PopMatrix();

        //Debug.Log(1.0/Time.deltaTime);

        //clear vertices
        //vertices = new List<Vector2>();
    }

   
}
