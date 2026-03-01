namespace Ai4Gamedev.Steerings
{
    using UnityEngine;
    using UnityEngine.InputSystem;

    public class VehicleManualMover : MonoBehaviour
    {
        [SerializeField]
        private float force = 5;
        
        private Vehicle vehicle;

        private void Awake()
        {
            vehicle = GetComponent<Vehicle>();
        }

        private void Update()
        {
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
            {
                vehicle.ApplyForce(-transform.right * force);
            }

            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
            {
                vehicle.ApplyForce(transform.right * force);
            }

            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
            {
                vehicle.ApplyForce(-transform.forward * force);
            }

            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
            {
                vehicle.ApplyForce(transform.forward * force);
            }
        }
    }
}