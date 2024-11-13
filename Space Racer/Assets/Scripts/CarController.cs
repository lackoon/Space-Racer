using UnityEngine;
using UnityEngine.InputSystem;

public class CarController : MonoBehaviour
{
    private PlayerInputActions playerInputActions;
    private void Awake()
    {
        playerInputActions = new PlayerInputActions();
        playerInputActions.Driving.Enable();
    }
    

    private void FixedUpdate()
    {
        float steeringInput = playerInputActions.Driving.Steer.ReadValue<float>();
        Debug.Log(steeringInput);
    }
}
