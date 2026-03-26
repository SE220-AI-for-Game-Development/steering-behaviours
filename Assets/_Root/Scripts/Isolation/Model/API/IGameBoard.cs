namespace Ai4Gamedev.MiniMax.Isolation
{
    using Cysharp.Threading.Tasks;

    public interface IGameBoard
    {
        Cell[,] Cells { get; }
        
        bool IsValidMove(Move move);

        /// <summary>Applies a move synchronously with no view side-effects. Use for simulation only.</summary>
        void SimulateMove(Move move);

        UniTask ApplyMove(Move move);
        
        IGameBoard Clone();
    }
    
    public interface IGameBoardView
    {
        void Initialize(IGameBoard board);
        
        UniTask ShowMove(Move move);
    }
}