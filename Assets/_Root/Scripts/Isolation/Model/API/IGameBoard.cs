namespace Ai4Gamedev.MiniMax.Isolation
{
    using Cysharp.Threading.Tasks;

    public interface IGameBoard
    {
        Cell[,] Cells { get; }
        
        bool IsValidMove(Move move);
        
        UniTask ApplyMove(Move move);
        
        IGameBoard Clone();
    }
    
    public interface IGameBoardView
    {
        void Initialize(IGameBoard board);
        
        UniTask ShowMove(Move move);
    }
}