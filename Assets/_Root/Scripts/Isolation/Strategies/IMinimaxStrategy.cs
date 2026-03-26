namespace Ai4Gamedev.MiniMax.Isolation
{
    using System.Collections.Generic;

    public interface IMinimaxStrategy
    {
        string Name { get; }
        
        int SearchDepth { get; }
        
        int EvaluateBoard(IGameBoard board, int playerId);
        
        List<Move> Sort(List<Move> moves);
    }
}
