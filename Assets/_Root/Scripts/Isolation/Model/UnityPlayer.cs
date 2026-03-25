namespace Ai4Gamedev.MiniMax.Isolation
{
    using System.Collections.Generic;
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using UnityEngine;

    public class UnityPlayer : MonoBehaviour, IPlayer
    {
        public int Id { get; set; }

        private BoardInput boardInput;
        private GameBoardView boardView;

        public void Setup(BoardInput boardInput, GameBoardView boardView)
        {
            this.boardInput = boardInput;
            this.boardView = boardView;
        }

        public async UniTask<Move> GetMove(List<Move> possibleMoves)
        {
            var destination = await SelectDestination(possibleMoves);
            var block = await SelectBlock(possibleMoves, destination);

            return possibleMoves.First(m =>
                m.DestinationPosition.Equals(destination) &&
                m.BlockPosition.Equals(block));
        }

        private async UniTask<Position> SelectDestination(List<Move> possibleMoves)
        {
            var legal = DistinctDestinations(possibleMoves);
            boardView.HighlightAsDestinations(legal);

            Position clicked;
            do
            {
                clicked = await boardInput.WaitForCellClick();
            }
            while (!legal.Contains(clicked));

            boardView.ClearAllHighlights();
            return clicked;
        }

        private async UniTask<Position> SelectBlock(List<Move> possibleMoves, Position destination)
        {
            var legal = LegalBlocksFor(possibleMoves, destination);
            boardView.HighlightAsDestinations(new[] { destination });
            boardView.HighlightAsBlocks(legal);

            Position clicked;
            do
            {
                clicked = await boardInput.WaitForCellClick();
            }
            while (!legal.Contains(clicked));

            boardView.ClearAllHighlights();
            return clicked;
        }

        private static HashSet<Position> DistinctDestinations(List<Move> moves)
        {
            return new HashSet<Position>(moves.Select(m => m.DestinationPosition));
        }

        private static HashSet<Position> LegalBlocksFor(List<Move> moves, Position destination)
        {
            return new HashSet<Position>(
                moves
                    .Where(m => m.DestinationPosition.Equals(destination))
                    .Select(m => m.BlockPosition));
        }
    }
}
