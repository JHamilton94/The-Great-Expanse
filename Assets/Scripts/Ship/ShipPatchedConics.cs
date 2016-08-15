using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

/**
*Must be executed after the preupdate
**/

public class ShipPatchedConics : MonoBehaviour
{

    //Data sources
    private GravityElements shipElements;
    private ShipGravityBehavior ship;
    private SpriteRenderer spriteRenderer;
    private LineDrawer lineDrawer;
    private NodeManager nodeManager;

    //Orbital POI
    public Button perigeeButton;
    public Image perigeePlacard;
    public Button apogeeButton;
    public Image apogeePlacard;

    private bool onEscapeTrajectory;
    public GameObject encounterIconPrefab;
    private Queue<GravityElementsClass> encounters; //Predicted exit of the sphere of influence, the predicted gravity elements for the sphere of influence
    private Queue<GravityElementsClass> potentialEncounters;
    private int maxSphereChanges;
    private int currentSphereChanges;

    private bool instantiated;

    void Start()
    {
        shipElements = GetComponent<GravityElements>();
        ship = GetComponent<ShipGravityBehavior>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>(true);
        lineDrawer = GetComponentInChildren<LineDrawer>();
        nodeManager = GetComponent<NodeManager>();

        encounters = new Queue<GravityElementsClass>();
        potentialEncounters = new Queue<GravityElementsClass>();
    }

    void FixedUpdate()
    {

        foreach (GravityElementsClass encounter in encounters)
        {
            encounter.GlobalTransformationVector = encounter.massiveBody.transform.position;
        }

        foreach (GravityElementsClass encounter in potentialEncounters)
        {
            encounter.GlobalTransformationVector = encounter.massiveBody.transform.position;
        }

        //Display ships patched conics
        //currentSphereChanges = 0;
        drawPatchedConics(false, shipElements.massiveBody.GetComponent<MassiveBodyElements>(), shipElements.TrueAnomaly, shipElements.GlobalTransformationVector, shipElements.Eccentricity, shipElements.GlobalRotationAngle, shipElements.SemiMajorAxis, shipElements.Clockwise, Color.red);

        if (encounters.Count > 0)
        {
            drawPatchedConics(true, encounters.Peek().massiveBody.GetComponent<MassiveBodyElements>(), encounters.Peek().TrueAnomaly, encounters.Peek().GlobalTransformationVector, encounters.Peek().Eccentricity, encounters.Peek().GlobalRotationAngle, encounters.Peek().SemiMajorAxis, encounters.Peek().Clockwise, Color.red);
        }
        if (potentialEncounters.Count > 0)
        {
            drawPatchedConics(true, potentialEncounters.Peek().massiveBody.GetComponent<MassiveBodyElements>(), potentialEncounters.Peek().TrueAnomaly, potentialEncounters.Peek().GlobalTransformationVector, potentialEncounters.Peek().Eccentricity, potentialEncounters.Peek().GlobalRotationAngle, potentialEncounters.Peek().SemiMajorAxis, potentialEncounters.Peek().Clockwise, Color.cyan);
        }


        //Display orbital poi
        /*float width = perigeePlacard.rectTransform.rect.width * perigeePlacard.transform.localScale.x;
        float height = perigeePlacard.rectTransform.rect.height * perigeePlacard.transform.localScale.y;
        Vector2 offsetVector = shipElements.Perigee.normalized * 
            Mathf.Sqrt(Mathf.Pow(width/2 , 2) + Mathf.Pow(height/2, 2));
        
        perigeeButton.transform.position = shipElements.GlobalTransformationVector + shipElements.Perigee + offsetVector;
        perigeeButton.transform.localScale = new Vector3(GlobalElements.zoomLevel/ GlobalElements.UI_SCALE_CONST, GlobalElements.zoomLevel / GlobalElements.UI_SCALE_CONST, 0);

        perigeePlacard.transform.position = shipElements.GlobalTransformationVector + shipElements.Perigee + offsetVector;
        perigeePlacard.transform.localScale = new Vector3(GlobalElements.zoomLevel / GlobalElements.UI_SCALE_CONST, GlobalElements.zoomLevel / GlobalElements.UI_SCALE_CONST, 0);
        perigeePlacard.transform.rotation = Quaternion.AngleAxis(Mathf.Atan2(shipElements.Perigee.y, shipElements.Perigee.x) * Mathf.Rad2Deg - 45, Vector3.forward);

        foreach (Tuple<Vector2, GameObject> tuple in encounterIcons){
            tuple.item2.transform.position = tuple.item1 + shipElements.GlobalTransformationVector;
        }

        if (onEscapeTrajectory)
        {
            if (shipElements.Clockwise)
            {
                if(shipElements.TrueAnomaly < 0)
                {
                    perigeePlacard.transform.localScale = new Vector3(0, 0, 0);
                    perigeeButton.transform.localScale = new Vector3(0, 0, 0);
                }
            }
            else
            {
                if(shipElements.TrueAnomaly > 0)
                {
                    perigeePlacard.transform.localScale = new Vector3(0, 0, 0);
                    perigeeButton.transform.localScale = new Vector3(0, 0, 0);
                }
            }
        }

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
        }*/
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

    public void drawPatchedConics(bool prediction, MassiveBodyElements currentMassiveBody, double startingTrueAnomaly, Vector2 globalTransformationVector, Vector2 eccentricity, double globalRotationAngle, double semiMajorAxis, bool clockwise, Color color)
    {

        //Display
        bool breakout = false;
        double angle = startingTrueAnomaly;
        switch (clockwise)
        {
            case true:
                while (angle > (2 * -Math.PI) && !breakout)
                {

                    lineDrawer.DrawLine(drawOrbitRadiusHelper(eccentricity, globalRotationAngle, semiMajorAxis, angle) + globalTransformationVector,
                        drawOrbitRadiusHelper(eccentricity, globalRotationAngle, semiMajorAxis, angle + 0.01f) + globalTransformationVector, color);
                    angle -= 0.01d;
                }
                angle = 0;
                while (angle > startingTrueAnomaly && !breakout)
                {

                    lineDrawer.DrawLine(drawOrbitRadiusHelper(eccentricity, globalRotationAngle, semiMajorAxis, angle) + globalTransformationVector,
                        drawOrbitRadiusHelper(eccentricity, globalRotationAngle, semiMajorAxis, angle + 0.01f) + globalTransformationVector, color);
                    angle -= 0.01d;
                }
                break;
            case false:
                while (angle < (2 * Math.PI) && !breakout)
                {

                    lineDrawer.DrawLine(drawOrbitRadiusHelper(eccentricity, globalRotationAngle, semiMajorAxis, angle) + globalTransformationVector,
                        drawOrbitRadiusHelper(eccentricity, globalRotationAngle, semiMajorAxis, angle + 0.01f) + globalTransformationVector, color);
                    angle += 0.01d;
                }
                angle = 0;
                while (angle < startingTrueAnomaly && !breakout)
                {

                    lineDrawer.DrawLine(drawOrbitRadiusHelper(eccentricity, globalRotationAngle, semiMajorAxis, angle) + globalTransformationVector,
                        drawOrbitRadiusHelper(eccentricity, globalRotationAngle, semiMajorAxis, angle + 0.01f) + globalTransformationVector, color);
                    angle += 0.01d;
                }
                break;
            default:
                break;
        }
    }

    //Takes in an amount of time x and a celestial body and predicts where it will be in x amount of time
    private Vector2 predictedPosition(double time, GravityElements celestialBody)
    {
        return new Vector2();
    }

    //Finds all encounters within one orbit of the craft
    public void updateEncounters()
    {
        if (encounters == null)
        {
            return;
        }

        encounters.Clear();
        GravityElementsClass firstEncounter;
        MassiveBodyElements shipsMassiveBody = shipElements.massiveBody.GetComponent<MassiveBodyElements>();
        GravityElements shipsMassivebodyGravityElements = shipElements.massiveBody.GetComponent<GravityElements>();

        double time = 0;
        double orbitalPeriod = calculatePeriod(shipElements.OrbitType, shipElements.SemiMajorAxis, shipElements.Mu);

        List<GameObject> satelites = shipElements.massiveBody.GetComponent<MassiveBodyElements>().satelites;
        GameObject encounteredMassiveBody = null;

        while (time < orbitalPeriod && encounteredMassiveBody == null)
        {
            time += 0.01;
            //Are we going to exit a soi?
            if (shipElements.calculateLocalPositionAtFutureTime(time).magnitude > shipsMassiveBody.SphereOfInfluence)
            {
                encounteredMassiveBody = shipElements.massiveBody.GetComponent<GravityElements>().massiveBody;
                Debug.Log("Exiting to " + encounteredMassiveBody.name + "'s soi");
            }

            foreach (GameObject satelite in satelites)
            {
                //Are we going to enter a new soi?
                Vector2 shipPredictedLocalPositionRelativeToCurrentMassiveBody;
                Vector2 satelitePredictedLocalPositionRelativeToCurrentMassiveBody;

                satelitePredictedLocalPositionRelativeToCurrentMassiveBody = satelite.GetComponent<GravityElements>().calculateLocalPositionAtFutureTime(time);
                shipPredictedLocalPositionRelativeToCurrentMassiveBody = shipElements.calculateLocalPositionAtFutureTime(time);
                if (Vector2.Distance(satelitePredictedLocalPositionRelativeToCurrentMassiveBody, shipPredictedLocalPositionRelativeToCurrentMassiveBody) < satelite.GetComponent<MassiveBodyElements>().SphereOfInfluence)
                {
                    encounteredMassiveBody = satelite;
                    Debug.Log("Entering " + encounteredMassiveBody.name + "'s soi");
                }
            }

            if (time > 10000)//prevent infinite loops in hyperbolic cases
            {
                break;
            }

        }

        if (encounteredMassiveBody == null)
        {
            return;
        }

        Tuple<Vector2, Vector2> returnInfo = encounteredMassiveBody.GetComponent<GravityElements>().calculateGlobalPositionAndVelocityAtFutureTime(time);
        Vector2 encounteredMassiveBodyPredictedGlobalPosition = returnInfo.item1;
        Vector2 encounteredMassiveBodyPredictedGlobalVelocity = returnInfo.item2;

        returnInfo = shipElements.calculateLocalPositionAndVelocityAtFutureTime(time);
        Vector2 shipPredictedLocalPosition = returnInfo.item1;
        Vector2 shipPredictedLocalVelocity = returnInfo.item2;

        returnInfo = shipElements.massiveBody.GetComponent<GravityElements>().calculateGlobalPositionAndVelocityAtFutureTime(time);
        Vector2 currentMassiveBodyPredictedGlobalPosition = returnInfo.item1;
        Vector2 currentMassiveBodyPredictedGlobalVelcity = returnInfo.item2;

        Vector2 shipPredictedEncounterLocalPosition = (shipPredictedLocalPosition + currentMassiveBodyPredictedGlobalPosition) - encounteredMassiveBodyPredictedGlobalPosition;
        Vector2 shipPredictedEncounterLocalVelocity = (shipPredictedLocalVelocity + currentMassiveBodyPredictedGlobalVelcity) - encounteredMassiveBodyPredictedGlobalVelocity;
        Vector2 predictedGlobalTransformationVector = encounteredMassiveBodyPredictedGlobalPosition;

        firstEncounter = calculateInitialOrbitalElements(shipPredictedEncounterLocalPosition, shipPredictedEncounterLocalVelocity, encounteredMassiveBody);

        encounters.Enqueue(firstEncounter);



    }

    public void updatePotentialEncounters(Vector2 maneuverVelocity, Vector2 maneuverPosition)
    {
        if (potentialEncounters == null)
        {
            return;
        }

        GravityElementsClass currentEncounter = calculateInitialOrbitalElements(maneuverPosition, maneuverVelocity, shipElements.massiveBody);

        potentialEncounters.Clear();
        potentialEncounters.Enqueue(currentEncounter);

        GravityElementsClass firstEncounter;
        MassiveBodyElements shipsMassiveBody = currentEncounter.massiveBody.GetComponent<MassiveBodyElements>();
        GravityElements shipsMassivebodyGravityElements = currentEncounter.massiveBody.GetComponent<GravityElements>();

        double time = 0;
        double orbitalPeriod = calculatePeriod(currentEncounter.OrbitType, currentEncounter.SemiMajorAxis, currentEncounter.Mu);

        List<GameObject> satelites = currentEncounter.massiveBody.GetComponent<MassiveBodyElements>().satelites;
        GameObject encounteredMassiveBody = null;

        while (time < orbitalPeriod && encounteredMassiveBody == null)
        {
            time += 0.01;
            //Are we going to exit a soi?
            if (currentEncounter.calculateLocalPositionAtFutureTime(time).magnitude > shipsMassiveBody.SphereOfInfluence)
            {
                encounteredMassiveBody = currentEncounter.massiveBody.GetComponent<GravityElements>().massiveBody;
                Debug.Log("Exiting to " + encounteredMassiveBody.name + "'s soi");
            }

            foreach (GameObject satelite in satelites)
            {
                //Are we going to enter a new soi?
                Vector2 shipPredictedLocalPositionRelativeToCurrentMassiveBody;
                Vector2 satelitePredictedLocalPositionRelativeToCurrentMassiveBody;

                satelitePredictedLocalPositionRelativeToCurrentMassiveBody = satelite.GetComponent<GravityElements>().calculateLocalPositionAtFutureTime(time);
                shipPredictedLocalPositionRelativeToCurrentMassiveBody = currentEncounter.calculateLocalPositionAtFutureTime(time);
                if (Vector2.Distance(satelitePredictedLocalPositionRelativeToCurrentMassiveBody, shipPredictedLocalPositionRelativeToCurrentMassiveBody) < satelite.GetComponent<MassiveBodyElements>().SphereOfInfluence)
                {
                    encounteredMassiveBody = satelite;
                    Debug.Log("Entering " + encounteredMassiveBody.name + "'s soi");
                }
            }

            if (time > 10000)//prevent infinite loops in hyperbolic cases
            {
                break;
            }

        }

        if (encounteredMassiveBody == null)
        {
            return;
        }

        Tuple<Vector2, Vector2> returnInfo = encounteredMassiveBody.GetComponent<GravityElements>().calculateGlobalPositionAndVelocityAtFutureTime(time);
        Vector2 encounteredMassiveBodyPredictedGlobalPosition = returnInfo.item1;
        Vector2 encounteredMassiveBodyPredictedGlobalVelocity = returnInfo.item2;

        returnInfo = currentEncounter.calculateLocalPositionAndVelocityAtFutureTime(time);
        Vector2 shipPredictedLocalPosition = returnInfo.item1;
        Vector2 shipPredictedLocalVelocity = returnInfo.item2;

        returnInfo = currentEncounter.massiveBody.GetComponent<GravityElements>().calculateGlobalPositionAndVelocityAtFutureTime(time);
        Vector2 currentMassiveBodyPredictedGlobalPosition = returnInfo.item1;
        Vector2 currentMassiveBodyPredictedGlobalVelcity = returnInfo.item2;

        Vector2 shipPredictedEncounterLocalPosition = (shipPredictedLocalPosition + currentMassiveBodyPredictedGlobalPosition) - encounteredMassiveBodyPredictedGlobalPosition;
        Vector2 shipPredictedEncounterLocalVelocity = (shipPredictedLocalVelocity + currentMassiveBodyPredictedGlobalVelcity) - encounteredMassiveBodyPredictedGlobalVelocity;
        Vector2 predictedGlobalTransformationVector = encounteredMassiveBodyPredictedGlobalPosition;

        firstEncounter = calculateInitialOrbitalElements(shipPredictedEncounterLocalPosition, shipPredictedEncounterLocalVelocity, encounteredMassiveBody);

        encounters.Enqueue(firstEncounter);
    }

    public void clearPotentialEncounters()
    {
        potentialEncounters.Clear();
    }

    private double calculatePeriod(OrbitTypes orbitType, double semiMajorAxis, double mu)
    {
        switch (orbitType)
        {
            case OrbitTypes.hyperbolic:
                return double.PositiveInfinity;
            case OrbitTypes.parabolic:
                return double.PositiveInfinity;
            default:
                double period = Math.Sqrt((4 * Math.Pow(Math.PI, 2) * Math.Pow(semiMajorAxis, 3)) / mu);
                return period;

        }
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
        gravityElements.GlobalTransformationVector = shipElements.GlobalTransformationVector;

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

        /*Debug.Log("Encounter info dump");
        Debug.Log("Eccentricity: " + gravityElements.Eccentricity);
        Debug.Log("Mechanical Energy: " + gravityElements.MechanicalEnergy);
        Debug.Log("SemiMajor Axis: " + gravityElements.SemiMajorAxis);
        Debug.Log("SemiLatus Rectum: " + gravityElements.SemiLatusRectum);
        Debug.Log("Perigee: " + gravityElements.Perigee);
        Debug.Log("Apogee: " + gravityElements.Apogee);
        Debug.Log("Center: " + gravityElements.Center);
        Debug.Log("Global Rotation Angle: " + gravityElements.GlobalRotationAngle);
        Debug.Log("Clockwise: " + gravityElements.Clockwise);
        Debug.Log("Towards Perigee: " + gravityElements.TowardsPerigee);
        Debug.Log("TrueAnomaly: " + gravityElements.TrueAnomaly);
        Debug.Log("EccentricAnomaly: " + gravityElements.EccentricAnomaly);
        Debug.Log("Anomaly At Epoch: " + gravityElements.AnomalyAtEpoch);
        Debug.Log("Mean Anomaly: " + gravityElements.MeanAnomaly);
        Debug.Log("Angular Momentum: " + gravityElements.AngularMomentum);
        Debug.Log("Time At Epoch: " + gravityElements.TimeAtEpoch);*/

        return gravityElements;
    }
}
