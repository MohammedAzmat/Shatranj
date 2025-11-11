using System.Collections.Generic;

namespace ShatranjCore.Abstractions.Interfaces
{
    /// <summary>
    /// Interface for executing chess moves
    /// </summary>
    public interface IMoveExecutor
    {
        void ExecuteMove(Location from, Location to);
        List<object> GetCapturedPieces();
    }
}
