using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CarPhysics : MonoBehaviour
{
    // Suspension
    float springOriginalLength = 1f;
    float suspensionForceMultiplier = 50f;
    float dampingForceMultiplier = 3f;
    // Reference for wheels
    // Wheel positions
    [SerializeField] Transform frontLeftWheel;
    [SerializeField] Transform frontRightWheel;
    [SerializeField] Transform backLeftWheel;
    [SerializeField] Transform backRightWheel;
    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    void ApplySuspensionForce(Transform wheel)
    {
        bool isHit = Physics.Raycast(wheel.position, -wheel.up, out RaycastHit hit, springOriginalLength + 0.7f);
        if (isHit)
        {
            float distance = hit.distance;
            float springExtension = distance - springOriginalLength;
            float springForce = -(springExtension * suspensionForceMultiplier);
            Vector3 wheelVelocity = rb.GetPointVelocity(wheel.position);
            float velocity = Vector3.Dot(wheelVelocity, transform.up);
            float dampingForce = velocity * dampingForceMultiplier;
            float resultantForce = Mathf.Clamp(springForce - dampingForce, -10000f, 10000f);
            Debug.Log($"Distance: {distance} Extension: {springExtension} SpringFoprce {springForce}");
            rb.AddForceAtPosition(resultantForce * transform.up, wheel.position);
        }
        
        
        //float resultantForce = Mathf.Clamp(springForce - dampingForce, -10000f, 10000f);


    }
    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void FixedUpdate()
    {
        ApplySuspensionForce(frontLeftWheel);
        ApplySuspensionForce(frontRightWheel);
        ApplySuspensionForce(backLeftWheel);
        ApplySuspensionForce(backRightWheel);
    }
}
