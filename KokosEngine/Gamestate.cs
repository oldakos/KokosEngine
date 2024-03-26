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
        /// <summary>
        /// Player to move
        /// </summary>
        internal Color P;
        internal bool IsWhiteMove
        {
            get { return P == Color.White; }
            set { P = value ? Color.White : Color.Black; }
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
        /// <summary>
        /// This should only get calculated if all other conditions allow castling
        /// </summary>
        internal Bitboard BB_EnemyAttacks { get; set; }

        #endregion
        #region Ally/Enemy Bitboard Properties - returned value depends on whose move it is

        internal Bitboard BB_AllyPawns => IsWhiteMove ? BB_WhitePawns : BB_BlackPawns;
        internal Bitboard BB_AllyKnights => IsWhiteMove ? BB_WhiteKnights : BB_BlackKnights;
        internal Bitboard BB_AllyBishops => IsWhiteMove ? BB_WhiteBishops : BB_BlackBishops;
        internal Bitboard BB_AllyRooks => IsWhiteMove ? BB_WhiteRooks : BB_BlackRooks;
        internal Bitboard BB_AllyQueens => IsWhiteMove ? BB_WhiteQueens : BB_BlackQueens;
        internal Bitboard BB_AllyKing => IsWhiteMove ? BB_WhiteKing : BB_BlackKing;
        internal Bitboard BB_Allies => IsWhiteMove ? BB_White : BB_Black;
        internal Bitboard BB_EnemyPawns => IsWhiteMove ? BB_BlackPawns : BB_WhitePawns;
        internal Bitboard BB_EnemyKnights => IsWhiteMove ? BB_BlackKnights : BB_WhiteKnights;
        internal Bitboard BB_EnemyBishops => IsWhiteMove ? BB_BlackBishops : BB_WhiteBishops;
        internal Bitboard BB_EnemyRooks => IsWhiteMove ? BB_BlackRooks : BB_WhiteRooks;
        internal Bitboard BB_EnemyQueens => IsWhiteMove ? BB_BlackQueens : BB_WhiteQueens;
        internal Bitboard BB_EnemyKing => IsWhiteMove ? BB_BlackKing : BB_WhiteKing;
        internal Bitboard BB_Enemies => IsWhiteMove ? BB_Black : BB_White;

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

            BB_Empty = Bitboard.Empty.Inverse(); //every square is empty before we set pieces

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
            ClearCoordFromPieceBitboard(coord);  //make sure that we don't store two pieces on one coordinate at the same time
            Bitboard mask = new Bitboard(coord);    //get coord as bitboard
            Bitboards[(int)piece] = Bitboards[(int)piece].Union(mask);  //set coord in the piece's bitboard
            Mailbox[coord] = piece;
        }
        internal void ClearCoordinate(Coordinate coord)
        {
            ClearCoordFromPieceBitboard(coord);
            Mailbox[coord] = Piece.None;
        }
        internal void ClearCoordFromPieceBitboard(Coordinate coord)
        {
            Piece piece = Mailbox[coord];
            if (piece == Piece.None) return;

            Bitboard mask = new Bitboard(coord).Inverse();  //get "all coords except the given one" as bitboard            
            Bitboards[(int)piece] = Bitboards[(int)piece].Overlap(mask);
        }

        internal void RefreshAllHelperBitboards()
        {
            BB_White = BB_WhitePawns.Union(BB_WhiteKing).Union(BB_WhiteKnights).Union(BB_WhiteBishops).Union(BB_WhiteRooks).Union(BB_WhiteQueens);
            BB_Black = BB_BlackPawns.Union(BB_BlackKing).Union(BB_BlackKnights).Union(BB_BlackBishops).Union(BB_BlackRooks).Union(BB_BlackQueens);
            BB_Occupied = BB_White.Union(BB_Black);
            BB_Empty = BB_Occupied.Inverse();
            BB_PossibleEP = Irreversibles.EPBitboard;
            BB_EnemyAttacks = 0;
        }

        #endregion

        internal bool IsPlayerToMoveInCheck()
        {
            Coordinate kingPosition = BB_AllyKing.BitScanForward();

            //checked by the other king? that's possible under pseudolegal generation
            Bitboard potentialAttackers = Bitboard.KingMoves[kingPosition];
            if (potentialAttackers.Overlap(BB_EnemyKing) != 0) return true;

            //by a knight?            
            potentialAttackers = Bitboard.KnightMoves[kingPosition];
            if (potentialAttackers.Overlap(BB_EnemyKnights) != 0) return true;

            //a pawn?
            potentialAttackers = BB_AllyKing.Shift_ForwardEast(P).Union(BB_AllyKing.Shift_ForwardWest(P));
            if (potentialAttackers.Overlap(BB_EnemyPawns) != 0) return true;

            //diagonally by a bishop or queen?
            potentialAttackers = BB_AllyKing.AttackFill_Bishop(BB_Empty);
            if (potentialAttackers.Overlap(BB_EnemyBishops.Union(BB_EnemyQueens)) != 0) return true;

            //orthogonally by a rook or queen?
            potentialAttackers = BB_AllyKing.AttackFill_Rook(BB_Empty);
            if (potentialAttackers.Overlap(BB_EnemyRooks.Union(BB_EnemyQueens)) != 0) return true;

            return false;
        }
        /// <summary>
        /// Return true if player to move can capture his opponent's king
        /// </summary>
        internal bool CanKingBeCaptured()
        {
            SwitchPlayerToMove();
            bool result = IsPlayerToMoveInCheck();
            SwitchPlayerToMove();
            return result;
        }

        internal List<IMove> GetPseudoLegalMoves()
        {            
            _moves.Clear();
            GeneratePawnMoves();
            GenerateKnightMoves();
            GenerateBishopMoves();
            GenerateRookMoves();
            GenerateQueenMoves();
            GenerateKingMoves();
            return _moves;
        }

        #region Move Generation        

        internal Bitboard GetAllAllyAttacks()
        {
            return
                GetPawnAttacks(BB_AllyPawns, P)
                | GetKnightAttacks(BB_AllyKnights)
                | GetBishopAttacks(BB_AllyBishops)
                | GetRookAttacks(BB_AllyRooks)
                | GetQueenAttacks(BB_AllyQueens)
                | GetKingAttacks(BB_AllyKing);
        }
        internal Bitboard GetAllEnemyAttacks()
        {
            if (BB_EnemyAttacks == 0)
            {
                BB_EnemyAttacks =
                    GetPawnAttacks(BB_EnemyPawns, IsWhiteMove ? Color.Black : Color.White)
                    | GetKnightAttacks(BB_EnemyKnights)
                    | GetBishopAttacks(BB_EnemyBishops)
                    | GetRookAttacks(BB_EnemyRooks)
                    | GetQueenAttacks(BB_EnemyQueens)
                    | GetKingAttacks(BB_EnemyKing);
            }
            return BB_EnemyAttacks;
        }
        internal Bitboard GetPawnAttacks(Bitboard bb, Color c)
        {
            Bitboard targets = bb.Shift_ForwardWest(c);
            targets |= bb.Shift_ForwardEast(c);
            return targets;
        }
        internal Bitboard GetKnightAttacks(Bitboard bb)
        {
            Bitboard result = Bitboard.Empty;
            var coords = bb.Serialize();
            foreach (var coord in coords)
            {
                result |= Bitboard.KnightMoves[coord];
            }
            return result;
        }
        internal Bitboard GetBishopAttacks(Bitboard bb)
        {
            return bb.AttackFill_Bishop(BB_Empty);
        }
        internal Bitboard GetRookAttacks(Bitboard bb)
        {
            return bb.AttackFill_Rook(BB_Empty);
        }
        internal Bitboard GetQueenAttacks(Bitboard bb)
        {
            return bb.AttackFill_Queen(BB_Empty);
        }
        internal Bitboard GetKingAttacks(Bitboard bb)
        {
            Bitboard result = Bitboard.Empty;
            var coords = bb.Serialize();
            foreach (var coord in coords)
            {
                result |= Bitboard.KingMoves[coord];
            }
            return result;
        }

        internal void GeneratePawnMoves()
        {
            GeneratePawnAdvances();
            GeneratePawnCaptures();
        }
        internal void GeneratePawnAdvances()
        {
            //double pushes
            Bitboard bb = BB_AllyPawns.Overlap(Bitboard.RelativeRank2(P));
            bb = bb.Shift_Forward(P).Overlap(BB_Empty);
            bb = bb.Shift_Forward(P).Overlap(BB_Empty);
            var targets = bb.Serialize();
            foreach (var target in targets)
            {
                _moves.Add(new DoublePawnPush(Irreversibles, target));
            }
            //promotions
            bb = BB_AllyPawns.Overlap(Bitboard.RelativeRank7(P));
            bb = bb.Shift_Forward(P).Overlap(BB_Empty);
            targets = bb.Serialize();
            foreach (var target in targets)
            {
                _moves.AddRange(Promotion.GetAll(Irreversibles, target));
            }
            //quiet
            bb = BB_AllyPawns.Overlap(Bitboard.RelativeRank7(P).Inverse());
            bb = bb.Shift_Forward(P).Overlap(BB_Empty);
            targets = bb.Serialize();
            foreach (var target in targets)
            {
                _moves.Add(new QuietMove(Irreversibles, target.Backward(P), target, IsWhiteMove ? Piece.WhitePawn : Piece.BlackPawn));
            }
        }
        internal void GeneratePawnCaptures()
        {
            Bitboard bb_captures = GetPawnAttacks(BB_AllyPawns, P).Overlap(BB_Enemies);

            //promotion captures
            Bitboard bb = bb_captures.Overlap(Bitboard.RelativeRank8(P));
            Bitboard bb_fromwest = bb.Shift_BackwardWest(P).Overlap(BB_AllyPawns);
            var sources = bb_fromwest.Serialize();
            foreach (var source in sources)
            {
                Coordinate target = source.ForwardEast(P);
                _moves.AddRange(PromotionCapture.GetAll(Irreversibles, source, target, Mailbox[target]));
            }
            Bitboard bb_fromeast = bb.Shift_BackwardEast(P).Overlap(BB_AllyPawns);
            sources = bb_fromeast.Serialize();
            foreach (var source in sources)
            {
                Coordinate target = source.ForwardWest(P);
                _moves.AddRange(PromotionCapture.GetAll(Irreversibles, source, target, Mailbox[target]));
            }
            //enpassant
            bb = BB_AllyPawns.Overlap(BB_PossibleEP.Shift_BackwardWest(P).Union(BB_PossibleEP.Shift_BackwardEast(P)));
            sources = bb.Serialize();
            foreach (var source in sources)
            {
                _moves.Add(new EnPassant(Irreversibles, source));
            }
            //normal captures
            bb = bb_captures.Overlap(Bitboard.RelativeRank8(P).Inverse());
            bb_fromwest = bb.Shift_BackwardWest(P).Overlap(BB_AllyPawns);
            sources = bb_fromwest.Serialize();
            foreach (var source in sources)
            {
                Coordinate target = source.ForwardEast(P);
                _moves.Add(new Capture(Irreversibles, source, target, IsWhiteMove ? Piece.WhitePawn : Piece.BlackPawn, Mailbox[target]));
            }
            bb_fromeast = bb.Shift_BackwardEast(P) & BB_AllyPawns;
            sources = bb_fromeast.Serialize();
            foreach (var source in sources)
            {
                Coordinate target = source.ForwardWest(P);
                _moves.Add(new Capture(Irreversibles, source, target, IsWhiteMove ? Piece.WhitePawn : Piece.BlackPawn, Mailbox[target]));
            }
        }

        internal void GenerateKnightMoves()
        {
            var knights = BB_AllyKnights.Serialize();

            foreach (var knight in knights)
            {
                Bitboard moves = Bitboard.KnightMoves[knight];
                var quietTargets = moves.Overlap(BB_Empty).Serialize();
                foreach (var target in quietTargets)
                {
                    _moves.Add(new QuietMove(Irreversibles, knight, target, IsWhiteMove ? Piece.WhiteKnight : Piece.BlackKnight));
                }
                var captureTargets = moves.Overlap(BB_Enemies).Serialize();
                foreach (var target in captureTargets)
                {
                    _moves.Add(new Capture(Irreversibles, knight, target, IsWhiteMove ? Piece.WhiteKnight : Piece.BlackKnight, Mailbox[target]));
                }
            }
        }

        internal void GenerateBishopMoves()
        {
            var bishops = BB_AllyBishops.Serialize();

            foreach (var bishop in bishops)
            {
                Bitboard moves = new Bitboard(bishop).AttackFill_Bishop(BB_Empty);
                var quietTargets = moves.Overlap(BB_Empty).Serialize();
                foreach (var target in quietTargets)
                {
                    _moves.Add(new QuietMove(Irreversibles, bishop, target, IsWhiteMove ? Piece.WhiteBishop : Piece.BlackBishop));
                }
                var captureTargets = moves.Overlap(BB_Enemies).Serialize();
                foreach (var target in captureTargets)
                {
                    _moves.Add(new Capture(Irreversibles, bishop, target, IsWhiteMove ? Piece.WhiteBishop : Piece.BlackBishop, Mailbox[target]));
                }
            }
        }

        internal void GenerateRookMoves()
        {
            var rooks = BB_AllyRooks.Serialize();

            foreach (var rook in rooks)
            {
                Bitboard moves = new Bitboard(rook).AttackFill_Rook(BB_Empty);
                var quietTargets = moves.Overlap(BB_Empty).Serialize();
                foreach (var target in quietTargets)
                {
                    _moves.Add(new QuietMove(Irreversibles, rook, target, IsWhiteMove ? Piece.WhiteRook : Piece.BlackRook));
                }
                var captureTargets = moves.Overlap(BB_Enemies).Serialize();
                foreach (var target in captureTargets)
                {
                    _moves.Add(new Capture(Irreversibles, rook, target, IsWhiteMove ? Piece.WhiteRook : Piece.BlackRook, Mailbox[target]));
                }
            }
        }

        internal void GenerateQueenMoves()
        {
            var queens = BB_AllyQueens.Serialize();

            foreach (var queen in queens)
            {
                Bitboard moves = new Bitboard(queen).AttackFill_Queen(BB_Empty);
                var quietTargets = moves.Overlap(BB_Empty).Serialize();
                foreach (var target in quietTargets)
                {
                    _moves.Add(new QuietMove(Irreversibles, queen, target, IsWhiteMove ? Piece.WhiteQueen : Piece.BlackQueen));
                }
                var captureTargets = moves.Overlap(BB_Enemies).Serialize();
                foreach (var target in captureTargets)
                {
                    _moves.Add(new Capture(Irreversibles, queen, target, IsWhiteMove ? Piece.WhiteQueen : Piece.BlackQueen, Mailbox[target]));
                }
            }
        }

        internal void GenerateKingMoves()
        {
            //castling
            if (CanAllyCastleShort() && (BB_Occupied.Overlap(Bitboard.HomeFG(P)) == 0) && (GetAllEnemyAttacks().Overlap(Bitboard.HomeEFG(P)) == 0))
            {
                _moves.Add(new ShortCastle(Irreversibles, P));
            }
            if (CanAllyCastleLong() && (BB_Occupied.Overlap(Bitboard.HomeDCB(P)) == 0) && (GetAllEnemyAttacks().Overlap(Bitboard.HomeEDC(P)) == 0))
            {
                _moves.Add(new LongCastle(Irreversibles, P));
            }
            //quiet+captures
            Coordinate king = BB_AllyKing.BitScanForward();
            Bitboard moves = GetKingAttacks(BB_AllyKing);
            var quietTargets = moves.Overlap(BB_Empty).Serialize();
            foreach (var target in quietTargets)
            {
                _moves.Add(new QuietMove(Irreversibles, king, target, IsWhiteMove ? Piece.WhiteKing : Piece.BlackKing));
            }
            var captureTargets = moves.Overlap(BB_Enemies).Serialize();
            foreach (var target in captureTargets)
            {
                _moves.Add(new Capture(Irreversibles, king, target, IsWhiteMove ? Piece.WhiteKing : Piece.BlackKing, Mailbox[target]));
            }
        }

        private bool CanAllyCastleLong() => IsWhiteMove ? Irreversibles.CastleWhiteLong : Irreversibles.CastleBlackLong;
        private bool CanAllyCastleShort() => IsWhiteMove ? Irreversibles.CastleWhiteShort : Irreversibles.CastleBlackShort;

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
        internal long Perft(int depth, bool detail = false)
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