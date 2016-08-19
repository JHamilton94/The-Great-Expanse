using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
public class InputController : MonoBehaviour
{

    private double oldTimeStep;
    private ShipGravityBehavior shipGravityBehavior;
    private NodeManager nodeManager;
    private ShipPatchedConics graphicsManager;
    private GravityElements shipElements;





    // Use this for initialization
    void Start()
    {
        shipGravityBehavior = GetComponent<ShipGravityBehavior>();
        nodeManager = GetComponent<NodeManager>();
        graphicsManager = GetComponent<ShipPatchedConics>();
        shipElements = GetComponent<GravityElements>();

        Camera.main.orthographicSize = GlobalElements.zoomLevel;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        //hovering over orbit
        nodeManager.hover(Camera.main.ScreenToWorldPoint(Input.mousePosition));

        //Clicking a position in an orbit
        if (Input.GetButton("Fire1"))
        {
            nodeManager.createManeuver();
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
        if (Input.GetAxis("RightVertical") < -0.01f)
        {
            GlobalElements.zoomLevel += 1;
            Camera.main.orthographicSize = GlobalElements.zoomLevel;
        }

        if (Input.GetButtonDown("Pause") && GlobalElements.timeStep != 0)
        {
            oldTimeStep = GlobalElements.timeStep;
            GlobalElements.timeStep = 0;
        }
        else if (Input.GetButtonDown("Pause") && GlobalElements.timeStep == 0)
        {
            GlobalElements.timeStep = oldTimeStep;
        }
    }



    //Helper funcs
    private Vector3 convertToVec3(Vector2 inVec)
    {
        return new Vector3(inVec.x, inVec.y, 0.0f);
    }


    //called when dragging or undragging buttons
    public void dragButton()
    {
        nodeManager.startDragging();
    }

    public void stopDragButton()
    {
        nodeManager.stoppedDragging();
    }

}
