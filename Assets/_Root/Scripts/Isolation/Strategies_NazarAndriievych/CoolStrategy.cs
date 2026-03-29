using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ai4Gamedev.MiniMax.Isolation
{
    public class CoolStrategy : IMinimaxStrategy
    {
        private readonly string name;
        private readonly int searchDepth;
        private readonly IPossibleMovesProvider movesProvider;

        public string Name => name;
        public int SearchDepth => searchDepth;

        public CoolStrategy(string name = "KIA K9 bot", int searchDepth = 4)
        {
            this.name = name;
            this.searchDepth = searchDepth;
            this.movesProvider = new PossibleMovesProvider();
        }

        public int EvaluateBoard(IGameBoard board, int playerId)
        {
            int opponentId = (playerId == 1) ? 2 : 1;

            Position myPos = GetPlayerPosition(board, playerId);
            Position oppPos = GetPlayerPosition(board, opponentId);

            if (myPos == null) return -10000;
            if (oppPos == null) return 10000;

            return GetWeightedFreeCells(board, myPos) - GetWeightedFreeCells(board, oppPos);
        }

        private int GetWeightedFreeCells(IGameBoard board, Position pos)
        {
            int score = 0;
            var cells = board.Cells;
            
            for (int x = 0; x < cells.GetLength(0); x++)
            {
                for (int y = 0; y < cells.GetLength(1); y++)
                {
                    if (cells[x, y].State == CellState.Free)
                    {
                        int radius = GetDistance(pos, new Position { Column = x, Row = y });
                        
                        if (radius == 1) score += 2;
                        else if (radius == 2) score += 1;
                    }
                }
            }
            return score;
        }

        public List<Move> Sort(IGameBoard board, List<Move> moves)
        {
            if (moves.Count == 0) return moves;

            int myId = moves[0].PlayerId;
            int oppId = (myId == 1) ? 2 : 1;
            Position oppPos = GetPlayerPosition(board, oppId);

            return moves.OrderByDescending(m =>
            {
                if (oppPos == null) return 0;

                int priority = 0;

                // 1. Move outside from the opponent
                priority += GetDistance(m.DestinationPosition, oppPos) * 100;

                // 2. Destroy nearest cell from enemy
                priority -= GetDistance(m.BlockPosition, oppPos) * 50;

                // 3. Destroy nearest cell to center
                priority += CalculateCentrality(m.BlockPosition) * 10;

                // 4. Tie-breaker: choose the nearest destroyed cell to us
                priority -= GetDistance(m.BlockPosition, m.DestinationPosition);

                return priority;
            }).ToList();
        }

        private int CalculateCentrality(Position pos)
        {
            if (pos == null) return 0;
            return (2 - Mathf.Abs(2 - pos.Column)) + (2 - Mathf.Abs(2 - pos.Row));
        }

        private int GetDistance(Position a, Position b)
        {
            if (a == null || b == null) return 0;
            return Mathf.Max(Mathf.Abs(a.Column - b.Column), Mathf.Abs(a.Row - b.Row));
        }

        private Position GetPlayerPosition(IGameBoard board, int playerId)
        {
            var cells = board.Cells;
            for (int x = 0; x < cells.GetLength(0); x++)
            {
                for (int y = 0; y < cells.GetLength(1); y++)
                {
                    if (cells[x, y].State == CellState.Occupied && cells[x, y].PlayerId == playerId)
                    {
                        return new Position { Column = x, Row = y };
                    }
                }
            }
            return null;
        }
    }
}