using System;

namespace ShatranjCore.Abstractions.Interfaces
{
    /// <summary>
    /// Interface for handling pawn promotion UI
    /// </summary>
    public interface IPromotionUI
    {
        Type PromptForPromotion(PieceColor color);
    }
}
