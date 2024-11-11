using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class Car : MonoBehaviour
{
    #region fields
    // Suspension
    float springOriginalLength = 1f;
    float suspensionForceMultiplier = 350f;
    float dampingForceMultiplier = 5f;

    // Steering
    float maxSteeringAngle = 35f;
    float gripForceMultiplier = 250f; // grip

    // Accelerating
    float EngineForce = 300f;

 // Reference for wheels
    // Wheel positions
    [SerializeField] Transform frontLeftWheel;
    [SerializeField] Transform frontRightWheel;
    [SerializeField] Transform backLeftWheel;
    [SerializeField] Transform backRightWheel;
    // Wheel meshes
    [SerializeField] Transform frontLeftWheelMesh;
    [SerializeField] Transform frontRightWheelMesh;
    [SerializeField] Transform backLeftWheelMesh;
    [SerializeField] Transform backRightWheelMesh;
    float wheelRadius = 0.5f;
    float wheelRotationSmoothness = 0.5f;
    float steeringInput;
    Transform[] wheels;
    Transform[] wheelMeshes;
    float maxAirborneTime = 0f;
    float[] airborneTimes;
    bool onGrounded = false;
    // Saved for efficiency
    Rigidbody rb;
    #endregion

    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        wheels = new Transform[] {frontLeftWheel, frontRightWheel, backLeftWheel, backRightWheel};
        wheelMeshes = new Transform[] {frontLeftWheelMesh, frontRightWheelMesh, backLeftWheelMesh, backRightWheelMesh};
        airborneTimes = new float[wheels.Length];
    }

    void Update()
    {

    }
    
    void FixedUpdate()
    {
        steeringInput = Input.GetAxis("Steering");
        for (int i = 0; i < wheels.Length; i++)
        {
            Transform wheel = wheels[i];
            Transform wheelMesh = wheelMeshes[i];
            RaycastHit hit = GroundCheck(wheel);
            UpdateWheelMesh(wheel, wheelMesh, hit);
            if (hit.collider == null)
            {
                airborneTimes[i] += Time.fixedDeltaTime;
                if (airborneTimes[i] >= maxAirborneTime)
                {
                    if (onGrounded == false)
                    {

                    }
                    onGrounded = true;
                    continue;
                }
            }
            else
            {
                onGrounded = false;
                airborneTimes[i] = 0;
            }
            ApplySuspensionForce(wheel, hit);
            if (i < 2)
            {
                ApplySteering(wheel, steeringInput);
            }
            else
            {
                ApplySteering(wheel);
            }
        }
        float accelerationInput = Input.GetAxis("Acceleration");
        
   
        Accelerate(accelerationInput);

    }

     RaycastHit GroundCheck(Transform wheel)
    {
        RaycastHit hit;
        bool isHit = Physics.Raycast(wheel.position, -wheel.up, out hit, springOriginalLength+0.7f);
        Debug.DrawRay(wheel.position, -wheel.up * 10f, isHit ? Color.green : Color.red);
        return hit;
    }

    /// <summary>
    /// Applies the suspension forces on the rigidbody
    /// Gets the current offset of the spring and applies the force based on this
    /// </summary>
    /// <param name="wheel">The wheel that the suspension force will be applied to</param>
    void ApplySuspensionForce(Transform wheel, RaycastHit hit)
    {
        // finds velocity of rigidbody at the point of the wheels in world-space
        Vector3 wheelVelocity = rb.GetPointVelocity(wheel.position);
        // finds velocity of wheel in the direction of the suspension
        // it uses wheel.up because velocity should be positive it wheel
        // travels upwards so that the damping can be negative that value (tf am i saying)
        float velocity = Vector3.Dot(wheelVelocity, wheel.up);
        float distance = hit.distance;
        // calculates extension of spring
        float springExtension = distance - springOriginalLength;
        // calculates force of spring by F = kx
        float springForce = -(springExtension * suspensionForceMultiplier);
        // finds the damping force by using the velocity
        float dampingForce = velocity * dampingForceMultiplier;

        // finds resultant force
        float resultantForce = Mathf.Clamp(springForce - dampingForce, -10000f, 10000f);
        // applies resultant force at the point of the wheels in the rigidbody
        rb.AddForceAtPosition(resultantForce * wheel.up, wheel.position);
    }

    /// <summary>
    /// Applies rotation to wheels if steeringInput is passed.
    /// Adds grip forces to allow car to steer straight along the steering direction.
    /// </summary>
    /// <param name="wheel">The wheel for steering to be applied to</param>
    /// <param name="steeringInput">The rotation of the wheels</param>
    void ApplySteering(Transform wheel, float steeringInput = 0)
    {
        // Gets velocity of rigidbody at the wheel position
        Vector3 wheelVelocity = rb.GetPointVelocity(wheel.position);

        // Changes the y rotation of the wheel to allow steering.
        // Max steering angle is halved because steering input is from -1 to +1.
        Quaternion wheelRotation = Quaternion.AngleAxis(maxSteeringAngle / 2 * steeringInput, wheel.up);
        wheel.localRotation = wheelRotation;

        // finds forward and right components of the velocity by finding the dot product.
        float forwardVelocity = Vector3.Dot(wheelVelocity, wheel.forward);
        float rightVelocity = Vector3.Dot(wheelVelocity, wheel.right);

        // finds the grip force to apply
        // negative velocity to counteract slipping.
        // Should ideally be done with a *LOOKUP CURVE* in the future to allow different grip at different slips.
        float gripForce = -rightVelocity * gripForceMultiplier/100;

        // applies resultant force at the point of the wheels in the rigidbody
        rb.AddForceAtPosition(gripForce*wheel.right, wheel.position);
    }

    /// <summary>
    /// Updates the position and rotation of the wheel mesh to reflect the behaviour of the car
    /// </summary>
    /// <param name="wheel">The transform of the simulated wheel position</param>
    /// <param name="wheelMesh">The transform of the wheel mesh</param>
    void UpdateWheelMesh(Transform wheel, Transform wheelMesh, RaycastHit hit)
    {
        Vector3 wheelMeshPos = wheel.position;
        if (hit.collider != null)
        {
            wheelMeshPos.y = hit.point.y;
        }
        else
        {
            wheelMeshPos = wheel.position-wheel.up*0.5f;
        } 
        wheelMesh.position = wheelMeshPos;
        Vector3 wheelMeshRotation = wheelMesh.localRotation.eulerAngles;
        wheelMeshRotation.y = wheel.localRotation.eulerAngles.y;
        wheelMesh.localRotation = Quaternion.Euler(wheelMeshRotation);
    }

    void Accelerate(float accelerationInput)
    {
        rb.AddForce(EngineForce * accelerationInput * transform.forward);
    }
}
