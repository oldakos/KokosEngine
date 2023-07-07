using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KokosEngine
{
    internal struct MoveAndIrreversibleInfo
    {
        private Move _move;
        private int _ep;
        private int _halfmoves;

        internal MoveAndIrreversibleInfo(Move move, int enPassantIndex, int halfmoves)
        {
            _move = move;
            _ep = enPassantIndex;
            _halfmoves = halfmoves;
        }
        internal Move Move
        {
            get
            {
                return _move;
            }
        }
        internal ulong EnPassantBB
        {
            get
            {
                if (_ep < 0 || _ep > 63) return 0;
                else return ((ulong)1) << _ep;
            }
        }
        internal int EnPassantIndex
        {
            get
            {
                return _ep;
            }
        }
        internal int Halfmoves
        {
            get
            {
                return _halfmoves;
            }
        }
    }
}
