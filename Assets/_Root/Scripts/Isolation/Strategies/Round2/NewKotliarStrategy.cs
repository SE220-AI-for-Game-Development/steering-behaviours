using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ai4Gamedev.MiniMax.Isolation
{
    public class NewKotliarStrategy : IMinimaxStrategy
    {
        private readonly string name;
        private readonly int searchDepth;
        private readonly IPossibleMovesProvider movesProvider;

        public string Name => name;
        public int SearchDepth => searchDepth;

        public NewKotliarStrategy(string name = "KIABot_2.0", int searchDepth = 4)
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

        public List<Move> Sort(IGameBoard board, List<Move> moves)
        {
            int playerId = moves.Count > 0 ? moves[0].PlayerId : 1;
            int opponentId = (playerId == 1) ? 2 : 1;
            Position oppPos = GetPlayerPosition(board, opponentId);

            var opponentMoves = movesProvider.GetPossibleMovesFor(board, opponentId);
            var opponentDestinations = opponentMoves.Select(m => m.DestinationPosition).Distinct().ToList();

            return moves.OrderByDescending(m =>
            {
                int priority = 0;

                if (opponentDestinations.Count == 1 && m.BlockPosition.Equals(opponentDestinations[0]))
                {
                    priority += 10000;
                }

                priority += CountFreeNeighbors(board, m.DestinationPosition) * 50;

                if (oppPos != null && IsAdjacent(m.BlockPosition, oppPos))
                {
                    priority += CountFreeNeighbors(board, m.BlockPosition) * 40;
                }

                priority += CalculateCentrality(m.BlockPosition) * 10;
                priority += CalculateCentrality(m.DestinationPosition) * 5;

                return priority;
            }).ToList();
        }

        private int CountFreeNeighbors(IGameBoard board, Position pos)
        {
            int count = 0;
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0) continue;

                    var neighbor = new Position { Column = pos.Column + dx, Row = pos.Row + dy };
                    if (IsWithinBounds(board, neighbor) &&
                        board.Cells[neighbor.Column, neighbor.Row].State == CellState.Free)
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        private bool IsWithinBounds(IGameBoard board, Position pos)
        {
            return pos.Column >= 0 && pos.Column < board.Cells.GetLength(0) &&
                   pos.Row >= 0 && pos.Row < board.Cells.GetLength(1);
        }

        private bool IsAdjacent(Position a, Position b)
        {
            return Mathf.Abs(a.Column - b.Column) <= 1 && Mathf.Abs(a.Row - b.Row) <= 1;
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