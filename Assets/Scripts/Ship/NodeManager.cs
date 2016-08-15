using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class NodeManager : MonoBehaviour {

    //World information
    private GravityElements shipElements;
    private ShipGravityBehavior ship;
    private LineDrawer lineDrawer;
    private ShipPatchedConics patchedConics;

    //What we're making here
    public Node node;
    
	// Input data
    private float hoverDistanceTolerance;
    private bool hovering;

    private Vector2 mouseLocation;
    private double mouseAltitude;
    private double mouseTrueAnomaly;
    private double orbitalAltitude;

    //node rose
    public Image nodePlacard;
    public Button nodeButton;
    public Button thrustVectorHandle;

    //dragged?
    private bool dragging;

    //thrust
    private Vector2 thrustVector;

    private double lastTrueAnomaly;

	// Use this for initialization
	void Start () {
        node = null;
        shipElements = GetComponent<GravityElements>();
        ship = GetComponent<ShipGravityBehavior>();
        lineDrawer = GetComponentInChildren<LineDrawer>();
        patchedConics = GetComponent<ShipPatchedConics>();
        
        hoverDistanceTolerance = 1f;
        hovering = false;
        lastTrueAnomaly = shipElements.TrueAnomaly;
        thrustVector = new Vector2(0, 0);
    }

	void FixedUpdate () {
        //Have we arrived at a node?
        if (node != null)
        {
            //Is the ship moving clockwise?
            if (shipElements.Clockwise) 
            {
                //TODO implement edge case when crossing over 0
                if(MiscHelperFuncs.convertTo360Angle(shipElements.TrueAnomaly) < MiscHelperFuncs.convertTo360Angle(node.getTrueAnomaly()) && 
                    MiscHelperFuncs.convertTo360Angle(lastTrueAnomaly) > MiscHelperFuncs.convertTo360Angle(node.getTrueAnomaly()) )
                {
					executeManeuver();
                } 
            }
            else
            {
                if (MiscHelperFuncs.convertTo360Angle(shipElements.TrueAnomaly) > MiscHelperFuncs.convertTo360Angle(node.getTrueAnomaly()) &&
                    MiscHelperFuncs.convertTo360Angle(lastTrueAnomaly) < MiscHelperFuncs.convertTo360Angle(node.getTrueAnomaly()))
				{
					executeManeuver();
                }
            }
        }

        //position maneuver node
        if (node != null)
        {
            positionNode();
        }
        else
        {
            nodeButton.transform.localScale = new Vector3(0, 0, 0);
            nodePlacard.transform.localScale = new Vector3(0, 0, 0);
            thrustVectorHandle.transform.localScale = new Vector3(0, 0, 0);
        }

        //Determine thrust vector
        if (dragging)
        {
            thrustVector = MiscHelperFuncs.convertToVec2(Camera.main.ScreenToWorldPoint(Input.mousePosition)) - (node.getNodePosition() + shipElements.GlobalTransformationVector);
            node.setThrustVector(thrustVector);
        }

        //do something
        lastTrueAnomaly = shipElements.TrueAnomaly;
	}

	private void executeManeuver() {
		Debug.LogWarning("need to implement edge case");
		ship.applyThrust(node.getThrustVector());
		thrustVector *= 0;
		node = null;
	}

    private void positionNode()
    {
        //position node
        float width = nodePlacard.rectTransform.rect.width * nodePlacard.transform.localScale.x;
        float height = nodePlacard.rectTransform.rect.height * nodePlacard.transform.localScale.y;
        double trueAnomaly = Math.Atan2(node.getNodePosition().y, node.getNodePosition().x) -
            Math.Atan2(shipElements.Eccentricity.y, shipElements.Eccentricity.x);
        if (trueAnomaly > Math.PI)
        {
            trueAnomaly -= 2 * Math.PI;
        }
        bool towardsPerigeeOrbit;
        if (trueAnomaly < 0)
        {
            if (shipElements.Clockwise)
            {
                towardsPerigeeOrbit = false;
            }
            else
            {
                towardsPerigeeOrbit = true;
            }
        }
        else
        {
            if (shipElements.Clockwise)
            {
                towardsPerigeeOrbit = true;
            }
            else
            {
                towardsPerigeeOrbit = false;
            }
        }
        double velocityAngle = OrbitalHelper.calculateVelocityAngle(node.getNodePosition(), shipElements.Eccentricity, shipElements.SemiMajorAxis, trueAnomaly, shipElements.GlobalRotationAngle, shipElements.Clockwise, towardsPerigeeOrbit, shipElements.OrbitType);

        Vector2 offsetVector;
        if (shipElements.Clockwise)
        {
            offsetVector = OrbitalHelper.assembleVelocityVector(velocityAngle + Math.PI / 2, 1).normalized *
                Mathf.Sqrt(Mathf.Pow(width / 2, 2) + Mathf.Pow(height / 2, 2));
        }
        else
        {
            offsetVector = OrbitalHelper.assembleVelocityVector(velocityAngle - Math.PI / 2, 1).normalized *
                Mathf.Sqrt(Mathf.Pow(width / 2, 2) + Mathf.Pow(height / 2, 2));
        }

        nodeButton.transform.position = node.getNodePosition() + offsetVector + shipElements.GlobalTransformationVector;
        nodeButton.transform.localScale = new Vector3(GlobalElements.zoomLevel / GlobalElements.UI_SCALE_CONST, GlobalElements.zoomLevel / GlobalElements.UI_SCALE_CONST, 0);

        nodePlacard.transform.position = node.getNodePosition() + offsetVector + shipElements.GlobalTransformationVector;
        nodePlacard.transform.localScale = new Vector3(GlobalElements.zoomLevel / GlobalElements.UI_SCALE_CONST, GlobalElements.zoomLevel / GlobalElements.UI_SCALE_CONST, 0);
        nodePlacard.transform.rotation = Quaternion.AngleAxis(Mathf.Atan2(offsetVector.y, offsetVector.x) * Mathf.Rad2Deg - 45, Vector3.forward);

        thrustVectorHandle.transform.position = node.getNodePosition() + shipElements.GlobalTransformationVector + thrustVector;
        thrustVectorHandle.transform.localScale = new Vector3(GlobalElements.zoomLevel / GlobalElements.UI_SCALE_CONST / 2, GlobalElements.zoomLevel / GlobalElements.UI_SCALE_CONST / 2, 0);
        lineDrawer.DrawLine(node.getNodePosition() + shipElements.GlobalTransformationVector, node.getNodePosition() + shipElements.GlobalTransformationVector + thrustVector, Color.red);
    }

    public void setDragging(bool drag)
    {
        dragging = drag;
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
        //Debugging
        if ((mouseTrueAnomaly > Math.PI || mouseTrueAnomaly < -Math.PI) && shipElements.OrbitType == OrbitTypes.elliptical)
        {
            //Debug.Break();
            //Debug.LogWarning("ERROR, mouse true anomaly out of bounds");
            /*Debug.Log("NODE");
            Debug.Log("Position: " + mouseLocation);
            Debug.Log("True Anomaly: " + mouseTrueAnomaly);
            Debug.Log("Thrust: " + "heehee");*/
        }
    }

    public void createNode()
    {
        if (hovering)
        {
            //create a new node
            if (node == null)
            {
                Vector2 nodePosition = new Vector2((float)Math.Cos(mouseTrueAnomaly + shipElements.GlobalRotationAngle),
                (float)Math.Sin(mouseTrueAnomaly + shipElements.GlobalRotationAngle)).normalized *
                (float)orbitalAltitude;
                node = new Node(mouseTrueAnomaly, new Vector2(0,0), nodePosition);
            }
            //move an existing node
            else if (node != null &&
                !dragging
                && Vector2.Distance(thrustVectorHandle.transform.position, MiscHelperFuncs.convertToVec2(Camera.main.ScreenToWorldPoint(Input.mousePosition))) >
                thrustVectorHandle.GetComponent<CircleCollider2D>().radius * thrustVectorHandle.GetComponent<CircleCollider2D>().transform.localScale.x)
            {
                Vector2 nodePosition = new Vector2((float)Math.Cos(mouseTrueAnomaly + shipElements.GlobalRotationAngle),
                (float)Math.Sin(mouseTrueAnomaly + shipElements.GlobalRotationAngle)).normalized *
                (float)orbitalAltitude;
                node = new Node(mouseTrueAnomaly, Vector2.right * 20, nodePosition);
			}
        }
        else
        {
            return;
        }
    }
    
    public Node getNode()
    {
        return node;
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (hovering)
        {
            Gizmos.DrawSphere((mouseLocation.normalized * (float)orbitalAltitude) + shipElements.GlobalTransformationVector, 0.1f);
        }
        if (node != null)
        {
            //Gizmos.DrawSphere(node.getNodePosition() + shipElements.GlobalTransformationVector, 0.1f);
        }
    }

    public void updatePatchedConics()
    {
        //update patched conics
        double velocityAngle = OrbitalHelper.calculateVelocityAngle(node.getNodePosition(), shipElements.Eccentricity, shipElements.SemiMajorAxis, node.getTrueAnomaly(), shipElements.GlobalRotationAngle, shipElements.Clockwise, shipElements.TowardsPerigee, shipElements.OrbitType);
        double speed = OrbitalHelper.calculateSpeed(node.getNodePosition(), shipElements.SemiMajorAxis, shipElements.Mu, shipElements.OrbitType);
        Debug.Log("Predicted speed: " + OrbitalHelper.assembleVelocityVector(velocityAngle, speed));
        Debug.Log("Thrust vector: " + node.getThrustVector());
        Debug.Log("New speed: " + (node.getThrustVector() + OrbitalHelper.assembleVelocityVector(velocityAngle, speed)));
        patchedConics.updatePotentialEncounters(node.getThrustVector() + OrbitalHelper.assembleVelocityVector(velocityAngle, speed), node.getNodePosition());
    }
}