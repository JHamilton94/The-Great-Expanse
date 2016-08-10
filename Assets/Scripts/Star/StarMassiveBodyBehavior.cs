using UnityEngine;
using System.Collections;
using System;

public class StarMassiveBodyBehavior : MonoBehaviour
{

    public bool debugMode;

    private MassiveBodyElements massiveBodyElements;
    private GravityElements gravityElements;

    // Use this for initialization
    void Start()
    {
        massiveBodyElements = GetComponent<MassiveBodyElements>();
        switch (massiveBodyElements.massiveBodyType)
        {
            case MassiveBodyType.Black_Hole:
                massiveBodyElements.SphereOfInfluence = double.PositiveInfinity;
                break;
            default:
                gravityElements = GetComponent<GravityElements>();
                massiveBodyElements.SphereOfInfluence = calculateSphereOfInfluence(gravityElements.SemiMajorAxis,
            massiveBodyElements.mass, gravityElements.MassiveBody.GetComponent<MassiveBodyElements>().mass);
                break;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {

    }

    private double calculateSphereOfInfluence(double semiMajorAxis, double smallMass, double bigmass)
    {
        return semiMajorAxis * Math.Pow(smallMass / bigmass, 0.4d);
    }

    public void OnDrawGizmos()
    {
        if (debugMode && massiveBodyElements != null)
        {
            //Draw sphere of influence
            Gizmos.color = Color.yellow;
            double angle = 0;
            while (angle <= 2 * Math.PI)
            {
                Vector2 dirVec1 = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
                dirVec1.Normalize();
                dirVec1 *= (float)massiveBodyElements.SphereOfInfluence;

                angle += 0.01f;

                Vector2 dirVec2 = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
                dirVec2.Normalize();
                dirVec2 *= (float)massiveBodyElements.SphereOfInfluence;

                Gizmos.DrawLine(dirVec1 + convertToVec2(transform.position), dirVec2 + convertToVec2(transform.position));
            }
        }
    }

    private Vector2 convertToVec2(Vector3 inVec)
    {
        return new Vector2(inVec.x, inVec.y);
    }

}
