using UnityEngine;
using System.Collections;
using System;

public class NodeManager : MonoBehaviour {

    //World information
    private GravityElements shipElements;
    private ShipGravityBehavior ship;

    //What we're making here
    private Node node;
    
    private float hoverDistanceTolerance;
    private bool hovering;

    private Vector2 mouseLocation;
    private double mouseAltitude;
    private double mouseTrueAnomaly;
    private double orbitalAltitude;

    private double lastTrueAnomaly;

	// Use this for initialization
	void Start () {
        node = null;
        shipElements = GetComponent<GravityElements>();
        ship = GetComponent<ShipGravityBehavior>();
        hoverDistanceTolerance = 1f;
        hovering = false;
        lastTrueAnomaly = shipElements.TrueAnomaly;
    }

	void FixedUpdate () {
        //Have we arrived at a node?
        if (node != null)
        {
            if (shipElements.Clockwise)
            {
                if (lastTrueAnomaly > node.getTrueAnomaly() && shipElements.TrueAnomaly < node.getTrueAnomaly())
                {
                    Debug.Log("Reached node, deleting node");
                    node = null;
                }
            }
            else
            {
                if (lastTrueAnomaly < node.getTrueAnomaly() && shipElements.TrueAnomaly > node.getTrueAnomaly())
                {
                    Debug.Log("Reached node, deleting node");
                    node = null;
                }
            }
        }

        lastTrueAnomaly = shipElements.TrueAnomaly;
	}

    public void hover(Vector2 mouseLocation)
    {
        this.mouseLocation = mouseLocation - MiscHelperFuncs.convertToVec2(shipElements.massiveBody.transform.position);
        mouseAltitude = this.mouseLocation.magnitude;
        mouseTrueAnomaly = Math.Atan2(this.mouseLocation.y, this.mouseLocation.x) - 
            Math.Atan2(shipElements.Eccentricity.y, shipElements.Eccentricity.x);
        if(mouseTrueAnomaly > Math.PI)
        {
            mouseTrueAnomaly -= 2 * Math.PI;
        }
        
        orbitalAltitude = OrbitalHelper.calculateAltitude(shipElements.Eccentricity, shipElements.SemiMajorAxis, shipElements.SemiLatusRectum, mouseTrueAnomaly, shipElements.OrbitType);

        if(mouseAltitude > orbitalAltitude - hoverDistanceTolerance && mouseAltitude < orbitalAltitude + hoverDistanceTolerance)
        {
            hovering = true;
        }
        else
        {
            hovering = false;
        }

    }

    public void createNode()
    {
        if (hovering)
        {
            Vector2 nodePosition = new Vector2((float)Math.Cos(mouseTrueAnomaly + shipElements.GlobalRotationAngle), 
                (float)Math.Sin(mouseTrueAnomaly + shipElements.GlobalRotationAngle)).normalized * 
                (float)orbitalAltitude;
            node = new Node(mouseTrueAnomaly, Vector2.right * 20, nodePosition);
        }
        else
        {
            return;
        }
    }
    
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (hovering)
        {
            Gizmos.DrawSphere((mouseLocation.normalized * (float)orbitalAltitude) + shipElements.GlobalTransformationVector, 1.0f);
        }
        if (node != null)
        {
            Gizmos.DrawSphere(node.getNodePosition() + shipElements.GlobalTransformationVector, 0.1f);
        }
        if (shipElements != null)
        {
            Gizmos.DrawLine(mouseLocation + shipElements.GlobalTransformationVector, shipElements.GlobalTransformationVector);
        }
    }
}