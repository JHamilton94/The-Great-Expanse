using UnityEngine;
using System.Collections;

public class InputController : MonoBehaviour {

    private Vector2 thrust;
    private double force;
    private ShipGravityBehavior shipGravityBehavior;

	// Use this for initialization
	void Start () {
        force = 0;
        shipGravityBehavior = GetComponent<ShipGravityBehavior>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        calculateThrustVector();

        //Time manipulation
        if (Input.GetButton("Fire2"))
        {
            GlobalElements.timeStep += 0.01f;
        }
        if (Input.GetButton("Fire3"))
        {
            GlobalElements.timeStep -= 0.01f;
        }

        //Thruster controls
        if (Input.GetButton("Fire1"))
        {
            force += 0.01;
            calculateThrustVector();
        }
        else if (force > 0)
        {
            shipGravityBehavior.applyThrust(thrust);
            force = 0;
        }
	}

    private void calculateThrustVector()
    {
        thrust = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        thrust.Normalize();
        thrust *= (float)force;
    }

    public void OnDrawGizmos()
    {
        //Draw thrust vector
        Gizmos.color = Color.red;
        Gizmos.DrawLine(this.transform.position, (this.transform.position + (convertToVec3(thrust) * 10)));
    }
    
    //Helper funcs
    private Vector3 convertToVec3(Vector2 inVec)
    {
        return new Vector3(inVec.x, inVec.y, 0.0f);
    }
}
