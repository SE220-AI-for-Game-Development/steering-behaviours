namespace Ai4Gamedev.MiniMax.Isolation
{
    using System;
    using System.Linq;
    using Unity.VisualScripting;
    using UnityEngine;

    public class GameController : MonoBehaviour
    {
        [SerializeField]
        private UnityPlayer unityPlayer;

        [SerializeField]
        private GameBoardView gameBoardView;

        private IPlayer secondPlayer;
        
        private IPlayer currentPlayer;
        
        private IGameBoard gameBoard;

        private readonly IPossibleMovesProvider movesProvider = new PossibleMovesProvider();
        
        private async void Start()
        {
            gameBoard = new GameBoard(gameBoardView);

            unityPlayer.Id = 1;
            secondPlayer = new MinimaxPlayer(2);
            currentPlayer = unityPlayer;

            while (!IsGameOver())
            {
                var possibleMoves = movesProvider.GetPossibleMovesFor(currentPlayer.Id);
                var nextMove = await currentPlayer.GetMove(possibleMoves);
                
                await gameBoard.ApplyMove(nextMove);
            }
        }

        private bool IsGameOver()
        {
            return !movesProvider.GetPossibleMovesFor(1).Any() ||
                !movesProvider.GetPossibleMovesFor(2).Any();
        }
    }
}