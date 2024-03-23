using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using KokosEngine.Moves;

namespace KokosEngine
{
    internal class Gamestate
    {
        private List<IMove> _moves = new List<IMove>(128);

        #region Basic Data

        internal Color PlayerToMove;
        internal bool IsWhiteMove
        {
            get { return PlayerToMove == Color.White; }
            set { PlayerToMove = value ? Color.White : Color.Black; }
        }
        internal bool IsBlackMove
        {
            get { return !IsWhiteMove; }
            set { IsWhiteMove = !value; }
        }

        internal int CurrentFullmove; //increments after black moves, starts at 1
        internal IrreversibleInformation Irreversibles;

        internal Bitboard[] Bitboards;
        #region Individual Bitboard Properties
        internal Bitboard BB_WhitePawns
        {
            get { return Bitboards[0]; }
            set { Bitboards[0] = value; }
        }
        internal Bitboard BB_WhiteKnights
        {
            get { return Bitboards[1]; }
            set { Bitboards[1] = value; }
        }
        internal Bitboard BB_WhiteBishops
        {
            get { return Bitboards[2]; }
            set { Bitboards[2] = value; }
        }
        internal Bitboard BB_WhiteRooks
        {
            get { return Bitboards[3]; }
            set { Bitboards[3] = value; }
        }
        internal Bitboard BB_WhiteQueens
        {
            get { return Bitboards[4]; }
            set { Bitboards[4] = value; }
        }
        internal Bitboard BB_WhiteKing
        {
            get { return Bitboards[5]; }
            set { Bitboards[5] = value; }
        }
        internal Bitboard BB_BlackPawns
        {
            get { return Bitboards[6]; }
            set { Bitboards[6] = value; }
        }
        internal Bitboard BB_BlackKnights
        {
            get { return Bitboards[7]; }
            set { Bitboards[7] = value; }
        }
        internal Bitboard BB_BlackBishops
        {
            get { return Bitboards[8]; }
            set { Bitboards[8] = value; }
        }
        internal Bitboard BB_BlackRooks
        {
            get { return Bitboards[9]; }
            set { Bitboards[9] = value; }
        }
        internal Bitboard BB_BlackQueens
        {
            get { return Bitboards[10]; }
            set { Bitboards[10] = value; }
        }
        internal Bitboard BB_BlackKing
        {
            get { return Bitboards[11]; }
            set { Bitboards[11] = value; }
        }
        #endregion
        #region Helper Bitboard Properties
        internal Bitboard BB_PossibleEP { get; set; }
        internal Bitboard BB_Occupied { get; set; }
        /// <summary>
        /// This is the list of squares without a piece on them, not to be confused with a bitboard of zero value
        /// </summary>
        internal Bitboard BB_Empty { get; set; }
        internal Bitboard BB_White { get; set; }
        internal Bitboard BB_Black { get; set; }

        internal Bitboard BB_AllWhiteAttacks { get; set; }
        internal Bitboard BB_AllBlackAttacks { get; set; }
        internal Bitboard BB_WhitePawnAttacks { get; set; }
        internal Bitboard BB_WhiteKnightAttacks { get; set; }
        internal Bitboard BB_WhiteBishopAttacks { get; set; }
        internal Bitboard BB_WhiteRookAttacks { get; set; }
        internal Bitboard BB_WhiteQueenAttacks { get; set; }
        internal Bitboard BB_WhiteKingAttacks { get; set; }
        internal Bitboard BB_BlackPawnAttacks { get; set; }
        internal Bitboard BB_BlackKnightAttacks { get; set; }
        internal Bitboard BB_BlackBishopAttacks { get; set; }
        internal Bitboard BB_BlackRookAttacks { get; set; }
        internal Bitboard BB_BlackQueenAttacks { get; set; }
        internal Bitboard BB_BlackKingAttacks { get; set; }


        #endregion

        internal Piece[] Mailbox;

        internal Stack<IMove> History;

        #endregion

        internal Gamestate()
        {
            Mailbox = new Piece[64];
            for (int i = 0; i < Mailbox.Length; i++)
            {
                Mailbox[i] = Piece.None;
            }
            Bitboards = new Bitboard[12];
            Irreversibles = IrreversibleInformation.GetStartingState();
            History = new Stack<IMove>();
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
                        queue.Enqueue(Piece.None);
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
                    if (p != Piece.None)
                    {
                        SetPieceOnCoordinate(p, 8 * rank + file);
                    }
                }
            }

            i++; //section: active color

            IsWhiteMove = (fen[i] == 'w');

            i += 2; //section: castling

            bool irr_cws = false, irr_cwl = false, irr_cbs = false, irr_cbl = false;

            while (fen[i] != ' ')
            {
                if (fen[i] == 'K') irr_cws = true;
                if (fen[i] == 'Q') irr_cwl = true;
                if (fen[i] == 'k') irr_cbs = true;
                if (fen[i] == 'q') irr_cbl = true;

                i++;
            }

            i++; //section: en passant

            Coordinate irr_ep;

            if (fen[i] == '-')
            {
                irr_ep = Coordinate.None;
                i += 2;
            }
            else
            {
                string epNotation = new string(new char[] { fen[i], fen[i + 1] });
                Coordinate epCoord = new Coordinate(epNotation);
                irr_ep = epCoord;
                i += 3;
            }

            //section: halfmove clock

            int irr_hm;

            StringBuilder sb = new StringBuilder();
            while (fen[i] != ' ')
            {
                sb.Append(fen[i]);
                i++;
            }
            irr_hm = int.Parse(sb.ToString());

            i++; //section: fullmove number

            sb = new StringBuilder();
            while (i < fen.Length && char.IsDigit(fen[i]))
            {
                sb.Append(fen[i]);
                i++;
            }
            CurrentFullmove = int.Parse(sb.ToString());

            Irreversibles = new IrreversibleInformation(irr_cws, irr_cwl, irr_cbs, irr_cbl, irr_ep, irr_hm);
            RefreshAllHelperBitboards();

            return this;
        }

        internal void MakeMove(IMove move)
        {
            UpdatePiecesState(move);
            History.Push(move);
            UpdateIrreversibles(move);
            IncrementFullmoveCounter();
            SwitchPlayerToMove();

            RefreshAllHelperBitboards();
        }

        internal void UnmakeMove()
        {
            UnmakeMove(History.Pop());
        }
        internal void UnmakeMove(IMove move)
        {
            SwitchPlayerToMove();
            DecrementFullmoveCounter();
            RestoreIrreversibles(move);
            RevertPiecesState(move);

            RefreshAllHelperBitboards();
        }

        #region Make/Unmake Move Helper Methods

        private void UpdateIrreversibles(IMove move)
        {
            Irreversibles = move.IrreversiblesAfterDo();
        }
        private void RestoreIrreversibles(IMove move)
        {
            Irreversibles = move.IrreversiblesAfterUndo();
        }

        internal void IncrementFullmoveCounter()
        {
            if (IsBlackMove)
            {
                CurrentFullmove++;
            }
        }
        internal void UpdatePiecesState(IMove move)
        {
            foreach (Square s in move.UpdatesAfterDo())
            {
                if (s.Piece == Piece.None) ClearCoordinate(s.Coordinate);
                else SetPieceOnCoordinate(s.Piece, s.Coordinate);
            }
        }
        internal void RevertPiecesState(IMove move)
        {
            foreach (Square s in move.UpdatesAfterUndo())
            {
                if (s.Piece == Piece.None) ClearCoordinate(s.Coordinate);
                else SetPieceOnCoordinate(s.Piece, s.Coordinate);
            }
        }
        internal void DecrementFullmoveCounter()
        {
            if (IsBlackMove)
            {
                CurrentFullmove--;
            }
        }
        internal void SwitchPlayerToMove()
        {
            IsWhiteMove = !IsWhiteMove;
        }
        internal void SetPieceOnCoordinate(Piece piece, Coordinate coord)
        {
            ClearBitInAllBitboards(coord);  //make sure that we don't store two pieces on one coordinate at the same time
            Bitboard mask = new Bitboard(coord);    //get coord as bitboard
            Bitboards[(int)piece] = Bitboards[(int)piece].Union(mask);  //union existing bitboard with the new coord
            Mailbox[coord] = piece;
        }
        internal void ClearCoordinate(Coordinate coord)
        {
            Mailbox[coord] = Piece.None;
            ClearBitInAllBitboards(coord);
        }
        internal void ClearBitInAllBitboards(Coordinate coord)
        {
            Bitboard mask = new Bitboard(coord).Inverse();  //get "all coords except the given one" as bitboard
            for (int i = 0; i < Bitboards.Length; i++)
            {
                Bitboards[i] = Bitboards[i].Overlap(mask);  //update each bitboard to overlap with the new "not that coord" bitboard
            }
        }

        internal void RefreshAllHelperBitboards()
        {
            BB_White = Bitboard.Empty;
            for (int i = 0; i < 6; i++)
            {
                BB_White = BB_White.Union(Bitboards[i]);
            }
            BB_Black = Bitboard.Empty;
            for (int i = 6; i < 12; i++)
            {
                BB_Black = BB_Black.Union(Bitboards[i]);
            }
            BB_Occupied = BB_White.Union(BB_Black);
            BB_Empty = BB_Occupied.Inverse();

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

            BB_PossibleEP = Irreversibles.EPBitboard;
        }

        #endregion

        internal bool IsWhiteInCheck()
        {
            return (BB_AllBlackAttacks & BB_WhiteKing) != 0;
        }
        internal bool IsBlackInCheck()
        {
            return (BB_AllWhiteAttacks & BB_BlackKing) != 0;
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

        internal List<IMove> GetPseudoLegalMoves()
        {
            _moves.Clear();
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
            return _moves;
        }

        #region Move Generation        

        internal Bitboard GetBB_AllWhiteAttacks()
        {
            return BB_WhitePawnAttacks | BB_WhiteKnightAttacks | BB_WhiteBishopAttacks | BB_WhiteRookAttacks | BB_WhiteQueenAttacks | BB_WhiteKingAttacks;
        }
        internal Bitboard GetBB_AllBlackAttacks()
        {
            return BB_BlackPawnAttacks | BB_BlackKnightAttacks | BB_BlackBishopAttacks | BB_BlackRookAttacks | BB_BlackQueenAttacks | BB_BlackKingAttacks;
        }
        internal Bitboard GetBB_WhitePawnAttacks()
        {
            Bitboard bb = BB_WhitePawns.Shift_NW();
            bb |= BB_WhitePawns.Shift_NE();
            return bb;
        }
        internal Bitboard GetBB_BlackPawnAttacks()
        {
            Bitboard bb = BB_BlackPawns.Shift_SW();
            bb |= BB_BlackPawns.Shift_SE();
            return bb;
        }
        internal Bitboard GetBB_WhiteKnightAttacks()
        {
            Bitboard result = Bitboard.Empty;
            var coords = BB_WhiteKnights.Serialize();
            foreach (var coord in coords)
            {
                result |= Bitboard.KnightMoves[coord];
            }
            return result;
        }
        internal Bitboard GetBB_BlackKnightAttacks()
        {
            Bitboard result = Bitboard.Empty;
            var coords = BB_BlackKnights.Serialize();
            foreach (var coord in coords)
            {
                result |= Bitboard.KnightMoves[coord];
            }
            return result;
        }
        internal Bitboard GetBB_WhiteBishopAttacks()
        {
            return BB_WhiteBishops.AttackFill_Bishop(BB_Empty);
        }
        internal Bitboard GetBB_BlackBishopAttacks()
        {
            return BB_BlackBishops.AttackFill_Bishop(BB_Empty);
        }
        internal Bitboard GetBB_WhiteRookAttacks()
        {
            return BB_WhiteRooks.AttackFill_Rook(BB_Empty);
        }
        internal Bitboard GetBB_BlackRookAttacks()
        {
            return BB_BlackRooks.AttackFill_Rook(BB_Empty);
        }
        internal Bitboard GetBB_WhiteQueenAttacks()
        {
            return BB_WhiteQueens.AttackFill_Queen(BB_Empty);
        }
        internal Bitboard GetBB_BlackQueenAttacks()
        {
            return BB_BlackQueens.AttackFill_Queen(BB_Empty);
        }
        internal Bitboard GetBB_WhiteKingAttacks()
        {
            Bitboard result = Bitboard.Empty;
            var coords = BB_WhiteKing.Serialize();
            foreach (var coord in coords)
            {
                result |= Bitboard.KingMoves[coord];
            }
            return result;
        }
        internal ulong GetBB_BlackKingAttacks()
        {
            Bitboard result = Bitboard.Empty;
            var coords = BB_BlackKing.Serialize();
            foreach (var coord in coords)
            {
                result |= Bitboard.KingMoves[coord];
            }
            return result;
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
            Bitboard bb = BB_WhitePawns.Overlap(Bitboard.Rank2);
            bb = bb.Shift_N().Overlap(BB_Empty);
            bb = bb.Shift_N().Overlap(BB_Empty);
            var targets = bb.Serialize();
            foreach (var target in targets)
            {
                _moves.Add(new Moves.White.DoublePawnPush(Irreversibles, target));
            }
            //promotions
            bb = BB_WhitePawns.Overlap(Bitboard.Rank7);
            bb = bb.Shift_N().Overlap(BB_Empty);
            targets = bb.Serialize();
            foreach (var target in targets)
            {
                _moves.AddRange(Moves.White.Promotion.GetAll(Irreversibles, target));
            }
            //quiet
            bb = BB_WhitePawns.Overlap(Bitboard.Rank7.Inverse());
            bb = bb.Shift_N().Overlap(BB_Empty);
            targets = bb.Serialize();
            foreach (var target in targets)
            {
                _moves.Add(new Moves.White.Quiet(Irreversibles, target.South, target, Piece.WhitePawn));
            }
        }
        internal void GetWhitePawnCaptures()
        {
            Bitboard bb_captures = BB_WhitePawnAttacks.Overlap(BB_Black);

            //promotion captures
            Bitboard bb = bb_captures.Overlap(Bitboard.Rank8);
            Bitboard bb_fromwest = bb.Shift_SW().Overlap(BB_WhitePawns);
            var sources = bb_fromwest.Serialize();
            foreach (var source in sources)
            {
                Coordinate target = source.NE;
                _moves.AddRange(Moves.White.PromotionCapture.GetAll(Irreversibles, source, target, Mailbox[target]));
            }
            Bitboard bb_fromeast = bb.Shift_SE().Overlap(BB_WhitePawns);
            sources = bb_fromeast.Serialize();
            foreach (var source in sources)
            {
                Coordinate target = source.NW;
                _moves.AddRange(Moves.White.PromotionCapture.GetAll(Irreversibles, source, target, Mailbox[target]));
            }
            //enpassant
            bb = BB_WhitePawns.Overlap(BB_PossibleEP.Shift_SW().Union(BB_PossibleEP.Shift_SE()));
            sources = bb.Serialize();
            foreach (var source in sources)
            {
                _moves.Add(new Moves.White.EnPassant(Irreversibles, source));
            }
            //normal captures
            bb = bb_captures.Overlap(Bitboard.Rank8.Inverse());
            bb_fromwest = bb.Shift_SW().Overlap(BB_WhitePawns);
            sources = bb_fromwest.Serialize();
            foreach (var source in sources)
            {
                Coordinate target = source.NE;
                _moves.Add(new Moves.White.Capture(Irreversibles, source, target, Piece.WhitePawn, Mailbox[target]));
            }
            bb_fromeast = bb.Shift_SE() & BB_WhitePawns;
            sources = bb_fromeast.Serialize();
            foreach (var source in sources)
            {
                Coordinate target = source.NW;
                _moves.Add(new Moves.White.Capture(Irreversibles, source, target, Piece.WhitePawn, Mailbox[target]));
            }
        }
        internal void GetBlackPawnAdvances()
        {
            //double pushes
            Bitboard bb = BB_BlackPawns.Overlap(Bitboard.Rank7);
            bb = bb.Shift_S().Overlap(BB_Empty);
            bb = bb.Shift_S().Overlap(BB_Empty);
            var targets = bb.Serialize();
            foreach (var target in targets)
            {
                _moves.Add(new Moves.Black.DoublePawnPush(Irreversibles, target));
            }
            //promotions
            bb = BB_BlackPawns.Overlap(Bitboard.Rank2);
            bb = bb.Shift_S().Overlap(BB_Empty);
            targets = bb.Serialize();
            foreach (var target in targets)
            {
                _moves.AddRange(Moves.Black.Promotion.GetAll(Irreversibles, target));
            }
            //quiet
            bb = BB_BlackPawns.Overlap(Bitboard.Rank2.Inverse());
            bb = bb.Shift_S().Overlap(BB_Empty);
            targets = bb.Serialize();
            foreach (var target in targets)
            {
                _moves.Add(new Moves.Black.Quiet(Irreversibles, target.North, target, Piece.BlackPawn));
            }
        }
        internal void GetBlackPawnCaptures()
        {
            Bitboard bb_captures = BB_BlackPawnAttacks.Overlap(BB_White);

            //promotion captures
            Bitboard bb = bb_captures.Overlap(Bitboard.Rank1);
            Bitboard bb_fromwest = bb.Shift_NW().Overlap(BB_BlackPawns);
            var sources = bb_fromwest.Serialize();
            foreach (var source in sources)
            {
                Coordinate target = source.SE;
                _moves.AddRange(Moves.Black.PromotionCapture.GetAll(Irreversibles, source, target, Mailbox[target]));
            }
            Bitboard bb_fromeast = bb.Shift_NE().Overlap(BB_BlackPawns);
            sources = bb_fromeast.Serialize();
            foreach (var source in sources)
            {
                Coordinate target = source.SW;
                _moves.AddRange(Moves.Black.PromotionCapture.GetAll(Irreversibles, source, target, Mailbox[target]));
            }
            //enpassant
            bb = BB_BlackPawns.Overlap(BB_PossibleEP.Shift_NW().Union(BB_PossibleEP.Shift_NE()));
            sources = bb.Serialize();
            foreach (var source in sources)
            {
                _moves.Add(new Moves.Black.EnPassant(Irreversibles, source));
            }
            //normal captures
            bb = bb_captures.Overlap(Bitboard.Rank1.Inverse());
            bb_fromwest = bb.Shift_NW().Overlap(BB_BlackPawns);
            sources = bb_fromwest.Serialize();
            foreach (var source in sources)
            {
                Coordinate target = source.SE;
                _moves.Add(new Moves.Black.Capture(Irreversibles, source, target, Piece.BlackPawn, Mailbox[target]));
            }
            bb_fromeast = bb.Shift_NE() & BB_BlackPawns;
            sources = bb_fromeast.Serialize();
            foreach (var source in sources)
            {
                Coordinate target = source.SW;
                _moves.Add(new Moves.Black.Capture(Irreversibles, source, target, Piece.BlackPawn, Mailbox[target]));
            }
        }

        internal void GetWhiteKnightMoves()
        {
            var knights = BB_WhiteKnights.Serialize();

            foreach (var knight in knights)
            {
                Bitboard moves = Bitboard.KnightMoves[knight];
                var quietTargets = moves.Overlap(BB_Empty).Serialize();
                foreach (var target in quietTargets)
                {
                    _moves.Add(new Moves.White.Quiet(Irreversibles, knight, target, Piece.WhiteKnight));
                }
                var captureTargets = moves.Overlap(BB_Black).Serialize();
                foreach (var target in captureTargets)
                {
                    _moves.Add(new Moves.White.Capture(Irreversibles, knight, target, Piece.WhiteKnight, Mailbox[target]));
                }
            }
        }
        internal void GetBlackKnightMoves()
        {
            var knights = BB_BlackKnights.Serialize();

            foreach (var knight in knights)
            {
                Bitboard moves = Bitboard.KnightMoves[knight];
                var quietTargets = moves.Overlap(BB_Empty).Serialize();
                foreach (var target in quietTargets)
                {
                    _moves.Add(new Moves.Black.Quiet(Irreversibles, knight, target, Piece.BlackKnight));
                }
                var captureTargets = moves.Overlap(BB_White).Serialize();
                foreach (var target in captureTargets)
                {
                    _moves.Add(new Moves.Black.Capture(Irreversibles, knight, target, Piece.BlackKnight, Mailbox[target]));
                }
            }
        }

        internal void GetWhiteBishopMoves()
        {
            var bishops = BB_WhiteBishops.Serialize();

            foreach (var bishop in bishops)
            {
                Bitboard moves = new Bitboard(bishop).AttackFill_Bishop(BB_Empty);
                var quietTargets = moves.Overlap(BB_Empty).Serialize();
                foreach (var target in quietTargets)
                {
                    _moves.Add(new Moves.White.Quiet(Irreversibles, bishop, target, Piece.WhiteBishop));
                }
                var captureTargets = moves.Overlap(BB_Black).Serialize();
                foreach (var target in captureTargets)
                {
                    _moves.Add(new Moves.White.Capture(Irreversibles, bishop, target, Piece.WhiteBishop, Mailbox[target]));
                }
            }
        }
        internal void GetBlackBishopMoves()
        {
            var bishops = BB_BlackBishops.Serialize();

            foreach (var bishop in bishops)
            {
                Bitboard moves = new Bitboard(bishop).AttackFill_Bishop(BB_Empty);
                var quietTargets = moves.Overlap(BB_Empty).Serialize();
                foreach (var target in quietTargets)
                {
                    _moves.Add(new Moves.Black.Quiet(Irreversibles, bishop, target, Piece.BlackBishop));
                }
                var captureTargets = moves.Overlap(BB_White).Serialize();
                foreach (var target in captureTargets)
                {
                    _moves.Add(new Moves.Black.Capture(Irreversibles, bishop, target, Piece.BlackBishop, Mailbox[target]));
                }
            }
        }

        internal void GetWhiteRookMoves()
        {
            var rooks = BB_WhiteRooks.Serialize();

            foreach (var rook in rooks)
            {
                Bitboard moves = new Bitboard(rook).AttackFill_Rook(BB_Empty);
                var quietTargets = moves.Overlap(BB_Empty).Serialize();
                foreach (var target in quietTargets)
                {
                    _moves.Add(new Moves.White.Quiet(Irreversibles, rook, target, Piece.WhiteRook));
                }
                var captureTargets = moves.Overlap(BB_Black).Serialize();
                foreach (var target in captureTargets)
                {
                    _moves.Add(new Moves.White.Capture(Irreversibles, rook, target, Piece.WhiteRook, Mailbox[target]));
                }
            }
        }
        internal void GetBlackRookMoves()
        {
            var rooks = BB_BlackRooks.Serialize();

            foreach (var rook in rooks)
            {
                Bitboard moves = new Bitboard(rook).AttackFill_Rook(BB_Empty);
                var quietTargets = moves.Overlap(BB_Empty).Serialize();
                foreach (var target in quietTargets)
                {
                    _moves.Add(new Moves.Black.Quiet(Irreversibles, rook, target, Piece.BlackRook));
                }
                var captureTargets = moves.Overlap(BB_White).Serialize();
                foreach (var target in captureTargets)
                {
                    _moves.Add(new Moves.Black.Capture(Irreversibles, rook, target, Piece.BlackRook, Mailbox[target]));
                }
            }
        }

        internal void GetWhiteQueenMoves()
        {
            var queens = BB_WhiteQueens.Serialize();

            foreach (var queen in queens)
            {
                Bitboard moves = new Bitboard(queen).AttackFill_Queen(BB_Empty);
                var quietTargets = moves.Overlap(BB_Empty).Serialize();
                foreach (var target in quietTargets)
                {
                    _moves.Add(new Moves.White.Quiet(Irreversibles, queen, target, Piece.WhiteQueen));
                }
                var captureTargets = moves.Overlap(BB_Black).Serialize();
                foreach (var target in captureTargets)
                {
                    _moves.Add(new Moves.White.Capture(Irreversibles, queen, target, Piece.WhiteQueen, Mailbox[target]));
                }
            }
        }
        internal void GetBlackQueenMoves()
        {
            var queens = BB_BlackQueens.Serialize();

            foreach (var queen in queens)
            {
                Bitboard moves = new Bitboard(queen).AttackFill_Queen(BB_Empty);
                var quietTargets = moves.Overlap(BB_Empty).Serialize();
                foreach (var target in quietTargets)
                {
                    _moves.Add(new Moves.Black.Quiet(Irreversibles, queen, target, Piece.BlackQueen));
                }
                var captureTargets = moves.Overlap(BB_White).Serialize();
                foreach (var target in captureTargets)
                {
                    _moves.Add(new Moves.Black.Capture(Irreversibles, queen, target, Piece.BlackQueen, Mailbox[target]));
                }
            }
        }

        internal void GetWhiteKingMoves()
        {
            //castling
            if (Irreversibles.CastleWhiteShort && (BB_AllBlackAttacks.Overlap(Bitboard.e1f1g1) == 0) && (BB_Occupied.Overlap(Bitboard.f1g1) == 0))
            {
                _moves.Add(new Moves.White.ShortCastle(Irreversibles));
            }
            if (Irreversibles.CastleWhiteLong && (BB_AllBlackAttacks.Overlap(Bitboard.e1d1c1) == 0) && (BB_Occupied.Overlap(Bitboard.d1c1b1) == 0))
            {
                _moves.Add(new Moves.White.LongCastle(Irreversibles));
            }
            //quiet+captures
            Coordinate king = BB_WhiteKing.BitScanForward();
            Bitboard moves = Bitboard.KingMoves[king];
            var quietTargets = moves.Overlap(BB_Empty).Serialize();
            foreach (var target in quietTargets)
            {
                _moves.Add(new Moves.White.Quiet(Irreversibles, king, target, Piece.WhiteKing));
            }
            var captureTargets = moves.Overlap(BB_Black).Serialize();
            foreach (var target in captureTargets)
            {
                _moves.Add(new Moves.White.Capture(Irreversibles, king, target, Piece.WhiteKing, Mailbox[target]));
            }
        }
        internal void GetBlackKingMoves()
        {
            //castling
            if (Irreversibles.CastleBlackShort && (BB_AllWhiteAttacks.Overlap(Bitboard.e8f8g8) == 0) && (BB_Occupied.Overlap(Bitboard.f8g8) == 0))
            {
                _moves.Add(new Moves.Black.ShortCastle(Irreversibles));
            }
            if (Irreversibles.CastleBlackLong && (BB_AllWhiteAttacks.Overlap(Bitboard.e8d8c8) == 0) && (BB_Occupied.Overlap(Bitboard.d8c8b8) == 0))
            {
                _moves.Add(new Moves.Black.LongCastle(Irreversibles));
            }
            //quiet+captures
            Coordinate king = BB_BlackKing.BitScanForward();
            Bitboard moves = Bitboard.KingMoves[king];
            var quietTargets = moves.Overlap(BB_Empty).Serialize();
            foreach (var target in quietTargets)
            {
                _moves.Add(new Moves.Black.Quiet(Irreversibles, king, target, Piece.BlackKing));
            }
            var captureTargets = moves.Overlap(BB_White).Serialize();
            foreach (var target in captureTargets)
            {
                _moves.Add(new Moves.Black.Capture(Irreversibles, king, target, Piece.BlackKing, Mailbox[target]));
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
                    if (p == Piece.None)
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

            if (!(Irreversibles.CastleWhiteShort || Irreversibles.CastleWhiteLong || Irreversibles.CastleBlackShort || Irreversibles.CastleBlackLong))
            {
                sb.Append('-');
            }
            else
            {
                if (Irreversibles.CastleWhiteShort) sb.Append('K');
                if (Irreversibles.CastleWhiteLong) sb.Append('Q');
                if (Irreversibles.CastleBlackShort) sb.Append('k');
                if (Irreversibles.CastleBlackLong) sb.Append('q');
            }

            sb.Append(' '); //section: en passant

            sb.Append(Irreversibles.PossibleEnPassant.ToString());

            sb.Append(' '); //section: halfmove clock

            sb.Append(Irreversibles.HalfmovesSinceCaptureOrPawnMove.ToString());

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
                        Console.WriteLine(move.GetPieceNotation() + " (" + move.GetSquareNotation() + "): " + subresult.ToString());
                    }
                }
                UnmakeMove();
            }
            return sum;
        }
    }
}