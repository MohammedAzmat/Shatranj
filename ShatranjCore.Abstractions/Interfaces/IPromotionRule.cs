namespace ShatranjCore.Abstractions.Interfaces
{
    /// <summary>
    /// Interface for pawn promotion rules
    /// </summary>
    public interface IPromotionRule
    {
        bool NeedsPromotion(object piece, Location location);
    }
}
