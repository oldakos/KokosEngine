using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static KokosEngine.Piece;

namespace KokosEngine
{
    internal enum Piece
    {
        WP = 0, WN = 1, WB = 2, WR = 3, WQ = 4, WK = 5, BP = 6, BN = 7, BB = 8, BR = 9, BQ = 10, BK = 11, X = 12
    }

    internal static class PieceMethods
    {
        internal static char ToFEN(this Piece piece)
        {
            switch (piece)
            {
                case WP: return 'P';
                case WN: return 'N';
                case WB: return 'B';
                case WR: return 'R';
                case WQ: return 'Q';
                case WK: return 'K';
                case BP: return 'p';
                case BN: return 'n';
                case BB: return 'b';
                case BR: return 'r';
                case BQ: return 'q';
                case BK: return 'k';
                default: return '.';
            }
        }
        internal static bool IsWhite(this Piece piece)
        {
            return (int)piece < 6;
        }
        internal static bool IsBlack(this Piece piece)
        {
            return ((int)piece >= 6) && piece != Piece.X;
        }
    }
}
