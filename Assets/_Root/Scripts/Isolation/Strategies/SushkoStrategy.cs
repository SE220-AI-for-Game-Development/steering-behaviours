namespace Ai4Gamedev.MiniMax.Isolation
{
    using System;
    using System.Collections.Generic;

    public class BestMiniMax : IMinimaxStrategy
    {
        private readonly string name;
        private readonly IPossibleMovesProvider movesProvider;
        private readonly IGameBoard board;

        public string Name => name;
        public int SearchDepth { get; }

        public BestMiniMax(IGameBoard board, string name = "Yurii", int searchDepth = 5)
        {
            this.name = name;
            this.board = board;
            SearchDepth = searchDepth;
            movesProvider = new PossibleMovesProvider();
        }

        public int EvaluateBoard(IGameBoard board, int playerId)
        {
            return movesProvider.GetPossibleMovesFor(board, playerId).Count;
        }

        public List<Move> Sort(IGameBoard board, List<Move> moves)
        {
            var best = new List<Move>();
            var normal = new List<Move>();
            var corners = new List<Move>();

            Position oponentPosition = new Position { Row = 0, Column = 0 };
            Position playerPosition = new Position { Row = 0, Column = 0 };

            for (var i = board.Cells.GetLength(0) - 1; i >= 0; i--)
            {
                for (var j = board.Cells.GetLength(1) - 1; j >= 0; j--)
                {
                    var cell = board.Cells[i, j];

                    if (cell.State == CellState.Occupied)
                    {
                        if (cell.PlayerId.Value == 1)
                        {
                            oponentPosition.Row = i;
                            oponentPosition.Column = j;
                        }
                        else
                        {
                            playerPosition.Row = i;
                            playerPosition.Column = j;
                        }

                    }
                }
            }

            int maxDistanceFromPlayer = int.MinValue;
            int minDistanceToOpponent = int.MaxValue;

            foreach (var move in moves)
            {
                var block = move.BlockPosition;

                int distanceFromPlayer =
                    Math.Abs(block.Row - playerPosition.Row) +
                    Math.Abs(block.Column - playerPosition.Column);

                int distanceToOpponent =
                    Math.Abs(block.Row - oponentPosition.Row) +
                    Math.Abs(block.Column - oponentPosition.Column);

                if (distanceFromPlayer > maxDistanceFromPlayer)
                    maxDistanceFromPlayer = distanceFromPlayer;

                if (distanceToOpponent < minDistanceToOpponent)
                    minDistanceToOpponent = distanceToOpponent;
            }

            for (var i = moves.Count - 1; i >= 0; i--)
            {
                var move = moves[i];
                var destPostition = moves[i].DestinationPosition;
                var blockPosition = moves[i].BlockPosition;

                bool isCorner =
                        (destPostition.Row == 0 && destPostition.Column == 0) ||
                        (destPostition.Row == 0 && destPostition.Column == 4) ||
                        (destPostition.Row == 4 && destPostition.Column == 0) ||
                        (destPostition.Row == 4 && destPostition.Column == 4);

                var notWallAdjacent = destPostition.Row != 0 && destPostition.Column != 0
                    && destPostition.Row != 4 && destPostition.Column != 4;

                int distanceFromPlayer =
                            Math.Abs(blockPosition.Row - playerPosition.Row) +
                            Math.Abs(blockPosition.Column - playerPosition.Column);

                int distanceToOpponent =
                    Math.Abs(blockPosition.Row - oponentPosition.Row) +
                    Math.Abs(blockPosition.Column - oponentPosition.Column);

                bool furthestFromPlayer = distanceFromPlayer == maxDistanceFromPlayer;
                bool closestToOpponent = distanceToOpponent == minDistanceToOpponent;

                bool bestBlock = furthestFromPlayer && closestToOpponent;

                if (isCorner)
                {
                    corners.Add(move);
                }
                else if (notWallAdjacent && bestBlock)
                {
                    best.Add(move);
                }
                else
                {
                    normal.Add(move);
                }
            }

            best.AddRange(normal);
            best.AddRange(corners);
            return best;
        }
    }
}