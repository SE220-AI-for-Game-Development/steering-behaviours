namespace Ai4Gamedev.MiniMax.Isolation
{
    using System.Collections.Generic;
    using System.Linq;

    public class LimitOpponentStrategy : IMinimaxStrategy
    {
        private readonly string name;
        private readonly IPossibleMovesProvider movesProvider;

        public string Name => name;

        public int SearchDepth => 1;

        public LimitOpponentStrategy(string name = "Limiter Bot")
        {
            this.name = name;
            movesProvider = new PossibleMovesProvider();
        }

        public int EvaluateBoard(IGameBoard board, int playerId)
        {
            var opponentId = playerId == 1 ? 2 : 1;
            var opponentMovementOptions = movesProvider
                .GetPossibleMovesFor(board, opponentId)
                .Select(move => move.DestinationPosition)
                .Distinct()
                .Count();

            return -opponentMovementOptions;
        }

        public List<Move> Sort(IGameBoard board, List<Move> moves)
        {
            return moves;
        }
    }
}
