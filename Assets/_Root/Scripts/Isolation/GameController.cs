namespace Ai4Gamedev.MiniMax.Isolation
{
    using Cysharp.Threading.Tasks;
    using UnityEngine;
    using Views;

    [RequireComponent(typeof(BoardInput))]
    public class GameController : MonoBehaviour
    {
        [SerializeField]
        private UnityPlayer unityPlayer;

        [SerializeField]
        private GameBoardView gameBoardView;

        [SerializeField]
        private GameOverPopUp gameOverPopUp;

        [SerializeField]
        private string botName = "Bot";

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

            secondPlayer = new MinimaxPlayer(2, new SampleStrategy(botName));
            currentPlayer = unityPlayer;

            await RunGameLoop();
        }

        private async UniTask RunGameLoop()
        {
            while (true)
            {
                var possibleMoves = movesProvider.GetPossibleMovesFor(gameBoard, currentPlayer.Id);
                var nextPlayer = currentPlayer == unityPlayer ? secondPlayer : unityPlayer;

                if (possibleMoves.Count == 0)
                {
                    gameOverPopUp.Show(nextPlayer);
                    break;
                }

                var move = await currentPlayer.GetMove(gameBoard, possibleMoves);
                if (move == null || !gameBoard.IsValidMove(move))
                {
                    Debug.LogWarning($"[Isolation] Player {currentPlayer.Name} returned invalid move and is disqualified.");
                    gameOverPopUp.Show(nextPlayer);
                    break;
                }

                await gameBoard.ApplyMove(move);

                currentPlayer = nextPlayer;
            }
        }
    }
}
