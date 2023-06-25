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
        internal void MakeMove(Move move)
        {
            ClearSquareFrom(move);
            ClearEnPassantSquare();

            switch (move.Type)
            {
                case MoveType.Quiet:
                    SetSquareTo(move);
                    break;
                case MoveType.DoublePawnPush:
                    SetSquareTo(move);
                    SetEnPassantSquare(move);
                    break;
                case MoveType.KingCastle:
                    MakeKingCastle();
                    break;
                case MoveType.QueenCastle:
                    MakeQueenCastle();
                    break;
                case MoveType.Capture:
                    ClearSquareTo(move);
                    SetSquareTo(move);
                    break;
                case MoveType.EnPassant:
                    SetSquareTo(move);
                    ClearEPdPawn(move);
                    break;
                case MoveType.PromoKnight:
                    break;
                case MoveType.PromoBishop:
                    break;
                case MoveType.PromoRook:
                    break;
                case MoveType.PromoQueen:
                    break;
                case MoveType.PromoCaptureKnight:
                    break;
                case MoveType.PromoCaptureBishop:
                    break;
                case MoveType.PromoCaptureRook:
                    break;
                case MoveType.PromoCaptureQueen:
                    break;
                default:
                    break;
            }
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
                        sb.Append(p.ToFEN());
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

            sb.Append(BBUtil.LSB1ToAlgebraic(enPassant));

            sb.Append(' '); //section: halfmove clock

            sb.Append(halfmovesReversible.ToString());

            sb.Append(' '); //section: fullmove number

            sb.Append(fullmovesTotal.ToString());

            return sb.ToString();
        }
    }
}