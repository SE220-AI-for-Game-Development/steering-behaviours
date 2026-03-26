namespace Ai4Gamedev.MiniMax.Isolation
{
    using System;
    using Cysharp.Threading.Tasks;
    using UnityEngine;

    public class GameBoard : IGameBoard
    {
        public Cell[,] Cells { get; set; }
        
        private IGameBoardView view;

        public GameBoard(IGameBoardView view)
        {
            Cells = new Cell[5, 5];
            for (int x = 0; x < Cells.GetLength(0); x++)
            {
                for (int y = 0; y < Cells.GetLength(1); y++)
                {
                    Cells[x, y] = new Cell();
                    if (x == 2 && y == 0)
                    {
                        Cells[x, y].Occupy(1);
                    }
                    if (x == 2 && y == 4)
                    {
                        Cells[x, y].Occupy(2);
                    }
                }
            }
            
            this.view = view;
            view.Initialize(this);
        }

        private GameBoard(Cell[,] cells)
        {
            Cells = cells;
            view = null;
        }


        public bool IsValidMove(Move move)
        {
            if (move == null)
            {
                Debug.LogWarning("[Isolation] Invalid move: move is null.");
                return false;
            }

            if (move.DestinationPosition == null || move.BlockPosition == null)
            {
                Debug.LogWarning($"[Isolation] Invalid move for P{move.PlayerId}: destination or block is null.");
                return false;
            }

            var cells = Cells;
            var boardWidth = cells.GetLength(0);
            var boardHeight = cells.GetLength(1);

            if (move.DestinationPosition.Column < 0 || move.DestinationPosition.Column >= boardWidth)
            {
                Debug.LogWarning($"[Isolation] Invalid move for P{move.PlayerId}: destination out of bounds. {move}");
                return false;
            }

            if (move.DestinationPosition.Row < 0 || move.DestinationPosition.Row >= boardHeight)
            {
                Debug.LogWarning($"[Isolation] Invalid move for P{move.PlayerId}: destination out of bounds. {move}");
                return false;
            }

            if (move.BlockPosition.Column < 0 || move.BlockPosition.Column >= boardWidth)
            {
                Debug.LogWarning($"[Isolation] Invalid move for P{move.PlayerId}: block out of bounds. {move}");
                return false;
            }

            if (move.BlockPosition.Row < 0 || move.BlockPosition.Row >= boardHeight)
            {
                Debug.LogWarning($"[Isolation] Invalid move for P{move.PlayerId}: block out of bounds. {move}");
                return false;
            }

            // Find current player's occupied cell.
            var startColumn = -1;
            var startRow = -1;
            for (int x = 0; x < boardWidth; x++)
            {
                for (int y = 0; y < boardHeight; y++)
                {
                    if (cells[x, y].State == CellState.Occupied &&
                        cells[x, y].PlayerId == move.PlayerId)
                    {
                        startColumn = x;
                        startRow = y;
                    }
                }
            }

            if (startColumn < 0 || startRow < 0)
            {
                Debug.LogWarning($"[Isolation] Invalid move for P{move.PlayerId}: player position not found. {move}");
                return false;
            }

            // king8 adjacency for destination (excluding staying in place).
            var dx = move.DestinationPosition.Column - startColumn;
            var dy = move.DestinationPosition.Row - startRow;

            if (dx == 0 && dy == 0)
            {
                Debug.LogWarning($"[Isolation] Invalid move for P{move.PlayerId}: destination equals start. {move}");
                return false;
            }

            if (Math.Abs(dx) > 1 || Math.Abs(dy) > 1)
            {
                Debug.LogWarning($"[Isolation] Invalid move for P{move.PlayerId}: destination not king8-adjacent. {move}");
                return false;
            }

            if (cells[move.DestinationPosition.Column, move.DestinationPosition.Row].State != CellState.Free)
            {
                Debug.LogWarning($"[Isolation] Invalid move for P{move.PlayerId}: destination not free. {move}");
                return false;
            }

            if (move.BlockPosition.Column == move.DestinationPosition.Column &&
                move.BlockPosition.Row == move.DestinationPosition.Row)
            {
                Debug.LogWarning($"[Isolation] Invalid move for P{move.PlayerId}: block equals destination. {move}");
                return false;
            }

            if (cells[move.BlockPosition.Column, move.BlockPosition.Row].State != CellState.Free)
            {
                Debug.LogWarning($"[Isolation] Invalid move for P{move.PlayerId}: block not free. {move}");
                return false;
            }

            return true;
        }

        public void SimulateMove(Move move)
        {
            FreePlayerCell(move.PlayerId);
            Cells[move.DestinationPosition.Column, move.DestinationPosition.Row].Occupy(move.PlayerId);
            Cells[move.BlockPosition.Column, move.BlockPosition.Row].Ruin();
        }

        public async UniTask ApplyMove(Move move)
        {
            if (!IsValidMove(move))
            {
                Debug.LogWarning($"[Isolation] ApplyMove rejected for P{move.PlayerId}: {move}.");
                throw new InvalidOperationException("Invalid move.");
            }

            SimulateMove(move);

            if (view != null)
            {
                await view.ShowMove(move);
            }

            Debug.Log($"[Isolation] Applied move for P{move.PlayerId}: {move}.");
        }

        private void FreePlayerCell(int playerId)
        {
            for (var x = 0; x < Cells.GetLength(0); x++)
            {
                for (var y = 0; y < Cells.GetLength(1); y++)
                {
                    if (Cells[x, y].State == CellState.Occupied && Cells[x, y].PlayerId == playerId)
                    {
                        Cells[x, y].Free();
                        return;
                    }
                }
            }
        }

        public IGameBoard Clone()
        {
            var width = Cells.GetLength(0);
            var height = Cells.GetLength(1);
            var cloned = new Cell[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var oldCell = Cells[x, y];
                    var newCell = new Cell();

                    if (oldCell.State == CellState.Free)
                    {
                        newCell.Free();
                    }
                    else if (oldCell.State == CellState.Occupied)
                    {
                        newCell.Occupy(oldCell.PlayerId.Value);
                    }
                    else if (oldCell.State == CellState.Ruined)
                    {
                        newCell.Ruin();
                    }

                    cloned[x, y] = newCell;
                }
            }

            return new GameBoard(cloned);
        }
    }
}