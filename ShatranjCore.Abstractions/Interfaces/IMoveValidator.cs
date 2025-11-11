namespace ShatranjCore.Abstractions.Interfaces
{
    /// <summary>
    /// Interface for validating chess moves
    /// </summary>
    public interface IMoveValidator
    {
        bool IsValid(Location from, Location to, PieceColor currentPlayer);
        ValidationResult ValidateMove(Location from, Location to, PieceColor currentPlayer);
    }

    /// <summary>
    /// Result of move validation
    /// </summary>
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; }
    }
}
