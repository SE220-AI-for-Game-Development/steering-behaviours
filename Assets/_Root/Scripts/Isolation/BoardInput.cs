namespace Ai4Gamedev.MiniMax.Isolation
{
    using Cysharp.Threading.Tasks;
    using UnityEngine;
    using UnityEngine.InputSystem;
    using Views;

    public class BoardInput : MonoBehaviour
    {
        [SerializeField]
        private Camera inputCamera;

        private void Awake()
        {
            if (inputCamera == null)
            {
                inputCamera = Camera.main;
            }
        }

        public async UniTask<Position> WaitForCellClick()
        {
            while (true)
            {
                await UniTask.Yield();

                if (!LeftButtonPressedThisFrame())
                {
                    continue;
                }

                var position = TryRaycastToCell();
                if (position != null)
                {
                    return position;
                }
            }
        }

        private bool LeftButtonPressedThisFrame()
        {
            var mouse = Mouse.current;
            return mouse != null && mouse.leftButton.wasPressedThisFrame;
        }

        private Position TryRaycastToCell()
        {
            var mouse = Mouse.current;
            if (mouse == null)
            {
                return null;
            }

            var ray = inputCamera.ScreenPointToRay(mouse.position.ReadValue());
            if (!Physics.Raycast(ray, out var hit, 100f))
            {
                return null;
            }

            var cellView = hit.collider.GetComponentInParent<CellView>();
            return cellView != null ? cellView.GetPosition() : null;
        }
    }
}
