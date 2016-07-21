using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
public class InputController : MonoBehaviour {

    private double force;
    private double oldTimeStep;
    private ShipGravityBehavior shipGravityBehavior;
    private NodeManager nodeManager;
    private ShipPatchedConics graphicsManager;
    private GravityElements shipElements;
    private LineDrawer lineDrawer;

    //node rose
    public Image nodePlacard;
    public Button node;
    public Button thrustVectorHandle;
    
    //dragged?
    private bool dragging;

    //thrust
    private Vector2 thrustVector;

    // Use this for initialization
    void Start()
    {
        force = 0;
        shipGravityBehavior = GetComponent<ShipGravityBehavior>();
        nodeManager = GetComponent<NodeManager>();
        graphicsManager = GetComponent<ShipPatchedConics>();
        shipElements = GetComponent<GravityElements>();
        lineDrawer = GetComponentInChildren<LineDrawer>();

        dragging = false;
        thrustVector = new Vector2(0, 0);

        Camera.main.orthographicSize = GlobalElements.zoomLevel;
    }
	
	// Update is called once per frame
	void FixedUpdate () {

        //hovering over orbit
        nodeManager.hover(Camera.main.ScreenToWorldPoint(Input.mousePosition));

        //Clicking a position in an orbit
        if (Input.GetButton("Fire1"))
        {
            //create a new node
            if (nodeManager.getNode() == null)
            {
                thrustVector = new Vector2(0, 0);
                nodeManager.createNode();
            }
            //move an existing node
            else if (nodeManager.getNode() != null && 
                !dragging
                && Vector2.Distance(thrustVectorHandle.transform.position, MiscHelperFuncs.convertToVec2(Camera.main.ScreenToWorldPoint(Input.mousePosition))) > 
                thrustVectorHandle.GetComponent<CircleCollider2D>().radius * thrustVectorHandle.GetComponent<CircleCollider2D>().transform.localScale.x) 
            {
                nodeManager.createNode();
            }
        }

        //Time manipulation
        if (Input.GetButton("Fire2"))
        {
            GlobalElements.timeStep += 0.01f;
        }
        if (Input.GetButton("Fire3"))
        {
            GlobalElements.timeStep -= 0.01f;
        }
        
        //Camera zoom
        if (Input.GetAxis("RightVertical") > 0.01f)
        {
            GlobalElements.zoomLevel -= 1;
            Camera.main.orthographicSize = GlobalElements.zoomLevel;
        }
        if(Input.GetAxis("RightVertical") < -0.01f)
        {
            GlobalElements.zoomLevel += 1;
            Camera.main.orthographicSize = GlobalElements.zoomLevel;
        }
        
        if (Input.GetButtonDown("Pause") && GlobalElements.timeStep != 0)
        {   
            oldTimeStep = GlobalElements.timeStep;
            GlobalElements.timeStep = 0;
        }
        else if(Input.GetButtonDown("Pause") && GlobalElements.timeStep == 0){
            GlobalElements.timeStep = oldTimeStep;
        }

        //Setup maneuver node
        if (nodeManager.getNode() != null)
        {
            positionNode();
            
        }
        else
        {
            node.transform.localScale = new Vector3(0, 0, 0);
            thrustVectorHandle.transform.localScale = new Vector3(0, 0, 0);
            nodePlacard.transform.localScale = new Vector3(0, 0, 0);
        }

        //update thrust vector
        if (nodeManager.getNode() != null && dragging)
        {
            thrustVector = MiscHelperFuncs.convertToVec2(Camera.main.ScreenToWorldPoint(Input.mousePosition)) - (nodeManager.getNode().getNodePosition() + shipElements.GlobalTransformationVector);
            nodeManager.getNode().setThrustVector(thrustVector);
        }        
    }
    
    public void OnDrawGizmos()
    {
        //Draw thrust vector
        Gizmos.color = Color.red;
    }

    private void positionNode()
    {
        //position node
        float width = nodePlacard.rectTransform.rect.width * nodePlacard.transform.localScale.x;
        float height = nodePlacard.rectTransform.rect.height * nodePlacard.transform.localScale.y;
        double trueAnomaly = Math.Atan2(nodeManager.getNode().getNodePosition().y, nodeManager.getNode().getNodePosition().x) -
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
        double velocityAngle = OrbitalHelper.calculateVelocityAngle(nodeManager.getNode().getNodePosition(), shipElements.Eccentricity, shipElements.SemiMajorAxis, trueAnomaly, shipElements.GlobalRotationAngle, shipElements.Clockwise, towardsPerigeeOrbit, shipElements.OrbitType);
        
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

        node.transform.position = nodeManager.getNode().getNodePosition() + offsetVector + shipElements.GlobalTransformationVector;
        node.transform.localScale = new Vector3(GlobalElements.zoomLevel / GlobalElements.UI_SCALE_CONST, GlobalElements.zoomLevel / GlobalElements.UI_SCALE_CONST, 0);

        nodePlacard.transform.position = nodeManager.getNode().getNodePosition() + offsetVector + shipElements.GlobalTransformationVector;
        nodePlacard.transform.localScale = new Vector3(GlobalElements.zoomLevel / GlobalElements.UI_SCALE_CONST, GlobalElements.zoomLevel / GlobalElements.UI_SCALE_CONST, 0);
        nodePlacard.transform.rotation = Quaternion.AngleAxis(Mathf.Atan2(offsetVector.y, offsetVector.x) * Mathf.Rad2Deg - 45, Vector3.forward);

        thrustVectorHandle.transform.position = nodeManager.getNode().getNodePosition() + shipElements.GlobalTransformationVector + thrustVector;
        thrustVectorHandle.transform.localScale = new Vector3(GlobalElements.zoomLevel / GlobalElements.UI_SCALE_CONST/2, GlobalElements.zoomLevel / GlobalElements.UI_SCALE_CONST/2, 0);
        lineDrawer.DrawLine(nodeManager.getNode().getNodePosition() + shipElements.GlobalTransformationVector, nodeManager.getNode().getNodePosition() + shipElements.GlobalTransformationVector + thrustVector, Color.red);
        lineDrawer.DrawLine(new Vector2(0,0), new Vector2(0, 1000), Color.green);
    }

    //Helper funcs
    private Vector3 convertToVec3(Vector2 inVec)
    {
        return new Vector3(inVec.x, inVec.y, 0.0f);
    }

    public void dragButton()
    {
        dragging = true;
    }

    public void stopDragButton()
    {
        dragging = false;
    }
    
}
