// This file now forwards to ShatranjCore.Abstractions
// All types have been moved to the abstractions layer

using System;
using ShatranjCore.Abstractions;

// Type aliases for backward compatibility within ShatranjAI namespace
using AIMove = ShatranjCore.Abstractions.AIMove;
using IChessAI = ShatranjCore.Abstractions.IChessAI;

namespace ShatranjAI.AI
{
    // IChessAI and AIMove are now defined in ShatranjCore.Abstractions
    // This file maintains backward compatibility through type aliases above
}
