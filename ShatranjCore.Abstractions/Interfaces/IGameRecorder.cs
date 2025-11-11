namespace ShatranjCore.Abstractions.Interfaces
{
    /// <summary>
    /// Interface for recording game moves for learning/analysis
    /// </summary>
    public interface IGameRecorder
    {
        void StartNewGame(GameMode mode, string whitePlayerType, string blackPlayerType);
        void RecordMove(PieceColor player, Location from, Location to, string pieceName,
                        string notation, bool wasCapture, bool causedCheck, bool causedCheckmate,
                        double evaluation = 0, long thinkingTimeMs = 0);
        void EndGame(string winner, string endReason);
        void SaveGame(string fileName);
    }
}
