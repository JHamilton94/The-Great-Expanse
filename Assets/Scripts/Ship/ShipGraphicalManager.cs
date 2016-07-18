using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

/**
*Must be executed after the preupdate
**/

public class ShipGraphicalManager : MonoBehaviour {

    private GravityElements shipElements;
    private SpriteRenderer spriteRenderer;
    private LineDrawer lineDrawer;

    public Button perigeeButton;
    public Image perigeePlacard;
    public Button apogeeButton;
    public Image apogeePlacard;

    public float UI_SCALE_CONST;

	void Start () {
        shipElements = GetComponent<GravityElements>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>(true);
        lineDrawer = GetComponentInChildren<LineDrawer>();
        Button[] buttons = GetComponentsInChildren<Button>();
        
    }
	
	void FixedUpdate () {
        //Render ships patched conics
        float angle = 0;
        while( angle < 2*Math.PI){
            lineDrawer.DrawLine(drawOrbitRadiusHelper(shipElements.Eccentricity, shipElements.GlobalRotationAngle, shipElements.SemiMajorAxis, angle) + shipElements.GlobalTransformationVector, 
                drawOrbitRadiusHelper(shipElements.Eccentricity, shipElements.GlobalRotationAngle, shipElements.SemiMajorAxis, angle + 0.01f) + shipElements.GlobalTransformationVector, Color.red);
            angle += 0.01f;
        }

        //Render orbital poi
        float width = perigeePlacard.rectTransform.rect.width * perigeePlacard.transform.localScale.x;
        float height = perigeePlacard.rectTransform.rect.height * perigeePlacard.transform.localScale.y;
        Vector2 offsetVector = shipElements.Perigee.normalized * 
            Mathf.Sqrt(Mathf.Pow(width/2 , 2) + Mathf.Pow(height/2, 2));
        
        perigeeButton.transform.position = shipElements.GlobalTransformationVector + shipElements.Perigee + offsetVector;
        perigeeButton.transform.localScale = new Vector3(GlobalElements.zoomLevel/ UI_SCALE_CONST, GlobalElements.zoomLevel / UI_SCALE_CONST, 0);

        perigeePlacard.transform.position = shipElements.GlobalTransformationVector + shipElements.Perigee + offsetVector;
        perigeePlacard.transform.localScale = new Vector3(GlobalElements.zoomLevel / UI_SCALE_CONST, GlobalElements.zoomLevel / UI_SCALE_CONST, 0);
        perigeePlacard.transform.rotation = Quaternion.AngleAxis(Mathf.Atan2(shipElements.Perigee.y, shipElements.Perigee.x) * Mathf.Rad2Deg - 45, Vector3.forward);
        
        switch (shipElements.OrbitType)
        {
            case OrbitTypes.circular:
                apogeeButton.transform.position = shipElements.GlobalTransformationVector + shipElements.Apogee - offsetVector;
                apogeeButton.transform.localScale = new Vector3(GlobalElements.zoomLevel / UI_SCALE_CONST, GlobalElements.zoomLevel / UI_SCALE_CONST, 0);

                apogeePlacard.transform.position = shipElements.GlobalTransformationVector + shipElements.Apogee - offsetVector;
                apogeePlacard.transform.localScale = new Vector3(GlobalElements.zoomLevel / UI_SCALE_CONST, GlobalElements.zoomLevel / UI_SCALE_CONST, 0);
                apogeePlacard.transform.rotation = Quaternion.AngleAxis(Mathf.Atan2(shipElements.Apogee.y, shipElements.Apogee.x) * Mathf.Rad2Deg - 45, Vector3.forward);
                break;
            case OrbitTypes.elliptical:
                apogeeButton.transform.position = shipElements.GlobalTransformationVector + shipElements.Apogee - offsetVector;
                apogeeButton.transform.localScale = new Vector3(GlobalElements.zoomLevel / UI_SCALE_CONST, GlobalElements.zoomLevel / UI_SCALE_CONST, 0);

                apogeePlacard.transform.position = shipElements.GlobalTransformationVector + shipElements.Apogee - offsetVector;
                apogeePlacard.transform.localScale = new Vector3(GlobalElements.zoomLevel / UI_SCALE_CONST, GlobalElements.zoomLevel / UI_SCALE_CONST, 0);
                apogeePlacard.transform.rotation = Quaternion.AngleAxis(Mathf.Atan2(shipElements.Apogee.y, shipElements.Apogee.x) * Mathf.Rad2Deg - 45, Vector3.forward);
                break;
            default:
                apogeePlacard.transform.localScale = new Vector3(0, 0, 0);
                apogeeButton.transform.localScale = new Vector3(0, 0, 0);
                break;

        }
        

        


        
    }

    private Vector2 drawOrbitRadiusHelper(Vector2 eccentricity, double globalRotationAngle, double semiMajorAxis, double angle)
    {
        double radius = (semiMajorAxis * (1 - Vector2.SqrMagnitude(eccentricity))) / (1 - eccentricity.magnitude * (Math.Cos(angle - globalRotationAngle + Math.PI)));
        return new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * (float)radius;
    }
}
