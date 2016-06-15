using UnityEngine;
using System.Collections;
using System;

public class ShipGravityElements : MonoBehaviour {

    enum OrbitTypes { circular, elliptical, parabolic, hyperbolic };

    public GameObject sun;
    public Vector2 velocity;
    private double velocityAngle;
    private double speed;
    public Vector2 position;
    private double altitude;
    public double GRAV_CONST;

    private OrbitTypes orbitType;

    private double celestialBodyMass;
    private double semiMajorAxis;
    private double semiLatusRectum;
    private Vector2 perigee;
    private Vector2 apogee;
    private Vector2 center;
    private Vector2 topAsymptote;
    private Vector2 bottomAsymptote;

    private Vector2 eccentricity;
    private double mechanicalEnergy;
    private double eccentricAnomaly;
    private double angularMomentum;

    private double meanAnomaly;
    private double anomalyAtEpoch;
    private double trueAnomaly;
    private double timeStep;
    private double timeAtEpoch;
    
    private Vector2 globalTransformationVector;
    private double globalRotationAngle;

    private bool clockwise;
    private bool towardsPerigee;

	// Use this for initialization
	void Start () {

        //Initialize public attributes if null
        if (velocity.magnitude == 0 && position.magnitude == 0)
        {
            velocity = new Vector2(0.0f, 2.0f);
            position = new Vector2(2.0f, 0.0f);
        }
        this.transform.position = position;
        if (GRAV_CONST == 0)
        {
            GRAV_CONST = 9.81f;
        }

        //initialize others
        timeStep = 0.01d;
        celestialBodyMass = 1.0d;

        //Calculate initial orbital elements
        calculateInitalOrbitalParameters(position, velocity);
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        //calculateNextOrbitalParameters();
        calculateInitalOrbitalParameters(position, velocity);
        moveShip();
	}

    public void applyThrust(Vector2 thrust)
    {
        calculateInitalOrbitalParameters(position, velocity + thrust);
        Debug.Break();
    }

    private void moveShip()
    {
        this.transform.position = position + globalTransformationVector;
    }

    private void calculateNextOrbitalParameters()
    {
        globalTransformationVector = sun.transform.position;
        
        //Calculate next meanAnomaly
        meanAnomaly = calculateMeanAnomaly( position,  velocity,  eccentricity,
         semiMajorAxis,  anomalyAtEpoch,  timeStep,  timeAtEpoch,  clockwise);

        //Calculate Eccentric Anomaly
        eccentricAnomaly = calculateEccentricAnomaly(eccentricity, meanAnomaly, eccentricAnomaly);

        //CalculateTrueAnomaly
        trueAnomaly = calculateTrueAnomaly(eccentricity, eccentricAnomaly, meanAnomaly);

        //Calculate Altitude
        altitude = calculateAltitude(eccentricity, semiMajorAxis, semiLatusRectum, trueAnomaly);

        //Calculate positionVector
        position = calculatePosition(perigee, trueAnomaly, globalRotationAngle, altitude);

        //Are we going towards the perigee?
        towardsPerigee = towardsPerigeeOrbit(meanAnomaly, clockwise);

        //Calculate velocity vector
        velocityAngle = calculateVelocityAngle(position, eccentricity, semiMajorAxis, trueAnomaly, globalRotationAngle, clockwise, towardsPerigee);
        speed = calculateSpeed(position, semiMajorAxis);
        velocity = assembleVelocityVector(velocityAngle, speed);

        //////Determine side of orbit
        //towardsPerigee = towardsPerigeeOrbit(velocity);

        //advance epoch
        anomalyAtEpoch = meanAnomaly;

        //Advance time
        timeAtEpoch = advanceTime(timeAtEpoch, timeStep);

    }

    public void calculateInitalOrbitalParameters(Vector2 position, Vector2 velocity)
    {

        //Find new planetary position
        globalTransformationVector = sun.transform.position;

        //Set velocity
        this.velocity = velocity;

        //Set position
        this.position = position;

        //Calculate eccentricAnomaly
        eccentricity = calculateEccentricity(position, velocity);

        //Calculate mechanical Energy
        mechanicalEnergy = calculateMechanicalEnergy(velocity, position);

        //Calculate SemiMajorAxis
        semiMajorAxis = calculateSemiMajorAxis(mechanicalEnergy);

        //Calculate SemiLatusRectum
        semiLatusRectum = calculateSemiLatusRectum(semiMajorAxis, eccentricity, perigee);

        //Calculate Perigee
        perigee = calculatePerigee(semiMajorAxis, eccentricity);

        //Calculate Apogee
        apogee = calculateApogee(semiMajorAxis, eccentricity);
        
        //Calculate Center
        center = calculateCenter(semiMajorAxis, perigee);

        //Calculate GlobalRotationAngle
        globalRotationAngle = calculateGlobalRotationAngle(eccentricity);

        //Calculate BottomAsymptote
        topAsymptote = calculateBottomAsymptote(eccentricity);

        //Calculate TopAsymptote
        bottomAsymptote = calculateTopAsymptote(eccentricity);

        //Find position and direction of orbit
        clockwise = clockwiseOrbit(position, velocity);
        towardsPerigee = towardsPerigeeOrbit(velocity);

        //Calculate trueAnomaly
        trueAnomaly = calculateTrueAnomaly(eccentricity, position);

        //Calculate Eccentric Anomaly
        eccentricAnomaly = calculateEccentricAnomaly(eccentricity, trueAnomaly, towardsPerigee);

        //Calculate Anomaly at current epoch
        anomalyAtEpoch = calculateAnomalyAtCurrentEpoch(eccentricity, eccentricAnomaly);

        //Calculate Angular Momentum
        angularMomentum = calculateAngularMomentum();

        //Calculate time at epoch
        timeAtEpoch = calculateTimeAtEpoch(position, velocity, eccentricity, trueAnomaly, eccentricAnomaly, anomalyAtEpoch);
    }

    private double calculateAngularMomentum()
    {
        return calculateSpeed(perigee, semiMajorAxis) * calculateAltitude(eccentricity, semiMajorAxis, semiLatusRectum, 0);
    }

    private double advanceTime(double timeAtEpoch, double timeStep)
    {
        if (orbitType == OrbitTypes.hyperbolic)
        {
            if (clockwise)
            {
                return timeAtEpoch - timeStep;
            }
            else
            {
                return timeAtEpoch + timeStep;
            }
        }else {
            return double.PositiveInfinity;
        }
    }


    private double calculateTimeAtEpoch(Vector2 position, Vector2 velocity, Vector2 eccentricity, double trueAnomaly, double eccentricAnomaly, double anomalyAtEpoch)
    {
        double returnTime = double.PositiveInfinity;
        switch (orbitType) {
            case OrbitTypes.circular:
                break;
            case OrbitTypes.elliptical:
                break;
            case OrbitTypes.parabolic:
                break;
            case OrbitTypes.hyperbolic:
                returnTime = (Math.Pow(angularMomentum, 3) / Math.Pow(GRAV_CONST * celestialBodyMass, 2)) *
                (anomalyAtEpoch / Math.Pow(Vector2.SqrMagnitude(eccentricity) - 1, 1.5));
                if (clockwise)
                {
                    if (towardsPerigee)
                    {
                        returnTime = Math.Abs(returnTime);
                    }
                    else
                    {
                        returnTime = -Math.Abs(returnTime);
                    }
                }
                else
                {
                    if (!towardsPerigee)
                    {
                        returnTime = Math.Abs(returnTime);
                    }
                    else
                    {
                        returnTime = -Math.Abs(returnTime);
                    }
                }
                break;
        }
        return returnTime;
    }

    private double calculateSemiLatusRectum(double semiMajorAxis, Vector2 eccentricity, Vector2 perigee)
    {
        if (orbitType != OrbitTypes.parabolic)
        {
            return semiMajorAxis * (1 - Vector2.SqrMagnitude(eccentricity));
        }
        else
        {
            return 2 * perigee.magnitude;
        }
    }

    private Vector2 calculateEccentricity(Vector2 inPosition, Vector2 inVelocity)
    {
        Vector2 returnEccentricity = ((inPosition * (float)(Vector2.SqrMagnitude(inVelocity) - ((GRAV_CONST * celestialBodyMass) / inPosition.magnitude))) - ((float)(Vector2.Dot(inPosition, inVelocity)) * inVelocity)) *
            (float)(1 / (GRAV_CONST * celestialBodyMass));

        if(returnEccentricity.magnitude == 0)
        {
            orbitType = OrbitTypes.circular;
        } else if (returnEccentricity.magnitude < 1 
            && returnEccentricity.magnitude > 0)
        {
            orbitType = OrbitTypes.elliptical;
        } else if ( returnEccentricity.magnitude == 1.0f)
        {
            orbitType = OrbitTypes.parabolic;
        } else if (returnEccentricity.magnitude > 1)
        {
            orbitType = OrbitTypes.hyperbolic;
        }
        return returnEccentricity;
    }

    private double calculateMechanicalEnergy(Vector2 velocity, Vector2 position)
    {
        switch (orbitType)
        {
            case OrbitTypes.circular:
                return (Vector2.SqrMagnitude(velocity) / 2) - ((GRAV_CONST * celestialBodyMass) / position.magnitude);
            case OrbitTypes.elliptical:
                return (Vector2.SqrMagnitude(velocity) / 2) - ((GRAV_CONST * celestialBodyMass) / position.magnitude);
            case OrbitTypes.parabolic:
                return 0;
            case OrbitTypes.hyperbolic:
                return (Vector2.SqrMagnitude(velocity) / 2) - ((GRAV_CONST * celestialBodyMass) / position.magnitude);
        }
        return double.PositiveInfinity;
    }

    private double calculateSemiMajorAxis(double mechanicalEnergy)
    {
        switch (orbitType)
        {
            case OrbitTypes.circular:
                return -((GRAV_CONST * celestialBodyMass) / (2 * mechanicalEnergy));
            case OrbitTypes.elliptical:
                return -((GRAV_CONST * celestialBodyMass) / (2 * mechanicalEnergy));
            case OrbitTypes.parabolic:
                return double.PositiveInfinity;
            case OrbitTypes.hyperbolic:
                return -((GRAV_CONST * celestialBodyMass) / (2 * mechanicalEnergy));
        }
        return -((GRAV_CONST*celestialBodyMass) / (2 * mechanicalEnergy));
    }

    private Vector2 calculatePerigee(double semiMajorAxis, Vector2 eccentricity)
    {
        double altitudeOfPerigee = 0;
        Vector2 returnPerigee = Vector2.right;
        switch (orbitType)
        {
            case OrbitTypes.circular:
                altitudeOfPerigee = semiMajorAxis;
                returnPerigee = Vector2.right;
                returnPerigee = returnPerigee * (float)altitudeOfPerigee;
                break;
            case OrbitTypes.elliptical:
                altitudeOfPerigee = semiMajorAxis * (1 - eccentricity.magnitude);
                returnPerigee = eccentricity.normalized;
                returnPerigee = returnPerigee * (float)altitudeOfPerigee;
                break;
            case OrbitTypes.parabolic:

                break;
            case OrbitTypes.hyperbolic:
                altitudeOfPerigee = -semiMajorAxis * (eccentricity.magnitude - 1);
                returnPerigee = eccentricity.normalized * (float)altitudeOfPerigee;
                break;
        }
        return returnPerigee;
    }

    private Vector2 calculateApogee(double semiMajorAxis, Vector2 eccentricity)
    {
        double altitudeOfApogee = 0;
        Vector2 returnApogee = Vector2.right;
        switch (orbitType)
        {
            case OrbitTypes.circular:
                altitudeOfApogee = semiMajorAxis;
                returnApogee = Vector2.right;
                returnApogee = -returnApogee * (float)altitudeOfApogee;
                break;
            case OrbitTypes.elliptical:
                altitudeOfApogee = semiMajorAxis * (1 + eccentricity.magnitude);
                returnApogee = -eccentricity.normalized;
                returnApogee = returnApogee * (float)altitudeOfApogee;
                break;
            case OrbitTypes.parabolic:
                break;
            case OrbitTypes.hyperbolic:
                altitudeOfApogee = double.PositiveInfinity;
                returnApogee = -Vector2.right * float.PositiveInfinity;
                break;
        }
        return returnApogee;
    }

    private double calculateGlobalRotationAngle(Vector2 eccentricity)
    {
        double returnGlobalRotationAngle = 0;
        switch (orbitType)
        {
            case OrbitTypes.circular:
                returnGlobalRotationAngle = 0;
                break;
            case OrbitTypes.elliptical:
                returnGlobalRotationAngle = Math.Atan2(eccentricity.y, eccentricity.x);
                break;
            case OrbitTypes.parabolic:
                returnGlobalRotationAngle = Math.Atan2(eccentricity.y, eccentricity.x);
                break;
            case OrbitTypes.hyperbolic:
                returnGlobalRotationAngle = Math.Atan2(eccentricity.y, eccentricity.x);
                break;
        }
        return returnGlobalRotationAngle;
    }

    private bool clockwiseOrbit(Vector2 position, Vector2 velocity)
    {
        Vector3 crossProduct;
        crossProduct = Vector3.Cross(convertToVec3(position), convertToVec3(velocity));
        if (crossProduct.z > 0)
        {
            return false;
        }
        return true;
    }

    private bool towardsPerigeeOrbit(Vector2 velocity)
    {
        switch (orbitType)
        {
            case OrbitTypes.circular:
                return (convertToRadians(Vector2.Angle(Vector2.right, velocity)) < Math.PI / 2);
            case OrbitTypes.elliptical:
                return (convertToRadians(Vector2.Angle(eccentricity, velocity)) < Math.PI / 2);
            case OrbitTypes.parabolic:
                return true;
            case OrbitTypes.hyperbolic:
                return (convertToRadians(Vector2.Angle(eccentricity, velocity)) < Math.PI / 2);
        }
        return true;
        
    }

    private bool towardsPerigeeOrbit(double meanAnomaly, bool clockwise)
    {
        switch (orbitType)
        {
            case OrbitTypes.circular:
                return (clockwise && meanAnomaly < 0) || (!clockwise && meanAnomaly < 0);
            case OrbitTypes.elliptical:
                return (clockwise && meanAnomaly < 0) || (!clockwise && meanAnomaly < 0);
            case OrbitTypes.parabolic:
                return true;
            case OrbitTypes.hyperbolic:
                return (clockwise && meanAnomaly > 0) || (!clockwise && meanAnomaly < 0);
        }
        return true;
    }

    private Vector2 calculateBottomAsymptote(Vector2 eccentricity)
    {
        switch (orbitType)
        {
            case OrbitTypes.circular:
                return Vector2.right * float.PositiveInfinity;
            case OrbitTypes.elliptical:
                return Vector2.right * float.PositiveInfinity;
            case OrbitTypes.parabolic:
                break;
            case OrbitTypes.hyperbolic:
                double asymptoteAngle;
                asymptoteAngle = Math.Asin(1 / eccentricity.magnitude) * 2;
                asymptoteAngle = Math.PI/2 - (asymptoteAngle/2);
                asymptoteAngle = asymptoteAngle + Math.PI;
                asymptoteAngle += globalRotationAngle;
                Vector2 returnVector = new Vector2((float)Math.Cos(asymptoteAngle), (float)Math.Sin(asymptoteAngle));
                return returnVector;
        }
        return Vector2.right * float.PositiveInfinity;
    }

    private Vector2 calculateTopAsymptote(Vector2 eccentricity)
    {
        switch (orbitType)
        {
            case OrbitTypes.circular:
                return Vector2.right * float.PositiveInfinity;
            case OrbitTypes.elliptical:
                return Vector2.right * float.PositiveInfinity;
            case OrbitTypes.parabolic:
                break;
            case OrbitTypes.hyperbolic:
                double asymptoteAngle;
                asymptoteAngle = Math.Asin(1 / eccentricity.magnitude) * 2;
                asymptoteAngle = Math.PI / 2 - (asymptoteAngle / 2);
                asymptoteAngle = asymptoteAngle + Math.PI;
                asymptoteAngle = -asymptoteAngle;
                asymptoteAngle += globalRotationAngle;
                Vector2 returnVector = new Vector2((float)Math.Cos(asymptoteAngle), (float)Math.Sin(asymptoteAngle));
                return returnVector;
        }
        return Vector2.right * float.PositiveInfinity;
    }

    private double calculateTrueAnomaly(Vector2 eccentricity, Vector2 position)
    {
        double returnTrueAnomaly = double.PositiveInfinity;
        switch (orbitType)
        {
            case OrbitTypes.circular:
                returnTrueAnomaly = Vector2.Angle(position, Vector2.right);
                returnTrueAnomaly = convertToRadians(returnTrueAnomaly);
                break;
            case OrbitTypes.elliptical:
                returnTrueAnomaly = Vector2.Angle(eccentricity, position);
                returnTrueAnomaly = convertToRadians(returnTrueAnomaly);
                if (clockwise)
                {
                    if (!towardsPerigee)
                    {
                        returnTrueAnomaly = -returnTrueAnomaly;
                    }
                }
                else
                {
                    if (towardsPerigee)
                    {
                        returnTrueAnomaly = -returnTrueAnomaly;
                    }
                }
                
                break;
            case OrbitTypes.parabolic:
                break;
            case OrbitTypes.hyperbolic:
                returnTrueAnomaly = Vector2.Angle(eccentricity, position);
                returnTrueAnomaly = convertToRadians(returnTrueAnomaly);
                if (timeAtEpoch > 0)
                {
                    returnTrueAnomaly = -Math.Abs(returnTrueAnomaly);
                } else if (timeAtEpoch < 0)
                {
                    returnTrueAnomaly = Math.Abs(returnTrueAnomaly);
                }
                break;
        }
    
        return returnTrueAnomaly;
    }

    private double calculateTrueAnomaly(Vector2 eccentricity, double eccentricAnomaly, double meanAnomaly)
    {
        double returnTrueAnomaly = double.PositiveInfinity;

        switch (orbitType)
        {
            case OrbitTypes.circular:
                //calculate true anomaly
                returnTrueAnomaly = Math.Acos(
                    (Math.Cos(eccentricAnomaly) - eccentricity.magnitude) /
                    (1 - (eccentricity.magnitude * Math.Cos(eccentricAnomaly)))
                    );
                
                if (meanAnomaly < 0)
                {
                    returnTrueAnomaly = -Math.Abs(returnTrueAnomaly);
                }
                break;
            case OrbitTypes.elliptical:
                //calculate true anomaly
                returnTrueAnomaly = Math.Acos(
                    (Math.Cos(eccentricAnomaly) - eccentricity.magnitude) /
                    (1 - (eccentricity.magnitude * Math.Cos(eccentricAnomaly)))
                    );


                if (clockwise)
                {
                    if (meanAnomaly < 0)
                    {
                        returnTrueAnomaly = Math.Abs(returnTrueAnomaly);
                    }
                    else
                    {
                        returnTrueAnomaly = -Math.Abs(returnTrueAnomaly);
                    }
                }
                else {
                    if (meanAnomaly < 0)
                    {
                        returnTrueAnomaly = -returnTrueAnomaly;
                    }
                }

                break;
            case OrbitTypes.parabolic:
                returnTrueAnomaly = Math.Atan(
                    (Math.Sqrt((eccentricity.magnitude + 1)/
                    (eccentricity.magnitude - 1))) * 
                    Math.Tanh(eccentricAnomaly/2)
                    );
                if (meanAnomaly < 0)
                {
                    returnTrueAnomaly = -returnTrueAnomaly;
                }
                break;
            case OrbitTypes.hyperbolic:
                returnTrueAnomaly = Math.Atan(Math.Tanh(eccentricAnomaly / 2) / Math.Sqrt((eccentricity.magnitude - 1) / (eccentricity.magnitude + 1))) * 2;
                
                break;
        }
        
        return returnTrueAnomaly;
    }

    private double calculateEccentricAnomaly(Vector2 eccentricity, double trueAnomaly, bool towardsPerigee)
    {
        double returnEccentricAnomaly = 0;
        switch (orbitType)
        {
            case OrbitTypes.circular:
                returnEccentricAnomaly = Math.Acos((eccentricity.magnitude + Math.Cos(trueAnomaly)) /
                (1 + (eccentricity.magnitude * Math.Cos(trueAnomaly))));
                if (towardsPerigee)
                {
                    returnEccentricAnomaly = -returnEccentricAnomaly;
                }
                break;
            case OrbitTypes.elliptical:
                returnEccentricAnomaly = Math.Acos((eccentricity.magnitude + Math.Cos(trueAnomaly)) /
                        (1 + (eccentricity.magnitude * Math.Cos(trueAnomaly))));
                if (towardsPerigee)
                {
                    returnEccentricAnomaly = -returnEccentricAnomaly;
                }
                break;
            case OrbitTypes.parabolic:
                break;
            case OrbitTypes.hyperbolic:
                double x = (eccentricity.magnitude + Math.Cos(trueAnomaly)) / (1 + (eccentricity.magnitude * Math.Cos(trueAnomaly)));
                returnEccentricAnomaly = Math.Log(x + Math.Sqrt((x * x) - 1));
                if (towardsPerigee)
                {
                    returnEccentricAnomaly = -returnEccentricAnomaly;
                }
                break;
        }
        
        return returnEccentricAnomaly;
    }

    private double calculateAnomalyAtCurrentEpoch(Vector2 eccentricity, double eccentricAnomaly)
    {
        switch (orbitType)
        {
            case OrbitTypes.circular:
                return eccentricAnomaly - eccentricity.magnitude * Math.Sin(eccentricAnomaly);
            case OrbitTypes.elliptical:
                return eccentricAnomaly - eccentricity.magnitude * Math.Sin(eccentricAnomaly);
            case OrbitTypes.parabolic:
                return eccentricAnomaly + (Math.Pow(eccentricAnomaly, 3) / 3);
            case OrbitTypes.hyperbolic:
                return (eccentricity.magnitude * Math.Sinh(eccentricAnomaly)) - eccentricAnomaly;
        }

        return eccentricAnomaly - eccentricity.magnitude * Math.Sin(eccentricAnomaly);
    }


    private double calculateMeanAnomaly(Vector2 position, Vector2 velocity, Vector2 eccentricity, 
        double semiMajorAxis, double anomalyAtEpoch, double timeStep, double timeAtEpoch, bool clockwise)
    {
        double orbitalSpeed;
        double returnMeanAnomaly = double.PositiveInfinity;

        switch (orbitType)
        {
            case OrbitTypes.circular:
                //Calculate percentage of orbit being crossed
                orbitalSpeed = Math.Sqrt((GRAV_CONST * celestialBodyMass) / Math.Abs(semiMajorAxis));
                returnMeanAnomaly = anomalyAtEpoch + (orbitalSpeed * timeStep);
                if (returnMeanAnomaly > Math.PI)
                {
                    returnMeanAnomaly = -(Math.PI - (returnMeanAnomaly - (Math.PI)));
                }
                break;
            case OrbitTypes.elliptical:
                //Calculate percentage of orbit being crossed
                orbitalSpeed = Math.Sqrt((GRAV_CONST * celestialBodyMass) / Math.Abs(semiMajorAxis));
                
                if (clockwise)
                {
                    returnMeanAnomaly = anomalyAtEpoch - (orbitalSpeed * timeStep);
                    if (returnMeanAnomaly < -Math.PI)
                    {
                        returnMeanAnomaly = (Math.PI - (returnMeanAnomaly - (Math.PI)));
                        Debug.Log(returnMeanAnomaly);
                        Debug.Break();
                    }
                }
                else
                {
                    returnMeanAnomaly = anomalyAtEpoch + (orbitalSpeed * timeStep);
                    if (returnMeanAnomaly > Math.PI)
                    {
                        returnMeanAnomaly = -(Math.PI - (returnMeanAnomaly - (Math.PI)));
                    }
                }
                
            break;
            case OrbitTypes.parabolic:
                break;
            case OrbitTypes.hyperbolic:
                double nextTime;
                if (clockwise)
                {
                    nextTime = timeAtEpoch - timeStep;
                }
                else
                {
                    nextTime = timeAtEpoch + timeStep;
                }

                returnMeanAnomaly = (Math.Pow(GRAV_CONST * celestialBodyMass, 2) *
                    Math.Pow(Vector2.SqrMagnitude(eccentricity) - 1, 1.5) *
                    nextTime * 2) /
                    Math.Pow(angularMomentum, 3);
                if (nextTime > 0)
                {
                    meanAnomaly = Math.Abs(meanAnomaly);
                }
                else
                {
                    meanAnomaly = -Math.Abs(meanAnomaly);
                }

                break;
        }

        return returnMeanAnomaly;
    }

    private double calculateAltitude(Vector2 eccentricity, double semiMajorAxis, double semiLatusRectum, double trueAnomaly)
    {
        double returnAltitude = double.PositiveInfinity;
        switch (orbitType)
        {
            case OrbitTypes.circular:
                returnAltitude = (semiMajorAxis * (1 - (eccentricity.magnitude * eccentricity.magnitude))) /
                    (1 + (eccentricity.magnitude * Math.Cos(trueAnomaly)));
                break;
            case OrbitTypes.elliptical:
                returnAltitude = (semiMajorAxis * (1 - (eccentricity.magnitude * eccentricity.magnitude))) /
                    (1 + (eccentricity.magnitude * Math.Cos(trueAnomaly)));
                break;
            case OrbitTypes.parabolic:
                returnAltitude = (semiMajorAxis * (1 - (eccentricity.magnitude * eccentricity.magnitude))) /
                    (1 + (eccentricity.magnitude * Math.Cos(trueAnomaly)));
                break;
            case OrbitTypes.hyperbolic:
                returnAltitude = semiLatusRectum / (1 + (eccentricity.magnitude * Math.Cos(trueAnomaly)));
                break;
        }
        return returnAltitude;
    }

    private Vector2 calculatePosition(Vector2 perigee, double trueAnomaly, double globalRotationAngle, double altitude)
    {
        Vector2 returnPosition = Vector2.right * float.PositiveInfinity;
        switch (orbitType) {
            case OrbitTypes.circular:
                returnPosition = new Vector2((float)Math.Cos(trueAnomaly + globalRotationAngle), (float)Math.Sin(trueAnomaly + globalRotationAngle));
                returnPosition = returnPosition.normalized * (float)altitude;
                break;
            case OrbitTypes.elliptical:
                returnPosition = new Vector2((float)Math.Cos(trueAnomaly + globalRotationAngle), (float)Math.Sin(trueAnomaly + globalRotationAngle));
                returnPosition = returnPosition.normalized * (float)altitude;
                break;
            case OrbitTypes.parabolic:
                returnPosition = new Vector2((float)Math.Cos(trueAnomaly + globalRotationAngle), (float)Math.Sin(trueAnomaly + globalRotationAngle));
                returnPosition = returnPosition.normalized * (float)altitude;
                break;
            case OrbitTypes.hyperbolic:
                returnPosition = new Vector2((float)Math.Cos(trueAnomaly + globalRotationAngle), (float)Math.Sin(trueAnomaly + globalRotationAngle));
                returnPosition = returnPosition.normalized * (float)altitude;
                break;
        }

        return returnPosition;
    }

    private double calculateVelocityAngle(Vector2 position, Vector2 eccentricity, double semiMajorAxis, double trueAnomaly, double globalRotationAngle, bool clockwise, bool towardsPerigee)
    {
        double returnVelocityAngle = 0;
        double trueAnomalyModifier = 0;
        double alpha = 0;
        double k = 0;

        switch (orbitType)
        {
            case OrbitTypes.circular:
                //find new velocity vector
                k = position.magnitude / (semiMajorAxis);
                alpha = Math.Acos(((2 - (2 * Vector2.SqrMagnitude(eccentricity))) /
                    (k * (2 - k))) - 1);

                trueAnomalyModifier = (Math.PI - alpha) / 2;

                if (clockwise)
                {
                    if (towardsPerigee)
                    {
                        returnVelocityAngle = trueAnomaly + trueAnomalyModifier + globalRotationAngle;
                        returnVelocityAngle -= Math.PI;
                    }
                    else
                    {
                        returnVelocityAngle = trueAnomaly - trueAnomalyModifier + globalRotationAngle;

                    }
                }
                else
                {
                    if (towardsPerigee)
                    {
                        returnVelocityAngle = trueAnomaly - trueAnomalyModifier + globalRotationAngle;
                        returnVelocityAngle -= Math.PI;
                    }
                    else
                    {
                        returnVelocityAngle = trueAnomaly + trueAnomalyModifier + globalRotationAngle;
                    }
                }
                break;
            case OrbitTypes.elliptical:
                //find new velocity vector
                k = position.magnitude / (semiMajorAxis);
                alpha = Math.Acos(((2 - (2 * Vector2.SqrMagnitude(eccentricity))) /
                    (k * (2 - k))) - 1);

                trueAnomalyModifier = (Math.PI - alpha) / 2;

                if (clockwise)
                {
                    if (towardsPerigee)
                    {
                        returnVelocityAngle = trueAnomaly + trueAnomalyModifier + globalRotationAngle;
                        returnVelocityAngle -= Math.PI;
                    }
                    else
                    {
                        returnVelocityAngle = trueAnomaly - trueAnomalyModifier + globalRotationAngle;

                    }
                }
                else
                {
                    if (towardsPerigee)
                    {
                        returnVelocityAngle = trueAnomaly - trueAnomalyModifier + globalRotationAngle;
                        returnVelocityAngle -= Math.PI;
                    }
                    else
                    {
                        returnVelocityAngle = trueAnomaly + trueAnomalyModifier + globalRotationAngle;
                    }
                }
                break;
            case OrbitTypes.parabolic:
                break;
            case OrbitTypes.hyperbolic:
                //find new velocity vector
                k = position.magnitude / (semiMajorAxis);
                alpha = Math.Acos(((2 - (2 * Vector2.SqrMagnitude(eccentricity))) /
                    (k * (2 - k))) - 1);

                trueAnomalyModifier = (Math.PI - alpha) / 2;

                if (clockwise)
                {
                    if (towardsPerigee)
                    {
                        returnVelocityAngle = trueAnomaly + trueAnomalyModifier + globalRotationAngle;
                        returnVelocityAngle -= Math.PI;
                    }
                    else
                    {
                        returnVelocityAngle = trueAnomaly - trueAnomalyModifier + globalRotationAngle;
                    }
                }
                else
                {
                    if (towardsPerigee)
                    {
                        returnVelocityAngle = trueAnomaly - trueAnomalyModifier + globalRotationAngle;
                        returnVelocityAngle -= Math.PI;
                    }
                    else
                    {
                        returnVelocityAngle = trueAnomaly + trueAnomalyModifier + globalRotationAngle;
                    }
                }
                break;
        }

        
        
        return returnVelocityAngle;
    }

    private double calculateSpeed(Vector2 position, double semiMajorAxis)
    {
        switch (orbitType)
        {
            case OrbitTypes.circular:
                return Math.Sqrt((GRAV_CONST * celestialBodyMass) * ((2 / position.magnitude) - (1 / semiMajorAxis)));
            case OrbitTypes.elliptical:
                return Math.Sqrt((GRAV_CONST * celestialBodyMass) * ((2 / position.magnitude) - (1 / semiMajorAxis)));
            case OrbitTypes.parabolic:
                return Math.Sqrt((GRAV_CONST * celestialBodyMass) * ((2 / position.magnitude) - (1 / semiMajorAxis)));
            case OrbitTypes.hyperbolic:
                double energy = 0.5 * Math.Pow(Math.Pow(((-(GRAV_CONST * celestialBodyMass)/semiMajorAxis) * (1 + eccentricity.magnitude)) / (eccentricity.magnitude - 1), 0.5), 2) - GRAV_CONST * celestialBodyMass/(semiMajorAxis*(1 - eccentricity.magnitude));
                return Math.Sqrt((energy + (GRAV_CONST / position.magnitude)) * 2);
                //return Math.Sqrt(((GRAV_CONST * celestialBodyMass) / semiLatusRectum) * (1 + Vector2.SqrMagnitude(eccentricity) - (2* Math.Cos(trueAnomaly)))) * 1.51120744328921d; //Not sure why this 1.511... is needed, but it is needed
        }
        return Math.Sqrt((GRAV_CONST * celestialBodyMass) * ((2 / position.magnitude) - (1 / semiMajorAxis)));
    }

    private Vector2 assembleVelocityVector(double velocityAngle, double speed)
    {
        return (new Vector2((float)Math.Cos(velocityAngle), (float)Math.Sin(velocityAngle)).normalized * (float)speed);
    }

    private double calculateEccentricAnomaly(Vector2 eccentricity, double meanAnomaly, double eccentricAnomaly)
    {
        double returnEccentricAnomaly = 0.0d;
        switch (orbitType)
        {
            case OrbitTypes.circular:
                returnEccentricAnomaly = meanAnomaly + (2 * eccentricity.magnitude * Math.Sin(meanAnomaly)) + (1.25 * Vector2.SqrMagnitude(eccentricity) * Math.Sin(2*meanAnomaly));
                break;
            case OrbitTypes.elliptical:
                while (returnEccentricAnomaly - eccentricity.magnitude * Math.Sin(returnEccentricAnomaly) < meanAnomaly)
                {
                    returnEccentricAnomaly += 1.0f;
                }

                while (returnEccentricAnomaly - eccentricity.magnitude * Math.Sin(returnEccentricAnomaly) > meanAnomaly)
                {
                    returnEccentricAnomaly -= 0.1f;
                }

                while (returnEccentricAnomaly - eccentricity.magnitude * Math.Sin(returnEccentricAnomaly) < meanAnomaly)
                {
                    returnEccentricAnomaly += 0.01f;
                }

                while (returnEccentricAnomaly - eccentricity.magnitude * Math.Sin(returnEccentricAnomaly) > meanAnomaly)
                {
                    returnEccentricAnomaly -= 0.001f;
                }

                while (returnEccentricAnomaly - eccentricity.magnitude * Math.Sin(returnEccentricAnomaly) < meanAnomaly)
                {
                    returnEccentricAnomaly += 0.0001f;
                }
                while (returnEccentricAnomaly - eccentricity.magnitude * Math.Sin(returnEccentricAnomaly) > meanAnomaly)
                {
                    returnEccentricAnomaly -= 0.00001f;
                }
                break;
            case OrbitTypes.parabolic:
                break;
            case OrbitTypes.hyperbolic:
                while (Math.Sqrt(Math.Pow(-semiMajorAxis, 3) / GRAV_CONST) *
            (((eccentricity.magnitude * Math.Sinh(eccentricAnomaly)) - eccentricAnomaly) -
            ((eccentricity.magnitude * Math.Sinh(returnEccentricAnomaly)) - returnEccentricAnomaly)) > timeStep)
                {
                    returnEccentricAnomaly += 1.0f;
                }

                while (Math.Sqrt(Math.Pow(-semiMajorAxis, 3) / GRAV_CONST) *
                    (((eccentricity.magnitude * Math.Sinh(eccentricAnomaly)) - eccentricAnomaly) -
                    ((eccentricity.magnitude * Math.Sinh(returnEccentricAnomaly)) - returnEccentricAnomaly)) < timeStep)
                {
                    returnEccentricAnomaly -= 0.1f;
                }

                while (Math.Sqrt(Math.Pow(-semiMajorAxis, 3) / GRAV_CONST) *
                    (((eccentricity.magnitude * Math.Sinh(eccentricAnomaly)) - eccentricAnomaly) -
                    ((eccentricity.magnitude * Math.Sinh(returnEccentricAnomaly)) - returnEccentricAnomaly)) > timeStep)
                {
                    returnEccentricAnomaly += 0.01f;
                }

                while (Math.Sqrt(Math.Pow(-semiMajorAxis, 3) / GRAV_CONST) *
                    (((eccentricity.magnitude * Math.Sinh(eccentricAnomaly)) - eccentricAnomaly) -
                    ((eccentricity.magnitude * Math.Sinh(returnEccentricAnomaly)) - returnEccentricAnomaly)) < timeStep)
                {
                    returnEccentricAnomaly -= 0.001f;
                }

                while (Math.Sqrt(Math.Pow(-semiMajorAxis, 3) / GRAV_CONST) *
                    (((eccentricity.magnitude * Math.Sinh(eccentricAnomaly)) - eccentricAnomaly) -
                    ((eccentricity.magnitude * Math.Sinh(returnEccentricAnomaly)) - returnEccentricAnomaly)) > timeStep)
                {
                    returnEccentricAnomaly += 0.0001f;
                }
                while (Math.Sqrt(Math.Pow(-semiMajorAxis, 3) / GRAV_CONST) *
                    (((eccentricity.magnitude * Math.Sinh(eccentricAnomaly)) - eccentricAnomaly) -
                    ((eccentricity.magnitude * Math.Sinh(returnEccentricAnomaly)) - returnEccentricAnomaly)) < timeStep)
                {
                    returnEccentricAnomaly -= 0.00001f;
                }
                break;
        }
        

        return returnEccentricAnomaly;
    }

    private Vector2 calculateCenter(double semiMajorAxis, Vector2 perigee)
    {
        switch (orbitType)
        {
            case OrbitTypes.circular:
                return -perigee.normalized * (float)semiMajorAxis + perigee;
            case OrbitTypes.elliptical:
                return -perigee.normalized * (float)semiMajorAxis + perigee;
            case OrbitTypes.parabolic:
                return -perigee.normalized * (float)semiMajorAxis + perigee;
            case OrbitTypes.hyperbolic:
                return -perigee.normalized * (float)semiMajorAxis + perigee;
        }

        return Vector2.right * float.PositiveInfinity;
    }

    private Vector3 convertToVec3(Vector2 inVec)
    {
        return new Vector3(inVec.x, inVec.y, 0.0f);
    }

    private double convertToRadians(double degrees)
    {
        return (degrees * Math.PI) / 180;
    }

    public void OnDrawGizmos()
    {

        

        //Draw perigee
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(globalTransformationVector + perigee, 0.1f);

        //draw apogee
        Gizmos.DrawSphere(globalTransformationVector + apogee, 0.1f);

        //Draw semiMajor Axis
        Gizmos.color = Color.red;
        Gizmos.DrawLine(globalTransformationVector + perigee, globalTransformationVector + perigee + (perigee.normalized * -(float)semiMajorAxis));

        //Draw eccentricity
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(globalTransformationVector, globalTransformationVector + eccentricity);

        //Draw the ellipse
        double angle = 0;
        while (angle < 2 * Math.PI)
        {
            Gizmos.DrawLine(drawOrbitRadiusHelper(angle) + globalTransformationVector, drawOrbitRadiusHelper(angle + 0.01d) + globalTransformationVector);
            angle += 0.01d;
        }

        //Draw Center
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(globalTransformationVector + center, 0.2f);

        //Draw asymptotes
        Gizmos.color = Color.green;
        Gizmos.DrawLine(globalTransformationVector + center, globalTransformationVector + center + topAsymptote * 100);
        Gizmos.DrawLine(globalTransformationVector + center, globalTransformationVector + center + bottomAsymptote * 100);

        //Draw position
        Gizmos.color = Color.white;
        Gizmos.DrawSphere(position, 0.1f);
        Gizmos.DrawLine(globalTransformationVector, position);

        //Draw velocity
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(position, position + velocity);

        //Draw mean anomaly
        Gizmos.color = Color.green;
        Gizmos.DrawLine(new Vector2(0, 0), new Vector2((float)Math.Cos(anomalyAtEpoch), (float)Math.Sin(anomalyAtEpoch)) * 10f);

    }

    private Vector2 drawOrbitRadiusHelper(double angle)
    {
        double radius = (semiMajorAxis * (1 - Vector2.SqrMagnitude(eccentricity))) / (1 - eccentricity.magnitude * (Math.Cos(angle - globalRotationAngle + Math.PI))); 
        return new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * (float)radius;
    }

}
