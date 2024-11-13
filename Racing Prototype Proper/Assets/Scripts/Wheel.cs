using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using UnityEngine;

public class Wheel : MonoBehaviour
{
    private Rigidbody rb;
    [Header("Suspension")]
    [SerializeField] float restlength;
    [SerializeField] float springTravel;
    [SerializeField] float springStiffness;
    [SerializeField] float damperStiffness;

    private float minLength;
    private float maxLength;

    [Header("Wheel")]
    [SerializeField] float wheelRadius;

    void Start()
    {
        rb = transform.root.GetComponent<Rigidbody>();
        minLength = restlength - springTravel;
        maxLength = restlength + springTravel;
    }
    void FixedUpdate()
    {
        if (Physics.Raycast(transform.position,-transform.up,out RaycastHit hit, maxLength + wheelRadius)){
            Vector3 tireWorldVel = rb.GetPointVelocity(transform.position);
            float springVelocity = Vector3.Dot(transform.up, tireWorldVel);
            //float springLength = hit.distance - wheelRadius;
            //springLength = Mathf.Clamp(springLength, minLength, maxLength);
            float springForce = springStiffness * (restlength - hit.distance);
            float damperForce = damperStiffness * springVelocity;
            Vector3 suspensionForce = (springForce - damperForce)* transform.up;
            
            rb.AddForceAtPosition(suspensionForce, transform.position);
            Debug.Log($"{transform.name}: SpringForce: {springForce} DamperForce = {damperForce} Spring Velocity = {springVelocity}");
        }
    }
}
