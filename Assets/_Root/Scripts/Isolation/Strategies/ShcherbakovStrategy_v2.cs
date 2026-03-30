namespace Ai4Gamedev.MiniMax.Isolation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Minimax strategy using Voronoi territory control as the primary heuristic.
    /// The core idea: count how many free cells each player can reach first via BFS.
    /// A player who owns more territory will have more future moves and is more likely to win.
    /// Combined with mobility differential and centrality for a strong overall evaluation.
    /// Move ordering prioritizes blocking cells adjacent to the opponent and central positions,
    /// which maximizes alpha-beta pruning efficiency.
    /// </summary>
    public class ShcherbakovStrategy_v2 : IMinimaxStrategy
    {
        public string Name { get; }
        public int SearchDepth { get; }

        private readonly IPossibleMovesProvider _movesProvider;

        private const int BoardSize = 5;
        private const int WinScore = 100_000;
        private const int LossScore = -100_000;

        // Evaluation weights
        private const int TerritoryWeight = 10;
        private const int MobilityWeight = 4;
        private const int CentralityWeight = 2;

        // Move ordering weights
        private const int BlockAdjacentToOppBonus = 50;
        private const int BlockNearOppBonus = 20;
        private const int BlockFreeNeighborsWeight = 5;
        private const int DestFreeNeighborsWeight = 8;
        private const int DestCentralityWeight = 3;

        public ShcherbakovStrategy_v2(string name = "Shcherbakov_v2", int searchDepth = 5)
        {
            Name = name;
            SearchDepth = searchDepth;
            _movesProvider = new PossibleMovesProvider();
        }

        public int EvaluateBoard(IGameBoard board, int playerId)
        {
            int opponentId = playerId == 1 ? 2 : 1;

            var myMoves = _movesProvider.GetPossibleMovesFor(board, playerId);
            var opponentMoves = _movesProvider.GetPossibleMovesFor(board, opponentId);

            if (opponentMoves.Count == 0) return WinScore;
            if (myMoves.Count == 0) return LossScore;

            var myPos = GetPlayerPosition(board, playerId);
            var oppPos = GetPlayerPosition(board, opponentId);

            // Voronoi territory: cells each player can reach first via BFS
            CalculateVoronoi(board, myPos, oppPos, out int myTerritory, out int oppTerritory);

            // Unique destination positions (direct movement freedom)
            int myMobility = myMoves.Select(m => (m.DestinationPosition.Column, m.DestinationPosition.Row))
                                    .Distinct().Count();
            int oppMobility = opponentMoves.Select(m => (m.DestinationPosition.Column, m.DestinationPosition.Row))
                                           .Distinct().Count();

            int centralityBonus = GetCentrality(myPos);

            return TerritoryWeight * (myTerritory - oppTerritory)
                   + MobilityWeight * (myMobility - oppMobility)
                   + CentralityWeight * centralityBonus;
        }

        public List<Move> Sort(IGameBoard board, List<Move> moves)
        {
            if (moves.Count == 0) return moves;

            int currentPlayerId = moves[0].PlayerId;
            int opponentId = currentPlayerId == 1 ? 2 : 1;
            var oppPos = GetPlayerPosition(board, opponentId);

            return moves.OrderByDescending(m => ScoreMove(board, m, oppPos)).ToList();
        }

        private int ScoreMove(IGameBoard board, Move move, Position oppPos)
        {
            int score = 0;

            // Prioritize blocking cells close to opponent — cuts off their territory
            if (oppPos != null)
            {
                int blockDist = ChebyshevDistance(move.BlockPosition, oppPos);
                if (blockDist == 1) score += BlockAdjacentToOppBonus;
                else if (blockDist == 2) score += BlockNearOppBonus;
            }

            // Prefer blocking high-value (well-connected) cells
            score += CountFreeNeighbors(board, move.BlockPosition) * BlockFreeNeighborsWeight;

            // Prefer moving to positions with many free neighbors (open space)
            score += CountFreeNeighbors(board, move.DestinationPosition) * DestFreeNeighborsWeight;

            // Prefer moving toward the center of the board
            score += GetCentrality(move.DestinationPosition) * DestCentralityWeight;

            return score;
        }

        // Simultaneous BFS from both players to determine Voronoi territory.
        // A free cell belongs to whichever player reaches it first.
        private void CalculateVoronoi(IGameBoard board, Position myPos, Position oppPos,
                                      out int myTerritory, out int oppTerritory)
        {
            myTerritory = 0;
            oppTerritory = 0;

            if (myPos == null || oppPos == null) return;

            var cells = board.Cells;
            var myDist = new int[BoardSize, BoardSize];
            var oppDist = new int[BoardSize, BoardSize];

            for (int c = 0; c < BoardSize; c++)
            for (int r = 0; r < BoardSize; r++)
            {
                myDist[c, r] = int.MaxValue;
                oppDist[c, r] = int.MaxValue;
            }

            myDist[myPos.Column, myPos.Row] = 0;
            oppDist[oppPos.Column, oppPos.Row] = 0;

            var myQueue = new Queue<Position>();
            var oppQueue = new Queue<Position>();
            myQueue.Enqueue(myPos);
            oppQueue.Enqueue(oppPos);

            while (myQueue.Count > 0 || oppQueue.Count > 0)
            {
                if (myQueue.Count > 0)
                    BfsExpand(cells, myQueue, myDist);

                if (oppQueue.Count > 0)
                    BfsExpand(cells, oppQueue, oppDist);
            }

            // Assign territory: each free/occupied cell goes to whoever reaches it first
            for (int c = 0; c < BoardSize; c++)
            for (int r = 0; r < BoardSize; r++)
            {
                if (cells[c, r].State == CellState.Ruined) continue;

                int md = myDist[c, r];
                int od = oppDist[c, r];

                bool myReachable = md != int.MaxValue;
                bool oppReachable = od != int.MaxValue;

                if (myReachable && (!oppReachable || md < od))
                    myTerritory++;
                else if (oppReachable && (!myReachable || od < md))
                    oppTerritory++;
            }
        }

        private void BfsExpand(Cell[,] cells, Queue<Position> queue, int[,] dist)
        {
            var pos = queue.Dequeue();
            int d = dist[pos.Column, pos.Row];

            for (int dc = -1; dc <= 1; dc++)
            for (int dr = -1; dr <= 1; dr++)
            {
                if (dc == 0 && dr == 0) continue;

                int nc = pos.Column + dc;
                int nr = pos.Row + dr;

                if (nc < 0 || nc >= BoardSize || nr < 0 || nr >= BoardSize) continue;
                if (cells[nc, nr].State != CellState.Free) continue;

                int newDist = d + 1;
                if (newDist < dist[nc, nr])
                {
                    dist[nc, nr] = newDist;
                    queue.Enqueue(new Position { Column = nc, Row = nr });
                }
            }
        }

        private int CountFreeNeighbors(IGameBoard board, Position pos)
        {
            if (pos == null) return 0;

            int count = 0;
            var cells = board.Cells;

            for (int dc = -1; dc <= 1; dc++)
            for (int dr = -1; dr <= 1; dr++)
            {
                if (dc == 0 && dr == 0) continue;

                int nc = pos.Column + dc;
                int nr = pos.Row + dr;

                if (nc < 0 || nc >= BoardSize || nr < 0 || nr >= BoardSize) continue;
                if (cells[nc, nr].State == CellState.Free) count++;
            }

            return count;
        }

        // Returns 4 for center (2,2), decreasing toward edges
        private int GetCentrality(Position pos)
        {
            if (pos == null) return 0;
            return 4 - Math.Abs(pos.Column - 2) - Math.Abs(pos.Row - 2);
        }

        // Chebyshev (king-move) distance
        private int ChebyshevDistance(Position a, Position b)
        {
            if (a == null || b == null) return int.MaxValue;
            return Math.Max(Math.Abs(a.Column - b.Column), Math.Abs(a.Row - b.Row));
        }

        private Position GetPlayerPosition(IGameBoard board, int playerId)
        {
            var cells = board.Cells;
            for (int c = 0; c < BoardSize; c++)
            for (int r = 0; r < BoardSize; r++)
            {
                if (cells[c, r].State == CellState.Occupied && cells[c, r].PlayerId == playerId)
                    return new Position { Column = c, Row = r };
            }
            return null;
        }
    }
}
