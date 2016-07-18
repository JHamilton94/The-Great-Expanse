using UnityEngine;
using System.Collections;

/**
*Must be called before any graphical classes
**/
public class PreUpdate : MonoBehaviour {

    LineDrawer lineDrawer;

	// Use this for initialization
	void Start () {
        lineDrawer = Camera.main.GetComponent<LineDrawer>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        //Clears the vertices to allow moving lines
        lineDrawer.clearVertices();
	}
}
