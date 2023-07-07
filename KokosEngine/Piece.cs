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
        internal static Piece FromFEN(char c)
        {
            switch (c)
            {
                case 'P': return WP;
                case 'N': return WN;
                case 'B': return WB;
                case 'R': return WR;
                case 'Q': return WQ;
                case 'K': return WK;
                case 'p': return BP;
                case 'n': return BN;
                case 'b': return BB;
                case 'r': return BR;
                case 'q': return BQ;
                case 'k': return BK;
                default: return X;
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
