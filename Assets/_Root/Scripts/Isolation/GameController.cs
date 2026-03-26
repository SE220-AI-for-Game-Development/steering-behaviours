namespace Ai4Gamedev.MiniMax.Isolation
{
    using Cysharp.Threading.Tasks;
    using UnityEngine;

    [RequireComponent(typeof(BoardInput))]
    public class GameController : MonoBehaviour
    {
        [SerializeField]
        private UnityPlayer unityPlayer;

        [SerializeField]
        private GameBoardView gameBoardView;

        private BoardInput boardInput;

        private readonly IPossibleMovesProvider movesProvider = new PossibleMovesProvider();

        private IPlayer secondPlayer;
        private IPlayer currentPlayer;
        private IGameBoard gameBoard;

        private void Awake()
        {
            boardInput = GetComponent<BoardInput>();
        }

        private async void Start()
        {
            gameBoard = new GameBoard(gameBoardView);

            unityPlayer.Id = 1;
            unityPlayer.Setup(boardInput, gameBoardView);

            secondPlayer = new MinimaxPlayer(2);
            currentPlayer = unityPlayer;

            await RunGameLoop();
        }

        private async UniTask RunGameLoop()
        {
            while (true)
            {
                var possibleMoves = movesProvider.GetPossibleMovesFor(gameBoard, currentPlayer.Id);
                if (possibleMoves.Count == 0)
                {
                    Debug.Log($"[Isolation] Game over. Player {currentPlayer.Id} has no legal moves.");
                    break;
                }

                var move = await currentPlayer.GetMove(possibleMoves);
                await gameBoard.ApplyMove(move);

                currentPlayer = currentPlayer == unityPlayer ? secondPlayer : unityPlayer;
            }
        }
    }
}
