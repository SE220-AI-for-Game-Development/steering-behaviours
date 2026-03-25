namespace Ai4Gamedev.MiniMax.Isolation.Views
{
    using UnityEngine;

    public class PlayerView : MonoBehaviour
    {
        [SerializeField]
        private Renderer renderer;

        private void Awake()
        {
            if (renderer == null)
            {
                renderer = GetComponent<Renderer>();
            }

            // Ensure player pieces don't block mouse clicks on board cells.
            var colliders = GetComponents<Collider>();
            for (int i = 0; i < colliders.Length; i++)
            {
                colliders[i].enabled = false;
            }
        }

        public void SetPlayerId(int playerId)
        {
            if (renderer == null)
            {
                return;
            }

            if (playerId == 1)
            {
                renderer.material.color = new Color(0.2f, 0.6f, 1f, 1f);
            }
            else if (playerId == 2)
            {
                renderer.material.color = new Color(0.9f, 0.85f, 0.2f, 1f);
            }
            else
            {
                renderer.material.color = Color.white;
            }
        }
    }
}