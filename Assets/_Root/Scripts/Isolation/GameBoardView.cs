namespace Ai4Gamedev.MiniMax.Isolation
{
    using System.Collections.Generic;
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
        private CellView[,] cellViews;
        private readonly Dictionary<int, PlayerView> playerViews = new Dictionary<int, PlayerView>();

        public void Initialize(IGameBoard board)
        {
            this.board = board;
            SpawnCells(board.Cells);
        }

        public async UniTask ShowMove(Move move)
        {
            if (!playerViews.TryGetValue(move.PlayerId, out var playerView))
            {
                RefreshBoard();
                return;
            }

            // Human player pre-animates the move on destination click, so skip it here.
            // Bot players have not yet moved visually, so animate them now.
            if (!IsAlreadyAtDestination(playerView, move.DestinationPosition))
            {
                await AnimatePlayerMove(move.PlayerId, move.DestinationPosition);
            }

            await cellViews[move.BlockPosition.Column, move.BlockPosition.Row].AnimateRuin();

            RefreshBoard();
        }

        public async UniTask AnimatePlayerMove(int playerId, Position destination)
        {
            if (!playerViews.TryGetValue(playerId, out var playerView))
            {
                return;
            }

            var originX = Mathf.RoundToInt(playerView.transform.position.x);
            var originZ = Mathf.RoundToInt(playerView.transform.position.z);
            cellViews[originX, originZ].SetState(CellState.Free);

            var worldDestination = new Vector3(destination.Column, 0, destination.Row);
            await playerView.AnimateMove(worldDestination);
        }

        private static bool IsAlreadyAtDestination(PlayerView playerView, Position destination)
        {
            return Mathf.RoundToInt(playerView.transform.position.x) == destination.Column &&
                   Mathf.RoundToInt(playerView.transform.position.z) == destination.Row;
        }

        public void HighlightAsDestinations(IEnumerable<Position> positions)
        {
            foreach (var position in positions)
            {
                cellViews[position.Column, position.Row].SetDestinationHighlight();
            }
        }

        public void HighlightAsBlocks(IEnumerable<Position> positions)
        {
            foreach (var position in positions)
            {
                cellViews[position.Column, position.Row].SetBlockHighlight();
            }
        }

        public void ClearAllHighlights()
        {
            if (cellViews == null)
            {
                return;
            }

            var width = cellViews.GetLength(0);
            var height = cellViews.GetLength(1);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    cellViews[x, y].ClearHighlight();
                }
            }
        }

        private void SpawnCells(Cell[,] field)
        {
            var width = field.GetLength(0);
            var height = field.GetLength(1);
            cellViews = new CellView[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    SpawnCell(field, x, y);
                }
            }
        }

        private void SpawnCell(Cell[,] field, int x, int y)
        {
            var cellView = Instantiate(cellViewPrefab, new Vector3(x, 0, y), Quaternion.identity, transform);
            cellView.name = $"Cell {x},{y}";
            cellView.SetCoordinates(x, y);
            cellView.SetState(field[x, y].State);
            cellViews[x, y] = cellView;

            var cell = field[x, y];
            if (cell.State == CellState.Occupied && cell.PlayerId.HasValue)
            {
                SpawnPlayer(cell.PlayerId.Value, x, y);
            }
        }

        private void SpawnPlayer(int playerId, int x, int y)
        {
            if (playerViews.ContainsKey(playerId))
            {
                return;
            }

            var playerView = Instantiate(playerViewPrefab, new Vector3(x, 0, y), Quaternion.identity, transform);
            playerView.name = $"Player {playerId}";
            playerView.SetPlayerId(playerId);
            playerViews[playerId] = playerView;
        }

        private void RefreshBoard()
        {
            RefreshCells();
            RefreshPlayers();
        }

        private void RefreshCells()
        {
            var field = board.Cells;
            var width = field.GetLength(0);
            var height = field.GetLength(1);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    cellViews[x, y].SetState(field[x, y].State);
                }
            }
        }

        private void RefreshPlayers()
        {
            var field = board.Cells;
            var width = field.GetLength(0);
            var height = field.GetLength(1);

            foreach (var kvp in playerViews)
            {
                kvp.Value.gameObject.SetActive(false);
            }

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var cell = field[x, y];
                    if (cell.State != CellState.Occupied || !cell.PlayerId.HasValue)
                    {
                        continue;
                    }

                    MovePlayerView(cell.PlayerId.Value, x, y);
                }
            }
        }

        private void MovePlayerView(int playerId, int x, int y)
        {
            if (!playerViews.TryGetValue(playerId, out var playerView))
            {
                SpawnPlayer(playerId, x, y);
                return;
            }

            playerView.gameObject.SetActive(true);
            playerView.transform.position = new Vector3(x, 0, y);
        }
    }
}
