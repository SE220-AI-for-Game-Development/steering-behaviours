namespace Ai4Gamedev.MiniMax.Isolation
{
    public interface IGameBoard
    {
        Cell[,] Cells { get; }
        
        bool IsValidMove(Move move);
        
        void ApplyMove(Move move);
        
        IGameBoard Clone();
    }
}