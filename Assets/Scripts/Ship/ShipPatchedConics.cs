using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

/**
*Must be executed after the preupdate
**/

public class ShipPatchedConics : MonoBehaviour {

    //Data sources
    private GravityElements shipElements;
    private SpriteRenderer spriteRenderer;
    private LineDrawer lineDrawer;
    private NodeManager nodeManager;

    //Orbital POI
    public Button perigeeButton;
    public Image perigeePlacard;
    public Button apogeeButton;
    public Image apogeePlacard;
    
	void Start () {
        shipElements = GetComponent<GravityElements>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>(true);
        lineDrawer = GetComponentInChildren<LineDrawer>();
        nodeManager = GetComponent<NodeManager>();
    }
	
	void FixedUpdate () {
        //Display ships patched conics
        

        //Are we going to come close to another orbit
        
        //Are we goign to be there at the right time?
        for(int i = 0; i < 3; i++)
        {
            drawPatchedConics(shipElements.TrueAnomaly, shipElements.GlobalTransformationVector, shipElements.Eccentricity, shipElements.GlobalRotationAngle, shipElements.SemiMajorAxis, shipElements.Clockwise);
        }
        

        //Display orbital poi
        float width = perigeePlacard.rectTransform.rect.width * perigeePlacard.transform.localScale.x;
        float height = perigeePlacard.rectTransform.rect.height * perigeePlacard.transform.localScale.y;
        Vector2 offsetVector = shipElements.Perigee.normalized * 
            Mathf.Sqrt(Mathf.Pow(width/2 , 2) + Mathf.Pow(height/2, 2));
        
        perigeeButton.transform.position = shipElements.GlobalTransformationVector + shipElements.Perigee + offsetVector;
        perigeeButton.transform.localScale = new Vector3(GlobalElements.zoomLevel/ GlobalElements.UI_SCALE_CONST, GlobalElements.zoomLevel / GlobalElements.UI_SCALE_CONST, 0);

        perigeePlacard.transform.position = shipElements.GlobalTransformationVector + shipElements.Perigee + offsetVector;
        perigeePlacard.transform.localScale = new Vector3(GlobalElements.zoomLevel / GlobalElements.UI_SCALE_CONST, GlobalElements.zoomLevel / GlobalElements.UI_SCALE_CONST, 0);
        perigeePlacard.transform.rotation = Quaternion.AngleAxis(Mathf.Atan2(shipElements.Perigee.y, shipElements.Perigee.x) * Mathf.Rad2Deg - 45, Vector3.forward);
        
        switch (shipElements.OrbitType)
        {
            case OrbitTypes.circular:
                apogeeButton.transform.position = shipElements.GlobalTransformationVector + shipElements.Apogee - offsetVector;
                apogeeButton.transform.localScale = new Vector3(GlobalElements.zoomLevel / GlobalElements.UI_SCALE_CONST, GlobalElements.zoomLevel / GlobalElements.UI_SCALE_CONST, 0);

                apogeePlacard.transform.position = shipElements.GlobalTransformationVector + shipElements.Apogee - offsetVector;
                apogeePlacard.transform.localScale = new Vector3(GlobalElements.zoomLevel / GlobalElements.UI_SCALE_CONST, GlobalElements.zoomLevel / GlobalElements.UI_SCALE_CONST, 0);
                apogeePlacard.transform.rotation = Quaternion.AngleAxis(Mathf.Atan2(shipElements.Apogee.y, shipElements.Apogee.x) * Mathf.Rad2Deg - 45, Vector3.forward);
                break;
            case OrbitTypes.elliptical:
                apogeeButton.transform.position = shipElements.GlobalTransformationVector + shipElements.Apogee - offsetVector;
                apogeeButton.transform.localScale = new Vector3(GlobalElements.zoomLevel / GlobalElements.UI_SCALE_CONST, GlobalElements.zoomLevel / GlobalElements.UI_SCALE_CONST, 0);

                apogeePlacard.transform.position = shipElements.GlobalTransformationVector + shipElements.Apogee - offsetVector;
                apogeePlacard.transform.localScale = new Vector3(GlobalElements.zoomLevel / GlobalElements.UI_SCALE_CONST, GlobalElements.zoomLevel / GlobalElements.UI_SCALE_CONST, 0);
                apogeePlacard.transform.rotation = Quaternion.AngleAxis(Mathf.Atan2(shipElements.Apogee.y, shipElements.Apogee.x) * Mathf.Rad2Deg - 45, Vector3.forward);
                break;
            default:
                apogeePlacard.transform.localScale = new Vector3(0, 0, 0);
                apogeeButton.transform.localScale = new Vector3(0, 0, 0);
                break;
        }
    }
    
    private Vector2 drawOrbitRadiusHelper(Vector2 eccentricity, double globalRotationAngle, double semiMajorAxis, double trueAnomaly)
    {
        trueAnomaly = MiscHelperFuncs.convertTo360Angle(trueAnomaly);
        double radius = (semiMajorAxis * (1 - eccentricity.sqrMagnitude)) / (1 + (eccentricity.magnitude * Math.Cos(trueAnomaly)));
        return (float)radius * new Vector2((float)Math.Cos(trueAnomaly + globalRotationAngle), (float)Math.Sin(trueAnomaly + globalRotationAngle));
        /*
        double radius = (semiMajorAxis * (1 - Vector2.SqrMagnitude(eccentricity))) / (1 - eccentricity.magnitude * (Math.Cos(angle - globalRotationAngle + Math.PI)));
        return new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * (float)radius;*/
    }

    public void drawPatchedConics(double startingTrueAnomaly, Vector2 globalTransformationVector, Vector2 eccentricity, double globalRotationAngle, double semiMajorAxis, bool clockwise)
    {
        //Display
        double angle = startingTrueAnomaly;
        switch (clockwise)
        {
            case true:
                while (angle > (2 * -Math.PI))
                {
                    if (encounter())
                    {
                        break;
                    }
                    lineDrawer.DrawLine(drawOrbitRadiusHelper(eccentricity, globalRotationAngle, semiMajorAxis, angle) + globalTransformationVector,
                        drawOrbitRadiusHelper(eccentricity, globalRotationAngle, semiMajorAxis, angle + 0.01f) + globalTransformationVector, Color.red);
                    angle -= 0.01d;
                }
                angle = 0;
                while(angle > startingTrueAnomaly)
                {
                    if (encounter())
                    {
                        break;
                    }
                    lineDrawer.DrawLine(drawOrbitRadiusHelper(eccentricity, globalRotationAngle, semiMajorAxis, angle) + globalTransformationVector,
                        drawOrbitRadiusHelper(eccentricity, globalRotationAngle, semiMajorAxis, angle + 0.01f) + globalTransformationVector, Color.red);
                    angle -= 0.01d;
                }
                break;
            case false:
                while (angle < (2 * Math.PI))
                {
                    if (encounter())
                    {
                        break;
                    }
                    lineDrawer.DrawLine(drawOrbitRadiusHelper(eccentricity, globalRotationAngle, semiMajorAxis, angle) + globalTransformationVector,
                        drawOrbitRadiusHelper(eccentricity, globalRotationAngle, semiMajorAxis, angle + 0.01f) + globalTransformationVector, Color.red);
                    angle += 0.01d;
                }
                angle = 0;
                while (angle < startingTrueAnomaly)
                {
                    if (encounter())
                    {
                        break;
                    }
                    lineDrawer.DrawLine(drawOrbitRadiusHelper(eccentricity, globalRotationAngle, semiMajorAxis, angle) + globalTransformationVector,
                        drawOrbitRadiusHelper(eccentricity, globalRotationAngle, semiMajorAxis, angle + 0.01f) + globalTransformationVector, Color.red);
                    angle += 0.01d;
                }
                break;
            default:
                break;
        }
    }

    private bool encounter()
    {
        return false;
    }
}
