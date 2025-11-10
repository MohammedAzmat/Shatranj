// This file now serves as a facade/forwarding layer for backward compatibility
// All types have been moved to ShatranjCore.Abstractions

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Type aliases for backward compatibility
using PieceColor = ShatranjCore.Abstractions.PieceColor;
using PawnMoves = ShatranjCore.Abstractions.PawnMoves;
using Location = ShatranjCore.Abstractions.Location;
using PlayerType = ShatranjCore.Abstractions.PlayerType;
using GameMode = ShatranjCore.Abstractions.GameMode;

namespace ShatranjCore
{
    // Types are now defined in ShatranjCore.Abstractions.CoreTypes
    // This file maintains backward compatibility through type aliases above
}
