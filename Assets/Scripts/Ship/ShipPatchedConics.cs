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
    private int maxSphereChanges;
    private int currentSphereChanges;

    private bool instantiated;

	void Start () {
        shipElements = GetComponent<GravityElements>();
        ship = GetComponent<ShipGravityBehavior>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>(true);
        lineDrawer = GetComponentInChildren<LineDrawer>();
        nodeManager = GetComponent<NodeManager>();

        encounters = new Queue<GravityElementsClass>();
    }
	
	void FixedUpdate () {
        //Display ships patched conics
        //currentSphereChanges = 0;
        drawPatchedConics(false, shipElements.massiveBody.GetComponent<MassiveBodyElements>(), shipElements.TrueAnomaly, shipElements.GlobalTransformationVector, shipElements.Eccentricity, shipElements.GlobalRotationAngle, shipElements.SemiMajorAxis, shipElements.Clockwise);
        
        if(encounters.Count > 0)
        {
            drawPatchedConics(true, encounters.Peek().massiveBody.GetComponent<MassiveBodyElements>(), encounters.Peek().TrueAnomaly, encounters.Peek().GlobalTransformationVector, encounters.Peek().Eccentricity, encounters.Peek().GlobalRotationAngle, encounters.Peek().SemiMajorAxis, encounters.Peek().Clockwise);
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

    public void drawPatchedConics(bool prediction, MassiveBodyElements currentMassiveBody, double startingTrueAnomaly, Vector2 globalTransformationVector, Vector2 eccentricity, double globalRotationAngle, double semiMajorAxis, bool clockwise)
    {

        //Display
        bool breakout = false;
        double angle = startingTrueAnomaly;
        switch (clockwise)
        {
            case true:
                while (angle > (2 * -Math.PI) && !breakout)
                {
                    if (encounter(drawOrbitRadiusHelper(eccentricity, globalRotationAngle, semiMajorAxis, angle + 0.01f), currentMassiveBody.SphereOfInfluence) && !prediction)
                    {
                        breakout = true;
                        break;
                    }
                    lineDrawer.DrawLine(drawOrbitRadiusHelper(eccentricity, globalRotationAngle, semiMajorAxis, angle) + globalTransformationVector,
                        drawOrbitRadiusHelper(eccentricity, globalRotationAngle, semiMajorAxis, angle + 0.01f) + globalTransformationVector, Color.red);
                    angle -= 0.01d;
                }
                angle = 0;
                while(angle > startingTrueAnomaly && !breakout)
                {
                    if (encounter(drawOrbitRadiusHelper(eccentricity, globalRotationAngle, semiMajorAxis, angle), currentMassiveBody.SphereOfInfluence) && !prediction)
                    {
                        breakout = true;
                        break;
                    }
                    lineDrawer.DrawLine(drawOrbitRadiusHelper(eccentricity, globalRotationAngle, semiMajorAxis, angle) + globalTransformationVector,
                        drawOrbitRadiusHelper(eccentricity, globalRotationAngle, semiMajorAxis, angle + 0.01f) + globalTransformationVector, Color.red);
                    angle -= 0.01d;
                }
                break;
            case false:
                while (angle < (2 * Math.PI) && !breakout)
                {
                    if (encounter(drawOrbitRadiusHelper(eccentricity, globalRotationAngle, semiMajorAxis, angle), currentMassiveBody.SphereOfInfluence) && !prediction)
                    {
                        breakout = true;
                        break;
                    }
                    lineDrawer.DrawLine(drawOrbitRadiusHelper(eccentricity, globalRotationAngle, semiMajorAxis, angle) + globalTransformationVector,
                        drawOrbitRadiusHelper(eccentricity, globalRotationAngle, semiMajorAxis, angle + 0.01f) + globalTransformationVector, Color.red);
                    angle += 0.01d;
                }
                angle = 0;
                while (angle < startingTrueAnomaly && !breakout)
                {
                    if (encounter(drawOrbitRadiusHelper(eccentricity, globalRotationAngle, semiMajorAxis, angle), currentMassiveBody.SphereOfInfluence) && !prediction)
                    {
                        breakout = true;
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

    private bool encounter(Vector2 predictedPosition, double sphereOfInfluence)
    {
        //Entering a sphere of influence

        //exiting a sphere of influence
        if (predictedPosition.magnitude > sphereOfInfluence)
        {
            //onEscapeTrajectory = true;
            if(currentSphereChanges < maxSphereChanges)
            {
                currentSphereChanges++;
            }
            return true;
        }
        else
        {
            //onEscapeTrajectory = false;
        }
        return false;
    }

    //Takes in an amount of time x and a celestial body and predicts where it will be in x amount of time
    private Vector2 predictedPosition(double time, GravityElements celestialBody)
    {
        return new Vector2();
    }

    //public GameObject encounteredMassiveBody;//debugging

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

        bool encounter = true;
        double time = 0;
        double timeToApoapsis = calculateTimeToApoapsis();
        while (positionAtPredictedTime(time).magnitude < shipsMassiveBody.SphereOfInfluence)
        {
            time+=0.01;
            if(time > timeToApoapsis)
            {
                encounter = false;
                break;
            }

            if(time > 10000)//prevent infinite loops
            {
                Debug.LogWarning("Stopped a possible infinite loop");
                break;
            }
            
        }

        if (!encounter)
        {
            return;
        }
        
        Tuple<Vector2, Vector2> returnedInfo = shipsMassivebodyGravityElements.calculateGlobalPositionAndVelocityAtFutureTime(time);
        Vector2 predictedGlobalCurrentMassiveBodyPosition = returnedInfo.item1;
        Vector2 predictedGlobalCurrentMassiveBodyVelocity = returnedInfo.item2;

        returnedInfo = shipElements.calculateLocalPositionAndVelocityAtFutureTime(time);
        Vector2 predictedLocalPlayerPosition = returnedInfo.item1;
        Vector2 predictedLocalPlayerVelocity = returnedInfo.item2;

        GameObject encounteredMassiveBody = findInfluencingCelestialBodyAtPredictedPosition(predictedGlobalCurrentMassiveBodyPosition + predictedLocalPlayerPosition, predictedLocalPlayerVelocity, shipElements.massiveBody, time);
        
        Debug.Log("Encountered body: " + encounteredMassiveBody.name);

        returnedInfo = encounteredMassiveBody.GetComponent<GravityElements>().calculateGlobalPositionAndVelocityAtFutureTime(time);
        Vector2 predictedNewBodyGlobalPosition = returnedInfo.item1;
        Vector2 predictedNewBodyGlobalVelocity = returnedInfo.item2;

        Vector2 positionRelativeToEncounter = (predictedLocalPlayerPosition + predictedGlobalCurrentMassiveBodyPosition) - predictedNewBodyGlobalPosition;
        Vector2 velocityRelativeToEncounter = (predictedLocalPlayerVelocity + predictedGlobalCurrentMassiveBodyVelocity) - predictedNewBodyGlobalVelocity;

        firstEncounter = calculateInitialOrbitalElements(positionRelativeToEncounter, velocityRelativeToEncounter, encounteredMassiveBody, predictedNewBodyGlobalPosition);

        encounters.Enqueue(firstEncounter);
    }

    

    private double calculateTimeToApoapsis()
    {
        switch(shipElements.OrbitType){
            case OrbitTypes.hyperbolic:
                return double.PositiveInfinity;
            case OrbitTypes.parabolic:
                return double.PositiveInfinity;
            default:
                double changeInMeanAnomaly = Math.PI - shipElements.MeanAnomaly;
                double meanMotion = Math.Sqrt(shipElements.Mu/Math.Pow(shipElements.SemiMajorAxis, 3));
                return changeInMeanAnomaly/meanMotion;
        }
    }

    private Vector2 positionAtPredictedTime(double time)
    {
        return shipElements.GetComponent<GravityElements>().calculateLocalPositionAtFutureTime(time);
    }

    //takes in everything relative to the body being orbited
    private GravityElementsClass calculateInitialOrbitalElements(Vector2 position, Vector2 velocity, GameObject massiveBody, Vector2 predictedGlobalTransformationVector)
    {
        
        GravityElementsClass gravityElements = new GravityElementsClass();

        gravityElements.massiveBody = massiveBody;

        gravityElements.Mu = GlobalElements.GRAV_CONST * 100;//gravityElements.MassiveBody.GetComponent<MassiveBodyElements>().mass;
        gravityElements.Position = position;
        gravityElements.velocity = velocity;

        //Calculate Global Tranformation Vector
        gravityElements.GlobalTransformationVector = predictedGlobalTransformationVector;

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

        Debug.Log("Encounter info dump");
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
        Debug.Log("Time At Epoch: " + gravityElements.TimeAtEpoch);

        return gravityElements;
    }

    //<summary>
    //Takes in everything in global coordinates (relative to the origin(0, 0)) and returns the game object that is influencing the craft
    //Except velocity, it seems to be relative to the current body
    //</summary>
    //<param name="position"> Re-read the summary
    //<param name="asdf"> Here's to the little guys
    private GameObject findInfluencingCelestialBodyAtPredictedPosition(Vector2 position, Vector2 velocity, GameObject currentMassiveBody, double timeStep)
    {
        GameObject[] massiveBodies = GameObject.FindGameObjectsWithTag("MassiveBody");
        List<GameObject> spheresOfInfluence = new List<GameObject>();

        //find which spheres of influence the player is in
        for (int i = 0; i < massiveBodies.Length; i++)
        {
            //quick and dirty calculation of altitude
            double mu = massiveBodies[i].GetComponent<MassiveBodyElements>().mass * GlobalElements.GRAV_CONST;
            Tuple<Vector2, Vector2> returneInfo = massiveBodies[i].GetComponent<GravityElements>().calculateGlobalPositionAndVelocityAtFutureTime(timeStep);
            Vector2 relativePosition = position - returneInfo.item1;
            Vector2 relativeVelocity = velocity + returneInfo.item2;
            
            Vector2 eccentricity = OrbitalHelper.calculateEccentricity(relativePosition, relativeVelocity, mu);
            OrbitTypes orbitType = OrbitalHelper.determineOrbitType(eccentricity);
            double mechanicalEnergy = OrbitalHelper.calculateMechanicalEnergy(relativePosition, relativeVelocity, mu, orbitType);
            double semiMajorAxis = OrbitalHelper.calculateSemiMajorAxis(mechanicalEnergy, mu, orbitType);
            Vector2 perigee = OrbitalHelper.calculatePerigee(semiMajorAxis, eccentricity, orbitType);
            double semiLatusRectum = OrbitalHelper.calculateSemiLatusRectum(semiMajorAxis, eccentricity, perigee, orbitType); //semiMajorAxis * (1 - Math.Pow(eccentricity.magnitude, 2));
            double trueAnomaly = Vector2.Angle(relativePosition, eccentricity);
            trueAnomaly = MiscHelperFuncs.convertToRadians(trueAnomaly);
            trueAnomaly = Math.Abs(MiscHelperFuncs.wrapAngle(trueAnomaly));
            double altitude = Vector2.Distance(massiveBodies[i].transform.position, transform.position);//semiLatusRectum / (1 + eccentricity.magnitude * Math.Cos(trueAnomaly));
            
            if (massiveBodies[i].GetComponent<MassiveBodyElements>().SphereOfInfluence > altitude && massiveBodies[i].transform.GetInstanceID() != currentMassiveBody.transform.GetInstanceID())
            {
                if (altitude < 0)
                {
                    Debug.Log("altitude: " + altitude);
                    Debug.Log("semilatusrectum: " + semiLatusRectum);
                    Debug.Log("Eccentricity: " + eccentricity.magnitude);
                    Debug.Log("true anomaly: " + trueAnomaly);
                }
                spheresOfInfluence.Add(massiveBodies[i]);

            }
        }

        //find the closest massive body
        Vector2 playerPredictedGlobalPosition = shipElements.calculateLocalPositionAtFutureTime(timeStep) + shipElements.massiveBody.GetComponent<GravityElements>().calculateGlobalPositionAtFutureTime(timeStep);
        double smallestDistance = double.PositiveInfinity;
        GameObject returnGameObject = currentMassiveBody;
        foreach (GameObject massiveBody in spheresOfInfluence)
        {
            Debug.Log(massiveBody.name);
            Vector2 massiveBodyPredictedGlobalPosition = massiveBody.GetComponent<GravityElements>().calculateGlobalPositionAtFutureTime(timeStep);
            double distance = Vector2.Distance(massiveBody.transform.position, playerPredictedGlobalPosition);
            if (distance < smallestDistance)
            {
                smallestDistance = distance;
                returnGameObject = massiveBody;
            }
        }

        return returnGameObject;
    }

    public void OnDrawGizmos()
    {
        /*if (encounters != null && encounters.Count != 0)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(encounters.Peek().Perigee, 1.5f);
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(encounters.Peek().Apogee, 1.5f);

            Gizmos.color = Color.magenta;
            double angle = 0;
            while (angle < 2 * Math.PI)
            {
                Gizmos.DrawLine(drawOrbitRadiusHelper(encounters.Peek().Eccentricity, encounters.Peek().GlobalRotationAngle, encounters.Peek().SemiMajorAxis, angle) + encounters.Peek().GlobalTransformationVector,
                    drawOrbitRadiusHelper(encounters.Peek().Eccentricity, encounters.Peek().GlobalRotationAngle, encounters.Peek().SemiMajorAxis, angle + 0.01f) + encounters.Peek().GlobalTransformationVector);
                angle += 0.01;
            }
        }*/
    }
}
