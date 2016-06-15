using UnityEngine;
using System.Collections;

public class StarBehavior : MonoBehaviour {

    StarElements starElements; 
	// Use this for initialization
	void Start () {
        starElements = this.GetComponent<StarElements>();
    }

    // Update is called once per frame
    void FixedUpdate () {

	}



    public void OnDrawGizmos()
    {
        
    }
}
