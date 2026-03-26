namespace Ai4Gamedev.MiniMax.Isolation
{
    using System;
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;

    public interface IPlayer
    {
        int Id { get; }
        string Name { get; }

        UniTask<Move> GetMove(IGameBoard board, List<Move> possibleMoves);
    }

    public class MinimaxPlayer : IPlayer
    {
        private const int MaxEvaluations = 30_000;

        private readonly IMinimaxStrategy strategy;
        private readonly IPossibleMovesProvider movesProvider;

        private int evaluationCount;

        public int Id { get; }
        public string Name => strategy.Name;

        public MinimaxPlayer(int id, IMinimaxStrategy strategy)
        {
            Id = id;
            this.strategy = strategy;
            movesProvider = new PossibleMovesProvider();
        }

        public UniTask<Move> GetMove(IGameBoard board, List<Move> possibleMoves)
        {
            if (possibleMoves.Count == 0)
            {
                return UniTask.FromResult<Move>(null);
            }

            evaluationCount = 0;
            var best = FindBestMove(board, possibleMoves);
            return UniTask.FromResult(best);
        }

        private Move FindBestMove(IGameBoard board, List<Move> possibleMoves)
        {
            var sorted = strategy.Sort(possibleMoves);
            var bestScore = int.MinValue;
            var bestMove = sorted[0];

            foreach (var move in sorted)
            {
                if (evaluationCount >= MaxEvaluations)
                {
                    break;
                }

                var simulated = board.Clone();
                simulated.SimulateMove(move);

                var depth = Math.Max(1, strategy.SearchDepth);
                var score = Minimax(simulated, depth - 1, int.MinValue, int.MaxValue, maximizing: false);

                if (score > bestScore)
                {
                    bestScore = score;
                    bestMove = move;
                }
            }

            return bestMove;
        }

        private int Minimax(IGameBoard board, int depth, int alpha, int beta, bool maximizing)
        {
            var currentId = maximizing ? Id : OpponentId();
            var moves = movesProvider.GetPossibleMovesFor(board, currentId);

            if (moves.Count == 0)
            {
                // Current side has no moves — they lose.
                return maximizing ? int.MinValue + 1 : int.MaxValue - 1;
            }

            if (evaluationCount >= MaxEvaluations)
            {
                return 0;
            }

            if (depth == 0)
            {
                evaluationCount++;
                return strategy.EvaluateBoard(board, Id);
            }

            var sorted = strategy.Sort(moves);

            if (maximizing)
            {
                return MaximizingSearch(board, sorted, depth, alpha, beta);
            }
            return MinimizingSearch(board, sorted, depth, alpha, beta);
        }

        private int MaximizingSearch(IGameBoard board, List<Move> moves, int depth, int alpha, int beta)
        {
            var best = int.MinValue;

            foreach (var move in moves)
            {
                if (evaluationCount >= MaxEvaluations)
                {
                    break;
                }

                var simulated = board.Clone();
                simulated.SimulateMove(move);

                var score = Minimax(simulated, depth - 1, alpha, beta, maximizing: false);
                best = Math.Max(best, score);
                alpha = Math.Max(alpha, score);

                if (beta <= alpha)
                {
                    break; // Beta cut-off.
                }
            }

            return best;
        }

        private int MinimizingSearch(IGameBoard board, List<Move> moves, int depth, int alpha, int beta)
        {
            var best = int.MaxValue;

            foreach (var move in moves)
            {
                if (evaluationCount >= MaxEvaluations)
                {
                    break;
                }

                var simulated = board.Clone();
                simulated.SimulateMove(move);

                var score = Minimax(simulated, depth - 1, alpha, beta, maximizing: true);
                best = Math.Min(best, score);
                beta = Math.Min(beta, score);

                if (beta <= alpha)
                {
                    break; // Alpha cut-off.
                }
            }

            return best;
        }

        private int OpponentId() => Id == 1 ? 2 : 1;
    }
}
