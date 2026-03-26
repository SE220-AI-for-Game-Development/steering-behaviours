namespace Ai4Gamedev.MiniMax.Isolation
{
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using UnityEngine;

    public interface IPlayer
    {
        int Id { get; }
        string Name { get; }

        UniTask<Move> GetMove(List<Move> possibleMoves);
    }

    public class MinimaxPlayer : IPlayer
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public UniTask<Move> GetMove(List<Move> possibleMoves)
        {
            if (possibleMoves == null)
            {
                throw new System.ArgumentNullException(nameof(possibleMoves));
            }

            if (possibleMoves.Count == 0)
            {
                return UniTask.FromResult<Move>(null);
            }

            var index = UnityEngine.Random.Range(0, possibleMoves.Count);
            var chosenMove = possibleMoves[index];

            return UniTask.FromResult(chosenMove);
        }

        public MinimaxPlayer(int id, string name)
        {
            Id = id;
            Name = name;
        }

    }

}