namespace Ai4Gamedev.MiniMax.Isolation
{
    public class Cell
    {
        public CellState State { get; private set; }
        
        public int? PlayerId { get; private set; }

        public void Ruin()
        {
            State = CellState.Ruined;
        }
        
        public void Occupy(int playerId)
        {
            State = CellState.Occupied;
            PlayerId = playerId;
        }
        
        public void Free()
        {
            State = CellState.Free;
            PlayerId = null;
        }
    }

    public enum CellState
    {
        Free, Occupied, Ruined
    }
}