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

        ulong possibleEPSquare; //bitboard with 0 or 1 squares where en passant is possible

        ulong[] bitboards;
        #region Bitboard Properties
        ulong WhitePawns
        {
            get { return bitboards[0]; }
            set { bitboards[0] = value; }
        }
        ulong WhiteKnights
        {
            get { return bitboards[1]; }
            set { bitboards[1] = value; }
        }
        ulong WhiteBishops
        {
            get { return bitboards[2]; }
            set { bitboards[2] = value; }
        }
        ulong WhiteRooks
        {
            get { return bitboards[3]; }
            set { bitboards[3] = value; }
        }
        ulong WhiteQueens
        {
            get { return bitboards[4]; }
            set { bitboards[4] = value; }
        }
        ulong WhiteKing
        {
            get { return bitboards[5]; }
            set { bitboards[5] = value; }
        }
        ulong BlackPawns
        {
            get { return bitboards[6]; }
            set { bitboards[6] = value; }
        }
        ulong BlackKnights
        {
            get { return bitboards[7]; }
            set { bitboards[7] = value; }
        }
        ulong BlackBishops
        {
            get { return bitboards[8]; }
            set { bitboards[8] = value; }
        }
        ulong BlackRooks
        {
            get { return bitboards[9]; }
            set { bitboards[9] = value; }
        }
        ulong BlackQueens
        {
            get { return bitboards[10]; }
            set { bitboards[10] = value; }
        }
        ulong BlackKing
        {
            get { return bitboards[11]; }
            set { bitboards[11] = value; }
        }
        #endregion

        bool CastleRightWhiteShort;
        bool CastleRightWhiteLong;
        bool CastleRightBlackShort;
        bool CastleRightBlackLong;

        Piece[] mailbox;
        internal Board()
        {
            mailbox = new Piece[64];
            bitboards = new ulong[12];
        }
        internal void MakeMove(Move move)
        {
            //todo castling rights, move count

            DisableEnPassant();

            switch (move.Type)
            {
                case MoveType.KingCastle:
                    MakeKingCastle();
                    break;
                case MoveType.QueenCastle:
                    MakeQueenCastle();
                    break;
                default:
                    ClearSquare(move.SquareFrom);
                    if (move.IsCapture) ClearCapturedPiece(move);
                    SetSquareTo(move);
                    break;
            }
            SwitchSideToMove();
        }
        internal void DisableEnPassant()
        {
            possibleEPSquare = 0;
        }
        internal void SwitchSideToMove()
        {
            isWhiteToMove = !isWhiteToMove;
        }
        internal void SetSquareTo(Move move)
        {
            //todo figure out a way to quickly get bitboard from IsWhiteToMove and move.piece

            if (!move.IsPromotion)
            {

            }
            else
            {
                MoveType t = move.Type;
                if (t == MoveType.PromoQueen || t == MoveType.PromoCaptureQueen)
                {

                }
                if (t == MoveType.PromoKnight || t == MoveType.PromoCaptureKnight)
                {

                }
                if (t == MoveType.PromoBishop || t == MoveType.PromoCaptureBishop)
                {

                }
                if (t == MoveType.PromoRook || t == MoveType.PromoCaptureRook)
                {

                }
            }
        }
        internal void SetBit(ref ulong bitboard, int index)
        {
            ulong mask = (ulong)1 << index;
            bitboard |= mask;
        }
        internal void ClearCapturedPiece(Move move)
        {
            if (move.Type == MoveType.EnPassant)
            {
                int offset;
                if (isWhiteToMove) offset = -8;
                else offset = 8;
                ClearSquare(move.SquareTo + offset);
            }
            else
            {
                ClearSquare(move.SquareTo);
            }
        }
        internal void ClearSquare(int square)
        {
            mailbox[square] = X;
            ClearBitInAllBitboards(square);
        }
        internal void ClearBitInAllBitboards(int square)
        {
            ulong mask = ~((ulong)1 << square);
            for (int i = 0; i < bitboards.Length; i++)
            {
                bitboards[i] &= mask;
            }
        }
        internal void SetStartingPosition()
        {
            isWhiteToMove = true;

            halfmovesReversible = 0;
            fullmovesTotal = 0;

            possibleEPSquare = 0x0000000000000000;

            WhitePawns = 0x000000000000FF00;
            WhiteKnights = 0x0000000000000042;
            WhiteBishops = 0x0000000000000024;
            WhiteRooks = 0x0000000000000081;
            WhiteQueens = 0x0000000000000010;
            WhiteKing = 0x0000000000000008;
            CastleRightWhiteShort = true;
            CastleRightWhiteLong = true;

            BlackPawns = 0x00FF000000000000;
            BlackKnights = 0x4200000000000000;
            BlackBishops = 0x2400000000000000;
            BlackRooks = 0x8100000000000000;
            BlackQueens = 0x1000000000000000;
            BlackKing = 0x0800000000000000;
            CastleRightBlackShort = true;
            CastleRightBlackLong = true;

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

            if (!(CastleRightWhiteShort || CastleRightWhiteLong || CastleRightBlackShort || CastleRightBlackLong))
            {
                sb.Append('-');
            }
            else
            {
                if (CastleRightWhiteShort) sb.Append('K');
                if (CastleRightWhiteLong) sb.Append('Q');
                if (CastleRightBlackShort) sb.Append('k');
                if (CastleRightBlackLong) sb.Append('q');
            }

            sb.Append(' '); //section: en passant

            sb.Append(BBUtil.LSB1ToAlgebraic(possibleEPSquare));

            sb.Append(' '); //section: halfmove clock

            sb.Append(halfmovesReversible.ToString());

            sb.Append(' '); //section: fullmove number

            sb.Append(fullmovesTotal.ToString());

            return sb.ToString();
        }
    }
}