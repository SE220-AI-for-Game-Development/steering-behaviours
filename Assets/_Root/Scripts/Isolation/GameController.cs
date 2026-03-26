namespace Ai4Gamedev.MiniMax.Isolation
{
    using System;
    using System.Threading.Tasks;
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
        private MatchStartPopUp matchStartPopUp;

        [SerializeField]
        private string botName = "Bot";

        [SerializeField]
        private bool minimaxVsMinimax = true;

        [SerializeField]
        private string firstBotName = "Bot A";

        [SerializeField]
        private string secondBotName = "Bot B";

        [SerializeField]
        private float delayBetweenMovesSeconds = 0.35f;

        [SerializeField]
        private float gameOverDelaySeconds = 1.0f;

        [SerializeField]
        private float startPopupSeconds = 2.0f;

        [SerializeField]
        private float moveTimeoutSeconds = 2.0f;

        [SerializeField]
        private bool loseOnTimeout = true;

        private BoardInput boardInput;

        private readonly IPossibleMovesProvider movesProvider = new PossibleMovesProvider();

        private IPlayer firstPlayer;
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
            if (minimaxVsMinimax)
            {
                firstPlayer = new MinimaxPlayer(1, new NykytaKasianenkoMinimaxStrategy(gameBoard));
                secondPlayer = new MinimaxPlayer(2, new LimitOpponentStrategy("Stranger bot"));
                currentPlayer = firstPlayer;
            }
            else
            {
                unityPlayer.Id = 1;
                unityPlayer.Setup(boardInput, gameBoardView);

                firstPlayer = unityPlayer;
                secondPlayer = new MinimaxPlayer(2, new LimitOpponentStrategy(botName));
                currentPlayer = firstPlayer;
            }

            await ShowStartPopup();
            await RunGameLoop();
        }

        private async UniTask RunGameLoop()
        {
            Debug.Log($"[Isolation] Match started: {firstPlayer.Name} (P{firstPlayer.Id}) vs {secondPlayer.Name} (P{secondPlayer.Id}).");

            while (true)
            {
                var possibleMoves = movesProvider.GetPossibleMovesFor(gameBoard, currentPlayer.Id);
                var nextPlayer = currentPlayer == firstPlayer ? secondPlayer : firstPlayer;
                Debug.Log($"[Isolation] Turn: {currentPlayer.Name} (P{currentPlayer.Id}), legal moves: {possibleMoves.Count}.");

                if (possibleMoves.Count == 0)
                {
                    Debug.Log($"[Isolation] {currentPlayer.Name} (P{currentPlayer.Id}) has no legal moves. Winner: {nextPlayer.Name} (P{nextPlayer.Id}).");
                    await ShowWinnerWithDelay(nextPlayer);
                    break;
                }

                var useTimeout = loseOnTimeout && currentPlayer is not UnityPlayer;
                var moveTask = currentPlayer.GetMove(gameBoard, possibleMoves).AsTask();
                if (useTimeout)
                {
                    var timeoutTask = Task.Delay(TimeSpan.FromSeconds(moveTimeoutSeconds));
                    var completedTask = await Task.WhenAny(moveTask, timeoutTask);
                    if (completedTask != moveTask)
                    {
                        Debug.LogWarning($"[Isolation] Disqualified: {currentPlayer.Name} (P{currentPlayer.Id}) exceeded move timeout ({moveTimeoutSeconds:0.##}s). Winner: {nextPlayer.Name} (P{nextPlayer.Id}).");
                        await ShowWinnerWithDelay(nextPlayer);
                        break;
                    }
                }

                var move = await moveTask;
                if (move == null || !gameBoard.IsValidMove(move))
                {
                    Debug.LogWarning($"[Isolation] Disqualified: {currentPlayer.Name} (P{currentPlayer.Id}) returned invalid move. Winner: {nextPlayer.Name} (P{nextPlayer.Id}).");
                    await ShowWinnerWithDelay(nextPlayer);
                    break;
                }

                Debug.Log($"[Isolation] Move: {currentPlayer.Name} (P{currentPlayer.Id}) -> {move}.");
                await gameBoard.ApplyMove(move);
                await UniTask.Delay(TimeSpan.FromSeconds(delayBetweenMovesSeconds));

                currentPlayer = nextPlayer;
            }
        }

        private async UniTask ShowWinnerWithDelay(IPlayer winner)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(gameOverDelaySeconds));
            await gameOverPopUp.ShowAnimated(winner);
        }

        private async UniTask ShowStartPopup()
        {
            if (matchStartPopUp == null)
            {
                return;
            }

            await matchStartPopUp.PlaySequence(firstPlayer, secondPlayer, startPopupSeconds);
        }
    }
}
