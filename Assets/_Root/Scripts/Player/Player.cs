namespace Ai4Gamedev.MiniMax
{
    using UnityEngine;

    public class Player : MonoBehaviour
    {
        [SerializeField]
        private Animator animator;

        [SerializeField]
        private string isMovingParam = "IsMoving";

        private PlayerMover mover;

        private void Awake()
        {
            mover = GetComponent<PlayerMover>();
        }

        private void Update()
        {
            animator.SetBool(isMovingParam, mover.IsMoving);
        }
    }
}