namespace Ai4Gamedev.Steerings
{
    using UnityEngine;
    using UnityEngine.InputSystem;

    public class VehicleManualMover : MonoBehaviour
    {
        [SerializeField]
        private float force = 5;
        
        private Agent agent;

        private void Awake()
        {
            agent = GetComponent<Agent>();
        }

        private void Update()
        {
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
            {
                agent.ApplyForce(-transform.right * force);
            }

            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
            {
                agent.ApplyForce(transform.right * force);
            }

            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
            {
                agent.ApplyForce(-transform.forward * force);
            }

            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
            {
                agent.ApplyForce(transform.forward * force);
            }
        }
    }
}