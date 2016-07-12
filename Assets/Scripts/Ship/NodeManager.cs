using UnityEngine;
using System.Collections;
using System;

public class NodeManager : MonoBehaviour {

    private Node node;
    private GravityElements shipElements;
    private ShipGravityBehavior ship;
    private float tolerance;

    private Vector2 locationVector;
    private double locationAltitude;
    private double trueAnomaly;
    private double desiredAltitude;

    private bool hovering;

	// Use this for initialization
	void Start () {
        node = null;
        shipElements = GetComponent<GravityElements>();
        ship = GetComponent<ShipGravityBehavior>();
        tolerance = 1f;
        hovering = false;
    }

	void FixedUpdate () {
        //Check if node is null
        if (node != null)
        {
            //check if we've reached the node
            if(shipElements.MeanAnomaly > node.getMeanAnomaly() - tolerance 
                || shipElements.MeanAnomaly < node.getMeanAnomaly() + tolerance)
            {
                ship.applyThrust(node.getThrustVector());
                node = null;
            }
        }	    
	}

    public void hover(Vector2 mouseLocation)
    {
        locationVector = mouseLocation - convertToVec2(shipElements.massiveBody.transform.position);
        locationAltitude = locationVector.magnitude;
        trueAnomaly = calculateTrueAnomaly(shipElements.Eccentricity, mouseLocation - shipElements.GlobalTransformationVector, shipElements.TowardsPerigee, shipElements.Clockwise, shipElements.OrbitType);
        desiredAltitude = calculateAltitude(shipElements.Eccentricity, shipElements.SemiMajorAxis, shipElements.SemiLatusRectum, trueAnomaly, shipElements.OrbitType);

        if(locationAltitude > desiredAltitude -tolerance && locationAltitude < desiredAltitude + tolerance)
        {
            hovering = true;
        }
        else
        {
            hovering = false;
        }

    }

    public void createNode(Vector2 thrustVector, float meanAnomaly)
    {

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
        Gizmos.DrawLine(locationVector+shipElements.GlobalTransformationVector, shipElements.massiveBody.transform.position);
        if (hovering)
        {
            Gizmos.DrawSphere((locationVector.normalized * (float)desiredAltitude) + shipElements.GlobalTransformationVector, 1.0f);
        }

    }
}