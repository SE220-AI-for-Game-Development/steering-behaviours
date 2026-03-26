namespace Ai4Gamedev.MiniMax.Isolation.Views
{
    using Cysharp.Threading.Tasks;
    using DG.Tweening;
    using TMPro;
    using UnityEngine;

    public class MatchStartPopUp : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text text;

        [SerializeField]
        private RectTransform panel;

        [SerializeField]
        private float showDurationSeconds = 0.45f;

        [SerializeField]
        private float hideDurationSeconds = 0.6f;

        [SerializeField]
        private float offscreenOffset = 900f;

        private void Awake()
        {
            if (panel == null)
            {
                panel = transform as RectTransform;
            }
        }

        public async UniTask PlaySequence(IPlayer firstPlayer, IPlayer secondPlayer, float visibleSeconds)
        {
            text.text = string.Format(text.text, firstPlayer.Name, secondPlayer.Name);
            gameObject.SetActive(false);

            panel.DOKill();
            gameObject.SetActive(true);

            var shownPosition = panel.anchoredPosition;
            var topHiddenPosition = new Vector2(shownPosition.x, shownPosition.y + offscreenOffset);
            var bottomHiddenPosition = new Vector2(shownPosition.x, shownPosition.y - offscreenOffset);

            panel.anchoredPosition = topHiddenPosition;
            await panel
                .DOAnchorPos(shownPosition, showDurationSeconds)
                .SetEase(Ease.OutCubic)
                .AsyncWaitForCompletion();

            await UniTask.Delay(System.TimeSpan.FromSeconds(visibleSeconds));

            await panel
                .DOAnchorPos(bottomHiddenPosition, hideDurationSeconds)
                .SetEase(Ease.InCubic)
                .AsyncWaitForCompletion();

            gameObject.SetActive(false);
            panel.anchoredPosition = shownPosition;
        }
    }
}
