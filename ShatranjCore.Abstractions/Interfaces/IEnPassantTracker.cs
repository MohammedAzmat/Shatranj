namespace ShatranjCore.Abstractions.Interfaces
{
    /// <summary>
    /// Interface for tracking en passant opportunities
    /// </summary>
    public interface IEnPassantTracker
    {
        void RecordPawnDoubleMove(Location from, Location to);
        Location? GetEnPassantTarget();
        Location? GetEnPassantCaptureLocation();
        void NextTurn();
        void Reset();
    }
}
