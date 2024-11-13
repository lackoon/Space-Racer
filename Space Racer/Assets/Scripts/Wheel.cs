using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using UnityEngine;

public class Wheel : MonoBehaviour
{
    public bool wheelFrontLeft;
    public bool wheelFrontRight;
    public bool wheelRearLeft;
    public bool wheelRearRight;

    private Rigidbody rb;
    private CarController carController;

    [Header("Forward and backwards")]
    public float accelerationForce;
    public float gripForceMultiplier;


    [Header("Suspension")]
    [SerializeField] float restlength;
    [SerializeField] float springTravel;
    [SerializeField] float springStiffness;
    [SerializeField] float damperStiffness;
    
    private float minLength;
    private float maxLength;

    [Header("Wheel")]
    [SerializeField] float wheelRadius;
    public float steerAngle;

    void Start()
    {
        rb = transform.root.GetComponent<Rigidbody>();
        carController = transform.root.GetComponent<CarController>();
        minLength = restlength - springTravel;
        maxLength = restlength + springTravel;
    }

    private void Update()
    {
        transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles.x, steerAngle, transform.localRotation.eulerAngles.z);
    }
    void FixedUpdate()
    {
        if (Physics.Raycast(transform.position,-transform.up,out RaycastHit hit, maxLength + wheelRadius)){
            Vector3 tireWorldVel = rb.GetPointVelocity(transform.position);
            float springVelocity = Vector3.Dot(transform.up, tireWorldVel);
            float springForce = springStiffness * (restlength - hit.distance);
            float damperForce = damperStiffness * springVelocity;
            Vector3 suspensionForce = (springForce - damperForce)* transform.up;
            
            rb.AddForceAtPosition(suspensionForce, transform.position);

            float Fx = carController.accelerationInput * accelerationForce;
            float gripForce = -Vector3.Dot(tireWorldVel,transform.right) * gripForceMultiplier/100;
            rb.AddForceAtPosition(Fx * transform.forward, transform.position);
            rb.AddForceAtPosition(gripForce * transform.right, transform.position,ForceMode.Impulse);
        }
    }
}
