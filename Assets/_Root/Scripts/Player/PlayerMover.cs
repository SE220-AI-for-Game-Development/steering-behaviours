namespace Ai4Gamedev.Steerings
{
    using UnityEngine;
    using UnityEngine.InputSystem;

    public class PlayerMover : MonoBehaviour
    {
        [SerializeField]
        private float moveSpeed = 5f;

        public bool IsMoving { get; private set; }

        private void Update()
        {
            var move = Vector3.zero;
            if (Keyboard.current.wKey.isPressed) move.z += 1f;
            if (Keyboard.current.sKey.isPressed) move.z -= 1f;
            if (Keyboard.current.aKey.isPressed) move.x -= 1f;
            if (Keyboard.current.dKey.isPressed) move.x += 1f;

            if (move.sqrMagnitude > 0.01f)
            {
                IsMoving = true;
                var dir = move.normalized;
                transform.rotation = Quaternion.LookRotation(dir);
                transform.position += dir * moveSpeed * Time.deltaTime;
            }
            else
            {
                IsMoving = false;
            }
        }
    }
}
