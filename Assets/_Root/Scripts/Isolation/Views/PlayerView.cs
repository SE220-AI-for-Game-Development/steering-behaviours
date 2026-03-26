namespace Ai4Gamedev.MiniMax.Isolation.Views
{
    using Cysharp.Threading.Tasks;
    using DG.Tweening;
    using UnityEngine;

    public class PlayerView : MonoBehaviour
    {
        [SerializeField]
        private Renderer playerRenderer;

        [SerializeField]
        private float moveDurationSeconds = 0.6f;

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
            if (Vector3.Distance(transform.position, destination) < 0.01f)
            {
                return;
            }

            transform.DOKill();

            var tcs = new UniTaskCompletionSource();
            transform.DOMove(destination, moveDurationSeconds)
                .SetEase(Ease.InOutSine)
                .OnComplete(() => tcs.TrySetResult())
                .OnKill(() => tcs.TrySetResult());

            await tcs.Task;
        }

        private static Color ColorForPlayer(int playerId)
        {
            return playerId switch
            {
                1 => new Color(0.2f, 0.6f, 1f, 1f),
                2 => new Color(0.9f, 0.11f, 0.11f),
                _ => Color.white
            };
        }
    }
}
