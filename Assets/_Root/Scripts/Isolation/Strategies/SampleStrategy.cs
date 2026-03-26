namespace Ai4Gamedev.MiniMax.Isolation
{
    using System.Collections.Generic;
    using UnityEngine;

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
            var shuffled = new List<Move>(moves);
            for (var i = shuffled.Count - 1; i > 0; i--)
            {
                var j = Random.Range(0, i + 1);
                (shuffled[i], shuffled[j]) = (shuffled[j], shuffled[i]);
            }

            return shuffled;
        }
    }
}
