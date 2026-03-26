namespace Ai4Gamedev.MiniMax.Isolation
{
    using System.Collections.Generic;

    public class SampleStrategy : IMinimaxStrategy
    {
        private readonly string name;
        private readonly IPossibleMovesProvider movesProvider;

        public string Name => name;
        public int SearchDepth { get; }

        public SampleStrategy(string name = "Bot", int searchDepth = 5)
        {
            this.name = name;
            SearchDepth = searchDepth;
            movesProvider = new PossibleMovesProvider();
        }

        public int EvaluateBoard(IGameBoard board, int playerId)
        {
            return movesProvider.GetPossibleMovesFor(board, playerId).Count;
        }

        public List<Move> Sort(List<Move> moves)
        {
            return moves;
        }
    }
}
