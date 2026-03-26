namespace Ai4Gamedev.MiniMax.Isolation.Views
{
    using Cysharp.Threading.Tasks;
    using DG.Tweening;
    using TMPro;
    using UnityEngine;

    public class GameOverPopUp : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text text;

        [SerializeField]
        private RectTransform panel;

        [SerializeField]
        private float showDurationSeconds = 0.5f;

        [SerializeField]
        private float offscreenOffset = 900f;

        private void Awake()
        {
            if (panel == null)
            {
                panel = transform as RectTransform;
            }
        }

        public async UniTask ShowAnimated(IPlayer winner)
        {
            text.text = string.Format(text.text, winner.Name);
            gameObject.SetActive(false);

            panel.DOKill();
            gameObject.SetActive(true);

            var shownPosition = panel.anchoredPosition;
            var hiddenPosition = new Vector2(shownPosition.x, shownPosition.y + offscreenOffset);
            panel.anchoredPosition = hiddenPosition;

            await panel
                .DOAnchorPos(shownPosition, showDurationSeconds)
                .SetEase(Ease.OutBack)
                .AsyncWaitForCompletion();
        }
    }
}