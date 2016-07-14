using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class StarGravityBehavior : MonoBehaviour
{

    public bool debugMode;
    public GravityElements gravityElements;

    // Use this for initialization
    void Start()
    {

        gravityElements = this.GetComponent<GravityElements>();
        gravityElements.massiveBody = GameObject.Find("black_hole");
        gravityElements.Mu = GlobalElements.GRAV_CONST * gravityElements.MassiveBody.GetComponent<MassiveBodyElements>().mass;
        gravityElements.TimeStep = GlobalElements.timeStep;
        gravityElements.Position = transform.position - gravityElements.MassiveBody.transform.position;

        //force the massive body to calculate its sphere of influence

        calculateInitialOrbitalElements(gravityElements.Position, gravityElements.velocity);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        calculateNextOrbitalElements();
        moveShip();
    }

    private void moveShip()
    {
        transform.position = gravityElements.Position + gravityElements.GlobalTransformationVector;
    }

    public void applyThrust(Vector2 thrust)
    {
        calculateInitialOrbitalElements(gravityElements.Position, gravityElements.velocity + thrust);
    }

    private GameObject findInfluencingCelestialBody()
    {

        GameObject returnObject;
        GameObject[] massiveBodies = GameObject.FindGameObjectsWithTag("MassiveBody");

        List<GameObject> influencedBy = new List<GameObject>();

        for (int i = 0; i < massiveBodies.Length; i++)
        {
            double distance = Vector2.Distance(massiveBodies[i].transform.position, transform.position);
            if (distance < massiveBodies[i].GetComponent<MassiveBodyElements>().SphereOfInfluence
                && massiveBodies[i].transform.GetInstanceID() != this.transform.GetInstanceID())
            {
                influencedBy.Add(massiveBodies[i]);
            }
        }

        returnObject = null;
        double maxDistance = double.PositiveInfinity;
        for (int i = 0; i < influencedBy.Count; i++)
        {
            double distance = Vector2.Distance(influencedBy[i].transform.position, transform.position);
            if (distance < maxDistance)
            {
                returnObject = influencedBy[i];
                maxDistance = distance;
            }
        }
        return returnObject;
    }

    private void calculateNextOrbitalElements()
    {

        //update timestep
        gravityElements.TimeStep = GlobalElements.timeStep;

        //Adjust tranformation vector
        gravityElements.GlobalTransformationVector = gravityElements.MassiveBody.transform.position;

        //Calculate next meanAnomaly
        gravityElements.MeanAnomaly = OrbitalHelper.calculateMeanAnomaly(gravityElements.Eccentricity, gravityElements.SemiMajorAxis, gravityElements.AnomalyAtEpoch,
            gravityElements.TimeStep, gravityElements.TimeAtEpoch, gravityElements.Clockwise, gravityElements.Mu, gravityElements.OrbitType);

        //Calculate Eccentric Anomaly
        gravityElements.EccentricAnomaly = OrbitalHelper.calculateEccentricAnomaly(gravityElements.Eccentricity, gravityElements.SemiMajorAxis, GlobalElements.GRAV_CONST, gravityElements.TimeStep, gravityElements.TimeAtEpoch,
            gravityElements.MeanAnomaly, gravityElements.EccentricAnomaly, gravityElements.Mu, gravityElements.Clockwise, gravityElements.OrbitType);

        //CalculateTrueAnomaly
        gravityElements.TrueAnomaly = OrbitalHelper.calculateTrueAnomaly(gravityElements.Eccentricity, gravityElements.EccentricAnomaly, gravityElements.MeanAnomaly, gravityElements.OrbitType);

        //Calculate Altitude
        gravityElements.Altitude = OrbitalHelper.calculateAltitude(gravityElements.Eccentricity, gravityElements.SemiMajorAxis, gravityElements.SemiLatusRectum, gravityElements.TrueAnomaly, gravityElements.OrbitType);

        //Calculate positionVector
        gravityElements.Position = OrbitalHelper.calculatePosition(gravityElements.Perigee, gravityElements.TrueAnomaly, gravityElements.GlobalRotationAngle, gravityElements.Altitude, gravityElements.OrbitType);

        //Are we going towards the perigee?
        gravityElements.TowardsPerigee = OrbitalHelper.towardsPerigeeOrbit(gravityElements.MeanAnomaly, gravityElements.Clockwise, gravityElements.TimeAtEpoch, gravityElements.OrbitType);

        //Calculate velocity angle
        gravityElements.VelocityAngle = OrbitalHelper.calculateVelocityAngle(gravityElements.Position, gravityElements.Eccentricity, gravityElements.SemiMajorAxis,
            gravityElements.TrueAnomaly, gravityElements.GlobalRotationAngle, gravityElements.Clockwise, gravityElements.TowardsPerigee, gravityElements.OrbitType);

        //Calculate Speed
        gravityElements.Speed = OrbitalHelper.calculateSpeed(gravityElements.Position, gravityElements.SemiMajorAxis, gravityElements.Mu, gravityElements.OrbitType);

        //Calculate Velocity
        gravityElements.velocity = OrbitalHelper.assembleVelocityVector(gravityElements.VelocityAngle, gravityElements.Speed);

        //advance epoch
        gravityElements.AnomalyAtEpoch = gravityElements.MeanAnomaly;

        //Advance time
        gravityElements.TimeAtEpoch = OrbitalHelper.advanceTime(gravityElements.TimeAtEpoch, gravityElements.TimeStep, gravityElements.Clockwise, gravityElements.OrbitType);

        //change spheres of influence
        changeSpheresOfInfluence(this.transform.position, gravityElements.velocity, gravityElements.massiveBody, gravityElements.Altitude);
    }

    //<summary>
    //Determines whether or not to change the sphere of influence then changes it
    //</summary>
    //<param name="position"> The position of the ship in global coordinates
    //<param name="velocity"> The velocity relative to the current body being orbited
    //<param name="the rest"> Fuck it you get the idea
    //<returns> Returns nothing, like a renter after blockbuster closed
    private void changeSpheresOfInfluence(Vector2 position, Vector2 velocity, GameObject massiveBody, double altitude)
    {
        switch (massiveBody.GetComponent<MassiveBodyElements>().massiveBodyType)
        {
            case MassiveBodyType.Black_Hole:
                if (altitude > massiveBody.GetComponent<MassiveBodyElements>().SphereOfInfluence)
                {
                    gravityElements.MassiveBody = findInfluencingCelestialBody();
                    calculateInitialOrbitalElements(MiscHelperFuncs.convertToVec3(position) - massiveBody.transform.position, velocity);
                }
                break;
            default:
                if (altitude > massiveBody.GetComponent<MassiveBodyElements>().SphereOfInfluence)
                {
                    Debug.Log("Not black hole");
                    Debug.Log("Altitude: " + altitude);
                    Debug.Log("sphere of influence: " + gravityElements.massiveBody.GetComponent<MassiveBodyElements>().SphereOfInfluence);
                    gravityElements.MassiveBody = findInfluencingCelestialBody();
                    calculateInitialOrbitalElements(MiscHelperFuncs.convertToVec3(position), velocity + massiveBody.GetComponent<GravityElements>().velocity);
                }
                break;
        }

    }
    
    //<summary>
    //Takes in position and velocity realtive to the body being orbited
    //</summary>
    private void calculateInitialOrbitalElements(Vector2 position, Vector2 velocity)
    {
        gravityElements.Mu = GlobalElements.GRAV_CONST * gravityElements.MassiveBody.GetComponent<MassiveBodyElements>().mass;
        gravityElements.Position = position;
        gravityElements.velocity = velocity;

        //Calculate Global Tranformation Vector
        gravityElements.GlobalTransformationVector = OrbitalHelper.calculateGlobalTranformationVector(gravityElements.MassiveBody);

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

    }

    

    //HELPER FUNCTIONS
    


    public void OnDrawGizmos()
    {
        if (debugMode && gravityElements != null)
        {
            //Draw perigee
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(gravityElements.GlobalTransformationVector + gravityElements.Perigee, 0.1f);

            //draw apogee
            Gizmos.DrawSphere(gravityElements.GlobalTransformationVector + gravityElements.Apogee, 0.1f);

            //Draw semiMajor Axis
            Gizmos.color = Color.red;
            Gizmos.DrawLine(gravityElements.GlobalTransformationVector + gravityElements.Perigee, gravityElements.GlobalTransformationVector + gravityElements.Perigee + (gravityElements.Perigee.normalized * -(float)gravityElements.SemiMajorAxis));

            //Draw eccentricity
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(gravityElements.GlobalTransformationVector, gravityElements.GlobalTransformationVector + gravityElements.Eccentricity);

            //Draw the ellipse
            double angle = 0;
            while (angle < 2 * Math.PI)
            {
                Gizmos.DrawLine(drawOrbitRadiusHelper(gravityElements.Eccentricity, gravityElements.GlobalRotationAngle, gravityElements.SemiMajorAxis, angle) + gravityElements.GlobalTransformationVector,
                    drawOrbitRadiusHelper(gravityElements.Eccentricity, gravityElements.GlobalRotationAngle, gravityElements.SemiMajorAxis, angle + 0.01d) + gravityElements.GlobalTransformationVector);
                angle += 0.01d;
            }

            //Draw Center
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(gravityElements.GlobalTransformationVector + gravityElements.Center, 0.2f);

            //Draw position
            Gizmos.color = Color.white;
            Gizmos.DrawSphere(gravityElements.Position + gravityElements.GlobalTransformationVector, 0.1f);
            Gizmos.DrawLine(gravityElements.GlobalTransformationVector, gravityElements.GlobalTransformationVector + gravityElements.Position);

            //Draw velocity
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(gravityElements.GlobalTransformationVector + gravityElements.Position, gravityElements.GlobalTransformationVector + gravityElements.Position + gravityElements.velocity);

            //Draw mean anomaly
            Gizmos.color = Color.green;
            Gizmos.DrawLine(gravityElements.GlobalTransformationVector, gravityElements.GlobalTransformationVector + new Vector2((float)Math.Cos(gravityElements.MeanAnomaly + gravityElements.GlobalRotationAngle), (float)Math.Sin(gravityElements.MeanAnomaly + gravityElements.GlobalRotationAngle)) * 10f);
        }
    }

    private Vector2 drawOrbitRadiusHelper(Vector2 eccentricity, double globalRotationAngle, double semiMajorAxis, double angle)
    {
        double radius = (semiMajorAxis * (1 - Vector2.SqrMagnitude(eccentricity))) / (1 - eccentricity.magnitude * (Math.Cos(angle - globalRotationAngle + Math.PI)));
        return new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * (float)radius;
    }
}
