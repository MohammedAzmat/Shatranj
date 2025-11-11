namespace ShatranjCore.Abstractions.Interfaces
{
    /// <summary>
    /// Interface for evaluating chess positions
    /// </summary>
    public interface IMoveEvaluator
    {
        double Evaluate(IBoardState board, PieceColor color);
    }
}
