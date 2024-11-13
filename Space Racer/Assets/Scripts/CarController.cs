using UnityEngine;
using UnityEngine.InputSystem;

public class CarController : MonoBehaviour
{
    public Wheel[] wheels;
    Rigidbody rb;
    [Header("Car Specs")]
    [SerializeField] private float wheelBase; // in meters
    [SerializeField] private float rearTrack; // in meters
    [SerializeField] private float turnRadius; // in meters

    [Header("Input System")]
    private PlayerInputActions playerInputActions;
    private float steeringInput;
    public float accelerationInput {get; set; }
    private float smoothSteeringInput;
    private float smoothTime = 0.35f;
    private float smoothVelocity;

    private float ackermannAngleLeft;
    private float ackermannAngleRight;
    public float maxSteeringAngle;

    private void Awake()
    {
        playerInputActions = new PlayerInputActions();
        playerInputActions.Driving.Enable();
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = rb.centerOfMass - new Vector3(0,0.3f,0);
    }


    private void Update()
    {
        steeringInput = playerInputActions.Driving.Steer.ReadValue<float>();
        accelerationInput = playerInputActions.Driving.Accelerate.ReadValue<float>();
        smoothSteeringInput = Mathf.SmoothDamp(smoothSteeringInput, steeringInput, ref smoothVelocity, smoothTime);
        // Speed relative turning
        float speed = Mathf.Abs(rb.linearVelocity.magnitude);
        ackermannAngleLeft = maxSteeringAngle / 2 * smoothSteeringInput * Mathf.Clamp(15 / (speed + 1),0.2f,2f);
        ackermannAngleRight = maxSteeringAngle / 2 * smoothSteeringInput * Mathf.Clamp(15 / (speed + 1),0.2f,2f);
        Debug.Log(speed.ToString() + " " + ackermannAngleLeft.ToString());
        //if (smoothSteeringInput > 0)
        //{
            
        //    //ackermannAngleLeft = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius + (rearTrack / 2))) * smoothSteeringInput;
        //    //ackermannAngleRight = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius - (rearTrack / 2))) * smoothSteeringInput;
        //}
        //else if (smoothSteeringInput < 0)
        //{
        //    //ackermannAngleLeft = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius - (rearTrack / 2))) * smoothSteeringInput;
        //    //ackermannAngleRight = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius + (rearTrack / 2))) * smoothSteeringInput;
        //}
        //else
        //{
        //    ackermannAngleLeft = 0;
        //    ackermannAngleRight = 0;
        //}

        foreach (Wheel wheel in wheels)
        {
            if (wheel.wheelFrontLeft)
            {
                wheel.steerAngle = ackermannAngleLeft;
            }
            if (wheel.wheelFrontRight)
            {
                wheel.steerAngle = ackermannAngleRight;
            }
        }
    }
}
