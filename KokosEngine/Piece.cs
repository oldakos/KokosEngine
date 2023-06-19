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
        X, WP, WN, WB, WR, WQ, WK, BP, BN, BB, BR, BQ, BK
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
                default: return '#';
            }
        }
    }
}
