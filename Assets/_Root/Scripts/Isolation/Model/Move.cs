namespace Ai4Gamedev.MiniMax.Isolation
{
    public class Move
    {
        public int PlayerId { get; set; }

        public Position DestinationPosition { get; set; }

        public Position BlockPosition { get; set; }
    }
}