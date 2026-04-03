using System;
using System.Collections.Generic;
using System.Linq;

namespace Ai4Gamedev.MiniMax.Isolation
{
    public class AniiaStrategy : IMinimaxStrategy
    {
        public string Name => "Aniia";
        private readonly IPossibleMovesProvider movesProvider;
        public int SearchDepth => 3;

        public AniiaStrategy()
        {
            movesProvider = new PossibleMovesProvider();
        }

        private int CountBlocking(List<Move> moves)
        {
            if (!moves.Any<Move>())
            {
                return 0;
            }
            int leftmost_column = moves.Min<Move>(m => m.DestinationPosition.Column);
            int bottom_row = moves.Min<Move>(m => m.DestinationPosition.Row);
            int rightmost_column = moves.Max<Move>(m => m.DestinationPosition.Column);
            int top_row = moves.Max<Move>(m => m.DestinationPosition.Row);
            int line_blocking =
                +moves.Where<Move>(m => m.DestinationPosition.Column == leftmost_column).ToList().Count
                + moves.Where<Move>(m => m.DestinationPosition.Column == rightmost_column).ToList().Count
                + moves.Where<Move>(m => m.DestinationPosition.Row == bottom_row).ToList().Count
                + moves.Where<Move>(m => m.DestinationPosition.Column == top_row).ToList().Count;


            List<Position> positions = moves.Select(m => m.DestinationPosition).ToList();

            int corner_blocking =
            +Convert.ToInt32(
                positions.Contains<Position>(new Position { Column = leftmost_column, Row = top_row })
                && positions.Contains<Position>(new Position { Column = leftmost_column + 1, Row = top_row })
                && positions.Contains<Position>(new Position { Column = leftmost_column, Row = top_row - 1 })
                )
            + Convert.ToInt32(
                positions.Contains<Position>(new Position { Column = rightmost_column, Row = top_row })
                && positions.Contains<Position>(new Position { Column = rightmost_column - 1, Row = top_row })
                && positions.Contains<Position>(new Position { Column = rightmost_column, Row = top_row - 1 })
                )
            + Convert.ToInt32(
                positions.Contains<Position>(new Position { Column = rightmost_column, Row = bottom_row })
                && positions.Contains<Position>(new Position { Column = rightmost_column - 1, Row = bottom_row })
                && positions.Contains<Position>(new Position { Column = rightmost_column, Row = bottom_row + 1 })
                )
            + Convert.ToInt32(
                positions.Contains<Position>(new Position { Column = leftmost_column, Row = bottom_row })
                && positions.Contains<Position>(new Position { Column = leftmost_column + 1, Row = bottom_row })
                && positions.Contains<Position>(new Position { Column = leftmost_column, Row = bottom_row + 1 })
                )
            ;

            return line_blocking + corner_blocking;
        }
        public int EvaluateBoard(IGameBoard board, int playerId)
        {
            var otherId = playerId % 2 + 1;
            var myMoves = movesProvider.GetPossibleMovesFor(board, playerId);
            var theirMoves = movesProvider.GetPossibleMovesFor(board, otherId);
            return CountBlocking(myMoves) - CountBlocking(theirMoves);
        }

        public List<Move> Sort(IGameBoard board, List<Move> moves)
        {
            var shuffled = new List<Move>(moves);
            for (var i = shuffled.Count - 1; i > 0; i--)
            {
                var j = UnityEngine.Random.Range(0, i + 1);
                (shuffled[i], shuffled[j]) = (shuffled[j], shuffled[i]);
            }

            return shuffled;
        }
    }
}