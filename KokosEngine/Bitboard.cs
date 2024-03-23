using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace KokosEngine
{
    /// <summary>
    /// Represents some subset of the 64 coordinates on a chess board.
    /// </summary>
    internal struct Bitboard
    {
        private readonly ulong _value;

        /// <summary>
        /// Instantiate a bitboard with the exact underlying ulong value
        /// </summary>
        internal Bitboard(ulong value)
        {
            this._value = value;
        }
        /// <summary>
        /// Instantiate a bitboard with a single coordinate
        /// </summary>
        internal Bitboard(Coordinate coordinate)
        {
            _value = 1UL << ((int)coordinate);
        }

        public static implicit operator Bitboard(ulong value) { return new Bitboard(value); }
        public static implicit operator ulong(Bitboard bb) { return bb._value; }

        internal Bitboard Inverse()
        {
            return new Bitboard(~_value);
        }
        internal Bitboard Union(Bitboard bb)
        {
            return new Bitboard(this._value | bb._value);
        }
        internal Bitboard Overlap(Bitboard bb)
        {
            return new Bitboard(this._value & bb._value);
        }
        internal bool IsEmpty()
        {
            return _value == 0;
        }

        #region Some useful constant bitboards

        static readonly internal Bitboard Empty = 0UL;

        static readonly internal Bitboard NotAFile = 0xfefefefefefefefe;
        static readonly internal Bitboard NotABFiles = 0xfcfcfcfcfcfcfcfc;
        static readonly internal Bitboard NotHFile = 0x7f7f7f7f7f7f7f7f;
        static readonly internal Bitboard NotGHFiles = 0x3f3f3f3f3f3f3f3f;

        static readonly internal Bitboard Rank1 = 0x00000000000000FF;
        static readonly internal Bitboard Rank2 = 0x000000000000FF00;
        static readonly internal Bitboard Rank7 = 0x00FF000000000000;
        static readonly internal Bitboard Rank8 = 0xFF00000000000000;

        static readonly internal Bitboard e1f1g1 = 0x0000000000000070;
        static readonly internal Bitboard f1g1 = 0x0000000000000060;
        static readonly internal Bitboard e1d1c1 = 0x000000000000001C;
        static readonly internal Bitboard d1c1b1 = 0x000000000000000E;
        static readonly internal Bitboard e8f8g8 = 0x7000000000000000;
        static readonly internal Bitboard f8g8 = 0x6000000000000000;
        static readonly internal Bitboard e8d8c8 = 0x1C00000000000000;
        static readonly internal Bitboard d8c8b8 = 0x0E00000000000000;

        /// <summary>
        /// Indexed by the coordinate of the knight. The bitboard contains the knight's possible moves.
        /// </summary>
        static readonly internal Bitboard[] KnightMoves =
        {
        0x0000000000020400,
        0x0000000000050800,
        0x00000000000A1100,
        0x0000000000142200,
        0x0000000000284400,
        0x0000000000508800,
        0x0000000000A01000,
        0x0000000000402000,
        0x0000000002040004,
        0x0000000005080008,
        0x000000000A110011,
        0x0000000014220022,
        0x0000000028440044,
        0x0000000050880088,
        0x00000000A0100010,
        0x0000000040200020,
        0x0000000204000402,
        0x0000000508000805,
        0x0000000A1100110A,
        0x0000001422002214,
        0x0000002844004428,
        0x0000005088008850,
        0x000000A0100010A0,
        0x0000004020002040,
        0x0000020400040200,
        0x0000050800080500,
        0x00000A1100110A00,
        0x0000142200221400,
        0x0000284400442800,
        0x0000508800885000,
        0x0000A0100010A000,
        0x0000402000204000,
        0x0002040004020000,
        0x0005080008050000,
        0x000A1100110A0000,
        0x0014220022140000,
        0x0028440044280000,
        0x0050880088500000,
        0x00A0100010A00000,
        0x0040200020400000,
        0x0204000402000000,
        0x0508000805000000,
        0x0A1100110A000000,
        0x1422002214000000,
        0x2844004428000000,
        0x5088008850000000,
        0xA0100010A0000000,
        0x4020002040000000,
        0x0400040200000000,
        0x0800080500000000,
        0x1100110A00000000,
        0x2200221400000000,
        0x4400442800000000,
        0x8800885000000000,
        0x100010A000000000,
        0x2000204000000000,
        0x0004020000000000,
        0x0008050000000000,
        0x00110A0000000000,
        0x0022140000000000,
        0x0044280000000000,
        0x0088500000000000,
        0x0010A00000000000,
        0x0020400000000000
        };
        /// <summary>
        /// Indexed by the coordinate of the king. The bitboard contains the king's possible moves.
        /// </summary>
        static readonly internal ulong[] KingMoves =
        {
        0x0000000000000302,
        0x0000000000000705,
        0x0000000000000E0A,
        0x0000000000001C14,
        0x0000000000003828,
        0x0000000000007050,
        0x000000000000E0A0,
        0x000000000000C040,
        0x0000000000030203,
        0x0000000000070507,
        0x00000000000E0A0E,
        0x00000000001C141C,
        0x0000000000382838,
        0x0000000000705070,
        0x0000000000E0A0E0,
        0x0000000000C040C0,
        0x0000000003020300,
        0x0000000007050700,
        0x000000000E0A0E00,
        0x000000001C141C00,
        0x0000000038283800,
        0x0000000070507000,
        0x00000000E0A0E000,
        0x00000000C040C000,
        0x0000000302030000,
        0x0000000705070000,
        0x0000000E0A0E0000,
        0x0000001C141C0000,
        0x0000003828380000,
        0x0000007050700000,
        0x000000E0A0E00000,
        0x000000C040C00000,
        0x0000030203000000,
        0x0000070507000000,
        0x00000E0A0E000000,
        0x00001C141C000000,
        0x0000382838000000,
        0x0000705070000000,
        0x0000E0A0E0000000,
        0x0000C040C0000000,
        0x0003020300000000,
        0x0007050700000000,
        0x000E0A0E00000000,
        0x001C141C00000000,
        0x0038283800000000,
        0x0070507000000000,
        0x00E0A0E000000000,
        0x00C040C000000000,
        0x0302030000000000,
        0x0705070000000000,
        0x0E0A0E0000000000,
        0x1C141C0000000000,
        0x3828380000000000,
        0x7050700000000000,
        0xE0A0E00000000000,
        0xC040C00000000000,
        0x0203000000000000,
        0x0507000000000000,
        0x0A0E000000000000,
        0x141C000000000000,
        0x2838000000000000,
        0x5070000000000000,
        0xA0E0000000000000,
        0x40C0000000000000
        };

        #endregion

        #region Shifting methods

        internal Bitboard Shift(Direction dir)
        {
            switch (dir)
            {
                case Direction.N: return Shift_N();
                case Direction.NE: return Shift_NE();
                case Direction.E: return Shift_E();
                case Direction.SE: return Shift_SE();
                case Direction.S: return Shift_S();
                case Direction.SW: return Shift_SW();
                case Direction.W: return Shift_W();
                case Direction.NW: return Shift_NW();
                default: return this;
            }
        }
        internal Bitboard Shift_N() { return _value << 8; }
        internal Bitboard Shift_NE() { return (_value << 9) & NotAFile; }
        internal Bitboard Shift_E() { return (_value << 1) & NotAFile; }
        internal Bitboard Shift_SE() { return (_value >> 7) & NotAFile; }
        internal Bitboard Shift_S() { return _value >> 8; }
        internal Bitboard Shift_SW() { return (_value >> 9) & NotHFile; }
        internal Bitboard Shift_W() { return (_value >> 1) & NotHFile; }
        internal Bitboard Shift_NW() { return (_value << 7) & NotHFile; }
        internal Bitboard Shift_NNE() { return (_value << 17) & NotAFile; }
        internal Bitboard Shift_NEE() { return (_value << 10) & NotABFiles; }
        internal Bitboard Shift_SEE() { return (_value >> 6) & NotABFiles; }
        internal Bitboard Shift_SSE() { return (_value >> 15) & NotAFile; }
        internal Bitboard Shift_SSW() { return (_value >> 17) & NotHFile; }
        internal Bitboard Shift_SWW() { return (_value >> 10) & NotGHFiles; }
        internal Bitboard Shift_NWW() { return (_value << 6) & NotGHFiles; }
        internal Bitboard Shift_NNW() { return (_value << 15) & NotHFile; }

        #endregion

        #region Propagation methods

        // From each 1 in "this" bitboard, propagate 1s through the Propagator coordinates in the given direction.
        // "Generator" = a bitboard from whose coordinates the propagation starts
        // "Propagator" = a bitboard that contains coordinates through which propagation is allowed to take a step
        // "Blocker" = the first non-propagator square which stopped a "ray" of propagation.

        // Occluded Fill includes the generator and excludes blockers.
        // Attack Fill excludes the generator and includes blockers.

        internal Bitboard OccludedFill(Direction direction, Bitboard propagator)
        {
            Bitboard generator = this;
            Bitboard result = Bitboard.Empty;
            while (!generator.IsEmpty())
            {
                result = result.Union(generator);
                generator = generator.Shift(direction).Overlap(propagator);
            }
            return result;
        }
        internal Bitboard AttackFill(Direction direction, Bitboard propagator)
        {
            return OccludedFill(direction, propagator).Shift(direction);
        }

        internal Bitboard AttackFill_Bishop(Bitboard propagator)
        {
            return AttackFill(Direction.NE, propagator).Union(
                   AttackFill(Direction.SE, propagator).Union(
                   AttackFill(Direction.SW, propagator).Union(
                   AttackFill(Direction.NW, propagator))));
        }
        internal Bitboard AttackFill_Rook(Bitboard propagator)
        {
            return AttackFill(Direction.N, propagator).Union(
                   AttackFill(Direction.E, propagator).Union(
                   AttackFill(Direction.S, propagator).Union(
                   AttackFill(Direction.W, propagator))));
        }
        internal Bitboard AttackFill_Queen(Bitboard propagator)
        {
            return AttackFill_Bishop(propagator).Union(AttackFill_Rook(propagator));
        }

        #endregion

        /// <summary>
        /// Get the index of the least significant set bit in the underlying ulong of this bitboard. Returns -1 if all bits are unset.
        /// </summary>
        internal int BitScanForward()
        {
            return BitScanForward(_value);
        }
        /// <summary>
        /// Get the index of the least significant set bit in a given ulong. Returns -1 if all bits are unset.
        /// </summary>
        static internal int BitScanForward(ulong val)
        {
            //This is from `https://www.chessprogramming.org/BitScan#De_Bruijn_Multiplication`

            if (val == 0UL) return -1;
            const ulong debruijn64 = 0x03f79d71b4cb0a89;
            ulong negative = (ulong)(-(long)val);
            return index64[((val & negative) * debruijn64) >> 58];
        }
        /// <summary>
        /// A magic constant required for BitScanForward.
        /// </summary>
        static readonly int[] index64 = {
             0,  1, 48,  2, 57, 49, 28,  3,
            61, 58, 50, 42, 38, 29, 17,  4,
            62, 55, 59, 36, 53, 51, 43, 22,
            45, 39, 33, 30, 24, 18, 12,  5,
            63, 47, 56, 27, 60, 41, 37, 16,
            54, 35, 52, 21, 44, 32, 23, 11,
            46, 26, 40, 15, 34, 20, 31, 10,
            25, 14, 19,  9, 13,  8,  7,  6
        };

        /// <summary>
        /// Return an IEnumerable containing all Coordinates found in this Bitboard.
        /// </summary>
        internal IEnumerable<Coordinate> Serialize()
        {
            List<Coordinate> list = new List<Coordinate>();
            ulong val = _value;
            while (val != 0)
            {
                int index = BitScanForward(val);
                list.Add(new Coordinate(index));
                val &= val - 1;
            }
            return list;
        }

    }
}
