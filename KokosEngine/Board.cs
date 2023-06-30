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
        int PlayerToMove; //white=1 black=-1
        bool WhiteMove
        {
            get { return PlayerToMove == 1; }
            set { PlayerToMove = value ? 1 : -1; }
        }

        int HalfmovesReversible; //halfmoves played since last capture or pawn move
        int FullmovesTotal; //increments after black moves

        ulong PossibleEPSquare; //bitboard with 0 or 1 squares where en passant is possible

        ulong[] Bitboards;
        #region Bitboard Properties
        ulong WhitePawns
        {
            get { return Bitboards[0]; }
            set { Bitboards[0] = value; }
        }
        ulong WhiteKnights
        {
            get { return Bitboards[1]; }
            set { Bitboards[1] = value; }
        }
        ulong WhiteBishops
        {
            get { return Bitboards[2]; }
            set { Bitboards[2] = value; }
        }
        ulong WhiteRooks
        {
            get { return Bitboards[3]; }
            set { Bitboards[3] = value; }
        }
        ulong WhiteQueens
        {
            get { return Bitboards[4]; }
            set { Bitboards[4] = value; }
        }
        ulong WhiteKing
        {
            get { return Bitboards[5]; }
            set { Bitboards[5] = value; }
        }
        ulong BlackPawns
        {
            get { return Bitboards[6]; }
            set { Bitboards[6] = value; }
        }
        ulong BlackKnights
        {
            get { return Bitboards[7]; }
            set { Bitboards[7] = value; }
        }
        ulong BlackBishops
        {
            get { return Bitboards[8]; }
            set { Bitboards[8] = value; }
        }
        ulong BlackRooks
        {
            get { return Bitboards[9]; }
            set { Bitboards[9] = value; }
        }
        ulong BlackQueens
        {
            get { return Bitboards[10]; }
            set { Bitboards[10] = value; }
        }
        ulong BlackKing
        {
            get { return Bitboards[11]; }
            set { Bitboards[11] = value; }
        }
        #endregion

        bool CastleRightWhiteShort;
        bool CastleRightWhiteLong;
        bool CastleRightBlackShort;
        bool CastleRightBlackLong;

        Piece[] Mailbox;

        internal Board()
        {
            Mailbox = new Piece[64];
            Bitboards = new ulong[12];
        }
        internal void MakeMove(Move move)
        {
            //todo move count

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
            SwitchPlayerToMove();
        }
        internal void MakeKingCastle()
        {
            if (WhiteMove)
            {
                ClearSquare(4);
                SetPieceOnSquare(WK, 6);
                ClearSquare(7);
                SetPieceOnSquare(WR, 5);
            }
            else
            {
                ClearSquare(60);
                SetPieceOnSquare(WK, 62);
                ClearSquare(63);
                SetPieceOnSquare(WR, 61);
            }
        }
        internal void MakeQueenCastle()
        {
            if (WhiteMove)
            {
                ClearSquare(4);
                SetPieceOnSquare(WK, 2);
                ClearSquare(0);
                SetPieceOnSquare(WR, 3);
            }
            else
            {
                ClearSquare(60);
                SetPieceOnSquare(WK, 58);
                ClearSquare(56);
                SetPieceOnSquare(WR, 59);
            }
        }
        internal void DisableEnPassant()
        {
            PossibleEPSquare = 0;
        }
        internal void SwitchPlayerToMove()
        {
            PlayerToMove = -PlayerToMove;
        }
        internal void SetSquareTo(Move move)
        {
            if (move.IsPromotion)
            {
                //decipher this i dare you
                int index = 4 + move.Special + (-3 * PlayerToMove);
                SetPieceOnSquare((Piece)index, move.SquareTo);
            }
            else
            {
                SetPieceOnSquare(move.PieceMoved, move.SquareTo);
            }
        }
        internal void SetPieceOnSquare(Piece piece, int index)
        {
            ulong mask = (ulong)1 << index;
            Bitboards[(int)piece] |= mask;
            Mailbox[index] = piece;
        }
        internal void ClearCapturedPiece(Move move)
        {
            if (move.Type == MoveType.EnPassant)
            {
                int offset = -8 * PlayerToMove;
                ClearSquare(move.SquareTo + offset);
            }
            else
            {
                ClearSquare(move.SquareTo);
            }
        }
        internal void ClearSquare(int square)
        {
            Mailbox[square] = X;
            ClearBitInAllBitboards(square);
        }
        internal void ClearBitInAllBitboards(int square)
        {
            ulong mask = ~((ulong)1 << square);
            for (int i = 0; i < Bitboards.Length; i++)
            {
                Bitboards[i] &= mask;
            }
        }
        internal void SetStartingPosition()
        {
            WhiteMove = true;

            HalfmovesReversible = 0;
            FullmovesTotal = 0;

            PossibleEPSquare = 0x0000000000000000;

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

            Mailbox = new Piece[]
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
                    p = Mailbox[8 * rank + file];
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

            if (WhiteMove) sb.Append('w');
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

            sb.Append(BBUtil.LSB1ToAlgebraic(PossibleEPSquare));

            sb.Append(' '); //section: halfmove clock

            sb.Append(HalfmovesReversible.ToString());

            sb.Append(' '); //section: fullmove number

            sb.Append(FullmovesTotal.ToString());

            return sb.ToString();
        }
    }
}