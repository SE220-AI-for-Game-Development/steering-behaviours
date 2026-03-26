namespace Ai4Gamedev.MiniMax.Isolation.Views
{
    using Ai4Gamedev.MiniMax.Isolation;
    using Cysharp.Threading.Tasks;
    using DG.Tweening;
    using UnityEngine;

    public class CellView : MonoBehaviour
    {
        [SerializeField]
        private Renderer cellRenderer;

        private CellState baseState;
        private bool isHighlighted;
        private int column;
        private int row;

        private void Awake()
        {
            if (cellRenderer == null)
            {
                cellRenderer = GetComponent<Renderer>();
            }
        }

        public void SetCoordinates(int column, int row)
        {
            this.column = column;
            this.row = row;
        }

        public Position GetPosition()
        {
            return new Position { Column = column, Row = row };
        }

        public void SetState(CellState state)
        {
            cellRenderer.material.DOKill();
            baseState = state;
            isHighlighted = false;
            ApplyColor(ColorForState(state));
        }

        public async UniTask AnimateRuin()
        {
            if (cellRenderer == null)
            {
                return;
            }

            cellRenderer.material.DOKill();
            baseState = CellState.Ruined;
            isHighlighted = false;

            var tcs = new UniTaskCompletionSource();
            cellRenderer.material.DOColor(ColorForState(CellState.Ruined), 0.35f)
                .SetEase(Ease.InQuad)
                .OnComplete(() => tcs.TrySetResult());

            await tcs.Task;
        }

        public void SetDestinationHighlight()
        {
            isHighlighted = true;
            ApplyColor(new Color(0.2f, 1f, 1f, 1f));
        }

        public void SetBlockHighlight()
        {
            isHighlighted = true;
            ApplyColor(new Color(0.2f, 1f, 0.2f, 1f));
        }

        public void ClearHighlight()
        {
            if (!isHighlighted)
            {
                return;
            }

            isHighlighted = false;
            ApplyColor(ColorForState(baseState));
        }

        private void ApplyColor(Color color)
        {
            if (cellRenderer == null)
            {
                return;
            }

            cellRenderer.material.color = color;
        }

        private static Color ColorForState(CellState state)
        {
            return state switch
            {
                CellState.Free     => new Color(0.8f, 0.8f, 0.8f, 1f),
                CellState.Occupied => new Color(0.35f, 0.35f, 0.35f, 1f),
                CellState.Ruined   => new Color(0.6f, 0.1f, 0.1f, 1f),
                _                  => Color.magenta
            };
        }
    }
}
