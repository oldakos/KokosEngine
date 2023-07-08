using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static KokosEngine.Piece;
using static KokosEngine.Util;

namespace KokosEngine
{
    internal class Gamestate
    {
        /// <summary>
        /// Is cleared & filled anew by every this.SerializeBitboard() call.
        /// </summary>
        private List<int> SerList = new List<int>(32);
        /// <summary>
        /// Clears & fills TmpIndexList by result of Util.SerializeBitboard()
        /// </summary>
        private void SerializeBitboard(ulong bb)
        {
            SerList.Clear();
            Util.SerializeBitboard(bb, SerList);
        }

        private List<Move> Moves = new List<Move>(128);


        #region Basic Data

        internal int PlayerToMove; //white=1 black=-1
        #region Player To Move Bool Properties
        internal bool IsWhiteMove
        {
            get { return PlayerToMove == 1; }
            set { PlayerToMove = value ? 1 : -1; }
        }
        internal bool IsBlackMove
        {
            get { return !IsWhiteMove; }
            set { IsWhiteMove = !value; }
        }
        #endregion

        internal int HalfmovesReversible; //halfmoves played since last capture or pawn move
        internal int CurrentFullmove; //increments after black moves, starts at 1

        internal ulong BB_PossibleEPSquare; //bitboard with 0 or 1 squares where en passant is possible
        internal int PossibleEPSquare_Index;

        internal ulong[] Bitboards;
        #region Individual Bitboard Properties
        internal ulong BB_WhitePawns
        {
            get { return Bitboards[0]; }
            set { Bitboards[0] = value; }
        }
        internal ulong BB_WhiteKnights
        {
            get { return Bitboards[1]; }
            set { Bitboards[1] = value; }
        }
        internal ulong BB_WhiteBishops
        {
            get { return Bitboards[2]; }
            set { Bitboards[2] = value; }
        }
        internal ulong BB_WhiteRooks
        {
            get { return Bitboards[3]; }
            set { Bitboards[3] = value; }
        }
        internal ulong BB_WhiteQueens
        {
            get { return Bitboards[4]; }
            set { Bitboards[4] = value; }
        }
        internal ulong BB_WhiteKing
        {
            get { return Bitboards[5]; }
            set { Bitboards[5] = value; }
        }
        internal ulong BB_BlackPawns
        {
            get { return Bitboards[6]; }
            set { Bitboards[6] = value; }
        }
        internal ulong BB_BlackKnights
        {
            get { return Bitboards[7]; }
            set { Bitboards[7] = value; }
        }
        internal ulong BB_BlackBishops
        {
            get { return Bitboards[8]; }
            set { Bitboards[8] = value; }
        }
        internal ulong BB_BlackRooks
        {
            get { return Bitboards[9]; }
            set { Bitboards[9] = value; }
        }
        internal ulong BB_BlackQueens
        {
            get { return Bitboards[10]; }
            set { Bitboards[10] = value; }
        }
        internal ulong BB_BlackKing
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

        internal ulong BB_AllWhiteAttacks { get; set; }
        internal ulong BB_AllBlackAttacks { get; set; }
        internal ulong BB_WhitePawnAttacks { get; set; }
        internal ulong BB_WhiteKnightAttacks { get; set; }
        internal ulong BB_WhiteBishopAttacks { get; set; }
        internal ulong BB_WhiteRookAttacks { get; set; }
        internal ulong BB_WhiteQueenAttacks { get; set; }
        internal ulong BB_WhiteKingAttacks { get; set; }
        internal ulong BB_BlackPawnAttacks { get; set; }
        internal ulong BB_BlackKnightAttacks { get; set; }
        internal ulong BB_BlackBishopAttacks { get; set; }
        internal ulong BB_BlackRookAttacks { get; set; }
        internal ulong BB_BlackQueenAttacks { get; set; }
        internal ulong BB_BlackKingAttacks { get; set; }


        #endregion

        internal int[] CastlingDisablingMoves; //number of move when each castling right has been lost
        #region Castling Rights Properties
        internal bool CanCastleWhiteShort
        {
            get { return CastlingDisablingMoves[0] == 0; }
            set
            {
                if (value == true) CastlingDisablingMoves[0] = 0;
                else CastlingDisablingMoves[0] = CurrentFullmove * PlayerToMove;
            }
        }
        internal bool CanCastleWhiteLong
        {
            get { return CastlingDisablingMoves[1] == 0; }
            set
            {
                if (value == true) CastlingDisablingMoves[1] = 0;
                else CastlingDisablingMoves[1] = CurrentFullmove * PlayerToMove;
            }
        }
        internal bool CanCastleBlackShort
        {
            get { return CastlingDisablingMoves[2] == 0; }
            set
            {
                if (value == true) CastlingDisablingMoves[2] = 0;
                else CastlingDisablingMoves[2] = CurrentFullmove * PlayerToMove;
            }
        }
        internal bool CanCastleBlackLong
        {
            get { return CastlingDisablingMoves[3] == 0; }
            set
            {
                if (value == true) CastlingDisablingMoves[3] = 0;
                else CastlingDisablingMoves[3] = CurrentFullmove * PlayerToMove;
            }
        }
        #endregion

        internal Piece[] Mailbox;

        internal Stack<MoveAndIrreversibleInfo> History;

        #endregion

        internal Gamestate()
        {
            Mailbox = new Piece[64];
            for (int i = 0; i < Mailbox.Length; i++)
            {
                Mailbox[i] = Piece.X;
            }
            Bitboards = new ulong[12];
            CastlingDisablingMoves = new int[4];
            for (int i = 0; i < CastlingDisablingMoves.Length; i++)
            {
                CastlingDisablingMoves[i] = int.MinValue;
            }
            History = new Stack<MoveAndIrreversibleInfo>();
        }
        internal Gamestate LoadFEN(string fen)
        {
            int i = 0;
            //section: piece placement
            Queue<Piece> queue = new Queue<Piece>();
            while (fen[i] != ' ')
            {
                if (char.IsDigit(fen[i]))
                {
                    for (int s = 0; s < int.Parse(fen[i].ToString()); s++)
                    {
                        queue.Enqueue(Piece.X);
                    }
                }
                else if (fen[i] != '/')
                {
                    queue.Enqueue(PieceMethods.FromFEN(fen[i]));
                }
                i++;
            }
            for (int rank = 7; rank >= 0; rank--)
            {
                for (int file = 0; file < 8; file++)
                {
                    Piece p = queue.Dequeue();
                    if (p != Piece.X)
                    {
                        SetPieceOnSquare(p, 8 * rank + file);
                    }
                }
            }

            i++; //section: active color

            IsWhiteMove = (fen[i] == 'w');

            i += 2; //section: castling

            while (fen[i] != ' ')
            {
                if (fen[i] == 'K') CanCastleWhiteShort = true;
                if (fen[i] == 'Q') CanCastleWhiteLong = true;
                if (fen[i] == 'k') CanCastleBlackShort = true;
                if (fen[i] == 'q') CanCastleBlackLong = true;

                i++;
            }

            i++; //section: en passant

            if (fen[i] == '-')
            {
                PossibleEPSquare_Index = -1;
                BB_PossibleEPSquare = 0;
                i += 2;
            }
            else
            {
                string algEP = new string(new char[] { fen[i], fen[i + 1] });
                int index = AlgebraicToIndex(algEP);
                BB_PossibleEPSquare = (ulong)1 << index;
                PossibleEPSquare_Index = index;

                i += 3;
            }

            //section: halfmove clock

            StringBuilder sb1 = new StringBuilder();
            while (fen[i] != ' ')
            {
                sb1.Append(fen[i]);
                i++;
            }
            HalfmovesReversible = int.Parse(sb1.ToString());

            i++; //section: fullmove number

            StringBuilder sb2 = new StringBuilder();
            while (i < fen.Length && char.IsDigit(fen[i]))
            {
                sb2.Append(fen[i]);
                i++;
            }
            CurrentFullmove = int.Parse(sb2.ToString());

            RefreshAllHelperBitboards();

            return this;
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

            RefreshAllHelperBitboards();
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
            BB_PossibleEPSquare = info.EnPassantBB;
            PossibleEPSquare_Index = info.EnPassantIndex;
            RevertPiecesState(info.Move);

            RefreshAllHelperBitboards();
        }

        #region Make/Unmake Move Helper Methods

        internal void IncrementFullmoveCounter()
        {
            if (IsBlackMove)
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
            if (IsBlackMove)
            {
                CurrentFullmove--;
            }
        }
        internal void StoreHistory(Move move)
        {
            History.Push(new MoveAndIrreversibleInfo(move, PossibleEPSquare_Index, HalfmovesReversible));
        }
        internal void DisableCastlingRights(Move move)
        {
            if (CanCastleWhiteShort)
            {
                if (move.PieceMoved == WK || move.SquareFrom == 7 || move.SquareTo == 7) CanCastleWhiteShort = false;
            }
            if (CanCastleWhiteLong)
            {
                if (move.PieceMoved == WK || move.SquareFrom == 0 || move.SquareTo == 0) CanCastleWhiteLong = false;
            }
            if (CanCastleBlackShort)
            {
                if (move.PieceMoved == BK || move.SquareFrom == 63 || move.SquareTo == 63) CanCastleBlackShort = false;
            }
            if (CanCastleBlackLong)
            {
                if (move.PieceMoved == BK || move.SquareFrom == 56 || move.SquareTo == 56) CanCastleBlackLong = false;
            }
        }
        internal void RevertCastlingRights()
        {
            if (CurrentFullmove * PlayerToMove == CastlingDisablingMoves[0]) CanCastleWhiteShort = true;
            if (CurrentFullmove * PlayerToMove == CastlingDisablingMoves[1]) CanCastleWhiteLong = true;
            if (CurrentFullmove * PlayerToMove == CastlingDisablingMoves[2]) CanCastleBlackShort = true;
            if (CurrentFullmove * PlayerToMove == CastlingDisablingMoves[3]) CanCastleBlackLong = true;
        }
        internal void UpdatePossibleEPSquare(Move move)
        {
            ClearPossibleEPSquare();
            if (move.Type != MoveType.DoublePawnPush) return;

            int offset = -8 * PlayerToMove;
            int index = move.SquareTo + offset;
            BB_PossibleEPSquare = (ulong)1 << index;
            PossibleEPSquare_Index = index;
        }
        internal void MakeKingCastle()
        {
            if (IsWhiteMove)
            {
                ClearSquare(4);
                SetPieceOnSquare(WK, 6);
                ClearSquare(7);
                SetPieceOnSquare(WR, 5);
            }
            else
            {
                ClearSquare(60);
                SetPieceOnSquare(BK, 62);
                ClearSquare(63);
                SetPieceOnSquare(BR, 61);
            }
        }
        internal void UnmakeKingCastle()
        {
            if (IsWhiteMove)
            {
                ClearSquare(6);
                SetPieceOnSquare(WK, 4);
                ClearSquare(5);
                SetPieceOnSquare(WR, 7);
            }
            else
            {
                ClearSquare(62);
                SetPieceOnSquare(BK, 60);
                ClearSquare(61);
                SetPieceOnSquare(BR, 63);
            }
        }
        internal void MakeQueenCastle()
        {
            if (IsWhiteMove)
            {
                ClearSquare(4);
                SetPieceOnSquare(WK, 2);
                ClearSquare(0);
                SetPieceOnSquare(WR, 3);
            }
            else
            {
                ClearSquare(60);
                SetPieceOnSquare(BK, 58);
                ClearSquare(56);
                SetPieceOnSquare(BR, 59);
            }
        }
        internal void UnmakeQueenCastle()
        {
            if (IsWhiteMove)
            {
                ClearSquare(2);
                SetPieceOnSquare(WK, 4);
                ClearSquare(3);
                SetPieceOnSquare(WR, 0);
            }
            else
            {
                ClearSquare(58);
                SetPieceOnSquare(BK, 60);
                ClearSquare(59);
                SetPieceOnSquare(BR, 56);
            }
        }
        internal void ClearPossibleEPSquare()
        {
            BB_PossibleEPSquare = 0;
            PossibleEPSquare_Index = -1;
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
        internal void RefreshHelperBitboards(Move move)
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

            var pm = move.PieceMoved;
            var pc = move.PieceCaptured;
            var pp = move.PiecePromotedTo;

            if (pm == Piece.WN || pc == Piece.WN || pp == Piece.WN) BB_WhiteKnightAttacks = GetBB_WhiteKnightAttacks();
            if (pm == Piece.BN || pc == Piece.BN || pp == Piece.BN) BB_BlackKnightAttacks = GetBB_BlackKnightAttacks();
            if (pm == Piece.WP || pc == Piece.WP) BB_WhitePawnAttacks = GetBB_WhitePawnAttacks();
            if (pm == Piece.BP || pc == Piece.BP) BB_BlackPawnAttacks = GetBB_BlackPawnAttacks();
            if (pm == Piece.WK) BB_WhiteKingAttacks = GetBB_WhiteKingAttacks();
            if (pm == Piece.BK) BB_BlackKingAttacks = GetBB_BlackKingAttacks();
            BB_WhiteBishopAttacks = GetBB_WhiteBishopAttacks();
            BB_WhiteRookAttacks = GetBB_WhiteRookAttacks();
            BB_WhiteQueenAttacks = GetBB_WhiteQueenAttacks();
            BB_BlackBishopAttacks = GetBB_BlackBishopAttacks();
            BB_BlackRookAttacks = GetBB_BlackRookAttacks();
            BB_BlackQueenAttacks = GetBB_BlackQueenAttacks();
            BB_AllWhiteAttacks = GetBB_AllWhiteAttacks();
            BB_AllBlackAttacks = GetBB_AllBlackAttacks();
        }
        internal void RefreshAllHelperBitboards()
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

            BB_WhiteKnightAttacks = GetBB_WhiteKnightAttacks();
            BB_BlackKnightAttacks = GetBB_BlackKnightAttacks();
            BB_WhitePawnAttacks = GetBB_WhitePawnAttacks();
            BB_BlackPawnAttacks = GetBB_BlackPawnAttacks();
            BB_WhiteKingAttacks = GetBB_WhiteKingAttacks();
            BB_BlackKingAttacks = GetBB_BlackKingAttacks();
            BB_WhiteBishopAttacks = GetBB_WhiteBishopAttacks();
            BB_WhiteRookAttacks = GetBB_WhiteRookAttacks();
            BB_WhiteQueenAttacks = GetBB_WhiteQueenAttacks();
            BB_BlackBishopAttacks = GetBB_BlackBishopAttacks();
            BB_BlackRookAttacks = GetBB_BlackRookAttacks();
            BB_BlackQueenAttacks = GetBB_BlackQueenAttacks();
            BB_AllWhiteAttacks = GetBB_AllWhiteAttacks();
            BB_AllBlackAttacks = GetBB_AllBlackAttacks();
        }

        #endregion

        internal bool IsWhiteInCheck()
        {
            return (BB_AllBlackAttacks & BB_WhiteKing) != 0;
        }
        internal bool IsBlackInCheck()
        {
            var value = (BB_AllWhiteAttacks & BB_BlackKing) != 0;
            if (value)
            {
                return true;
            }
            return false;
        }
        internal bool CanKingBeCaptured()
        {
            if (IsWhiteMove && IsBlackInCheck())
            {
                return true;
            }
            if (IsBlackMove && IsWhiteInCheck())
            {
                return true;
            }
            return false;
        }

        internal List<Move> GetPseudoLegalMoves()
        {
            Moves.Clear();
            if (IsWhiteMove)
            {
                GetWhitePawnMoves();
                GetWhiteKnightMoves();
                GetWhiteBishopMoves();
                GetWhiteRookMoves();
                GetWhiteQueenMoves();
                GetWhiteKingMoves();
            }
            else
            {
                GetBlackPawnMoves();
                GetBlackKnightMoves();
                GetBlackBishopMoves();
                GetBlackRookMoves();
                GetBlackQueenMoves();
                GetBlackKingMoves();
            }
            return Moves;
        }

        #region Move Generation        

        internal ulong GetBB_AllWhiteAttacks()
        {
            return BB_WhitePawnAttacks | BB_WhiteKnightAttacks | BB_WhiteBishopAttacks | BB_WhiteRookAttacks | BB_WhiteQueenAttacks | BB_WhiteKingAttacks;
        }
        internal ulong GetBB_AllBlackAttacks()
        {
            return BB_BlackPawnAttacks | BB_BlackKnightAttacks | BB_BlackBishopAttacks | BB_BlackRookAttacks | BB_BlackQueenAttacks | BB_BlackKingAttacks;
        }


        internal ulong GetBB_WhitePawnAttacks()
        {
            ulong bb = Shift_NW(BB_WhitePawns);
            bb |= Shift_NE(BB_WhitePawns);
            return bb;
        }
        internal ulong GetBB_BlackPawnAttacks()
        {
            ulong bb = Shift_SW(BB_BlackPawns);
            bb |= Shift_SE(BB_BlackPawns);
            return bb;
        }
        internal ulong GetBB_WhiteKnightAttacks()
        {
            ulong result = 0;
            SerializeBitboard(BB_WhiteKnights);
            for (int i = 0; i < SerList.Count; i++)
            {
                result |= KnightMoves[SerList[i]];
            }
            return result;
        }
        internal ulong GetBB_BlackKnightAttacks()
        {
            ulong result = 0;
            SerializeBitboard(BB_BlackKnights);
            for (int i = 0; i < SerList.Count; i++)
            {
                result |= KnightMoves[SerList[i]];
            }
            return result;
        }
        internal ulong GetBB_WhiteBishopAttacks()
        {
            return AttackFill_Bishop(BB_WhiteBishops, BB_Empty);
        }
        internal ulong GetBB_BlackBishopAttacks()
        {
            return AttackFill_Bishop(BB_BlackBishops, BB_Empty);
        }
        internal ulong GetBB_WhiteRookAttacks()
        {
            return AttackFill_Rook(BB_WhiteRooks, BB_Empty);
        }
        internal ulong GetBB_BlackRookAttacks()
        {
            return AttackFill_Rook(BB_BlackRooks, BB_Empty);
        }
        internal ulong GetBB_WhiteQueenAttacks()
        {
            return AttackFill_Queen(BB_WhiteQueens, BB_Empty);
        }
        internal ulong GetBB_BlackQueenAttacks()
        {
            return AttackFill_Queen(BB_BlackQueens, BB_Empty);
        }
        internal ulong GetBB_WhiteKingAttacks()
        {
            int index = BitScanForward(BB_WhiteKing);
            return KingMoves[index];
        }
        internal ulong GetBB_BlackKingAttacks()
        {
            int index = BitScanForward(BB_BlackKing);
            return KingMoves[index];
        }

        internal void GetWhitePawnMoves()
        {
            GetWhitePawnAdvances();
            GetWhitePawnCaptures();
        }
        internal void GetBlackPawnMoves()
        {
            GetBlackPawnAdvances();
            GetBlackPawnCaptures();
        }
        internal void GetWhitePawnAdvances()
        {
            //double pushes
            ulong bb = BB_WhitePawns & BB_2ndRank;
            bb = Shift_N(bb) & BB_Empty;
            bb = Shift_N(bb) & BB_Empty;
            SerializeBitboard(bb);
            for (int i = 0; i < SerList.Count; i++)
            {
                Moves.Add(MoveFactory.WhiteDoublePawnPush(SerList[i]));
            }
            //promotions
            bb = BB_WhitePawns & BB_7thRank;
            bb = Shift_N(bb) & BB_Empty;
            SerializeBitboard(bb);
            for (int i = 0; i < SerList.Count; i++)
            {
                Moves.AddRange(MoveFactory.WhitePushPromotions(SerList[i]));
            }
            //quiet
            bb = BB_WhitePawns & (~BB_7thRank);
            bb = Shift_N(bb) & BB_Empty;
            SerializeBitboard(bb);
            for (int i = 0; i < SerList.Count; i++)
            {
                Moves.Add(MoveFactory.QuietMove(SerList[i] - 8, SerList[i], Piece.WP));
            }
        }
        internal void GetWhitePawnCaptures()
        {
            ulong bb_captures = BB_WhitePawnAttacks & BB_Black;

            //promotion captures
            ulong bb = bb_captures & BB_8thRank;
            ulong bb_fromwest = Shift_SW(bb) & BB_WhitePawns;
            SerializeBitboard(bb_fromwest);
            for (int i = 0; i < SerList.Count; i++)
            {
                int target = SerList[i] + 9;
                Moves.AddRange(MoveFactory.WhiteCapturePromotions(SerList[i], target, Mailbox[target]));
            }
            ulong bb_fromeast = Shift_SE(bb) & BB_WhitePawns;
            SerializeBitboard(bb_fromeast);
            for (int i = 0; i < SerList.Count; i++)
            {
                int target = SerList[i] + 7;
                Moves.AddRange(MoveFactory.WhiteCapturePromotions(SerList[i], target, Mailbox[target]));
            }
            //enpassant
            bb = BB_WhitePawns & (Shift_SW(BB_PossibleEPSquare) | Shift_SE(BB_PossibleEPSquare));
            SerializeBitboard(bb);
            for (int i = 0; i < SerList.Count; i++)
            {
                Moves.Add(MoveFactory.WhiteEnPassant(SerList[i], PossibleEPSquare_Index));
            }
            //normal captures
            bb = bb_captures & (~BB_8thRank);
            bb_fromwest = Shift_SW(bb) & BB_WhitePawns;
            SerializeBitboard(bb_fromwest);
            for (int i = 0; i < SerList.Count; i++)
            {
                int target = SerList[i] + 9;
                Moves.Add(MoveFactory.Capture(SerList[i], target, Piece.WP, Mailbox[target]));
            }
            bb_fromeast = Shift_SE(bb) & BB_WhitePawns;
            SerializeBitboard(bb_fromeast);
            for (int i = 0; i < SerList.Count; i++)
            {
                int target = SerList[i] + 7;
                Moves.Add(MoveFactory.Capture(SerList[i], target, Piece.WP, Mailbox[target]));
            }
        }
        internal void GetBlackPawnAdvances()
        {
            //double pushes
            ulong bb = BB_BlackPawns & BB_7thRank;
            bb = Shift_S(bb) & BB_Empty;
            bb = Shift_S(bb) & BB_Empty;
            SerializeBitboard(bb);
            for (int i = 0; i < SerList.Count; i++)
            {
                Moves.Add(MoveFactory.BlackDoublePawnPush(SerList[i]));
            }
            //promotions
            bb = BB_BlackPawns & BB_2ndRank;
            bb = Shift_S(bb) & BB_Empty;
            SerializeBitboard(bb);
            for (int i = 0; i < SerList.Count; i++)
            {
                Moves.AddRange(MoveFactory.BlackPushPromotions(SerList[i]));
            }
            //quiet
            bb = BB_BlackPawns & (~BB_2ndRank);
            bb = Shift_S(bb) & BB_Empty;
            SerializeBitboard(bb);
            for (int i = 0; i < SerList.Count; i++)
            {
                Moves.Add(MoveFactory.QuietMove(SerList[i] + 8, SerList[i], Piece.BP));
            }
        }
        internal void GetBlackPawnCaptures()
        {
            ulong bb_captures = BB_BlackPawnAttacks & BB_White;

            //promotion captures
            ulong bb = bb_captures & BB_1stRank;
            ulong bb_fromwest = Shift_NW(bb) & BB_BlackPawns;
            SerializeBitboard(bb_fromwest);
            for (int i = 0; i < SerList.Count; i++)
            {
                int target = SerList[i] - 7;
                Moves.AddRange(MoveFactory.BlackCapturePromotions(SerList[i], target, Mailbox[target]));
            }
            ulong bb_fromeast = Shift_NE(bb) & BB_BlackPawns;
            SerializeBitboard(bb_fromeast);
            for (int i = 0; i < SerList.Count; i++)
            {
                int target = SerList[i] - 9;
                Moves.AddRange(MoveFactory.BlackCapturePromotions(SerList[i], target, Mailbox[target]));
            }
            //enpassant
            bb = BB_BlackPawns & (Shift_NW(BB_PossibleEPSquare) | Shift_NE(BB_PossibleEPSquare));
            SerializeBitboard(bb);
            for (int i = 0; i < SerList.Count; i++)
            {
                Moves.Add(MoveFactory.BlackEnPassant(SerList[i], PossibleEPSquare_Index));
            }
            //normal captures
            bb = bb_captures & (~BB_1stRank);
            bb_fromwest = Shift_NW(bb) & BB_BlackPawns;
            SerializeBitboard(bb_fromwest);
            for (int i = 0; i < SerList.Count; i++)
            {
                int target = SerList[i] - 7;
                Moves.Add(MoveFactory.Capture(SerList[i], target, Piece.BP, Mailbox[target]));
            }
            bb_fromeast = Shift_NE(bb) & BB_BlackPawns;
            SerializeBitboard(bb_fromeast);
            for (int i = 0; i < SerList.Count; i++)
            {
                int target = SerList[i] - 9;
                Moves.Add(MoveFactory.Capture(SerList[i], target, Piece.BP, Mailbox[target]));
            }
        }

        internal void GetWhiteKnightMoves()
        {
            SerializeBitboard(BB_WhiteKnights);
            int[] knights = SerList.ToArray();

            for (int n = 0; n < knights.Length; n++)
            {
                ulong moves = KnightMoves[knights[n]];
                SerializeBitboard(moves & BB_Empty);
                for (int i = 0; i < SerList.Count; i++)
                {
                    Moves.Add(MoveFactory.QuietMove(knights[n], SerList[i], Piece.WN));
                }
                SerializeBitboard(moves & BB_Black);
                for (int i = 0; i < SerList.Count; i++)
                {
                    Moves.Add(MoveFactory.Capture(knights[n], SerList[i], Piece.WN, Mailbox[SerList[i]]));
                }
            }
        }
        internal void GetBlackKnightMoves()
        {
            SerializeBitboard(BB_BlackKnights);
            int[] knights = SerList.ToArray();

            for (int n = 0; n < knights.Length; n++)
            {
                ulong moves = KnightMoves[knights[n]];
                SerializeBitboard(moves & BB_Empty);
                for (int i = 0; i < SerList.Count; i++)
                {
                    Moves.Add(MoveFactory.QuietMove(knights[n], SerList[i], Piece.BN));
                }
                SerializeBitboard(moves & BB_White);
                for (int i = 0; i < SerList.Count; i++)
                {
                    Moves.Add(MoveFactory.Capture(knights[n], SerList[i], Piece.BN, Mailbox[SerList[i]]));
                }
            }
        }

        internal void GetWhiteBishopMoves()
        {
            SerializeBitboard(BB_WhiteBishops);
            int[] bishops = SerList.ToArray();

            for (int b = 0; b < bishops.Length; b++)
            {
                ulong moves = AttackFill_Bishop((ulong)1 << bishops[b], BB_Empty);
                SerializeBitboard(moves & BB_Empty);
                for (int i = 0; i < SerList.Count; i++)
                {
                    Moves.Add(MoveFactory.QuietMove(bishops[b], SerList[i], Piece.WB));
                }
                SerializeBitboard(moves & BB_Black);
                for (int i = 0; i < SerList.Count; i++)
                {
                    Moves.Add(MoveFactory.Capture(bishops[b], SerList[i], Piece.WB, Mailbox[SerList[i]]));
                }
            }
        }
        internal void GetBlackBishopMoves()
        {
            SerializeBitboard(BB_BlackBishops);
            int[] bishops = SerList.ToArray();

            for (int b = 0; b < bishops.Length; b++)
            {
                ulong moves = AttackFill_Bishop((ulong)1 << bishops[b], BB_Empty);
                SerializeBitboard(moves & BB_Empty);
                for (int i = 0; i < SerList.Count; i++)
                {
                    Moves.Add(MoveFactory.QuietMove(bishops[b], SerList[i], Piece.BB));
                }
                SerializeBitboard(moves & BB_White);
                for (int i = 0; i < SerList.Count; i++)
                {
                    Moves.Add(MoveFactory.Capture(bishops[b], SerList[i], Piece.BB, Mailbox[SerList[i]]));
                }
            }
        }

        internal void GetWhiteRookMoves()
        {
            SerializeBitboard(BB_WhiteRooks);
            int[] rooks = SerList.ToArray();

            for (int r = 0; r < rooks.Length; r++)
            {
                ulong moves = AttackFill_Rook((ulong)1 << rooks[r], BB_Empty);
                SerializeBitboard(moves & BB_Empty);
                for (int i = 0; i < SerList.Count; i++)
                {
                    Moves.Add(MoveFactory.QuietMove(rooks[r], SerList[i], Piece.WR));
                }
                SerializeBitboard(moves & BB_Black);
                for (int i = 0; i < SerList.Count; i++)
                {
                    Moves.Add(MoveFactory.Capture(rooks[r], SerList[i], Piece.WR, Mailbox[SerList[i]]));
                }
            }
        }
        internal void GetBlackRookMoves()
        {
            SerializeBitboard(BB_BlackRooks);
            int[] rooks = SerList.ToArray();

            for (int r = 0; r < rooks.Length; r++)
            {
                ulong moves = AttackFill_Rook((ulong)1 << rooks[r], BB_Empty);
                SerializeBitboard(moves & BB_Empty);
                for (int i = 0; i < SerList.Count; i++)
                {
                    Moves.Add(MoveFactory.QuietMove(rooks[r], SerList[i], Piece.BR));
                }
                SerializeBitboard(moves & BB_White);
                for (int i = 0; i < SerList.Count; i++)
                {
                    Moves.Add(MoveFactory.Capture(rooks[r], SerList[i], Piece.BR, Mailbox[SerList[i]]));
                }
            }
        }

        internal void GetWhiteQueenMoves()
        {
            SerializeBitboard(BB_WhiteQueens);
            int[] queens = SerList.ToArray();

            for (int q = 0; q < queens.Length; q++)
            {
                ulong moves = AttackFill_Queen((ulong)1 << queens[q], BB_Empty);
                SerializeBitboard(moves & BB_Empty);
                for (int i = 0; i < SerList.Count; i++)
                {
                    Moves.Add(MoveFactory.QuietMove(queens[q], SerList[i], Piece.WQ));
                }
                SerializeBitboard(moves & BB_Black);
                for (int i = 0; i < SerList.Count; i++)
                {
                    Moves.Add(MoveFactory.Capture(queens[q], SerList[i], Piece.WQ, Mailbox[SerList[i]]));
                }
            }
        }
        internal void GetBlackQueenMoves()
        {
            SerializeBitboard(BB_BlackQueens);
            int[] queens = SerList.ToArray();

            for (int q = 0; q < queens.Length; q++)
            {
                ulong moves = AttackFill_Queen((ulong)1 << queens[q], BB_Empty);
                SerializeBitboard(moves & BB_Empty);
                for (int i = 0; i < SerList.Count; i++)
                {
                    Moves.Add(MoveFactory.QuietMove(queens[q], SerList[i], Piece.BQ));
                }
                SerializeBitboard(moves & BB_White);
                for (int i = 0; i < SerList.Count; i++)
                {
                    Moves.Add(MoveFactory.Capture(queens[q], SerList[i], Piece.BQ, Mailbox[SerList[i]]));
                }
            }
        }

        internal void GetWhiteKingMoves()
        {
            //castling
            if (CanCastleWhiteShort && ((BB_AllBlackAttacks & BB_e1f1g1) == 0) && ((BB_Occupied & BB_f1g1) == 0))
            {
                Moves.Add(MoveFactory.WhiteShortCastle());
            }
            if (CanCastleWhiteLong && ((BB_AllBlackAttacks & BB_e1d1c1) == 0) && ((BB_Occupied & BB_d1c1b1) == 0))
            {
                Moves.Add(MoveFactory.WhiteLongCastle());
            }
            //quiet+captures
            int kingindex = BitScanForward(BB_WhiteKing);
            ulong moves = KingMoves[kingindex];
            SerializeBitboard(moves & BB_Empty);
            for (int i = 0; i < SerList.Count; i++)
            {
                Moves.Add(MoveFactory.QuietMove(kingindex, SerList[i], Piece.WK));
            }
            SerializeBitboard(moves & BB_Black);
            for (int i = 0; i < SerList.Count; i++)
            {
                Moves.Add(MoveFactory.Capture(kingindex, SerList[i], Piece.WK, Mailbox[SerList[i]]));
            }
        }
        internal void GetBlackKingMoves()
        {
            //castling
            if (CanCastleBlackShort && ((BB_AllWhiteAttacks & BB_e8f8g8) == 0) && ((BB_Occupied & BB_f8g8) == 0))
            {
                Moves.Add(MoveFactory.BlackShortCastle());
            }
            if (CanCastleBlackLong && ((BB_AllWhiteAttacks & BB_e8d8c8) == 0) && ((BB_Occupied & BB_d8c8b8) == 0))
            {
                Moves.Add(MoveFactory.BlackLongCastle());
            }
            //quiet+captures
            int kingindex = BitScanForward(BB_BlackKing);
            ulong moves = KingMoves[kingindex];
            SerializeBitboard(moves & BB_Empty);
            for (int i = 0; i < SerList.Count; i++)
            {
                Moves.Add(MoveFactory.QuietMove(kingindex, SerList[i], Piece.BK));
            }
            SerializeBitboard(moves & BB_White);
            for (int i = 0; i < SerList.Count; i++)
            {
                Moves.Add(MoveFactory.Capture(kingindex, SerList[i], Piece.BK, Mailbox[SerList[i]]));
            }
        }

        #endregion

        internal void SetStartingPosition()
        {
            LoadFEN("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
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

            if (IsWhiteMove) sb.Append('w');
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

            sb.Append(IndexToAlgebraic(PossibleEPSquare_Index));

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
                if (rank > 0) sb.AppendLine();
            }
            return sb.ToString();
        }
        internal long Perft(int depth, bool detail)
        {
            if (depth == 0) return 1;

            long sum = 0;
            var movelist = GetPseudoLegalMoves().ToArray();
            for (int i = 0; i < movelist.Length; i++)
            {
                var move = movelist[i];
                MakeMove(move);
                if (!CanKingBeCaptured())
                {
                    long subresult = Perft(depth - 1, false);
                    sum += subresult;
                    if (detail)
                    {
                        Console.WriteLine(IndexToAlgebraic(move.SquareFrom) + IndexToAlgebraic(move.SquareTo) + ": " + subresult.ToString());
                    }
                }
                UnmakeMove();
            }
            return sum;
        }
    }
}