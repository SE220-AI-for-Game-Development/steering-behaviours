namespace Ai4Gamedev.MiniMax
{
    using UnityEngine;
    using UnityEngine.InputSystem;

    public class Cursor : MonoBehaviour
    {
        [SerializeField]
        private Camera camera;

        private void Awake()
        {
            UnityEngine.Cursor.visible = false;
        }

        private void Update()
        {
            var ray = camera.ScreenPointToRay(Mouse.current.position.ReadValue());
            var plane = new Plane(Vector3.up, Vector3.zero);
            if (plane.Raycast(ray, out float distance))
            {
                transform.position = ray.GetPoint(distance);
            }
        }
    }
}