namespace Ai4Gamedev.MiniMax.Isolation
{
    public class Position
    {
        public int Column { get; set; }

        public int Row { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is not Position other)
            {
                return false;
            }

            return Column == other.Column && Row == other.Row;
        }

        public override int GetHashCode()
        {
            return Column * 31 + Row;
        }

        public override string ToString()
        {
            var colChar = (char)('A' + Column);

            return $"{colChar}{Row + 1}";
        }
    }
}
