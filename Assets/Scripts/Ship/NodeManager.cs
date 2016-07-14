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

	// Use this for initialization
	void Start () {
        node = null;
        shipElements = GetComponent<GravityElements>();
        ship = GetComponent<ShipGravityBehavior>();
        hoverDistanceTolerance = 1f;
        hovering = false;
    }

	void FixedUpdate () {
        //Fuck all is happening here... for now
	}

    public void hover(Vector2 mouseLocation)
    {
        this.mouseLocation = mouseLocation - convertToVec2(shipElements.massiveBody.transform.position);
        mouseAltitude = this.mouseLocation.magnitude;
        mouseTrueAnomaly = Math.Atan2(this.mouseLocation.y, this.mouseLocation.x) - 
            Math.Atan2(shipElements.Eccentricity.y, shipElements.Eccentricity.x);
        if(mouseTrueAnomaly > Math.PI)
        {
            mouseTrueAnomaly -= 2 * Math.PI;
        }
        
        orbitalAltitude = calculateAltitude(shipElements.Eccentricity, shipElements.SemiMajorAxis, shipElements.SemiLatusRectum, mouseTrueAnomaly, shipElements.OrbitType);

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

    private double calculateTrueAnomaly(Vector2 eccentricity, Vector2 position, bool towardsPerigee, bool clockwise, OrbitTypes orbitType)
    {
        double returnTrueAnomaly = double.PositiveInfinity;
        switch (orbitType)
        {
            case OrbitTypes.circular:
                returnTrueAnomaly = Vector2.Angle(position, Vector2.right);
                returnTrueAnomaly = convertToRadians(returnTrueAnomaly);
                break;
            case OrbitTypes.elliptical:
                returnTrueAnomaly = Vector2.Angle(eccentricity, position);
                returnTrueAnomaly = convertToRadians(returnTrueAnomaly);
                if (towardsPerigee)
                {
                    returnTrueAnomaly = -Math.Abs(returnTrueAnomaly);
                }
                else
                {
                    returnTrueAnomaly = Math.Abs(returnTrueAnomaly);
                }

                break;
            case OrbitTypes.parabolic:
                break;
            case OrbitTypes.hyperbolic:
                returnTrueAnomaly = Vector2.Angle(eccentricity, position);
                returnTrueAnomaly = convertToRadians(returnTrueAnomaly);
                if (clockwise)
                {
                    if (towardsPerigee)
                    {
                        returnTrueAnomaly = Math.Abs(returnTrueAnomaly);
                    }
                    else
                    {
                        returnTrueAnomaly = -Math.Abs(returnTrueAnomaly);
                    }
                }
                else
                {
                    if (towardsPerigee)
                    {
                        returnTrueAnomaly = -Math.Abs(returnTrueAnomaly);
                    }
                    else
                    {
                        returnTrueAnomaly = Math.Abs(returnTrueAnomaly);
                    }
                }
                break;
        }

        return returnTrueAnomaly;
    }

    private double calculateAltitude(Vector2 eccentricity, double semiMajorAxis, double semiLatusRectum, double trueAnomaly, OrbitTypes orbitType)
    {
        double returnAltitude = double.PositiveInfinity;
        switch (orbitType)
        {
            case OrbitTypes.circular:
                returnAltitude = (semiMajorAxis * (1 - (eccentricity.magnitude * eccentricity.magnitude))) /
                    (1 + (eccentricity.magnitude * Math.Cos(trueAnomaly)));
                break;
            case OrbitTypes.elliptical:
                returnAltitude = (semiMajorAxis * (1 - (eccentricity.magnitude * eccentricity.magnitude))) /
                    (1 + (eccentricity.magnitude * Math.Cos(trueAnomaly)));
                break;
            case OrbitTypes.parabolic:
                returnAltitude = (semiMajorAxis * (1 - (eccentricity.magnitude * eccentricity.magnitude))) /
                    (1 + (eccentricity.magnitude * Math.Cos(trueAnomaly)));
                break;
            case OrbitTypes.hyperbolic:
                returnAltitude = semiLatusRectum / (1 + (eccentricity.magnitude * Math.Cos(trueAnomaly)));
                break;
        }
        return returnAltitude;
    }

    private double convertToRadians(double degrees)
    {
        return (degrees * Math.PI) / 180;
    }

    private Vector2 convertToVec2(Vector3 inVec)
    {
        return new Vector2(inVec.x, inVec.y);
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

        Gizmos.DrawLine(mouseLocation + shipElements.GlobalTransformationVector, shipElements.GlobalTransformationVector);
    }
}