namespace Ai4Gamedev.MiniMax.Isolation
{
	using System.Collections.Generic;

	public class ShcherbakovStrategy : IMinimaxStrategy
	{
		public string Name { get; }
		public int SearchDepth { get; }

		private readonly IPossibleMovesProvider _movesProvider;

		// Центр дошки 5х5 = [3,3]
		private const int CenterX = 3;
		private const int CenterY = 3;

		public ShcherbakovStrategy(string name = "Shcherbakov", int searchDepth = 5)
		{
			Name = name;
			SearchDepth = searchDepth;
			_movesProvider = new PossibleMovesProvider();
		}

		public int EvaluateBoard(IGameBoard board, int playerId)
		{
			int opponentId = playerId == 1 ? 2 : 1;

			int myMoves = _movesProvider.GetPossibleMovesFor(board, playerId).Count;
			int opponentMoves = _movesProvider.GetPossibleMovesFor(board, opponentId).Count;

			// Чим більше ходів у нас і менше у суперника, тим краща позиція
			return myMoves - opponentMoves;
		}

		public List<Move> Sort(List<Move> moves)
		{
			List<Move> sorted = new List<Move>(moves);
			sorted.Sort((a, b) => {
				int scoreA = MoveScore(a);
				int scoreB = MoveScore(b);
				return scoreB.CompareTo(scoreA);
			});
			return sorted;
		}

		private static int MoveScore(Move move)
		{
			// Відстань від клітинки куди йде гравець до центру дошки.
			// Ходи ближче до центру отримують вищу оцінку
			int destinationDx = move.DestinationPosition.Column - CenterX;
			int destinationDy = move.DestinationPosition.Row - CenterY;
			int destDistanceSquare = GetNumberSecondPower(destinationDx) + GetNumberSecondPower(destinationDy);

			// Відстань від блокованої клітинки до центру дошки.
			// Блокуємо краще ті клітинки що далі від центру, бо вони менш цінні
			int blockDx = move.BlockPosition.Column - CenterX;
			int blockDy = move.BlockPosition.Row - CenterY;
			int blockDistanceSquare = GetNumberSecondPower(blockDx) + GetNumberSecondPower(blockDy);

			// Хочемо йти до центру, а блокувати далі від центру
			return -destDistanceSquare + blockDistanceSquare;
		}

		private static int GetNumberSecondPower(int number)
		{
			return number * number;
		}
	}
}