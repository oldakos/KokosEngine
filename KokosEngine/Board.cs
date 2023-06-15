using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static KokosEngine.Piece;

namespace KokosEngine
{
    internal class Board
    {
        bool isWhiteToMove;

        int halfmovesReversible; //halfmoves played since last capture or pawn move
        int fullmovesTotal; //increments after black moves

        ulong enPassant; //bitboard with 0 or 1 squares where en passant is possible

        ulong whitePawns;
        ulong whiteKnights;
        ulong whiteBishops;
        ulong whiteRooks;
        ulong whiteQueens;
        ulong whiteKing;
        bool whiteCastleShort;
        bool whiteCastleLong;

        ulong blackPawns;
        ulong blackKnights;
        ulong blackBishops;
        ulong blackRooks;
        ulong blackQueens;
        ulong blackKing;
        bool blackCastleShort;
        bool blackCastleLong;

        Piece[] mailbox;

        internal Board()
        {
            mailbox = new Piece[64];
        }

        internal void SetStartingPosition()
        {
            isWhiteToMove = true;

            halfmovesReversible = 0;
            fullmovesTotal = 0;

            enPassant = 0x0000000000000000;

            whitePawns = 0x000000000000FF00;
            whiteKnights = 0x0000000000000042;
            whiteBishops = 0x0000000000000024;
            whiteRooks = 0x0000000000000081;
            whiteQueens = 0x0000000000000010;
            whiteKing = 0x0000000000000008;
            whiteCastleShort = true;
            whiteCastleLong = true;

            blackPawns = 0x00FF000000000000;
            blackKnights = 0x4200000000000000;
            blackBishops = 0x2400000000000000;
            blackRooks = 0x8100000000000000;
            blackQueens = 0x1000000000000000;
            blackKing = 0x0800000000000000;
            blackCastleShort = true;
            blackCastleLong = true;

            mailbox = new Piece[]
            {
                WR, WN, WB, WQ, WK, WB, WN, WR,
                WP, WP, WP, WP, WP, WP, WP, WP,
                X, X, X, X, X, X, X, X,
                X, X, X, X, X, X, X, X,
                X, X, X, X, X, X, X, X,
                X, X, X, X, X, X, X, X,
                BP, BP, BP, BP, BP, BP, BP, BP,
                BR, BN, BB, BQ, BK, BB, BN, BR
            };
        }

        internal string AsFEN()
        {
            StringBuilder sb = new StringBuilder();

            //section: piece placement

            int emptyCounter = 0;
            Piece p;
            for (int rank = 7; rank >= 0; rank--)
            {
                for (int file = 0; file < 8; file++)
                {
                    p = mailbox[8 * rank + file];
                    if (p == X)
                    {
                        emptyCounter++;
                    }
                    else //there is a piece
                    {
                        if (emptyCounter > 0)
                        {
                            sb.Append(emptyCounter.ToString());
                            emptyCounter = 0;
                        }
                        sb.Append(PieceToFEN(p));
                    }
                }
                //end of file
                if (emptyCounter > 0)
                {
                    sb.Append(emptyCounter.ToString());
                    emptyCounter = 0;
                }
                if (rank > 0)
                {
                    sb.Append('/');
                }
            }

            sb.Append(' '); //section: active color

            if (isWhiteToMove) sb.Append('w');
            else sb.Append('b');

            sb.Append(' '); //section: castling

            if (!(whiteCastleShort || whiteCastleLong || blackCastleShort || blackCastleLong))
            {
                sb.Append('-');
            }
            else
            {
                if (whiteCastleShort) sb.Append('K');
                if (whiteCastleLong) sb.Append('Q');
                if (blackCastleShort) sb.Append('k');
                if (blackCastleLong) sb.Append('q');
            }

            sb.Append(' '); //section: en passant

            sb.Append(LSB1ToAlgebraic(enPassant));

            sb.Append(' '); //section: halfmove clock

            sb.Append(halfmovesReversible.ToString());

            sb.Append(' '); //section: fullmove number

            sb.Append(fullmovesTotal.ToString());

            return sb.ToString();
        }

        readonly int[] index64 = {
             0,  1, 48,  2, 57, 49, 28,  3,
            61, 58, 50, 42, 38, 29, 17,  4,
            62, 55, 59, 36, 53, 51, 43, 22,
            45, 39, 33, 30, 24, 18, 12,  5,
            63, 47, 56, 27, 60, 41, 37, 16,
            54, 35, 52, 21, 44, 32, 23, 11,
            46, 26, 40, 15, 34, 20, 31, 10,
            25, 14, 19,  9, 13,  8,  7,  6
        };
        int bitScanForward(ulong bb)
        {
            if (bb == 0) throw new ArgumentException("you bitscanned a zero bitboard");
            const ulong debruijn64 = 0x03f79d71b4cb0a89;
            ulong negative = (ulong)(-(long)bb);
            return index64[((bb & negative) * debruijn64) >> 58];
        }

        /// <summary>
        /// Return algebraic notation of the LSB1 square in given bitboard.
        /// </summary>
        /// <returns>Algebraic notation of a square, or "-" if bitboard is empty.</returns>
        string LSB1ToAlgebraic(ulong bitboard)
        {
            if (bitboard == 0)
            {
                return "-";
            }
            int index = bitScanForward(bitboard);
            return IndexToAlgebraic(index);
        }

        string IndexToAlgebraic(int index)
        {
            if (index < 0 || index > 63) throw new ArgumentException("you used an index outside 0-63");
            int rankindex = index / 8;
            int fileindex = index % 8;
            char rank = (char)('1' + rankindex);
            char file = (char)('a' + fileindex);
            return new string(new char[] { file, rank });
        }

        char PieceToFEN(Piece piece)
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



    enum Piece
    {
        X, WP, WN, WB, WR, WQ, WK, BP, BN, BB, BR, BQ, BK
    }

}
