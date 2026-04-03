using System.Linq;
using Unity.VisualScripting;

namespace Ai4Gamedev.MiniMax.Isolation
{
    using System.Collections.Generic;
    using UnityEngine;
    
    public class DemchyshynStrategy : IMinimaxStrategy
    {
        private readonly string name;
        private readonly IPossibleMovesProvider movesProvider;
        public string Name => name;
        
        public int SearchDepth { get; }

        public DemchyshynStrategy(string name = "Bot", int searchDepth = 5)
        {
            this.name = name;
            SearchDepth = searchDepth;
            movesProvider = new PossibleMovesProvider();
        }
        
        public int EvaluateBoard(IGameBoard board, int playerId)
        {
            int enemyId = (playerId == 1)? 2 : 1;
            int result = 0;
            
            // add all possible players moves
            result += movesProvider.GetPossibleMovesFor(board, playerId).Count;
            
            // remove all possible enemy moves
            result -= movesProvider.GetPossibleMovesFor(board, enemyId).Count;
            
            // add player possible space
            result += GetPossibleSpace(board, playerId);
            
            // remove player enemy space
            result -= GetPossibleSpace(board, enemyId);
            
            return result;
        }

        public List<Move> Sort(IGameBoard board, List<Move> moves)
        {
            return moves;
        }
        
        private int GetPossibleSpace(IGameBoard board, int playerId)
        {
            var cells = board.Cells;
            var boardWidth = cells.GetLength(0);
            var boardHeight = cells.GetLength(1);
            
            // Find current player's occupied cell.
            List<(int, int)> freeCells = new List<(int, int)>();
            var startColumn = -1;
            var startRow = -1;
            for (int x = 0; x < boardWidth; x++)
            {
                for (int y = 0; y < boardHeight; y++)
                {
                    // if (cells[x, y].State == CellState.Free)
                    // {
                    //     freeCells.Add((x, y));
                    // }
                    if (cells[x, y].State == CellState.Occupied &&
                        cells[x, y].PlayerId == playerId)
                    {
                        startColumn = x;
                        startRow = y;
                    }
                }
            }
            
            Queue<(int, int)> cellsQueue = new Queue<(int, int)>();
            cellsQueue.Enqueue((startColumn, startRow));
            
            List<(int, int)> visitedCells = new List<(int, int)>();

            while (cellsQueue.Count > 0)
            {
                (int, int) currentCell = cellsQueue.Dequeue();
                
                // Find all neighbours
                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        if (dx == 0 && dy == 0)
                        {
                            continue;
                        }
                        
                        var destinationColumn = currentCell.Item1 + dx;
                        var destinationRow = currentCell.Item2 + dy;
                        
                        if (destinationColumn < 0 || destinationColumn >= boardWidth)
                        {
                            continue;
                        }

                        if (destinationRow < 0 || destinationRow >= boardHeight)
                        {
                            continue;
                        }

                        if (cells[destinationColumn, destinationRow].State != CellState.Free)
                        {
                            continue;
                        }
                        
                        if(visitedCells.Contains((dx, dy)))
                        {
                            continue;
                        }
                        
                        cellsQueue.Enqueue((dx, dy));
                    }
                }
                
                visitedCells.Add((currentCell.Item1, currentCell.Item2));
            }
            
            return visitedCells.Count;
        }
    }
}