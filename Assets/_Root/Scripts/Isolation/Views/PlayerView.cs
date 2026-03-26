namespace Ai4Gamedev.MiniMax.Isolation.Views
{
    using Cysharp.Threading.Tasks;
    using DG.Tweening;
    using UnityEngine;

    public class PlayerView : MonoBehaviour
    {
        [SerializeField]
        private Renderer playerRenderer;

        private void Awake()
        {
            if (playerRenderer == null)
            {
                playerRenderer = GetComponent<Renderer>();
            }

            var colliders = GetComponents<Collider>();
            for (int i = 0; i < colliders.Length; i++)
            {
                colliders[i].enabled = false;
            }
        }

        public void SetPlayerId(int playerId)
        {
            if (playerRenderer == null)
            {
                return;
            }

            playerRenderer.material.color = ColorForPlayer(playerId);
        }

        public async UniTask AnimateMove(Vector3 destination)
        {
            transform.DOKill();

            var tcs = new UniTaskCompletionSource();
            transform.DOMove(destination, 0.4f)
                .SetEase(Ease.OutCubic)
                .OnComplete(() => tcs.TrySetResult());

            await tcs.Task;
        }

        private static Color ColorForPlayer(int playerId)
        {
            return playerId switch
            {
                1 => new Color(0.2f, 0.6f, 1f, 1f),
                2 => new Color(0.9f, 0.85f, 0.2f, 1f),
                _ => Color.white
            };
        }
    }
}
