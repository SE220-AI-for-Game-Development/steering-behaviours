using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ai4Gamedev.MiniMax.Isolation
{
    public class EntryStrategy : IMinimaxStrategy
    {
        private readonly string name;
        private readonly int searchDepth;
        private readonly IPossibleMovesProvider movesProvider;

        public string Name => name;
        public int SearchDepth => searchDepth;

        public EntryStrategy(string name = "KIABot", int searchDepth = 4)
        {
            this.name = name;
            this.searchDepth = searchDepth;
            this.movesProvider = new PossibleMovesProvider();
        }

        public int EvaluateBoard(IGameBoard board, int playerId)
        {
            int opponentId = (playerId == 1) ? 2 : 1;

            var myMoves = movesProvider.GetPossibleMovesFor(board, playerId);
            var opponentMoves = movesProvider.GetPossibleMovesFor(board, opponentId);

            if (opponentMoves.Count == 0) return 10000;
            if (myMoves.Count == 0) return -10000;

            Position myPos = GetPlayerPosition(board, playerId);
            Position oppPos = GetPlayerPosition(board, opponentId);
            int distToOpponent = GetDistance(myPos, oppPos);

            int score = 0;
            if (opponentMoves.Count < 3 || distToOpponent > 4)
            {
                score = (myMoves.Count * 5) - (opponentMoves.Count * 100);
            }
            else
            {
                score = (myMoves.Count * 10) - (opponentMoves.Count * 40);
                if (distToOpponent <= 2) score += 40;
                score += CalculateCentrality(myPos) * 8;
            }

            return score;
        }

        public List<Move> Sort(List<Move> moves)
        {
            return moves.OrderByDescending(m =>
            {
                int priority = 0;

                priority += CalculateCentrality(m.BlockPosition) * 20;

                if (GetDistance(m.DestinationPosition, m.BlockPosition) > 1) priority += 15;

                priority += CalculateCentrality(m.DestinationPosition) * 5;

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
            return Mathf.Abs(a.Column - b.Column) + Mathf.Abs(a.Row - b.Row);
        }

        private Position GetPlayerPosition(IGameBoard board, int playerId)
        {
            var cells = board.Cells;
            for (int x = 0; x < cells.GetLength(0); x++)
            for (int y = 0; y < cells.GetLength(1); y++)
            {
                if (cells[x, y].State == CellState.Occupied && cells[x, y].PlayerId == playerId)
                    return new Position { Column = x, Row = y };
            }

            return null;
        }
    }
}