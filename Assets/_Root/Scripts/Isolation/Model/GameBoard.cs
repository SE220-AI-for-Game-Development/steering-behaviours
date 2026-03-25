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
                Debug.Log("[Isolation] IsValidMove: move is null.");
                return false;
            }

            if (move.DestinationPosition == null || move.BlockPosition == null)
            {
                Debug.Log("[Isolation] IsValidMove: destination or block is null.");
                return false;
            }

            var cells = Cells;
            var boardWidth = cells.GetLength(0);
            var boardHeight = cells.GetLength(1);

            if (move.DestinationPosition.Column < 0 || move.DestinationPosition.Column >= boardWidth)
            {
                Debug.Log("[Isolation] IsValidMove: destination column out of bounds.");
                return false;
            }

            if (move.DestinationPosition.Row < 0 || move.DestinationPosition.Row >= boardHeight)
            {
                Debug.Log("[Isolation] IsValidMove: destination row out of bounds.");
                return false;
            }

            if (move.BlockPosition.Column < 0 || move.BlockPosition.Column >= boardWidth)
            {
                Debug.Log("[Isolation] IsValidMove: block column out of bounds.");
                return false;
            }

            if (move.BlockPosition.Row < 0 || move.BlockPosition.Row >= boardHeight)
            {
                Debug.Log("[Isolation] IsValidMove: block row out of bounds.");
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
                Debug.Log("[Isolation] IsValidMove: could not find player's occupied cell.");
                return false;
            }

            // king8 adjacency for destination (excluding staying in place).
            var dx = move.DestinationPosition.Column - startColumn;
            var dy = move.DestinationPosition.Row - startRow;

            if (dx == 0 && dy == 0)
            {
                Debug.Log("[Isolation] IsValidMove: destination equals start.");
                return false;
            }

            if (Math.Abs(dx) > 1 || Math.Abs(dy) > 1)
            {
                Debug.Log("[Isolation] IsValidMove: destination not king8-adjacent.");
                return false;
            }

            if (cells[move.DestinationPosition.Column, move.DestinationPosition.Row].State != CellState.Free)
            {
                Debug.Log("[Isolation] IsValidMove: destination cell not free.");
                return false;
            }

            if (move.BlockPosition.Column == move.DestinationPosition.Column &&
                move.BlockPosition.Row == move.DestinationPosition.Row)
            {
                Debug.Log("[Isolation] IsValidMove: block equals destination.");
                return false;
            }

            if (cells[move.BlockPosition.Column, move.BlockPosition.Row].State != CellState.Free)
            {
                Debug.Log("[Isolation] IsValidMove: block cell not free.");
                return false;
            }

            return true;
        }

        public async UniTask ApplyMove(Move move)
        {
            if (!IsValidMove(move))
            {
                Debug.Log(
                    $"[Isolation] ApplyMove rejected: Player={move.PlayerId}, Dest={move.DestinationPosition}, Block={move.BlockPosition}.");
                throw new InvalidOperationException("Invalid move.");
            }

            var cells = Cells;
            var boardWidth = cells.GetLength(0);
            var boardHeight = cells.GetLength(1);

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

            cells[startColumn, startRow].Free();
            cells[move.DestinationPosition.Column, move.DestinationPosition.Row].Occupy(move.PlayerId);
            cells[move.BlockPosition.Column, move.BlockPosition.Row].Ruin();

            if (view != null)
            {
                await view.ShowMove(move);
            }

            Debug.Log(
                $"[Isolation] ApplyMove: Player={move.PlayerId}, Dest={move.DestinationPosition}, Block={move.BlockPosition}.");
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