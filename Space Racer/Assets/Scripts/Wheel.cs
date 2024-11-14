using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Wheel : MonoBehaviour
{
    public bool wheelFrontLeft;
    public bool wheelFrontRight;
    public bool wheelRearLeft;
    public bool wheelRearRight;

    public GameObject wheelMesh;

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
    [SerializeField] float wheelDiameter;
    [SerializeField] float wheelOffset;
    [HideInInspector] public float steerAngle;

    private Vector3 defaultWheelMeshPosition;

    private float wheelCircumference;
    private float wheelxRotation; // in degrees

    void Start()
    {
        rb = transform.root.GetComponent<Rigidbody>();
        carController = transform.root.GetComponent<CarController>();
        minLength = restlength - springTravel;
        maxLength = restlength + springTravel;

        defaultWheelMeshPosition = new Vector3(wheelMesh.transform.localPosition.x, -restlength+wheelDiameter/2+wheelOffset, wheelMesh.transform.localPosition.z);

        wheelCircumference = 2*Mathf.PI*(wheelDiameter/2);
    }

    private void Update()
    {
        transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles.x, steerAngle, transform.localRotation.eulerAngles.z);

        wheelxRotation = Vector3.Dot(rb.GetPointVelocity(wheelMesh.transform.position), transform.forward) * Time.deltaTime / wheelCircumference * 360/10;
        wheelMesh.transform.Rotate(new Vector3(-wheelxRotation,0,0),Space.Self);
        wheelMesh.transform.localRotation = Quaternion.Euler(wheelMesh.transform.localRotation.eulerAngles.x, steerAngle, 0);
        Debug.Log(wheelxRotation);
    }
    void FixedUpdate()
    {
        if (Physics.Raycast(transform.position,-transform.up,out RaycastHit hit, maxLength + wheelDiameter)){
            Vector3 tireWorldVel = rb.GetPointVelocity(transform.position);
            float springVelocity = Vector3.Dot(transform.up, tireWorldVel);
            float springForce = springStiffness * (restlength - hit.distance);
            float damperForce = damperStiffness * springVelocity;
            Vector3 suspensionForce = (springForce - damperForce)* transform.up;

            Debug.DrawLine(transform.position, hit.point, Color.red);
             
            rb.AddForceAtPosition(suspensionForce, transform.position);

            float Fx = carController.accelerationInput * accelerationForce;
            float gripForce = -Vector3.Dot(tireWorldVel,transform.right) * gripForceMultiplier/100;
            rb.AddForceAtPosition(Fx * transform.forward, transform.position);
            rb.AddForceAtPosition(gripForce * transform.right, transform.position,ForceMode.Impulse);
            Vector3 targetWheelMeshPosition = Vector3.zero;
            if (hit.distance >= wheelDiameter)
            {
                targetWheelMeshPosition = new Vector3(wheelMesh.transform.localPosition.x, -hit.distance + wheelDiameter + wheelOffset, wheelMesh.transform.localPosition.z);
                //Debug.Log($"{transform.name}: NORMAL, Hit Distance: {hit.distance}");
            }
            else
            {
                targetWheelMeshPosition = new Vector3(wheelMesh.transform.localPosition.x, wheelOffset, wheelMesh.transform.localPosition.z);
                //Debug.Log($"{transform.name}: COMPRESSED, Hit Distance: {hit.distance}");
            }
            wheelMesh.transform.localPosition = Vector3.Lerp(wheelMesh.transform.localPosition, targetWheelMeshPosition, 0.6f);
            
        }
        else
        {
            wheelMesh.transform.localPosition = Vector3.Lerp(wheelMesh.transform.localPosition, defaultWheelMeshPosition, 0.05f);
        }
    }
}
