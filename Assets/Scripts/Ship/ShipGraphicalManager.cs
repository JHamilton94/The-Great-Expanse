using UnityEngine;
using System.Collections;

public class ShipGraphicalManager : MonoBehaviour {

    GravityElements shipElements;
    SpriteRenderer spriteRenderer;
    LineRenderer lineRenderer;

	// Use this for initialization
	void Start () {
        shipElements = GetComponent<GravityElements>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        lineRenderer = GetComponent<LineRenderer>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
	    //Render orbital path
        
	}
}
