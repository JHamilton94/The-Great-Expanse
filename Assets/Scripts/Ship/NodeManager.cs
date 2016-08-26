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
    private GameObject currentMassiveBody;
    
	// Input data
    private float hoverDistanceTolerance;
    public bool hovering;

    private Vector2 mouseLocation;
    private double mouseAltitude;
    private double mouseTrueAnomaly;
    private double orbitalAltitude;

    //node rose
    public Image nodePlacard;
    public Button nodeButton;
    public Button thrustVectorHandle;

    //dragged?
    public bool dragging;

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

        currentMassiveBody = shipElements.massiveBody;

        hoverDistanceTolerance = 1f;
        hovering = false;
        lastTrueAnomaly = shipElements.TrueAnomaly;
        thrustVector = new Vector2(0, 0);

    }

	void FixedUpdate () {

        //detect soi change
        if (currentMassiveBody.name != shipElements.massiveBody.name)
        {
            deleteNode();
        }

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
        }

        //do something
        lastTrueAnomaly = shipElements.TrueAnomaly;
	}

    public void deleteNode()
    {
        currentMassiveBody = shipElements.massiveBody;
        node = null;
        patchedConics.clearPotentialEncounters();
    }

	private void executeManeuver() {
		Debug.LogWarning("need to implement edge case");
        ship.applyThrust(node.getManeuver());
		thrustVector *= 0;
        patchedConics.clearPotentialEncounters();
        patchedConics.updateEncounters();
		node = null;
	}

    private void positionNode()
    {
        //position node
        float width = nodePlacard.rectTransform.rect.width * nodePlacard.transform.localScale.x;
        float height = nodePlacard.rectTransform.rect.height * nodePlacard.transform.localScale.y;
        double trueAnomaly = MiscHelperFuncs.AngleBetweenVector2(shipElements.Eccentricity, node.getNodePosition());
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

    public void startDragging()
    {
        dragging = true;
    }

    public void stoppedDragging()
    {
        updateNode();
        dragging = false;
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

        if(mouseAltitude > orbitalAltitude - hoverDistanceTolerance && mouseAltitude < orbitalAltitude + hoverDistanceTolerance && !dragging)
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
    
    public void createManeuver(Vector2 mousePosition)
    {
        if (node == null && hovering)
        {
            createNode();
        }
        else if(!dragging && hovering && Vector2.Distance(thrustVectorHandle.transform.position, mousePosition) > 
            thrustVectorHandle.GetComponent<CircleCollider2D>().radius * thrustVectorHandle.GetComponent<CircleCollider2D>().transform.localScale.x)
        {
            createNode();
        }
    }
    
    public Node getNode()
    {
        return node;
    }

    public void updatePatchedConics()
    {
        patchedConics.updatePotentialEncounters(node.getManeuver());
    }


    private void createNode()
    {
        Vector2 nodePosition = new Vector2((float)Math.Cos(mouseTrueAnomaly + shipElements.GlobalRotationAngle),
                (float)Math.Sin(mouseTrueAnomaly + shipElements.GlobalRotationAngle)).normalized *
                (float)orbitalAltitude;

        bool nodeTowardsPerigee = OrbitalHelper.towardsPerigeeOrbit(mouseTrueAnomaly, shipElements.Clockwise);
        double nodeSpeed = OrbitalHelper.calculateSpeed(nodePosition, shipElements.SemiMajorAxis, shipElements.Mu, shipElements.OrbitType);
        double nodeVelocityAngle = OrbitalHelper.calculateVelocityAngle(nodePosition, shipElements.Eccentricity, shipElements.SemiMajorAxis, mouseTrueAnomaly, shipElements.GlobalRotationAngle, shipElements.Clockwise, nodeTowardsPerigee, shipElements.OrbitType);

        Vector2 nodeVelocity = OrbitalHelper.assembleVelocityVector(nodeVelocityAngle, nodeSpeed);

        GravityElementsClass newOrbit = calculateInitialOrbitalElements(nodePosition, nodeVelocity + thrustVector, shipElements.massiveBody);

        node = new Node(newOrbit, mouseTrueAnomaly, nodePosition);

        patchedConics.updatePotentialEncounters(node.getManeuver());
    }

    private void updateNode()
    {
        Vector2 nodePosition = node.getNodePosition();

        bool nodeTowardsPerigee = OrbitalHelper.towardsPerigeeOrbit(mouseTrueAnomaly, shipElements.Clockwise);
        double nodeSpeed = OrbitalHelper.calculateSpeed(nodePosition, shipElements.SemiMajorAxis, shipElements.Mu, shipElements.OrbitType);
        double nodeVelocityAngle = OrbitalHelper.calculateVelocityAngle(nodePosition, shipElements.Eccentricity, shipElements.SemiMajorAxis, mouseTrueAnomaly, shipElements.GlobalRotationAngle, shipElements.Clockwise, nodeTowardsPerigee, shipElements.OrbitType);

        Vector2 nodeVelocity = OrbitalHelper.assembleVelocityVector(nodeVelocityAngle, nodeSpeed);

        GravityElementsClass newOrbit = calculateInitialOrbitalElements(nodePosition, nodeVelocity + thrustVector, shipElements.massiveBody);

        node = new Node(newOrbit, mouseTrueAnomaly, nodePosition);

        patchedConics.updatePotentialEncounters(node.getManeuver());
    }

    //takes in everything relative to the body being orbited
    private GravityElementsClass calculateInitialOrbitalElements(Vector2 position, Vector2 velocity, GameObject massiveBody)
    {
        GravityElementsClass gravityElements = new GravityElementsClass();

        gravityElements.massiveBody = massiveBody;

        gravityElements.Mu = GlobalElements.GRAV_CONST * gravityElements.MassiveBody.GetComponent<MassiveBodyElements>().mass;
        gravityElements.Position = position;
        gravityElements.velocity = velocity;

        //Calculate Global Tranformation Vector
        gravityElements.GlobalTransformationVector = massiveBody.transform.position;

        //Calculate eccentricity
        gravityElements.Eccentricity = OrbitalHelper.calculateEccentricity(position, velocity, gravityElements.Mu);

        //Determine orbit type
        gravityElements.OrbitType = OrbitalHelper.determineOrbitType(gravityElements.Eccentricity);

        //Calculate Mechanical Energy
        gravityElements.MechanicalEnergy = OrbitalHelper.calculateMechanicalEnergy(gravityElements.Position, gravityElements.velocity, gravityElements.Mu, gravityElements.OrbitType);

        //Calculate Semi Major Axis
        gravityElements.SemiMajorAxis = OrbitalHelper.calculateSemiMajorAxis(gravityElements.MechanicalEnergy, gravityElements.Mu, gravityElements.OrbitType);

        //Calculate SemiLatusRectum
        gravityElements.SemiLatusRectum = OrbitalHelper.calculateSemiLatusRectum(gravityElements.SemiMajorAxis, gravityElements.Eccentricity, gravityElements.Perigee, gravityElements.OrbitType);

        //Calculate Perigee
        gravityElements.Perigee = OrbitalHelper.calculatePerigee(gravityElements.SemiMajorAxis, gravityElements.Eccentricity, gravityElements.OrbitType);

        //Calculate Apogee
        gravityElements.Apogee = OrbitalHelper.calculateApogee(gravityElements.SemiMajorAxis, gravityElements.Eccentricity, gravityElements.OrbitType);

        //Calculate Center
        gravityElements.Center = OrbitalHelper.calculateCenter(gravityElements.SemiMajorAxis, gravityElements.Perigee, gravityElements.OrbitType);

        //Calculate GlobalRotationAngle
        gravityElements.GlobalRotationAngle = OrbitalHelper.calculateGlobalRotationAngle(gravityElements.Eccentricity, gravityElements.OrbitType);

        //Find orbital directions
        gravityElements.Clockwise = OrbitalHelper.clockwiseOrbit(gravityElements.Position, gravityElements.velocity);
        gravityElements.TowardsPerigee = OrbitalHelper.towardsPerigeeOrbit(gravityElements.velocity, gravityElements.Eccentricity, gravityElements.OrbitType);

        //Calculate trueAnomaly
        gravityElements.TrueAnomaly = OrbitalHelper.calculateTrueAnomaly(gravityElements.Eccentricity, gravityElements.Position, gravityElements.TowardsPerigee, gravityElements.Clockwise, gravityElements.OrbitType);

        //Calculate Eccentric Anomaly
        gravityElements.EccentricAnomaly = OrbitalHelper.calculateEccentricAnomaly(gravityElements.Eccentricity, gravityElements.TrueAnomaly, gravityElements.TowardsPerigee, gravityElements.OrbitType);

        //Calculate Anomaly at current epoch
        gravityElements.AnomalyAtEpoch = OrbitalHelper.calculateAnomalyAtCurrentEpoch(gravityElements.Eccentricity, gravityElements.EccentricAnomaly, gravityElements.Clockwise, gravityElements.OrbitType);
        gravityElements.MeanAnomaly = gravityElements.AnomalyAtEpoch;

        //Calculate Angular Momentum
        gravityElements.AngularMomentum = OrbitalHelper.calculateAngularMomentum(gravityElements.Eccentricity, gravityElements.Perigee, gravityElements.SemiMajorAxis, gravityElements.SemiLatusRectum,
            gravityElements.Mu, gravityElements.OrbitType);

        //Calculate time at epoch
        gravityElements.TimeAtEpoch = OrbitalHelper.calculateTimeAtEpoch(gravityElements.Eccentricity, gravityElements.EccentricAnomaly, gravityElements.SemiMajorAxis, gravityElements.Mu,
            gravityElements.Clockwise, gravityElements.TowardsPerigee, gravityElements.OrbitType);

        return gravityElements;
    }
}