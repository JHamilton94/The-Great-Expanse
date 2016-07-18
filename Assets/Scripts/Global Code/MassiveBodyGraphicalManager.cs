using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class MassiveBodyGraphicalManager : MonoBehaviour {

    private GravityElements gravElements;
    private MassiveBodyElements massiveElements;
    private LineDrawer lineDrawer;

    private Color patchedConicsColor;

    public Button perigeeButton;
    public Image perigeePlacard;
    public Button apogeeButton;
    public Image apogeePlacard;
    public float UI_SCALE_CONST;


    // Use this for initialization
    void Start () {

        gravElements = GetComponentInChildren<GravityElements>();
        lineDrawer = Camera.main.GetComponent<LineDrawer>();
        massiveElements = GetComponent<MassiveBodyElements>();

        Debug.Log(Camera.main.transform.position);

        switch (gravElements.gravitationalType)
        {
            case GravitationalType.black_hole:
                patchedConicsColor = Color.black;
                break;
            case GravitationalType.star:
                patchedConicsColor = Color.yellow;
                break;
            case GravitationalType.planet:
                patchedConicsColor = Color.blue;
                break;
                
        }
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        //Display Patched Conics
        float angle = 0;
        while (angle < 2 * Math.PI)
        {
            lineDrawer.DrawLine(drawOrbitRadiusHelper(gravElements.Eccentricity, gravElements.GlobalRotationAngle, gravElements.SemiMajorAxis, angle) + gravElements.GlobalTransformationVector,
                drawOrbitRadiusHelper(gravElements.Eccentricity, gravElements.GlobalRotationAngle, gravElements.SemiMajorAxis, angle + 0.01f) + gravElements.GlobalTransformationVector, patchedConicsColor);
            angle += 0.01f;
        }

        //Display SOI
        angle = 0;
        while (angle <= 2 * Math.PI)
        {
            Vector2 dirVec1 = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
            dirVec1.Normalize();
            dirVec1 *= (float)massiveElements.SphereOfInfluence;

            angle += 0.01f;

            Vector2 dirVec2 = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
            dirVec2.Normalize();
            dirVec2 *= (float)massiveElements.SphereOfInfluence;

            lineDrawer.DrawLine(dirVec1 + MiscHelperFuncs.convertToVec2(transform.position), dirVec2 + MiscHelperFuncs.convertToVec2(transform.position), patchedConicsColor);
            
        }

        /**
        Messy and unnecessary, i might bring it back though
        */

        //Display Orbital POI
        /*float width = perigeePlacard.rectTransform.rect.width * perigeePlacard.transform.localScale.x;
        float height = perigeePlacard.rectTransform.rect.height * perigeePlacard.transform.localScale.y;
        Vector2 offsetVector = gravElements.Perigee.normalized *
            Mathf.Sqrt(Mathf.Pow(width / 2, 2) + Mathf.Pow(height / 2, 2));

        perigeeButton.transform.position = gravElements.GlobalTransformationVector + gravElements.Perigee + offsetVector;
        perigeeButton.transform.localScale = new Vector3(GlobalElements.zoomLevel / UI_SCALE_CONST, GlobalElements.zoomLevel / UI_SCALE_CONST, 0);

        perigeePlacard.transform.position = gravElements.GlobalTransformationVector + gravElements.Perigee + offsetVector;
        perigeePlacard.transform.localScale = new Vector3(GlobalElements.zoomLevel / UI_SCALE_CONST, GlobalElements.zoomLevel / UI_SCALE_CONST, 0);
        perigeePlacard.transform.rotation = Quaternion.AngleAxis(Mathf.Atan2(gravElements.Perigee.y, gravElements.Perigee.x) * Mathf.Rad2Deg - 45, Vector3.forward);

        switch (gravElements.OrbitType)
        {
            case OrbitTypes.circular:
                apogeeButton.transform.position = gravElements.GlobalTransformationVector + gravElements.Apogee - offsetVector;
                apogeeButton.transform.localScale = new Vector3(GlobalElements.zoomLevel / UI_SCALE_CONST, GlobalElements.zoomLevel / UI_SCALE_CONST, 0);

                apogeePlacard.transform.position = gravElements.GlobalTransformationVector + gravElements.Apogee - offsetVector;
                apogeePlacard.transform.localScale = new Vector3(GlobalElements.zoomLevel / UI_SCALE_CONST, GlobalElements.zoomLevel / UI_SCALE_CONST, 0);
                apogeePlacard.transform.rotation = Quaternion.AngleAxis(Mathf.Atan2(gravElements.Apogee.y, gravElements.Apogee.x) * Mathf.Rad2Deg - 45, Vector3.forward);
                break;
            case OrbitTypes.elliptical:
                apogeeButton.transform.position = gravElements.GlobalTransformationVector + gravElements.Apogee - offsetVector;
                apogeeButton.transform.localScale = new Vector3(GlobalElements.zoomLevel / UI_SCALE_CONST, GlobalElements.zoomLevel / UI_SCALE_CONST, 0);

                apogeePlacard.transform.position = gravElements.GlobalTransformationVector + gravElements.Apogee - offsetVector;
                apogeePlacard.transform.localScale = new Vector3(GlobalElements.zoomLevel / UI_SCALE_CONST, GlobalElements.zoomLevel / UI_SCALE_CONST, 0);
                apogeePlacard.transform.rotation = Quaternion.AngleAxis(Mathf.Atan2(gravElements.Apogee.y, gravElements.Apogee.x) * Mathf.Rad2Deg - 45, Vector3.forward);
                break;
            default:
                apogeePlacard.transform.localScale = new Vector3(0, 0, 0);
                apogeeButton.transform.localScale = new Vector3(0, 0, 0);
                break;

        }*/
    }

    private Vector2 drawOrbitRadiusHelper(Vector2 eccentricity, double globalRotationAngle, double semiMajorAxis, double angle)
    {
        double radius = (semiMajorAxis * (1 - Vector2.SqrMagnitude(eccentricity))) / (1 - eccentricity.magnitude * (Math.Cos(angle - globalRotationAngle + Math.PI)));
        return new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * (float)radius;
    }
}
