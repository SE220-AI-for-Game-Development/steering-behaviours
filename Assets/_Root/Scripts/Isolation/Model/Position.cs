namespace Ai4Gamedev.MiniMax.Isolation
{
    public class Position
    {
        public int Column { get; set; }
        
        public int Row { get; set; }

        public override string ToString()
        {
            return $"{'A' + Column}{Row + 1}";
        }
    }
}