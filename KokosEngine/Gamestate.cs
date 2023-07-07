using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static KokosEngine.Piece;

namespace KokosEngine
{
    internal class Gamestate
    {
        //TODO: make/unmake doesn't update the mailbox

        #region Basic Data

        internal int PlayerToMove; //white=1 black=-1
        #region Player To Move Bool Properties
        internal bool WhiteMove
        {
            get { return PlayerToMove == 1; }
            set { PlayerToMove = value ? 1 : -1; }
        }
        internal bool BlackMove
        {
            get { return !WhiteMove; }
            set { WhiteMove = !value; }
        }
        #endregion

        internal int HalfmovesReversible; //halfmoves played since last capture or pawn move
        internal int CurrentFullmove; //increments after black moves, starts at 1

        internal ulong PossibleEPSquareBB; //bitboard with 0 or 1 squares where en passant is possible

        internal ulong[] Bitboards;
        #region Individual Bitboard Properties
        internal ulong WhitePawns
        {
            get { return Bitboards[0]; }
            set { Bitboards[0] = value; }
        }
        internal ulong WhiteKnights
        {
            get { return Bitboards[1]; }
            set { Bitboards[1] = value; }
        }
        internal ulong WhiteBishops
        {
            get { return Bitboards[2]; }
            set { Bitboards[2] = value; }
        }
        internal ulong WhiteRooks
        {
            get { return Bitboards[3]; }
            set { Bitboards[3] = value; }
        }
        internal ulong WhiteQueens
        {
            get { return Bitboards[4]; }
            set { Bitboards[4] = value; }
        }
        internal ulong WhiteKing
        {
            get { return Bitboards[5]; }
            set { Bitboards[5] = value; }
        }
        internal ulong BlackPawns
        {
            get { return Bitboards[6]; }
            set { Bitboards[6] = value; }
        }
        internal ulong BlackKnights
        {
            get { return Bitboards[7]; }
            set { Bitboards[7] = value; }
        }
        internal ulong BlackBishops
        {
            get { return Bitboards[8]; }
            set { Bitboards[8] = value; }
        }
        internal ulong BlackRooks
        {
            get { return Bitboards[9]; }
            set { Bitboards[9] = value; }
        }
        internal ulong BlackQueens
        {
            get { return Bitboards[10]; }
            set { Bitboards[10] = value; }
        }
        internal ulong BlackKing
        {
            get { return Bitboards[11]; }
            set { Bitboards[11] = value; }
        }
        #endregion
        #region Helper Bitboard Properties
        internal ulong BB_Occupied { get; set; }
        internal ulong BB_Empty { get; set; }
        internal ulong BB_White { get; set; }
        internal ulong BB_Black { get; set; }
        #endregion

        internal int[] CastlingDisablingMoves; //number of move when each castling right has been lost
        #region Castling Rights Properties
        internal bool CanCastleWhiteShort
        {
            get { return CastlingDisablingMoves[0] == 0; }
            set
            {
                if (value == true) CastlingDisablingMoves[0] = 0;
                else CastlingDisablingMoves[0] = CurrentFullmove;
            }
        }
        internal bool CanCastleWhiteLong
        {
            get { return CastlingDisablingMoves[1] == 0; }
            set
            {
                if (value == true) CastlingDisablingMoves[1] = 0;
                else CastlingDisablingMoves[1] = CurrentFullmove;
            }
        }
        internal bool CanCastleBlackShort
        {
            get { return CastlingDisablingMoves[2] == 0; }
            set
            {
                if (value == true) CastlingDisablingMoves[2] = 0;
                else CastlingDisablingMoves[2] = CurrentFullmove;
            }
        }
        internal bool CanCastleBlackLong
        {
            get { return CastlingDisablingMoves[3] == 0; }
            set
            {
                if (value == true) CastlingDisablingMoves[3] = 0;
                else CastlingDisablingMoves[3] = CurrentFullmove;
            }
        }
        #endregion

        internal Piece[] Mailbox;

        internal Stack<MoveAndIrreversibleInfo> History;

        #endregion

        internal Gamestate()
        {
            Mailbox = new Piece[64];
            Bitboards = new ulong[12];
            CastlingDisablingMoves = new int[4];
            History = new Stack<MoveAndIrreversibleInfo>();
        }
        internal void MakeMove(Move move)
        {
            UpdatePiecesState(move);
            StoreHistory(move);
            UpdatePossibleEPSquare(move);
            AdjustReversibleHalfmoveCounter(move);
            DisableCastlingRights(move);
            IncrementFullmoveCounter();
            SwitchPlayerToMove();

            RefreshHelperBitboards();
        }
        internal void UnmakeMove()
        {
            UnmakeMove(History.Pop());
        }
        internal void UnmakeMove(MoveAndIrreversibleInfo info)
        {
            SwitchPlayerToMove();
            DecrementFullmoveCounter();
            RevertCastlingRights();
            HalfmovesReversible = info.Halfmoves;
            PossibleEPSquareBB = info.EnPassantBB;
            RevertPiecesState(info.Move);

            RefreshHelperBitboards();
        }

        #region Make/Unmake Move Helper Methods
        internal void IncrementFullmoveCounter()
        {
            if (BlackMove)
            {
                CurrentFullmove++;
            }
        }
        internal void AdjustReversibleHalfmoveCounter(Move move)
        {
            if (move.IsCapture || (move.PieceMoved == WP) || (move.PieceMoved == BP))
            {
                HalfmovesReversible = 0;
            }
            else
            {
                HalfmovesReversible++;
            }
        }
        internal void UpdatePiecesState(Move move)
        {
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
        }
        internal void RevertPiecesState(Move move)
        {
            switch (move.Type)
            {
                case MoveType.KingCastle:
                    UnmakeKingCastle();
                    break;
                case MoveType.QueenCastle:
                    UnmakeQueenCastle();
                    break;
                default:
                    ClearSquare(move.SquareTo);
                    if (move.IsCapture) ResetCapturedPiece(move);
                    ResetSquareFrom(move);
                    break;
            }
        }
        internal void DecrementFullmoveCounter()
        {
            if (BlackMove)
            {
                CurrentFullmove--;
            }
        }
        internal void StoreHistory(Move move)
        {
            History.Push(new MoveAndIrreversibleInfo(move, PossibleEPSquareBB, HalfmovesReversible));
        }
        internal void DisableCastlingRights(Move move)
        {
            if (CanCastleWhiteShort)
            {
                if (move.PieceMoved == WK || (move.PieceMoved == WR && move.SquareFrom == 7)) CanCastleWhiteShort = false;
            }
            if (CanCastleWhiteLong)
            {
                if (move.PieceMoved == WK || (move.PieceMoved == WR && move.SquareFrom == 0)) CanCastleWhiteLong = false;
            }
            if (CanCastleBlackShort)
            {
                if (move.PieceMoved == BK || (move.PieceMoved == BR && move.SquareFrom == 63)) CanCastleBlackShort = false;
            }
            if (CanCastleBlackLong)
            {
                if (move.PieceMoved == BK || (move.PieceMoved == BR && move.SquareFrom == 56)) CanCastleBlackLong = false;
            }
        }
        internal void RevertCastlingRights()
        {
            if (WhiteMove && CurrentFullmove == CastlingDisablingMoves[0]) CanCastleWhiteShort = true;
            if (WhiteMove && CurrentFullmove == CastlingDisablingMoves[1]) CanCastleWhiteLong = true;
            if (BlackMove && CurrentFullmove == CastlingDisablingMoves[2]) CanCastleBlackShort = true;
            if (BlackMove && CurrentFullmove == CastlingDisablingMoves[3]) CanCastleBlackLong = true;
        }
        internal void UpdatePossibleEPSquare(Move move)
        {
            ClearPossibleEPSquare();
            if (move.Type != MoveType.DoublePawnPush) return;

            int offset = -8 * PlayerToMove;
            int index = move.SquareTo + offset;
            PossibleEPSquareBB = (ulong)1 << index;
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
        internal void UnmakeKingCastle()
        {
            if (WhiteMove)
            {
                ClearSquare(6);
                SetPieceOnSquare(WK, 4);
                ClearSquare(5);
                SetPieceOnSquare(WR, 7);
            }
            else
            {
                ClearSquare(62);
                SetPieceOnSquare(WK, 60);
                ClearSquare(61);
                SetPieceOnSquare(WR, 63);
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
        internal void UnmakeQueenCastle()
        {
            if (WhiteMove)
            {
                ClearSquare(2);
                SetPieceOnSquare(WK, 4);
                ClearSquare(3);
                SetPieceOnSquare(WR, 0);
            }
            else
            {
                ClearSquare(58);
                SetPieceOnSquare(WK, 60);
                ClearSquare(59);
                SetPieceOnSquare(WR, 56);
            }
        }
        internal void ClearPossibleEPSquare()
        {
            PossibleEPSquareBB = 0;
        }
        internal void SwitchPlayerToMove()
        {
            PlayerToMove = -PlayerToMove;
        }
        internal void SetSquareTo(Move move)
        {
            if (move.IsPromotion)
            {
                SetPieceOnSquare(move.PiecePromotedTo, move.SquareTo);
            }
            else
            {
                SetPieceOnSquare(move.PieceMoved, move.SquareTo);
            }
        }
        internal void ResetSquareFrom(Move move)
        {
            SetPieceOnSquare(move.PieceMoved, move.SquareFrom);
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
        internal void ResetCapturedPiece(Move move)
        {
            if (move.Type == MoveType.EnPassant)
            {
                int offset = -8 * PlayerToMove;
                SetPieceOnSquare(move.PieceCaptured, move.SquareTo + offset);
            }
            else
            {
                SetPieceOnSquare(move.PieceCaptured, move.SquareTo);
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
        internal void RefreshHelperBitboards()
        {
            BB_White = 0;
            for (int i = 0; i < 6; i++)
            {
                BB_White |= Bitboards[i];
            }
            BB_Black = 0;
            for (int i = 6; i < 12; i++)
            {
                BB_Black |= Bitboards[i];
            }
            BB_Occupied = BB_White | BB_Black;
            BB_Empty = ~BB_Occupied;
        }
        #endregion

        #region Move Generation



        #endregion

        internal void SetStartingPosition()
        {
            WhiteMove = true;

            HalfmovesReversible = 0;
            CurrentFullmove = 1;

            PossibleEPSquareBB = 0x0000000000000000;

            WhitePawns = 0x000000000000FF00;
            WhiteKnights = 0x0000000000000042;
            WhiteBishops = 0x0000000000000024;
            WhiteRooks = 0x0000000000000081;
            WhiteQueens = 0x0000000000000010;
            WhiteKing = 0x0000000000000008;
            CanCastleWhiteShort = true;
            CanCastleWhiteLong = true;

            BlackPawns = 0x00FF000000000000;
            BlackKnights = 0x4200000000000000;
            BlackBishops = 0x2400000000000000;
            BlackRooks = 0x8100000000000000;
            BlackQueens = 0x1000000000000000;
            BlackKing = 0x0800000000000000;
            CanCastleBlackShort = true;
            CanCastleBlackLong = true;

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

            if (!(CanCastleWhiteShort || CanCastleWhiteLong || CanCastleBlackShort || CanCastleBlackLong))
            {
                sb.Append('-');
            }
            else
            {
                if (CanCastleWhiteShort) sb.Append('K');
                if (CanCastleWhiteLong) sb.Append('Q');
                if (CanCastleBlackShort) sb.Append('k');
                if (CanCastleBlackLong) sb.Append('q');
            }

            sb.Append(' '); //section: en passant

            sb.Append(Util.LSB1ToAlgebraic(PossibleEPSquareBB));

            sb.Append(' '); //section: halfmove clock

            sb.Append(HalfmovesReversible.ToString());

            sb.Append(' '); //section: fullmove number

            sb.Append(CurrentFullmove.ToString());

            return sb.ToString();
        }
        internal string PrintMailbox()
        {
            StringBuilder sb = new StringBuilder();
            for (int rank = 7; rank >= 0; rank--)
            {
                for (int file = 0; file <= 7; file++)
                {
                    sb.Append(Mailbox[8 * rank + file].ToFEN());
                }
                if (rank > 0) sb.Append('\n');
            }
            return sb.ToString();
        }
    }
}