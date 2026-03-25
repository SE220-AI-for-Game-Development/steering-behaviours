namespace Ai4Gamedev.MiniMax.Isolation
{
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;

    public interface IPlayer
    {
        public int Id { get; }
        
        UniTask<Move> GetMove(List<Move> possibleMoves);
    }

    public class MinimaxPlayer : IPlayer
    {
        public int Id { get; set; }

        public UniTask<Move> GetMove(List<Move> possibleMoves)
        {
            throw new System.NotImplementedException();
        }

        public MinimaxPlayer(int id)
        {
            Id = id;
        }

    }

}