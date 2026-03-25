namespace Ai4Gamedev.MiniMax.Isolation
{
    using System.Collections.Generic;

    public interface IPossibleMovesProvider
    {
        List<Move> GetPossibleMovesFor(int playerId);
    }

    public class PossibleMovesProvider : IPossibleMovesProvider
    {
        public List<Move> GetPossibleMovesFor(int playerId)
        {
            throw new System.NotImplementedException();
        }
    }
}