using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static KokosEngine.Piece;
using static KokosEngine.Util;

namespace KokosEngine
{
    internal class Gamestate
    {
        //TODO: make/unmake doesn't update the mailbox

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
            for (int i = 0; i < Mailbox.Length; i++)
            {
                Mailbox[i] = Piece.X;
            }
            Bitboards = new ulong[12];
            CastlingDisablingMoves = new int[4];
            for (int i = 0; i < CastlingDisablingMoves.Length; i++)
            {
                CastlingDisablingMoves[i] = -1;
            }
            History = new Stack<MoveAndIrreversibleInfo>();
        }
        internal Gamestate(string fen)
        {
            Mailbox = new Piece[64];
            for (int m = 0; m < Mailbox.Length; m++)
            {
                Mailbox[m] = Piece.X;
            }
            Bitboards = new ulong[12];
            CastlingDisablingMoves = new int[4];
            for (int m = 0; m < CastlingDisablingMoves.Length; m++)
            {
                CastlingDisablingMoves[m] = -1;
            }
            History = new Stack<MoveAndIrreversibleInfo>();

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


            RefreshHelperBitboards();
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
            BB_PossibleEPSquare = info.EnPassantBB;
            PossibleEPSquare_Index = info.EnPassantIndex;
            RevertPiecesState(info.Move);

            RefreshHelperBitboards();
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
            if (IsWhiteMove && CurrentFullmove == CastlingDisablingMoves[0]) CanCastleWhiteShort = true;
            if (IsWhiteMove && CurrentFullmove == CastlingDisablingMoves[1]) CanCastleWhiteLong = true;
            if (IsBlackMove && CurrentFullmove == CastlingDisablingMoves[2]) CanCastleBlackShort = true;
            if (IsBlackMove && CurrentFullmove == CastlingDisablingMoves[3]) CanCastleBlackLong = true;
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

        internal bool IsWhiteInCheck()
        {
            return (GetBB_AllBlackAttacks() & BB_WhiteKing) != 0;
        }
        internal bool IsBlackInCheck()
        {
            var value = (GetBB_AllWhiteAttacks() & BB_BlackKing) != 0;
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
            List<Move> result = new List<Move>();
            if (IsWhiteMove)
            {
                result.AddRange(GetWhitePawnMoves());
                result.AddRange(GetWhiteKnightMoves());
                result.AddRange(GetWhiteBishopMoves());
                result.AddRange(GetWhiteRookMoves());
                result.AddRange(GetWhiteQueenMoves());
                result.AddRange(GetWhiteKingMoves());
            }
            else
            {
                result.AddRange(GetBlackPawnMoves());
                result.AddRange(GetBlackKnightMoves());
                result.AddRange(GetBlackBishopMoves());
                result.AddRange(GetBlackRookMoves());
                result.AddRange(GetBlackQueenMoves());
                result.AddRange(GetBlackKingMoves());
            }
            return result;
        }

        #region Move Generation        

        internal ulong GetBB_AllWhiteAttacks()
        {
            return GetBB_WhitePawnAttacks() | GetBB_WhiteKnightAttacks() | GetBB_WhiteBishopAttacks() | GetBB_WhiteRookAttacks() | GetBB_WhiteQueenAttacks() | GetBB_WhiteKingAttacks();
        }
        internal ulong GetBB_AllBlackAttacks()
        {
            return GetBB_BlackPawnAttacks() | GetBB_BlackKnightAttacks() | GetBB_BlackBishopAttacks() | GetBB_BlackRookAttacks() | GetBB_BlackQueenAttacks() | GetBB_BlackKingAttacks();
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
            foreach (int knightindex in SerializeBitboard(BB_WhiteKnights))
            {
                result |= KnightMoves[knightindex];
            }
            return result;
        }
        internal ulong GetBB_BlackKnightAttacks()
        {
            ulong result = 0;
            foreach (int knightindex in SerializeBitboard(BB_BlackKnights))
            {
                result |= KnightMoves[knightindex];
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
            ulong result = 0;
            foreach (int kingindex in SerializeBitboard(BB_WhiteKing))
            {
                result |= KingMoves[kingindex];
            }
            return result;
        }
        internal ulong GetBB_BlackKingAttacks()
        {
            ulong result = 0;
            foreach (int kingindex in SerializeBitboard(BB_BlackKing))
            {
                result |= KingMoves[kingindex];
            }
            return result;
        }

        internal List<Move> GetWhitePawnMoves()
        {
            List<Move> result = new List<Move>();
            result.AddRange(GetWhitePawnAdvances());
            result.AddRange(GetWhitePawnCaptures());
            return result;
        }
        internal List<Move> GetBlackPawnMoves()
        {
            List<Move> result = new List<Move>();
            result.AddRange(GetBlackPawnAdvances());
            result.AddRange(GetBlackPawnCaptures());
            return result;
        }
        internal List<Move> GetWhitePawnAdvances()
        {
            List<Move> list = new List<Move>();
            //double pushes
            ulong bb = BB_WhitePawns & BB_2ndRank;
            bb = Shift_N(bb) & BB_Empty;
            bb = Shift_N(bb) & BB_Empty;
            foreach (var target in SerializeBitboard(bb))
            {
                list.Add(MoveFactory.WhiteDoublePawnPush(target));
            }
            //promotions
            bb = BB_WhitePawns & BB_7thRank;
            bb = Shift_N(bb) & BB_Empty;
            foreach (var target in SerializeBitboard(bb))
            {
                list.AddRange(MoveFactory.WhitePushPromotions(target - 8));
            }
            //quiet
            bb = BB_WhitePawns & (~BB_7thRank);
            bb = Shift_N(bb) & BB_Empty;
            foreach (var target in SerializeBitboard(bb))
            {
                list.Add(MoveFactory.QuietMove(target - 8, target, Piece.WP));
            }

            return list;
        }
        internal List<Move> GetWhitePawnCaptures()
        {
            List<Move> list = new List<Move>();

            ulong bb_captures = GetBB_WhitePawnAttacks() & BB_Black;

            //promotion captures
            ulong bb = bb_captures & BB_8thRank;
            ulong bb_fromwest = Shift_SW(bb) & BB_WhitePawns;
            foreach (var source in SerializeBitboard(bb_fromwest))
            {
                int target = source + 9;
                list.AddRange(MoveFactory.WhiteCapturePromotions(source, target, Mailbox[target]));
            }
            ulong bb_fromeast = Shift_SE(bb) & BB_WhitePawns;
            foreach (var source in SerializeBitboard(bb_fromeast))
            {
                int target = source + 7;
                list.AddRange(MoveFactory.WhiteCapturePromotions(source, target, Mailbox[target]));
            }
            //enpassant
            bb = BB_WhitePawns & (Shift_SW(BB_PossibleEPSquare) | Shift_SE(BB_PossibleEPSquare));
            foreach (var source in SerializeBitboard(bb))
            {
                list.Add(MoveFactory.WhiteEnPassant(source, PossibleEPSquare_Index));
            }
            //normal captures
            bb = bb_captures & (~BB_8thRank);
            bb_fromwest = Shift_SW(bb) & BB_WhitePawns;
            foreach (var source in SerializeBitboard(bb_fromwest))
            {
                int target = source + 9;
                list.Add(MoveFactory.Capture(source, target, Piece.WP, Mailbox[target]));
            }
            bb_fromeast = Shift_SE(bb) & BB_WhitePawns;
            foreach (var source in SerializeBitboard(bb_fromeast))
            {
                int target = source + 7;
                list.Add(MoveFactory.Capture(source, target, Piece.WP, Mailbox[target]));
            }

            return list;
        }
        internal List<Move> GetBlackPawnAdvances()
        {
            List<Move> list = new List<Move>();
            //double pushes
            ulong bb = BB_BlackPawns & BB_7thRank;
            bb = Shift_S(bb) & BB_Empty;
            bb = Shift_S(bb) & BB_Empty;
            foreach (var target in SerializeBitboard(bb))
            {
                list.Add(MoveFactory.BlackDoublePawnPush(target));
            }
            //promotions
            bb = BB_BlackPawns & BB_2ndRank;
            bb = Shift_S(bb) & BB_Empty;
            foreach (var target in SerializeBitboard(bb))
            {
                list.AddRange(MoveFactory.BlackPushPromotions(target + 8));
            }
            //quiet
            bb = BB_BlackPawns & (~BB_2ndRank);
            bb = Shift_S(bb) & BB_Empty;
            foreach (var target in SerializeBitboard(bb))
            {
                list.Add(MoveFactory.QuietMove(target + 8, target, Piece.BP));
            }

            return list;
        }
        internal List<Move> GetBlackPawnCaptures()
        {
            List<Move> list = new List<Move>();

            ulong bb_captures = GetBB_BlackPawnAttacks() & BB_White;

            //promotion captures
            ulong bb = bb_captures & BB_1stRank;
            ulong bb_fromwest = Shift_NW(bb) & BB_BlackPawns;
            foreach (var source in SerializeBitboard(bb_fromwest))
            {
                int target = source - 7;
                list.AddRange(MoveFactory.BlackCapturePromotions(source, target, Mailbox[target]));
            }
            ulong bb_fromeast = Shift_NE(bb) & BB_BlackPawns;
            foreach (var source in SerializeBitboard(bb_fromeast))
            {
                int target = source - 9;
                list.AddRange(MoveFactory.BlackCapturePromotions(source, target, Mailbox[target]));
            }
            //enpassant
            bb = BB_BlackPawns & (Shift_NW(BB_PossibleEPSquare) | Shift_NE(BB_PossibleEPSquare));
            foreach (var source in SerializeBitboard(bb))
            {
                list.Add(MoveFactory.BlackEnPassant(source, PossibleEPSquare_Index));
            }
            //normal captures
            bb = bb_captures & (~BB_1stRank);
            bb_fromwest = Shift_NW(bb) & BB_BlackPawns;
            foreach (var source in SerializeBitboard(bb_fromwest))
            {
                int target = source - 7;
                list.Add(MoveFactory.Capture(source, target, Piece.BP, Mailbox[target]));
            }
            bb_fromeast = Shift_NE(bb) & BB_BlackPawns;
            foreach (var source in SerializeBitboard(bb_fromeast))
            {
                int target = source - 9;
                list.Add(MoveFactory.Capture(source, target, Piece.BP, Mailbox[target]));
            }

            return list;
        }

        internal List<Move> GetWhiteKnightMoves()
        {
            List<Move> list = new List<Move>();

            foreach (int knightindex in SerializeBitboard(BB_WhiteKnights))
            {
                ulong moves = KnightMoves[knightindex];
                foreach (int quietindex in SerializeBitboard(moves & BB_Empty))
                {
                    list.Add(MoveFactory.QuietMove(knightindex, quietindex, Piece.WN));
                }
                foreach (int captureindex in SerializeBitboard(moves & BB_Black))
                {
                    list.Add(MoveFactory.Capture(knightindex, captureindex, Piece.WN, Mailbox[captureindex]));
                }
            }

            return list;
        }
        internal List<Move> GetBlackKnightMoves()
        {
            List<Move> list = new List<Move>();

            foreach (int knightindex in SerializeBitboard(BB_BlackKnights))
            {
                ulong moves = KnightMoves[knightindex];
                foreach (int quietindex in SerializeBitboard(moves & BB_Empty))
                {
                    list.Add(MoveFactory.QuietMove(knightindex, quietindex, Piece.BN));
                }
                foreach (int captureindex in SerializeBitboard(moves & BB_White))
                {
                    list.Add(MoveFactory.Capture(knightindex, captureindex, Piece.BN, Mailbox[captureindex]));
                }
            }

            return list;
        }

        internal List<Move> GetWhiteBishopMoves()
        {
            List<Move> list = new List<Move>();

            foreach (int bishopindex in SerializeBitboard(BB_WhiteBishops))
            {
                ulong moves = AttackFill_Bishop((ulong)1 << bishopindex, BB_Empty);
                foreach (int quietindex in SerializeBitboard(moves & BB_Empty))
                {
                    list.Add(MoveFactory.QuietMove(bishopindex, quietindex, Piece.WB));
                }
                foreach (int captureindex in SerializeBitboard(moves & BB_Black))
                {
                    list.Add(MoveFactory.Capture(bishopindex, captureindex, Piece.WB, Mailbox[captureindex]));
                }
            }

            return list;
        }
        internal List<Move> GetBlackBishopMoves()
        {
            List<Move> list = new List<Move>();

            foreach (int bishopindex in SerializeBitboard(BB_BlackBishops))
            {
                ulong moves = AttackFill_Bishop((ulong)1 << bishopindex, BB_Empty);
                foreach (int quietindex in SerializeBitboard(moves & BB_Empty))
                {
                    list.Add(MoveFactory.QuietMove(bishopindex, quietindex, Piece.BB));
                }
                foreach (int captureindex in SerializeBitboard(moves & BB_White))
                {
                    list.Add(MoveFactory.Capture(bishopindex, captureindex, Piece.BB, Mailbox[captureindex]));
                }
            }

            return list;
        }

        internal List<Move> GetWhiteRookMoves()
        {
            List<Move> list = new List<Move>();

            foreach (int rookindex in SerializeBitboard(BB_WhiteRooks))
            {
                ulong moves = AttackFill_Rook((ulong)1 << rookindex, BB_Empty);
                foreach (int quietindex in SerializeBitboard(moves & BB_Empty))
                {
                    list.Add(MoveFactory.QuietMove(rookindex, quietindex, Piece.WR));
                }
                foreach (int captureindex in SerializeBitboard(moves & BB_Black))
                {
                    list.Add(MoveFactory.Capture(rookindex, captureindex, Piece.WR, Mailbox[captureindex]));
                }
            }

            return list;
        }
        internal List<Move> GetBlackRookMoves()
        {
            List<Move> list = new List<Move>();

            foreach (int rookindex in SerializeBitboard(BB_BlackRooks))
            {
                ulong moves = AttackFill_Rook((ulong)1 << rookindex, BB_Empty);
                foreach (int quietindex in SerializeBitboard(moves & BB_Empty))
                {
                    list.Add(MoveFactory.QuietMove(rookindex, quietindex, Piece.BR));
                }
                foreach (int captureindex in SerializeBitboard(moves & BB_White))
                {
                    list.Add(MoveFactory.Capture(rookindex, captureindex, Piece.BR, Mailbox[captureindex]));
                }
            }

            return list;
        }

        internal List<Move> GetWhiteQueenMoves()
        {
            List<Move> list = new List<Move>();

            foreach (int queenindex in SerializeBitboard(BB_WhiteQueens))
            {
                ulong moves = AttackFill_Queen((ulong)1 << queenindex, BB_Empty);
                foreach (int quietindex in SerializeBitboard(moves & BB_Empty))
                {
                    list.Add(MoveFactory.QuietMove(queenindex, quietindex, Piece.WQ));
                }
                foreach (int captureindex in SerializeBitboard(moves & BB_Black))
                {
                    list.Add(MoveFactory.Capture(queenindex, captureindex, Piece.WQ, Mailbox[captureindex]));
                }
            }

            return list;
        }
        internal List<Move> GetBlackQueenMoves()
        {
            List<Move> list = new List<Move>();

            foreach (int queenindex in SerializeBitboard(BB_BlackQueens))
            {
                ulong moves = AttackFill_Queen((ulong)1 << queenindex, BB_Empty);
                foreach (int quietindex in SerializeBitboard(moves & BB_Empty))
                {
                    list.Add(MoveFactory.QuietMove(queenindex, quietindex, Piece.BQ));
                }
                foreach (int captureindex in SerializeBitboard(moves & BB_White))
                {
                    list.Add(MoveFactory.Capture(queenindex, captureindex, Piece.BQ, Mailbox[captureindex]));
                }
            }

            return list;
        }

        internal List<Move> GetWhiteKingMoves()
        {
            List<Move> list = new List<Move>();

            //castling
            if (CanCastleWhiteShort && ((GetBB_AllBlackAttacks() & BB_e1f1g1) == 0) && ((BB_Occupied & BB_f1g1) == 0))
            {
                list.Add(MoveFactory.WhiteShortCastle());
            }
            if (CanCastleWhiteLong && ((GetBB_AllBlackAttacks() & BB_e1d1c1) == 0) && ((BB_Occupied & BB_d1c1b1) == 0))
            {
                list.Add(MoveFactory.WhiteLongCastle());
            }
            //quiet+captures
            foreach (int kingindex in SerializeBitboard(BB_WhiteKing))
            {
                ulong moves = KingMoves[kingindex];
                foreach (int quietindex in SerializeBitboard(moves & BB_Empty))
                {
                    list.Add(MoveFactory.QuietMove(kingindex, quietindex, Piece.WK));
                }
                foreach (int captureindex in SerializeBitboard(moves & BB_Black))
                {
                    list.Add(MoveFactory.Capture(kingindex, captureindex, Piece.WK, Mailbox[captureindex]));
                }
            }

            return list;
        }
        internal List<Move> GetBlackKingMoves()
        {
            List<Move> list = new List<Move>();

            //castling
            if (CanCastleBlackShort && ((GetBB_AllWhiteAttacks() & BB_e8f8g8) == 0) && ((BB_Occupied & BB_f8g8) == 0))
            {
                list.Add(MoveFactory.BlackShortCastle());
            }
            if (CanCastleBlackLong && ((GetBB_AllWhiteAttacks() & BB_e8d8c8) == 0) && ((BB_Occupied & BB_d8c8b8) == 0))
            {
                list.Add(MoveFactory.BlackLongCastle());
            }
            //quiet+captures
            foreach (int kingindex in SerializeBitboard(BB_BlackKing))
            {
                ulong moves = KingMoves[kingindex];
                foreach (int quietindex in SerializeBitboard(moves & BB_Empty))
                {
                    list.Add(MoveFactory.QuietMove(kingindex, quietindex, Piece.BK));
                }
                foreach (int captureindex in SerializeBitboard(moves & BB_White))
                {
                    list.Add(MoveFactory.Capture(kingindex, captureindex, Piece.BK, Mailbox[captureindex]));
                }
            }

            return list;
        }

        #endregion

        internal void SetStartingPosition()
        {
            IsWhiteMove = true;

            HalfmovesReversible = 0;
            CurrentFullmove = 1;

            BB_PossibleEPSquare = 0;
            PossibleEPSquare_Index = -1;

            BB_WhitePawns = 0x000000000000FF00;
            BB_WhiteKnights = 0x0000000000000042;
            BB_WhiteBishops = 0x0000000000000024;
            BB_WhiteRooks = 0x0000000000000081;
            BB_WhiteQueens = 0x0000000000000010;
            BB_WhiteKing = 0x0000000000000008;
            CanCastleWhiteShort = true;
            CanCastleWhiteLong = true;

            BB_BlackPawns = 0x00FF000000000000;
            BB_BlackKnights = 0x4200000000000000;
            BB_BlackBishops = 0x2400000000000000;
            BB_BlackRooks = 0x8100000000000000;
            BB_BlackQueens = 0x1000000000000000;
            BB_BlackKing = 0x0800000000000000;
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

            RefreshHelperBitboards();
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
            var movelist = GetPseudoLegalMoves();
            foreach (var move in movelist)
            {
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