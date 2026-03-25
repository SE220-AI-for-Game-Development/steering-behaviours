namespace Ai4Gamedev.MiniMax.Isolation
{
    using Cysharp.Threading.Tasks;
    using UnityEngine;
    using Views;

    public class GameBoardView : MonoBehaviour, IGameBoardView
    {
        [SerializeField]
        private CellView cellViewPrefab;

        [SerializeField]
        private PlayerView playerViewPrefab;
        
        private IGameBoard board;

        public void Initialize(IGameBoard board)
        {
            this.board = board;
            var field = board.Cells;
            for (int x = 0; x < field.GetLength(0); x++)
            {
                for (int y = 0; y < field.GetLength(1); y++)
                {
                    var cell = field[x, y];
                    var cellView = Instantiate(cellViewPrefab, new Vector3(x, 0, y), Quaternion.identity, transform);
                    cellView.name = $"Cell {x},{y}";

                    if (cell.State == CellState.Occupied)
                    {
                        var playerView = Instantiate(playerViewPrefab, new Vector3(x, 0, y), Quaternion.identity, transform);
                        playerView.name = $"Player {cell.PlayerId}";
                    }
                }
            }
        }

        public async UniTask ShowMove(Move move)
        {
            // Do stuff
        }
    }
}