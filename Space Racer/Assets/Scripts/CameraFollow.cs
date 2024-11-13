using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform targetObject;
    public Transform cameraTarget;
    public float smoothTime = 0.5f;
    private Vector3 currentVelocity1;
    private Vector3 currentVelocity2;
    
    void Start()
    {
        
    }


    void Update()
    {
        transform.position = Vector3.SmoothDamp(transform.position, cameraTarget.position, ref currentVelocity1, smoothTime);
        transform.LookAt(targetObject);
    }
}
