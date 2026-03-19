namespace Ai4Gamedev.MiniMax.Isolation
{
    public interface IEvaluationFunction
    {
        int Evaluate(IGameBoard board);
    }
}