namespace Ai4Gamedev.MiniMax.Isolation
{
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using UnityEngine;

    public class UnityPlayer : MonoBehaviour, IPlayer
    {
        public int Id { get; set; }

        public UniTask<Move> GetMove(List<Move> possibleMoves)
        {
            throw new System.NotImplementedException();
        }
    }
}