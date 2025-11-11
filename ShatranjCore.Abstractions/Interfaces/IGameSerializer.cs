namespace ShatranjCore.Abstractions.Interfaces
{
    /// <summary>
    /// Interface for serializing/deserializing game state
    /// </summary>
    public interface IGameSerializer
    {
        string Serialize(object snapshot);
        object Deserialize(string json);
    }
}
