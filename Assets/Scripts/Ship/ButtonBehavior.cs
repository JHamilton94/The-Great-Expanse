using UnityEngine;
using System.Collections;

public class ButtonBehavior : MonoBehaviour {

    private InputController ic;
    

	// Use this for initialization
	void Start () {
        ic = GetComponentInParent<InputController>();
	}
	
    public void OnMouseDrag()
    {
        ic.dragButton();   
    }
    public void OnMouseUp()
    {
        ic.stopDragButton();
    }
}
