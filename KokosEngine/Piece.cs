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
        WhitePawn = 0, WhiteKnight = 1, WhiteBishop = 2, WhiteRook = 3, WhiteQueen = 4, WhiteKing = 5, BlackPawn = 6, BlackKnight = 7, BlackBishop = 8, BlackRook = 9, BlackQueen = 10, BlackKing = 11, None = 12
    }

    internal static class PieceMethods
    {
        internal static string ToNotation(this Piece piece)
        {
            return new string(new char[] { char.ToUpper(piece.ToFEN()) });
        }
        internal static char ToFEN(this Piece piece)
        {
            switch (piece)
            {
                case WhitePawn: return 'P';
                case WhiteKnight: return 'N';
                case WhiteBishop: return 'B';
                case WhiteRook: return 'R';
                case WhiteQueen: return 'Q';
                case WhiteKing: return 'K';
                case BlackPawn: return 'p';
                case BlackKnight: return 'n';
                case BlackBishop: return 'b';
                case BlackRook: return 'r';
                case BlackQueen: return 'q';
                case BlackKing: return 'k';
                default: return '.';
            }
        }
        internal static Piece FromFEN(char c)
        {
            switch (c)
            {
                case 'P': return WhitePawn;
                case 'N': return WhiteKnight;
                case 'B': return WhiteBishop;
                case 'R': return WhiteRook;
                case 'Q': return WhiteQueen;
                case 'K': return WhiteKing;
                case 'p': return BlackPawn;
                case 'n': return BlackKnight;
                case 'b': return BlackBishop;
                case 'r': return BlackRook;
                case 'q': return BlackQueen;
                case 'k': return BlackKing;
                default: return None;
            }
        }
        internal static bool IsWhite(this Piece piece)
        {
            return (int)piece < 6;
        }
        internal static bool IsBlack(this Piece piece)
        {
            return ((int)piece >= 6) && piece != Piece.None;
        }
    }
}
